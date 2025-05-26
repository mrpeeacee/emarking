using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AESCryptography;
using LicensingAndTransfer.API.Contracts;
using Newtonsoft.Json;
using AWSS3.FileManagement;

namespace LicensingAndTransfer.API
{
    public class CommonDAL
    {
        int commandTimeout = 0;
        static string DataEncryptionKey = System.Configuration.ConfigurationManager.AppSettings["DataEncryptionKey"].ToString();

        public CommonDAL()
        {
            commandTimeout = 2147483647;
        }

        public static System.Data.SqlClient.SqlParameter BuildSqlParameter(string parameterName, System.Data.SqlDbType sqlDbType, int size, string sourceColumn, object values, System.Data.ParameterDirection paramDirection)
        {
            System.Data.SqlClient.SqlParameter objParameter = new System.Data.SqlClient.SqlParameter(parameterName, sqlDbType, size, sourceColumn);
            objParameter.Value = values;
            objParameter.Direction = paramDirection;
            return objParameter;
        }

        public static System.Data.SqlClient.SqlConnection GetConnection()
        {
            string connectionString = Constants.DBConnectionString;
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }
        public static System.Data.SqlClient.SqlConnection GetCOEConnection()
        {
            string COEconnectionString = Constants.COEDBConnectionString;
            return new System.Data.SqlClient.SqlConnection(COEconnectionString);
        }

        public static System.Data.SqlClient.SqlConnection GetConnectionforReportingServer()
        {
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["connectionStringforReportingServer"].ToString();
            //  Decrypting connection string
            connectionString = AESCrypto.Decrypt(connectionString);
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }
        public static System.Data.SqlClient.SqlConnection GeteMarkingConnection()
        {
            string connectionString = Constants.eMarkingConnectionString;
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        public static System.Data.SqlClient.SqlConnection GetSEABConnection()
        {
            string connectionString = Constants.SEABConnectionString;
            return new System.Data.SqlClient.SqlConnection(connectionString);
        } 

    }


    public class GenericDAL
    {
        int commandTimeout = 0;
        public GenericDAL()
        {
            commandTimeout = 2147483647;
        }
        public string ValidateMacID(string MacID, DataContracts.ServerTypes ServerType)
        {
            string Status = "E000";
            Constants.Log.Info("Begin ValidateMacID() - MacID: " + MacID);
            Constants.Log.Info("Begin ValidateMacID() - Server Type: " + ServerType);
            using (System.Data.SqlClient.SqlConnection connectionValidateMacID = CommonDAL.GetConnection())
            {
                connectionValidateMacID.Open();
                using (System.Data.SqlClient.SqlCommand commandValidateMacID = new System.Data.SqlClient.SqlCommand())
                {
                    commandValidateMacID.CommandTimeout = commandTimeout;
                    commandValidateMacID.Connection = connectionValidateMacID;
                    commandValidateMacID.CommandText = "ValidateMacID";
                    commandValidateMacID.CommandType = System.Data.CommandType.StoredProcedure;
                    #region Building Parameters
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@MacID", System.Data.SqlDbType.NVarChar, 2147483646, "MacID", MacID, System.Data.ParameterDirection.Input));
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@ServerType", System.Data.SqlDbType.NVarChar, 2147483646, "ServerType", ServerType.ToString(), System.Data.ParameterDirection.Input));
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@Status", System.Data.SqlDbType.NVarChar, 2147483646, "Status", Status, System.Data.ParameterDirection.Output));
                    #endregion
                    commandValidateMacID.ExecuteNonQuery();
                    if (commandValidateMacID != null && commandValidateMacID.Parameters != null && commandValidateMacID.Parameters["@Status"] != null
                        && commandValidateMacID.Parameters["@Status"].Value != null && commandValidateMacID.Parameters["@Status"].Value != DBNull.Value)
                        Status = commandValidateMacID.Parameters["@Status"].Value.ToString();
                }
                connectionValidateMacID.Close();
            }
            Constants.Log.Info("End ValidateMacID()");
            return Status;
        }
        public string ValidateMSIInstallation(string MacID)
        {
            string Status = "S000";
            Constants.Log.Info("Begin ValidateMSIInstallation() " + MacID);

            using (System.Data.SqlClient.SqlConnection conValidateMSIInstallation = CommonDAL.GetConnection())
            {
                conValidateMSIInstallation.Open();
                using (System.Data.SqlClient.SqlCommand commandValidateMSI = new System.Data.SqlClient.SqlCommand())
                {
                    commandValidateMSI.CommandTimeout = commandTimeout;
                    commandValidateMSI.Connection = conValidateMSIInstallation;
                    commandValidateMSI.CommandText = "UspValidateMacId";
                    commandValidateMSI.CommandType = System.Data.CommandType.StoredProcedure;
                    #region Building Parameters
                    commandValidateMSI.Parameters.Add(CommonDAL.BuildSqlParameter("@MACID", System.Data.SqlDbType.NVarChar, 2147483646, "MacID", MacID, System.Data.ParameterDirection.Input));
                    commandValidateMSI.Parameters.Add(CommonDAL.BuildSqlParameter("@Status", System.Data.SqlDbType.NVarChar, 2147483646, "Status", Status, System.Data.ParameterDirection.Output));
                    #endregion
                    commandValidateMSI.ExecuteNonQuery();
                    if (commandValidateMSI != null && commandValidateMSI.Parameters != null && commandValidateMSI.Parameters["@Status"] != null
                        && commandValidateMSI.Parameters["@Status"].Value != null && commandValidateMSI.Parameters["@Status"].Value != DBNull.Value)
                        Status = commandValidateMSI.Parameters["@Status"].Value.ToString();
                }
                conValidateMSIInstallation.Close();
            }
            Constants.Log.Info("End ValidateMSIInstallation()");
            return Status;
        }


