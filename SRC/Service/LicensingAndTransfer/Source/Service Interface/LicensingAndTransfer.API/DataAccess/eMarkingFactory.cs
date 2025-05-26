using HmacHashing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LicensingAndTransfer.API
{
    public class eMarkingFactory
    {
        private int commandTimeout = 0;

        public eMarkingFactory()
        {
            commandTimeout = 2147483647;
        }

        public List<eMarkingSyncUserResponse> SynceMarkingUser()
        {
            List<eMarkingSyncUserResponse> eMarkingSyncUserResponses = new List<eMarkingSyncUserResponse>();
            string Status = String.Empty;
            try
            {
                Constants.Log.Info("Begin SynceMarkingUser()");
                DataTable tblResponse = new DataTable();
                DataTable InsertedtblResponse = new DataTable();
                SqlDataReader dataReader;
                DataTable filteredDT = null;
                DataTable filteredDatatbl = null;

                // Get Paper Proctor info for importing into eMarking DB
                SqlConnection userSyncFrom = CommonDAL.GetSEABConnection();
                string isSyncFromDel = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["IsUserSyncFromDelivery"]);
                bool IsUserSyncFromDelivery = true;
                if (!string.IsNullOrEmpty(isSyncFromDel))
                {
                    IsUserSyncFromDelivery = Convert.ToBoolean(isSyncFromDel);
                }
                if (!IsUserSyncFromDelivery)
                {
                    userSyncFrom = CommonDAL.GeteMarkingConnection();
                }

                string passphrasecode = GetActivePassPhrase();

                using (System.Data.SqlClient.SqlConnection connectionSEAB = userSyncFrom)
                {
                    Constants.Log.Info("Begin to Get eMarking users from PaperProctorInfo table");
                    connectionSEAB.Open();
                    using (System.Data.SqlClient.SqlCommand commandSynceMarkingUser = new SqlCommand("[Marking].[UspGetPaperProctorInfoForEmarking]"))
                    {
                        commandSynceMarkingUser.CommandTimeout = commandTimeout;
                        commandSynceMarkingUser.Connection = connectionSEAB;
                        commandSynceMarkingUser.CommandType = CommandType.StoredProcedure;
                        System.Data.SqlClient.SqlDataAdapter adap = new SqlDataAdapter(commandSynceMarkingUser);
                        adap.Fill(tblResponse);

                        var query = from r in tblResponse.AsEnumerable().ToList()
                                    where r.Field<string>("ExamLevel") != null && r.Field<string>("ExamSeries") != null &&
                                    r.Field<string>("ExamYear") != null && r.Field<string>("ModeofAssessment") != null &&
                                    r.Field<string>("PaperNumber") != null && r.Field<string>("SubjectCode") != null &&
                                    (r.Field<string>("MarkerLevel") == "L0" || r.Field<string>("MarkerLevel") == "L1" ||
                                    r.Field<string>("MarkerLevel") == "L2" || r.Field<string>("MarkerLevel") == "L3" || r.Field<string>("MarkerLevel") == "L4" || r.Field<string>("MarkerLevel") == "L5")
                                    select r;

                        if (query != null && query.Count() > 0)
                        {
                            filteredDT = query.Distinct(DataRowComparer.Default).CopyToDataTable();

                            if (!string.IsNullOrEmpty(passphrasecode))
                            {
                                //Password encryption
                                foreach (DataRow row in filteredDT.Rows)
                                {
                                    if (!IsUserSyncFromDelivery)
                                    {
                                        row["ModeofAssessment"] = AlterModeOdAssessement(row["ModeofAssessment"]);
                                    }
                                    var nricstr = row["NRIC/FIN"].ToString();
                                    row["Password"] = HmacHash.Encrypt(ConfigurationManager.AppSettings["encryptionKey_SSO"].ToString(), passphrasecode.Trim() + nricstr.Substring(nricstr.Length - 4));
                                }
                            }
                            Status = JsonConvert.SerializeObject(filteredDT);
                        }
                        else
                        {
                            Status = "NoRecords";
                        }
                    }
                    connectionSEAB.Close();
                }
                Constants.Log.Info("End of Getting eMarking users from PaperProctorInfo table" + " Data: " + (tblResponse == null ? " " : Status));

                // Insert MP Info into Marking Tables from T&A tbl
                if (Status != "NoRecords")
                {
                    var filteredDatatable = filteredDT.AsEnumerable()
                                        .GroupBy(r => new
                                        {
                                            ExamLevel = r["ExamLevel"],
                                            ExamSeries = r["ExamSeries"],
                                            ExamYear = r["ExamYear"],
                                            ModeofAssessment = r["ModeofAssessment"],
                                            PaperNumber = r["PaperNumber"],
                                            SubjectCode = r["SubjectCode"],
                                            SubjectName = r["SubjectName"]
                                        })
                                        .ToList();
                    foreach (var tbldata in filteredDatatable)
                    {
                        eMarkingSyncUserResponse objResponse = new eMarkingSyncUserResponse();

                        var filtablesdata = filteredDT.AsEnumerable().Where(row => row.Field<string>("ExamLevel") == tbldata.Key.ExamLevel.ToString() &&
                        row.Field<string>("ExamSeries") == tbldata.Key.ExamSeries.ToString() &&
                        row.Field<string>("ExamYear") == tbldata.Key.ExamYear.ToString() &&
                        row.Field<string>("ModeofAssessment") == tbldata.Key.ModeofAssessment.ToString() &&
                        row.Field<string>("PaperNumber") == tbldata.Key.PaperNumber.ToString() &&
                        row.Field<string>("SubjectCode") == tbldata.Key.SubjectCode.ToString());
                        if (filtablesdata != null && filtablesdata.Any())
                        {
                            filteredDatatbl = filtablesdata.CopyToDataTable();
                        }
                        ProjectInfo projectInfo = new ProjectInfo
                        {
                            ExamLevel = tbldata.Key.ExamLevel.ToString(),
                            ExamSeries = tbldata.Key.ExamSeries.ToString(),
                            ExamYear = tbldata.Key.ExamYear.ToString(),
                            ModeofAssessment = tbldata.Key.ModeofAssessment.ToString(),
                            PaperNumber = tbldata.Key.PaperNumber.ToString(),
                            SubjectCode = tbldata.Key.SubjectCode.ToString()
                        };

                        if (filtablesdata != null && filtablesdata.Any() && filteredDatatbl.Rows.Count > 0)
                        {
                            Constants.Log.Info("Begin SynceMarkingUser() to eMarking DB");

                            try
                            {
                                using (SqlConnection connectionSynceMarkingUser = CommonDAL.GeteMarkingConnection())
                                {
                                    using (SqlCommand commandSynceMarkingUser = new SqlCommand("[Marking].[USPImportMarkingPersonnelUserInfo]", connectionSynceMarkingUser))
                                    {
                                        SqlParameter sqlParameter;
                                        commandSynceMarkingUser.CommandType = CommandType.StoredProcedure;
                                        commandSynceMarkingUser.Parameters.Clear();
                                        connectionSynceMarkingUser.Open();
                                        sqlParameter = commandSynceMarkingUser.Parameters.AddWithValue("@UDTUserInfo", filteredDatatbl);
                                        sqlParameter.SqlDbType = SqlDbType.Structured;
                                        sqlParameter.TypeName = "Marking.UDTUserInfo";

                                        // Fill result set to datatable
                                        dataReader = commandSynceMarkingUser.ExecuteReader();
                                        if (dataReader.HasRows)
                                        {
                                            while (dataReader.Read())
                                            {
                                                Status = dataReader.GetValue(0).ToString();
                                            }

                                            dataReader.NextResult();
                                            InsertedtblResponse.Load(dataReader);
                                        }
                                        if (!dataReader.IsClosed) { dataReader.Close(); }
                                    }
                                    connectionSynceMarkingUser.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                Constants.Log.Error(ex.Message, ex);
                                Status = "InValidRecordsFound";
                            }
                        }
                        Constants.Log.Info("End of SynceMarkingUser() to eMarking DB" + " Status: " + InsertedtblResponse);

                        // Update Paper Proctor Info which have been processed in the eMarking DB

                        Constants.Log.Info("Begin to Update Paper Proctor Info which have been processed in the eMarking DB");

                        userSyncFrom = CommonDAL.GetSEABConnection();
                        if (!IsUserSyncFromDelivery)
                        {
                            userSyncFrom = CommonDAL.GeteMarkingConnection();
                        }
                        try
                        {
                            using (SqlConnection connectionSEAB = userSyncFrom)
                            {
                                connectionSEAB.Open();
                                using (SqlCommand commandSynceMarkingUser = new SqlCommand("[Marking].[USPUpdatePaperProctorInfo]"))
                                {
                                    SqlParameter sqlParameter;
                                    commandSynceMarkingUser.Connection = connectionSEAB;
                                    commandSynceMarkingUser.CommandType = CommandType.StoredProcedure;
                                    commandSynceMarkingUser.Parameters.Clear();
                                    sqlParameter = commandSynceMarkingUser.Parameters.AddWithValue("@UDTProctorProcessedInfo", InsertedtblResponse);
                                    sqlParameter.SqlDbType = SqlDbType.Structured;
                                    sqlParameter.TypeName = "Marking.UDTProctorProcessedInfo_V1";
                                    sqlParameter = commandSynceMarkingUser.Parameters.Add("@Status", SqlDbType.NVarChar, 20);
                                    sqlParameter.Direction = ParameterDirection.Output;
                                    commandSynceMarkingUser.ExecuteNonQuery();
                                    Status = (string)sqlParameter.Value;
                                }
                                connectionSEAB.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            Constants.Log.Error(ex.Message, ex);
                            Status = "InValidRecordsFound";
                        }
                        if (Status == "S001")
                        {
                            Status = "User has been Added to the Project Successfully";
                        }
                        else if (Status == "EOO1")
                        {
                            Status = "Error while inserting the record";
                        }
                        else if (Status == "E002")
                        {
                            Status = "Invalid Exam Level";
                        }
                        else if (Status == "E003")
                        {
                            Status = "Invalid Exam Series";
                        }
                        else if (Status == "E004")
                        {
                            Status = "Invalid Exam Year";
                        }
                        else if (Status == "E005")
                        {
                            Status = "Invalid MoA";
                        }
                        else if (Status == "E006")
                        {
                            Status = "Invalid Subject Paper Information";
                        }
                        else if (Status == "E007")
                        {
                            Status = "Invalid Subject info";
                        }
                        else if (Status == "E008")
                        {
                            Status = "Invalid Project info";
                        }

                        objResponse.Status = Status;
                        objResponse.ProjectData = JsonConvert.SerializeObject(projectInfo);
                        objResponse.ResponseData = JsonConvert.SerializeObject(InsertedtblResponse);

                        eMarkingSyncUserResponses.Add(objResponse);
                        Constants.Log.Info("End of Update Paper Proctor Info which have been processed in the eMarking DB" + " Status: " + Status);
                    }
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error(ex.Message, ex);
                Status = "InValidRecordsFound";
            }
            return eMarkingSyncUserResponses;
        }

        private string GetActivePassPhrase()
        {
            string passPhrase = string.Empty;
            try
            {
                SqlConnection eMarkCon = CommonDAL.GeteMarkingConnection();
                Constants.Log.Info("Begin to Get eMarking users Active Pass Phrase");
                eMarkCon.Open();
                SqlCommand pwdcmd = new SqlCommand("SELECT top 1 PassPharseCode FROM Marking.PassPharse WHERE IsActive = 1", eMarkCon);

                passPhrase = Convert.ToString(pwdcmd.ExecuteScalar());

                if (eMarkCon.State == ConnectionState.Open)
                {
                    eMarkCon.Close();
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error(ex.Message, ex);
            }

            return passPhrase;
        }

        private UserPassPhraseDetails GetUserPassPhraseDetails()
        {
            string Status = String.Empty;
            UserPassPhraseDetails userPassPhraseDetails = new UserPassPhraseDetails();

            try
            {
                SqlConnection userSyncFrom = CommonDAL.GeteMarkingConnection();
                using (SqlConnection eMarkCon = userSyncFrom)
                {
                    Constants.Log.Info("Begin to Get eMarking users from PaperProctorInfo table");
                    eMarkCon.Open();
                    using (SqlCommand commandSynceMarkingUser = new SqlCommand("[Marking].[UspGetUserPassphraseDetails]"))
                    {
                        commandSynceMarkingUser.CommandTimeout = commandTimeout;
                        commandSynceMarkingUser.Connection = eMarkCon;
                        commandSynceMarkingUser.CommandType = CommandType.StoredProcedure;
                        SqlDataReader reader = commandSynceMarkingUser.ExecuteReader();
                        if (reader != null)
                        {
                            if (reader.HasRows)
                            {
                                userPassPhraseDetails.UserDetails = new List<EmailUsers>();
                                while (reader.Read())
                                {
                                    userPassPhraseDetails.UserDetails.Add(new EmailUsers()
                                    {
                                        ID = reader["userid"] != DBNull.Value ? Convert.ToInt64(reader["userid"]) : 0,
                                        ToLoginID = reader["LoginID"] != DBNull.Value ? Convert.ToString(reader["LoginID"]) : string.Empty,
                                        Nric = reader["NRIC"] != DBNull.Value ? Convert.ToString(reader["NRIC"]) : string.Empty
                                    });
                                }
                            }
                            // this advances to the next resultset
                            reader.NextResult();

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    userPassPhraseDetails.PassPhraseText = (string)reader["PassPharseCode"];
                                }
                            }
                            if (!reader.IsClosed) { reader.Close(); }
                            Constants.Log.Info("End of Getting eMarking users from PaperProctorInfo table" + " Data: " + (userPassPhraseDetails == null ? " " : Status));
                        }
                    }
                    if (eMarkCon.State == ConnectionState.Open)
                    {
                        eMarkCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error(ex.Message, ex);
                Status = "InValidRecordsFound";
            }
            return userPassPhraseDetails;
        }

        /// <summary>
        /// Reset password for the users who has not yet logged in by IsReGenerateDefaultPwd key
        /// </summary>
        public void ReGenerateDefaultPwd()
        {
            var PassPhraseDetails = GetUserPassPhraseDetails();
            string encKeySso = Convert.ToString(ConfigurationManager.AppSettings["encryptionKey_SSO"]);
            if (PassPhraseDetails != null
                && PassPhraseDetails.UserDetails != null
                && PassPhraseDetails.UserDetails.Count > 0
                && !string.IsNullOrEmpty(PassPhraseDetails.PassPhraseText))
            {
                PassPhraseDetails.UserDetails.ForEach(pasUser =>
                {
                    if (!string.IsNullOrEmpty(pasUser.Nric))
                    {
                        string DefPwd = HmacHash.Encrypt(encKeySso, PassPhraseDetails.PassPhraseText.Trim() + pasUser.Nric.Substring(pasUser.Nric.Length - 4));
                        UpdateDefaultPwd(pasUser, DefPwd);
                    }
                    else
                    {
                        Constants.Log.Info("NRIC null for " + pasUser.ID);
                    }
                });
            }
        }

        private void UpdateDefaultPwd(EmailUsers pasUser, string defPwd)
        {
            using (SqlConnection connectioneMarkingUser = CommonDAL.GeteMarkingConnection())
            {
                using (SqlCommand commandeMarkingUser = new SqlCommand("UPDATE Marking.Userinfo SET Password = @Password WHERE UserID=@UserID",
                    connectioneMarkingUser))
                {
                    Constants.Log.Info("Begin to Update UpdateDefaultPwd to eMarking table");

                    commandeMarkingUser.CommandType = CommandType.Text;
                    commandeMarkingUser.Parameters.Clear();
                    connectioneMarkingUser.Open();
                    commandeMarkingUser.Parameters.AddWithValue("@Password", defPwd);
                    commandeMarkingUser.Parameters.AddWithValue("@UserID", pasUser.ID);
                    int a = commandeMarkingUser.ExecuteNonQuery();
                    Constants.Log.Info("Begin to Update UpdateDefaultPwd to eMarking table status " + a);
                }
                if (connectioneMarkingUser.State == ConnectionState.Open)
                {
                    connectioneMarkingUser.Close();
                }
            }
        }

        private object AlterModeOdAssessement(object v)
        {
            string Moa = Convert.ToString(v);
            bool MoaReplaceRequired = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["MoaReplaceRequired"]);
            string MoaToReplace = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["MOAsToReplace"]);

            if (!string.IsNullOrEmpty(MoaToReplace) && MoaReplaceRequired)
            {
                string[] moalist = MoaToReplace.ToUpper().Split(',');

                if (moalist.Contains(Moa.ToUpper()))
                {
                    Moa = "eWritten";
                }
            }
            return Moa;
        }
    }
}