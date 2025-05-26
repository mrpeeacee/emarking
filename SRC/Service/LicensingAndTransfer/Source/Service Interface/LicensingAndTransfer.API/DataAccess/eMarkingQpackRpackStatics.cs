using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LicensingAndTransfer.API
{
    public class eMarkingQpackRpackStatics
    {
        private int commandTimeout = 0;

        public eMarkingQpackRpackStatics()
        {
            commandTimeout = 2147483647;
        }

        public List<eMarkingSyncUserResponse> eMarkingQRLpackStatics()
        {
            List<eMarkingSyncUserResponse> eMarkingSyncUserResponses = new List<eMarkingSyncUserResponse>();
            string Status = String.Empty;
            eMarkingSyncUserResponse objResponse = new eMarkingSyncUserResponse();
            try
            {
                Constants.Log.Info("Begin eMarkingQRLpackStatics()");
                DataTable scheduledetails = new DataTable();
                DataTable scheduleCountsAndXMLDatafromTA = new DataTable();
                DataTable scheduleCountsfromeMarking = new DataTable();
                DataTable filteredscheduleCountsAndXMLDatafromTA = new DataTable();
                DataRow filteredscheduleCountsfromeMarking = null;
                DataTable questionxml = new DataTable();
                DataTable testxml = new DataTable();
                SqlDataReader dataReader;
                SqlDataReader sqldr;
                StringBuilder sbstatus = new StringBuilder();

                // Insert ScheduleDetails To CourseMovement table(eMarking table)
                using (System.Data.SqlClient.SqlConnection connectioneMarking = CommonDAL.GeteMarkingConnection())
                {
                    Constants.Log.Info("Begin to Insert ScheduleDetails To CourseMovement table");
                    sbstatus.Append("Begin to Insert ScheduleDetails To CourseMovement table, ");
                    connectioneMarking.Open();
                    using (System.Data.SqlClient.SqlCommand commandSynceMarkingUser = new SqlCommand("[Marking].[USPInsertScheduleDetailsToCourseMovement]"))
                    {
                        commandSynceMarkingUser.CommandTimeout = commandTimeout;
                        commandSynceMarkingUser.Connection = connectioneMarking;
                        commandSynceMarkingUser.CommandType = CommandType.StoredProcedure;
                        System.Data.SqlClient.SqlDataAdapter adap = new SqlDataAdapter(commandSynceMarkingUser);
                        adap.Fill(scheduledetails);
                    }
                    connectioneMarking.Close();
                }

                eMarkingSyncUserResponses.Add(objResponse);
                Constants.Log.Info("End of Insert ScheduleDetails To CourseMovement table in the eMarking DB" + " Response: " + scheduledetails);
                sbstatus.Append("End of Insert ScheduleDetails To CourseMovement table in the eMarking DB, ");

                if (scheduledetails != null)
                {
                    //Get Schedule Counts And XML Data from T&A table
                    SqlParameter sqlParameter;
                    using (System.Data.SqlClient.SqlConnection connectionseab = CommonDAL.GetSEABConnection())
                    {
                        using (System.Data.SqlClient.SqlCommand commandseab = new SqlCommand("[Marking].[UspGetScheduleCountsAndXMLData]", connectionseab))
                        {
                            Constants.Log.Info("Begin to Get Schedule Counts And XML Data from T&A table");
                            sbstatus.Append("Begin to Get Schedule Counts And XML Data from T&A table, ");
                            commandseab.CommandType = CommandType.StoredProcedure;
                            commandseab.Parameters.Clear();
                            connectionseab.Open();
                            sqlParameter = commandseab.Parameters.AddWithValue("@UDTScheduleInfo", scheduledetails);
                            sqlParameter.SqlDbType = SqlDbType.Structured;
                            sqlParameter.TypeName = "Marking.UDTScheduleInfo";

                            // Fill result set to datatable
                            dataReader = commandseab.ExecuteReader();

                            if (dataReader.HasRows)
                            {
                                scheduleCountsAndXMLDatafromTA.Load(dataReader);
                                questionxml.Load(dataReader);
                                testxml.Load(dataReader);
                            }
                            if (!dataReader.IsClosed) { dataReader.Close(); }
                        }
                        connectionseab.Close();
                        Constants.Log.Info("End of Get Schedule Counts And XML Data from T&A table" + " Response: " + scheduleCountsAndXMLDatafromTA);
                        sbstatus.Append("End of Get Schedule Counts And XML Data from T&A table, ").Append(scheduleCountsAndXMLDatafromTA);
                    }

                    //Get Schedule Counts And XML Data from eMarking table
                    SqlParameter sqlParameterr;

                    using (System.Data.SqlClient.SqlConnection connectioneMarkingUser = CommonDAL.GeteMarkingConnection())
                    {
                        using (System.Data.SqlClient.SqlCommand commandeMarkingUser = new SqlCommand("[Marking].[UspGetScheduleCounts]", connectioneMarkingUser))
                        {
                            Constants.Log.Info("Begin to Get Schedule Counts And XML Data from eMarking table");
                            sbstatus.Append("Begin to Get Schedule Counts And XML Data from eMarking table, ");
                            commandeMarkingUser.CommandType = CommandType.StoredProcedure;
                            commandeMarkingUser.Parameters.Clear();
                            connectioneMarkingUser.Open();
                            sqlParameterr = commandeMarkingUser.Parameters.AddWithValue("@UDTScheduleInfo", scheduledetails);
                            sqlParameterr.SqlDbType = SqlDbType.Structured;
                            sqlParameterr.TypeName = "Marking.UDTScheduleInfo";

                            // Fill result set to datatable
                            sqldr = commandeMarkingUser.ExecuteReader();
                            if (sqldr.HasRows)
                            {
                                scheduleCountsfromeMarking.Load(sqldr);
                            }
                            if (!sqldr.IsClosed) { sqldr.Close(); }
                        }
                        connectioneMarkingUser.Close();
                        Constants.Log.Info("End of Get Schedule Counts And XML Data from eMarking table" + " Response: " + scheduleCountsfromeMarking);
                        sbstatus.Append("End of Get Schedule Counts And XML Data from eMarking table, ").Append(scheduleCountsAndXMLDatafromTA);
                    }
                }

                // comparing counts in the delivery and emarking are equal
                if (scheduleCountsAndXMLDatafromTA.Rows.Count > 0 && scheduleCountsfromeMarking.Rows.Count > 0)
                {
                    Constants.Log.Info("Begin to comparing counts in the delivery and emarking datatables");
                    sbstatus.Append("Begin to comparing counts in the delivery and emarking datatables, ");
                    var IsCloseExamData = scheduleCountsAndXMLDatafromTA.AsEnumerable().Where(a => a.Field<bool>("IsCloseExam"));
                    if (IsCloseExamData != null && IsCloseExamData.Any())
                    {
                        filteredscheduleCountsAndXMLDatafromTA = IsCloseExamData.CopyToDataTable();
                    }

                    if (filteredscheduleCountsAndXMLDatafromTA.Rows.Count > 0)
                    {
                        foreach (DataRow taxml in filteredscheduleCountsAndXMLDatafromTA.Rows)
                        {
                            filteredscheduleCountsfromeMarking = scheduleCountsfromeMarking.AsEnumerable().Where(s => s.Field<long>("ScheduleID") == taxml.Field<long>("ScheduleID")).Select(d => d).First();

                            if (taxml.Field<long>("ScheduleID") == filteredscheduleCountsfromeMarking.Field<long>("ScheduleID") &&
                                taxml.Field<int>("QPackCount") == filteredscheduleCountsfromeMarking.Field<int>("QPackCount") &&
                                taxml.Field<int>("LPackCount") == filteredscheduleCountsfromeMarking.Field<int>("LPackCount") &&
                                taxml.Field<int>("RPackCount") == filteredscheduleCountsfromeMarking.Field<int>("RPackCount") &&
                                taxml.Field<bool>("IsCloseExam") == filteredscheduleCountsfromeMarking.Field<bool>("IsCloseExam") &&
                                taxml.Field<int>("SCheduleDetailCount") == filteredscheduleCountsfromeMarking.Field<int>("SCheduleDetailCount") &&
                                taxml.Field<int>("ScheduleUserCount") == filteredscheduleCountsfromeMarking.Field<int>("ScheduleUserCount") &&
                                taxml.Field<int>("UserResponseCount") == filteredscheduleCountsfromeMarking.Field<int>("UserResponseCount") &&
                                taxml.Field<int>("NoOfUsersExamTaken") == filteredscheduleCountsfromeMarking.Field<int>("NoOfUsersExamTaken"))
                            {
                                // If equal update IsReadyForEmarkingProcess field(GeteMarkingConnection)
                                using (System.Data.SqlClient.SqlConnection connectioneMarkingUser = CommonDAL.GeteMarkingConnection())
                                {
                                    using (System.Data.SqlClient.SqlCommand commandeMarkingUser = new SqlCommand("UPDATE Marking.CourseMovementValidation SET IsReadyForEmarkingProcess = @IsReadyForEmarkingProcess,IsExamClosed = @IsExamClosed,IsJobRunRequired = @IsJobRunRequired,JobStatus = @JobStatus,JobRunDate = @JobRunDate WHERE ScheduleID=@ScheduleID", connectioneMarkingUser))
                                    {
                                        Constants.Log.Info("Begin to Update IsReadyForEmarkingProcess to eMarking table");
                                        sbstatus.Append("Begin to Update IsReadyForEmarkingProcess to eMarking table, ");
                                        commandeMarkingUser.CommandType = CommandType.Text;
                                        commandeMarkingUser.Parameters.Clear();
                                        connectioneMarkingUser.Open();
                                        commandeMarkingUser.Parameters.AddWithValue("@IsReadyForEmarkingProcess", true);
                                        commandeMarkingUser.Parameters.AddWithValue("@IsExamClosed", taxml.Field<bool>("IsCloseExam"));
                                        commandeMarkingUser.Parameters.AddWithValue("@IsJobRunRequired", false);
                                        commandeMarkingUser.Parameters.AddWithValue("@JobStatus", "Counts are matched");
                                        commandeMarkingUser.Parameters.AddWithValue("@JobRunDate", DateTime.UtcNow);
                                        commandeMarkingUser.Parameters.AddWithValue("@ScheduleID", filteredscheduleCountsfromeMarking.Field<long>("ScheduleID"));
                                        int a = commandeMarkingUser.ExecuteNonQuery();
                                        if (a == 1)
                                        {
                                            Status = "U001";
                                        }

                                        // update/insert question xml
                                        if (questionxml != null)
                                        {
                                            foreach (DataRow qnsxml in questionxml.Rows)
                                            {
                                                SqlCommand qnsxmlcheckcmd = new SqlCommand(
                                                  "SELECT QuestionID FROM dbo.QuestionXML WHERE QuestionID = @QuestionID",
                                                  connectioneMarkingUser);
                                                qnsxmlcheckcmd.Parameters.AddWithValue("@QuestionID", qnsxml.Field<long>("QuestionID"));
                                                qnsxmlcheckcmd.ExecuteNonQuery();
                                                int QuestionId = Convert.ToInt32(qnsxmlcheckcmd.ExecuteScalar());

                                                // If QuestionId exists update QuestionXML else insert
                                                if (QuestionId > 0)
                                                {
                                                    Constants.Log.Info("Begin to update QuestionXML to emarking datatables,");
                                                    sbstatus.Append("Begin to update QuestionXML to emarking datatables " + ' ').Append(QuestionId + ',');
                                                    SqlCommand qnsxmlupdatecmd = new SqlCommand("UPDATE dbo.QuestionXML SET QuestionXML = @QuestionXML WHERE QuestionID = @QuestionID", connectioneMarkingUser);
                                                    qnsxmlupdatecmd.Parameters.AddWithValue("@QuestionXML", qnsxml.Field<string>("QuestionXML"));
                                                    qnsxmlupdatecmd.Parameters.AddWithValue("@QuestionID", QuestionId);
                                                    qnsxmlupdatecmd.ExecuteNonQuery();
                                                    Constants.Log.Info("End of update QuestionXML to emarking datatables,QuestionID:" + QuestionId);
                                                    sbstatus.Append("Begin to update QuestionXML to emarking datatables " + ' ').Append(QuestionId + ',');
                                                }
                                                else
                                                {
                                                    Constants.Log.Info("Begin to insert QuestionXML to emarking datatables");
                                                    sbstatus.Append("Begin to insert QuestionXML to emarking datatables " + ' ').Append(QuestionId + ',');
                                                    SqlCommand qnsxmlinsertcmd = new SqlCommand("insert into dbo.QuestionXML(QuestionID,QuestionXML) values (@QuestionID, @QuestionXML)", connectioneMarkingUser);
                                                    qnsxmlinsertcmd.Parameters.AddWithValue("@QuestionXML", qnsxml.Field<string>("QuestionXML"));
                                                    qnsxmlinsertcmd.Parameters.AddWithValue("@QuestionID", qnsxml.Field<long>("QuestionID"));
                                                    qnsxmlinsertcmd.ExecuteNonQuery();
                                                    Constants.Log.Info("End of insert QuestionXML to emarking datatables,QuestionID:" + qnsxml.Field<long>("QuestionID"));
                                                    sbstatus.Append("End of insert QuestionXML to emarking datatables " + ' ').Append(qnsxml.Field<long>("QuestionID") + ',');
                                                }

                                                //Updating QuestionXML to ProjectQuestions table
                                                Constants.Log.Info("Begin to Update QuestionXML to ProjectQuestions table of emarking,");
                                                sbstatus.Append("Begin to Update QuestionXML to ProjectQuestions table of emarking " + ' ').Append(filteredscheduleCountsfromeMarking.Field<long>("ScheduleID") + ',');
                                                SqlCommand qnsxmlpqcmd = new SqlCommand("UPDATE PQ SET PQ.QuestionXML = Q.QuestionXML FROM Marking.ProjectQuestions PQ INNER JOIN dbo.QuestionXML Q ON Q.QuestionID = PQ.QUESTIONID INNER JOIN Marking.CourseMovementValidation CMV ON CMV.ProjectID = PQ.ProjectID WHERE CMV.ScheduleID = @ScheduleID", connectioneMarkingUser);
                                                qnsxmlpqcmd.Parameters.AddWithValue("@ScheduleID", filteredscheduleCountsfromeMarking.Field<long>("ScheduleID"));
                                                qnsxmlpqcmd.ExecuteNonQuery();
                                                Constants.Log.Info("End of Update QuestionXML to ProjectQuestions table of emarking,QuestionID:" + filteredscheduleCountsfromeMarking.Field<long>("ScheduleID"));
                                                sbstatus.Append("End of Update QuestionXML to ProjectQuestions table of emarking " + ' ').Append(filteredscheduleCountsfromeMarking.Field<long>("ScheduleID") + ',');
                                            }
                                        }

                                        // update/insert test xml
                                        if (testxml != null)
                                        {
                                            foreach (DataRow txml in testxml.Rows)
                                            {
                                                SqlCommand testxmlcheckcmd = new SqlCommand(
                                                  "SELECT TestID FROM dbo.TestXML WHERE TestID = @TestID and Version = @Version and IsDeleted = @IsDeleted",
                                                  connectioneMarkingUser);
                                                testxmlcheckcmd.Parameters.AddWithValue("@TestID", txml.Field<long>("TestID"));
                                                testxmlcheckcmd.Parameters.AddWithValue("@Version", txml.Field<decimal>("Version"));
                                                testxmlcheckcmd.Parameters.AddWithValue("@IsDeleted", false);
                                                testxmlcheckcmd.ExecuteNonQuery();
                                                int testId = Convert.ToInt32(testxmlcheckcmd.ExecuteScalar());

                                                // If testId exists update QuestionXML else insert
                                                if (testId > 0)
                                                {
                                                    Constants.Log.Info("Begin to update testxml to emarking datatables");
                                                    sbstatus.Append("Begin to update testxml to emarking datatables, ").Append(testId);
                                                    SqlCommand txmlupdatecmd = new SqlCommand("UPDATE dbo.TestXML SET TestXML = @TestXML WHERE TestID = @TestID and Version = @Version", connectioneMarkingUser);
                                                    txmlupdatecmd.Parameters.AddWithValue("@TestXML", txml.Field<string>("TestXML"));
                                                    txmlupdatecmd.Parameters.AddWithValue("@TestID", testId);
                                                    txmlupdatecmd.Parameters.AddWithValue("@Version", txml.Field<decimal>("Version"));
                                                    txmlupdatecmd.ExecuteNonQuery();
                                                    Constants.Log.Info("End of insert testxml to emarking datatables,TestID:" + testId);
                                                    sbstatus.Append("End of insert testxml to emarking datatables " + ' ').Append(testId + ',');
                                                }
                                                else
                                                {
                                                    Constants.Log.Info("Begin to update testxml to emarking datatables");
                                                    sbstatus.Append("Begin to update testxml to emarking datatables ,").Append(txml.Field<long>("TestID"));
                                                    SqlCommand txmlinsertcmd = new SqlCommand("insert into dbo.TestXML(TestID,TestXML,LanguageID,Version) values (@TestID, @TestXML,@LanguageID,@Version)", connectioneMarkingUser);
                                                    txmlinsertcmd.Parameters.AddWithValue("@TestXML", txml.Field<string>("TestXML"));
                                                    txmlinsertcmd.Parameters.AddWithValue("@TestID", txml.Field<long>("TestID"));
                                                    txmlinsertcmd.Parameters.AddWithValue("@Version", txml.Field<decimal>("Version"));
                                                    txmlinsertcmd.Parameters.AddWithValue("@LanguageID", txml.Field<long>("LanguageID"));
                                                    txmlinsertcmd.ExecuteNonQuery();
                                                    Constants.Log.Info("End of insert testxml to emarking datatables,TestID:" + txml.Field<long>("TestID"));
                                                    sbstatus.Append("End of insert testxml to emarking datatables " + ' ').Append(txml.Field<long>("TestID") + ',');
                                                }
                                            }
                                        }
                                    }
                                    connectioneMarkingUser.Close();
                                }
                                Constants.Log.Info("End comparing counts in the delivery and emarking data tables" + "EqualCounts: " + filteredscheduleCountsfromeMarking);
                                sbstatus.Append("End comparing counts in the delivery and emarking data tables " + ',');
                            }
                            else
                            {
                                using (System.Data.SqlClient.SqlConnection connectioneMarkingUser = CommonDAL.GeteMarkingConnection())
                                {
                                    using (System.Data.SqlClient.SqlCommand commandeMarkingUser = new SqlCommand("UPDATE Marking.CourseMovementValidation SET IsJobRunRequired = @IsJobRunRequired,JobRunDate = @JobRunDate,JobStatus = @JobStatus, IsExamClosed = @IsExamClosed WHERE ScheduleID=@ScheduleID", connectioneMarkingUser))
                                    {
                                        Constants.Log.Info("Begin to Update IsJobRunRequired to eMarking table");
                                        sbstatus.Append("Begin to Update IsReadyForEmarkingProcess to eMarking table ,");
                                        commandeMarkingUser.CommandType = CommandType.Text;
                                        commandeMarkingUser.Parameters.Clear();
                                        connectioneMarkingUser.Open();
                                        commandeMarkingUser.Parameters.AddWithValue("@IsJobRunRequired", true);
                                        commandeMarkingUser.Parameters.AddWithValue("@JobRunDate", DateTime.UtcNow);
                                        commandeMarkingUser.Parameters.AddWithValue("@JobStatus", "Counts not matched");
                                        commandeMarkingUser.Parameters.AddWithValue("@IsExamClosed", true);
                                        commandeMarkingUser.Parameters.AddWithValue("@ScheduleID", filteredscheduleCountsfromeMarking.Field<long>("ScheduleID"));
                                        int a = commandeMarkingUser.ExecuteNonQuery();
                                        if (a == 1)
                                        {
                                            Status = "U001";
                                        }
                                    }
                                    connectioneMarkingUser.Close();
                                    Constants.Log.Info("End Update IsJobRunRequired to eMarking table" + "Sataus: " + Status);
                                    sbstatus.Append("End Update IsJobRunRequired to eMarking table " + ' ').Append(Status + ',');
                                }
                            }
                        }
                    }
                }
                objResponse.Status = Status;
                objResponse.Message = sbstatus;
                objResponse.ResponseData = JsonConvert.SerializeObject(scheduledetails);
            }
            catch (Exception ex)
            {
                Constants.Log.Error(ex.Message, ex);
                Status = "Error Found";
                objResponse.Status = Status;
            }
            return eMarkingSyncUserResponses;
        }
    }
}