        public List<String> FetchLoadedMediaDetails(string MacID)
        {

            Constants.Log.Info("Begin FetchLoadedMediaDetails() " + MacID);
            List<string> lstMedia = new List<string>();
            using (System.Data.SqlClient.SqlConnection conLoadMedia = CommonDAL.GetConnection())
            {
                conLoadMedia.Open();
                using (System.Data.SqlClient.SqlCommand commandLoadMedia = new System.Data.SqlClient.SqlCommand())
                {
                    commandLoadMedia.CommandTimeout = commandTimeout;
                    commandLoadMedia.Connection = conLoadMedia;
                    commandLoadMedia.CommandText = "UspGetLoadedMediaPackageDetails";
                    commandLoadMedia.CommandType = System.Data.CommandType.StoredProcedure;

                    commandLoadMedia.Parameters.AddWithValue("@MACID", MacID);
                    using (System.Data.SqlClient.SqlDataReader reader = commandLoadMedia.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            int Index;
                            while (reader.Read())
                            {
                                Index = reader.GetOrdinal("mediapackagename");
                                if (!reader.IsDBNull(Index))
                                {
                                    string name = reader.GetString(Index);
                                    lstMedia.Add(name);
                                }
                            }
                        }
                    }
                }
                conLoadMedia.Close();
            }
            Constants.Log.Info("End FetchLoadedMediaDetails()");
            return lstMedia;
        }

        /// <summary>
        /// Fetch Test Center Details from Testcenter Table
        /// </summary>
        /// <param name="CenterID">ID</param>
        /// <param name="CenterType">CenterType</param>
        /// <returns>List of Test Center</returns>
        public List<TestCenterDeatil> GetTestCenterDeatils(string CenterID, string CenterType)
        {
            Constants.Log.Info("Begin GetTestCenterDeatils() - MacID: " + CenterID);
            Constants.Log.Info("Center Type: " + CenterType);
            List<TestCenterDeatil> lstUserResponses = new List<TestCenterDeatil>();

            using (System.Data.SqlClient.SqlConnection connection = CommonDAL.GetCOEConnection())
            {
                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand())
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                        connection.Open();
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "UspGetUserSchoolExamCenterVenueDetails";
                    command.Parameters.AddWithValue("@CenterID", CenterID);
                    command.Parameters.AddWithValue("@CenterType", CenterType);
                    using (System.Data.SqlClient.SqlDataAdapter sqlAdap = new SqlDataAdapter(command))
                    {
                        System.Data.DataTable dtUserResposes = new DataTable();
                        sqlAdap.Fill(dtUserResposes);

                        if (dtUserResposes.Rows.Count > 0)
                        {
                            TestCenterDeatil objResponse = null;
                            foreach (DataRow drUserResponse in dtUserResposes.Rows)
                            {
                                objResponse = new Contracts.TestCenterDeatil();
                                objResponse.CenterID = Convert.ToString(drUserResponse["CenterID"]);
                                objResponse.CenterName = Convert.ToString(drUserResponse["CenterName"]);
                                lstUserResponses.Add(objResponse);
                            }
                        }
                    }
                }
                connection.Close();
            }
            return lstUserResponses;
        }

        /// <summary>
        /// Register Test Center to DX db and to main db.
        /// </summary>
        /// <param name="MacID"></param>
        /// <param name="VenueID"></param>
        /// <param name="IpAddress"></param>
        /// <param name="CenterCode"></param>
        /// <param name="CenterTypeId"></param>
        /// <param name="ExamCenterID"></param>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public string TestCenterRegistrationDX(string MacID, int VenueID, string IpAddress, string CenterCode, int CenterTypeId, int ExamCenterID, int SchoolID)
        {
            string Status = "E000";
            Constants.Log.Info("Begin TestCenterRegistration()");
            Constants.Log.Info("MacID: " + MacID);
            Constants.Log.Info("VenueID: " + VenueID);
            Constants.Log.Info("IpAddress: " + IpAddress);
            Constants.Log.Info("CenterCode: " + CenterCode);
            Constants.Log.Info("CenterTypeId: " + CenterTypeId);
            Constants.Log.Info("ExamCenterID: " + ExamCenterID);
            Constants.Log.Info("SchoolID: " + SchoolID);

            using (System.Data.SqlClient.SqlConnection connectionValidateMacID = CommonDAL.GetCOEConnection())
            {
                connectionValidateMacID.Open();
                using (System.Data.SqlClient.SqlCommand commandValidateMacID = new System.Data.SqlClient.SqlCommand())
                {
                    commandValidateMacID.CommandTimeout = commandTimeout;
                    commandValidateMacID.Connection = connectionValidateMacID;
                    commandValidateMacID.CommandText = "UspInsertTestCenterRegistration";
                    commandValidateMacID.CommandType = System.Data.CommandType.StoredProcedure;
                    #region Building Parameters
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@MacID", System.Data.SqlDbType.NVarChar, 2147483646, "MacID", MacID, System.Data.ParameterDirection.Input));
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@VenueID", System.Data.SqlDbType.Int, sizeof(int), "VenueID", VenueID, System.Data.ParameterDirection.Input));
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@IpAddress", System.Data.SqlDbType.NVarChar, 2147483646, "IpAddress", IpAddress, System.Data.ParameterDirection.Input));
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@CenterName", System.Data.SqlDbType.NVarChar, 2147483646, "CenterName", CenterCode, System.Data.ParameterDirection.Input));
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@CenterCode", System.Data.SqlDbType.NVarChar, 2147483646, "CenterCode", CenterCode, System.Data.ParameterDirection.Input));
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@CenterTypeId", System.Data.SqlDbType.Int, sizeof(int), "CenterTypeId", CenterTypeId, System.Data.ParameterDirection.Input));
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@ExamCenterID", System.Data.SqlDbType.Int, sizeof(int), "ExamCenterID", ExamCenterID, System.Data.ParameterDirection.Input));
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@SchoolID", System.Data.SqlDbType.Int, sizeof(int), "SchoolID", SchoolID, System.Data.ParameterDirection.Input));
                    commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@Status", System.Data.SqlDbType.NVarChar, 2147483646, "Status", Status, System.Data.ParameterDirection.Output));
                    #endregion

                    commandValidateMacID.ExecuteNonQuery();
                    if (commandValidateMacID != null && commandValidateMacID.Parameters != null && commandValidateMacID.Parameters["@Status"] != null
                        && commandValidateMacID.Parameters["@Status"].Value != null && commandValidateMacID.Parameters["@Status"].Value != DBNull.Value)
                        Status = commandValidateMacID.Parameters["@Status"].Value.ToString();
                }
                connectionValidateMacID.Close();
            }
            Constants.Log.Info("End TestCenterRegistration()");
            return Status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StorageConfiguration GetS3Configuration()
        {
            StorageConfiguration objResponse = null;
            try
            {
                using (System.Data.SqlClient.SqlConnection Connection = CommonDAL.GetCOEConnection())
                {
                    if (Connection.State == ConnectionState.Closed)
                        Connection.Open();
                    Constants.Log.Info("Conn:" + Connection.ConnectionString);
                    using (System.Data.SqlClient.SqlCommand Command = new SqlCommand())
                    {
                        Command.Connection = Connection;
                        Command.CommandText = "dbo.uspgeturlconfigurationsdetail";
                        Command.Parameters.AddWithValue("@ApplicationTypeName", "eAssessment");
                        Command.Parameters.AddWithValue("@ApplicationModuleCode", "itemauthoring");
                        Command.Parameters.AddWithValue("@ProjectCode", "SEAB");
                        Command.CommandType = CommandType.StoredProcedure;
                        using (System.Data.SqlClient.SqlDataAdapter sqlAdap = new SqlDataAdapter(Command))
                        {

                            DataTable dtConfig = new DataTable();
                            sqlAdap.Fill(dtConfig);
                            if (dtConfig != null && dtConfig.Rows.Count > 0)
                            {
                                objResponse = JsonConvert.DeserializeObject<StorageConfiguration>(dtConfig.Rows[0]["URLINFO"].ToString());
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Constants.Log.Info("Error in GetS3Configuration()");
            }
            return objResponse;
        }

    } 
    

}

