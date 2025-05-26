using AESCryptography;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LicensingAndTransfer.ServiceImplementation
{
    public class CommonDAL : Generic
    {
        private int commandTimeout = 0;

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
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["connectionString"].ToString();
            //  Decrypting connection string
            connectionString = AESCrypto.Decrypt(connectionString);
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        public static System.Data.SqlClient.SqlConnection GetConnectionforReportingServer()
        {
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["connectionStringforReportingServer"].ToString();
            //  Decrypting connection string
            connectionString = AESCrypto.Decrypt(connectionString);
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        public void UpdateBatchEndTime(System.Collections.Generic.List<DataContracts.Batch> listBatch)
        {
            Log.LogInfo("Begin UpdateBatchEndTime() - Batch(s) Count : " + (listBatch == null ? "0" : listBatch.Count.ToString()));
            if (listBatch != null && listBatch.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                {
                    #region Building DailyScheduleSummaryForUser DataTable

                    System.Data.DataTable datatableBatch = new System.Data.DataTable();
                    datatableBatch.Columns.Add("ScheduleDetailID");
                    datatableBatch.Columns.Add("UserID");
                    datatableBatch.Columns.Add("TestCenterID");
                    datatableBatch.Columns.Add("PreviousEndDate");
                    datatableBatch.Columns.Add("PresentEndDate");
                    datatableBatch.Columns.Add("IsNotified");

                    #endregion Building DailyScheduleSummaryForUser DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterBatch = new System.Data.SqlClient.SqlDataAdapter("PersistExtendBatchDetailsToDX", ConnectionBatch))
                    {
                        foreach (DataContracts.Batch objPersitstBatch in listBatch)
                        {
                            System.Data.DataRow datarowBatch = datatableBatch.NewRow();

                            #region Binding Values to Batch DataTable

                            datarowBatch["ScheduleDetailID"] = objPersitstBatch.ScheduleDetailID;
                            datarowBatch["UserID"] = objPersitstBatch.UserID;
                            datarowBatch["TestCenterID"] = objPersitstBatch.TestCenterID;
                            if (objPersitstBatch.PreviousEndDate != null && objPersitstBatch.PreviousEndDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowBatch["PreviousEndDate"] = System.DBNull.Value;
                            else
                                datarowBatch["PreviousEndDate"] = objPersitstBatch.PreviousEndDate;
                            if (objPersitstBatch.PresentEndDate != null && objPersitstBatch.PresentEndDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowBatch["PresentEndDate"] = System.DBNull.Value;
                            else
                                datarowBatch["PresentEndDate"] = objPersitstBatch.PresentEndDate;
                            datarowBatch["IsNotified"] = true;

                            #endregion Binding Values to Batch DataTable

                            datatableBatch.Rows.Add(datarowBatch);
                        }
                        dataAdapterBatch.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterBatch.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterBatch.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterBatch.UpdateBatchSize = 50;
                        dataAdapterBatch.InsertCommand.Connection = ConnectionBatch;
                        dataAdapterBatch.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterBatch.InsertCommand.CommandText = "PersistExtendBatchDetailsToDX";

                        #region Binding Command Parameters to stored procedure from DataTable

                        dataAdapterBatch.InsertCommand.Parameters.Add("@ScheduleDetailID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleDetailID");
                        dataAdapterBatch.InsertCommand.Parameters.Add("@UserID", System.Data.SqlDbType.NVarChar, 2147483646, "UserID");
                        dataAdapterBatch.InsertCommand.Parameters.Add("@TestCenterID", System.Data.SqlDbType.BigInt, sizeof(Int64), "TestCenterID");
                        dataAdapterBatch.InsertCommand.Parameters.Add("@PreviousEndDate", System.Data.SqlDbType.DateTime, 200, "PreviousEndDate");
                        dataAdapterBatch.InsertCommand.Parameters.Add("@PresentEndDate", System.Data.SqlDbType.DateTime, 200, "PresentEndDate");
                        dataAdapterBatch.InsertCommand.Parameters.Add("@IsNotified", System.Data.SqlDbType.Bit, sizeof(bool), "IsNotified");

                        #endregion Binding Command Parameters to stored procedure from DataTable

                        dataAdapterBatch.Update(datatableBatch);
                    }
                }
            }
            Log.LogInfo("End UpdateBatchEndTime()");
        }

        public void PersistDailyScheduleSummary(System.Collections.Generic.List<DataContracts.PersistDailyScheduleSummary> listPersistDailyScheduleSummary)
        {
            Log.LogInfo("Begin PersistDailyScheduleSummary() - Package(s) Count : " + (listPersistDailyScheduleSummary == null ? "0" : listPersistDailyScheduleSummary.Count.ToString()));
            if (listPersistDailyScheduleSummary != null && listPersistDailyScheduleSummary.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionDailyScheduleSummary = CommonDAL.GetConnection())
                {
                    #region Building DailyScheduleSummary DataTable

                    System.Data.DataTable datatableDailyScheduleSummary = new System.Data.DataTable();
                    datatableDailyScheduleSummary.Columns.Add("ScheduleDetailID");
                    datatableDailyScheduleSummary.Columns.Add("ProductID");
                    datatableDailyScheduleSummary.Columns.Add("TestCenterID");
                    datatableDailyScheduleSummary.Columns.Add("NoofCandidatesInBatch");
                    datatableDailyScheduleSummary.Columns.Add("NoofCandidatesRegistered");
                    datatableDailyScheduleSummary.Columns.Add("NumberOfCandidateStartedExam");
                    datatableDailyScheduleSummary.Columns.Add("NumberOfCandidateCompletedExam");
                    datatableDailyScheduleSummary.Columns.Add("FirstPersonStartedTime");
                    datatableDailyScheduleSummary.Columns.Add("FirstPersonClosedTime");
                    datatableDailyScheduleSummary.Columns.Add("LastPersonStartedTime");
                    datatableDailyScheduleSummary.Columns.Add("LastPersonClosedTime");
                    datatableDailyScheduleSummary.Columns.Add("BatchID");
                    datatableDailyScheduleSummary.Columns.Add("ScheduleID");
                    datatableDailyScheduleSummary.Columns.Add("TestStartDate");
                    datatableDailyScheduleSummary.Columns.Add("TestEndDate");

                    #endregion Building DailyScheduleSummary DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterDailyScheduleSummary = new System.Data.SqlClient.SqlDataAdapter("PersistSummary_DailySchedules", connectionDailyScheduleSummary))
                    {
                        foreach (DataContracts.PersistDailyScheduleSummary objPersistDailyScheduleSummary in listPersistDailyScheduleSummary)
                        {
                            System.Data.DataRow datarowDailyScheduleSummary = datatableDailyScheduleSummary.NewRow();

                            #region Binding Values to DailyScheduleSummary DataTable

                            datarowDailyScheduleSummary["ScheduleDetailID"] = objPersistDailyScheduleSummary.ScheduleDetailID;
                            datarowDailyScheduleSummary["ProductID"] = objPersistDailyScheduleSummary.ProductID;
                            datarowDailyScheduleSummary["TestCenterID"] = objPersistDailyScheduleSummary.TestCenterID;
                            datarowDailyScheduleSummary["NoofCandidatesInBatch"] = objPersistDailyScheduleSummary.NoofCandidatesInBatch;
                            datarowDailyScheduleSummary["NoofCandidatesRegistered"] = objPersistDailyScheduleSummary.NoofCandidatesRegistered;
                            datarowDailyScheduleSummary["NumberOfCandidateStartedExam"] = objPersistDailyScheduleSummary.NumberOfCandidateStartedExam;
                            datarowDailyScheduleSummary["NumberOfCandidateCompletedExam"] = objPersistDailyScheduleSummary.NumberOfCandidateCompletedExam;
                            if (objPersistDailyScheduleSummary.FirstPersonStartedTime != null && objPersistDailyScheduleSummary.FirstPersonStartedTime == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowDailyScheduleSummary["FirstPersonStartedTime"] = System.DBNull.Value;
                            else
                                datarowDailyScheduleSummary["FirstPersonStartedTime"] = objPersistDailyScheduleSummary.FirstPersonStartedTime;

                            if (objPersistDailyScheduleSummary.FirstPersonClosedTime != null && objPersistDailyScheduleSummary.FirstPersonClosedTime == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowDailyScheduleSummary["FirstPersonClosedTime"] = System.DBNull.Value;
                            else
                                datarowDailyScheduleSummary["FirstPersonClosedTime"] = objPersistDailyScheduleSummary.FirstPersonClosedTime;
                            if (objPersistDailyScheduleSummary.LastPersonStartedTime != null && objPersistDailyScheduleSummary.LastPersonStartedTime == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowDailyScheduleSummary["LastPersonStartedTime"] = System.DBNull.Value;
                            else
                                datarowDailyScheduleSummary["LastPersonStartedTime"] = objPersistDailyScheduleSummary.LastPersonStartedTime;

                            if (objPersistDailyScheduleSummary.LastPersonClosedTime != null && objPersistDailyScheduleSummary.LastPersonClosedTime == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowDailyScheduleSummary["LastPersonClosedTime"] = System.DBNull.Value;
                            else
                                datarowDailyScheduleSummary["LastPersonClosedTime"] = objPersistDailyScheduleSummary.LastPersonClosedTime;
                            datarowDailyScheduleSummary["BatchID"] = objPersistDailyScheduleSummary.BatchID;
                            datarowDailyScheduleSummary["ScheduleID"] = objPersistDailyScheduleSummary.ScheduleID;
                            datarowDailyScheduleSummary["TestStartDate"] = objPersistDailyScheduleSummary.TestStartDate;
                            datarowDailyScheduleSummary["TestEndDate"] = objPersistDailyScheduleSummary.TestEndDate;

                            #endregion Binding Values to DailyScheduleSummary DataTable

                            datatableDailyScheduleSummary.Rows.Add(datarowDailyScheduleSummary);
                            PersistDailyScheduleForUser(objPersistDailyScheduleSummary.BatchID, objPersistDailyScheduleSummary.ScheduleDetailID, objPersistDailyScheduleSummary.UserList);
                        }
                        dataAdapterDailyScheduleSummary.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterDailyScheduleSummary.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterDailyScheduleSummary.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterDailyScheduleSummary.UpdateBatchSize = 50;
                        dataAdapterDailyScheduleSummary.InsertCommand.CommandText = "PersistSummary_DailySchedules";
                        dataAdapterDailyScheduleSummary.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterDailyScheduleSummary.InsertCommand.Connection = connectionDailyScheduleSummary;

                        #region Binding Command Parameters to stored procedure from PersistSummary_DailySchedules DataTable

                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@ScheduleDetailID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleDetailID");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@ProductID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ProductID");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@TestCenterID", System.Data.SqlDbType.BigInt, sizeof(Int64), "TestCenterID");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@NoofCandidatesInBatch", System.Data.SqlDbType.Int, sizeof(Int32), "NoofCandidatesInBatch");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@NoofCandidatesRegistered", System.Data.SqlDbType.Int, sizeof(Int32), "NoofCandidatesRegistered");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@NumberOfCandidateStartedExam", System.Data.SqlDbType.Int, sizeof(Int32), "NumberOfCandidateStartedExam");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@NumberOfCandidateCompletedExam", System.Data.SqlDbType.Int, sizeof(Int32), "NumberOfCandidateCompletedExam");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@FirstPersonStartedTime", System.Data.SqlDbType.DateTime, 20, "FirstPersonStartedTime");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@FirstPersonClosedTime", System.Data.SqlDbType.DateTime, 20, "FirstPersonClosedTime");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@LastPersonStartedTime", System.Data.SqlDbType.DateTime, 20, "LastPersonStartedTime");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@LastPersonClosedTime", System.Data.SqlDbType.DateTime, 20, "LastPersonClosedTime");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@BatchID", System.Data.SqlDbType.BigInt, sizeof(Int64), "BatchID");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@ScheduleID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleID");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@TestStartDate", System.Data.SqlDbType.DateTime, 20, "TestStartDate");
                        dataAdapterDailyScheduleSummary.InsertCommand.Parameters.Add("@TestEndDate", System.Data.SqlDbType.DateTime, 20, "TestEndDate");

                        #endregion Binding Command Parameters to stored procedure from PersistSummary_DailySchedules DataTable

                        dataAdapterDailyScheduleSummary.Update(datatableDailyScheduleSummary);
                    }
                }
            }
            Log.LogInfo("End PersistDailyScheduleSummary()");
        }

        public void PersistDailyScheduleForUser(Int64 BatchID, Int64 ScheduleDetailID, System.Collections.Generic.List<DataContracts.PersistDailyScheduleSummaryForUser> listPersistDailyscheduleSummaryForUser)
        {
            Log.LogInfo("Begin PersistDailyScheduleSummaryForUser() - User(s) Count : " + (listPersistDailyscheduleSummaryForUser == null ? "0" : listPersistDailyscheduleSummaryForUser.Count.ToString()));
            if (listPersistDailyscheduleSummaryForUser != null && listPersistDailyscheduleSummaryForUser.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection ConnectionPersistDailySummaryForUser = CommonDAL.GetConnection())
                {
                    #region Building DailyScheduleSummaryForUser DataTable

                    System.Data.DataTable datatableDailyScheduleForUser = new System.Data.DataTable();
                    datatableDailyScheduleForUser.Columns.Add("ScheduleDetailID");
                    datatableDailyScheduleForUser.Columns.Add("LoginName");
                    datatableDailyScheduleForUser.Columns.Add("UserResponseCount");
                    datatableDailyScheduleForUser.Columns.Add("StartedDateTime");
                    datatableDailyScheduleForUser.Columns.Add("UserStatus");
                    datatableDailyScheduleForUser.Columns.Add("EndDateTime");
                    datatableDailyScheduleForUser.Columns.Add("TimeRemaining");
                    datatableDailyScheduleForUser.Columns.Add("ScheduleUserID");
                    datatableDailyScheduleForUser.Columns.Add("IsRegistered");
                    datatableDailyScheduleForUser.Columns.Add("Responses");
                    datatableDailyScheduleForUser.Columns.Add("ProductID");
                    datatableDailyScheduleForUser.Columns.Add("TestCenterID");
                    datatableDailyScheduleForUser.Columns.Add("BatchID");
                    datatableDailyScheduleForUser.Columns.Add("SubmittedDate");
                    datatableDailyScheduleForUser.Columns.Add("TestStartDate");
                    datatableDailyScheduleForUser.Columns.Add("TestEndDate");

                    #endregion Building DailyScheduleSummaryForUser DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterDailySchedleSummaryForUser = new System.Data.SqlClient.SqlDataAdapter("PersistSummary_DailySchedulesForUser", ConnectionPersistDailySummaryForUser))
                    {
                        foreach (DataContracts.PersistDailyScheduleSummaryForUser objPersitstDailyScheduleSummaryForUser in listPersistDailyscheduleSummaryForUser)
                        {
                            System.Data.DataRow datarowDailyScheduleSummaryForUser = datatableDailyScheduleForUser.NewRow();

                            #region Binding Values to DailyScheduleSummaryForUser DataTable

                            datarowDailyScheduleSummaryForUser["ScheduleDetailID"] = ScheduleDetailID;
                            datarowDailyScheduleSummaryForUser["LoginName"] = objPersitstDailyScheduleSummaryForUser.LoginName;
                            datarowDailyScheduleSummaryForUser["UserResponseCount"] = objPersitstDailyScheduleSummaryForUser.UserResonseCount;
                            if (objPersitstDailyScheduleSummaryForUser.StartedDateTime != null && objPersitstDailyScheduleSummaryForUser.StartedDateTime == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowDailyScheduleSummaryForUser["StartedDateTime"] = System.DBNull.Value;
                            else
                                datarowDailyScheduleSummaryForUser["StartedDateTime"] = objPersitstDailyScheduleSummaryForUser.StartedDateTime;
                            datarowDailyScheduleSummaryForUser["UserStatus"] = objPersitstDailyScheduleSummaryForUser.UserStatus;
                            if (objPersitstDailyScheduleSummaryForUser.EndDateTime != null && objPersitstDailyScheduleSummaryForUser.EndDateTime == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowDailyScheduleSummaryForUser["EndDateTime"] = System.DBNull.Value;
                            else
                                datarowDailyScheduleSummaryForUser["EndDateTime"] = objPersitstDailyScheduleSummaryForUser.EndDateTime;
                            datarowDailyScheduleSummaryForUser["TimeRemaining"] = objPersitstDailyScheduleSummaryForUser.TimeRemaining;
                            datarowDailyScheduleSummaryForUser["ScheduleUserID"] = objPersitstDailyScheduleSummaryForUser.ScheduleUserID;
                            datarowDailyScheduleSummaryForUser["IsRegistered"] = objPersitstDailyScheduleSummaryForUser.IsRegistered;
                            datarowDailyScheduleSummaryForUser["Responses"] = objPersitstDailyScheduleSummaryForUser.Responses;
                            datarowDailyScheduleSummaryForUser["ProductID"] = objPersitstDailyScheduleSummaryForUser.ProductID;
                            datarowDailyScheduleSummaryForUser["TestCenterID"] = objPersitstDailyScheduleSummaryForUser.TestCenterID;
                            datarowDailyScheduleSummaryForUser["BatchID"] = BatchID;
                            if (objPersitstDailyScheduleSummaryForUser.SubmittedDate != null && objPersitstDailyScheduleSummaryForUser.SubmittedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowDailyScheduleSummaryForUser["SubmittedDate"] = System.DBNull.Value;
                            else
                                datarowDailyScheduleSummaryForUser["SubmittedDate"] = objPersitstDailyScheduleSummaryForUser.SubmittedDate;
                            datarowDailyScheduleSummaryForUser["TestStartDate"] = objPersitstDailyScheduleSummaryForUser.TestStartDate;
                            datarowDailyScheduleSummaryForUser["TestEndDate"] = objPersitstDailyScheduleSummaryForUser.TestEndDate;

                            #endregion Binding Values to DailyScheduleSummaryForUser DataTable

                            datatableDailyScheduleForUser.Rows.Add(datarowDailyScheduleSummaryForUser);
                        }
                        dataAdapterDailySchedleSummaryForUser.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterDailySchedleSummaryForUser.UpdateBatchSize = 50;
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Connection = ConnectionPersistDailySummaryForUser;
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.CommandText = "PersistSummary_DailySchedulesForUser";

                        #region Binding Command Parameters to stored procedure from DataTable

                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@ScheduleDetailID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleDetailID");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@LoginName", System.Data.SqlDbType.NVarChar, 2147483646, "LoginName");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@UserResponseCount", System.Data.SqlDbType.Int, sizeof(Int32), "UserResponseCount");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@StartedDateTime", System.Data.SqlDbType.DateTime, 20, "StartedDateTime");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@UserStatus", System.Data.SqlDbType.Int, sizeof(Int32), "UserStatus");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@EndDateTime", System.Data.SqlDbType.DateTime, 20, "EndDateTime");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@TimeRemaining", System.Data.SqlDbType.Int, sizeof(Int32), "TimeRemaining");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@ScheduleUserID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleUserID");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@IsRegistered", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsRegistered");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@Responses", System.Data.SqlDbType.NVarChar, 99999, "Responses");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@ProductID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ProductID");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@TestCenterID", System.Data.SqlDbType.BigInt, sizeof(Int64), "TestCenterID");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@BatchID", System.Data.SqlDbType.BigInt, sizeof(Int64), "BatchID");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@SubmittedDate", System.Data.SqlDbType.DateTime, 20, "SubmittedDate");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@TestStartDate", System.Data.SqlDbType.DateTime, 20, "TestStartDate");
                        dataAdapterDailySchedleSummaryForUser.InsertCommand.Parameters.Add("@TestEndDate", System.Data.SqlDbType.DateTime, 20, "TestEndDate");

                        #endregion Binding Command Parameters to stored procedure from DataTable

                        dataAdapterDailySchedleSummaryForUser.Update(datatableDailyScheduleForUser);
                    }
                }
            }
        }

        public List<DataContracts.ScheduledDetails> SynchronizeCandidates(List<DataContracts.ScheduledDetails> ListScheduledDetails)
        {
            Log.LogInfo("Begin SynchronizeCandidates() - ListScheduledDetails Count : " + (ListScheduledDetails != null && ListScheduledDetails.Count > 0 ? ListScheduledDetails.Count.ToString() : "0"));
            if (ListScheduledDetails != null && ListScheduledDetails.Count > 0)
            {
                #region Building ScheduledDetails DataTable

                System.Data.DataTable datatableScheduledDetails = new System.Data.DataTable();
                datatableScheduledDetails.Columns.Add("ScheduleDetailID");
                datatableScheduledDetails.Columns.Add("NumberOfScheduledUsers");
                datatableScheduledDetails.Columns.Add("NumberOfSubmittedUsers");

                #endregion Building ScheduledDetails DataTable

                using (System.Data.SqlClient.SqlConnection connectionScheduleDetails = CommonDAL.GetConnectionforReportingServer())
                {
                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterScheduleDetails = new System.Data.SqlClient.SqlDataAdapter("SynchronizeCandidates", connectionScheduleDetails))
                    {
                        System.Data.DataRow datarowScheduledDetails;
                        foreach (DataContracts.ScheduledDetails objScheduledDetails in ListScheduledDetails)
                        {
                            datarowScheduledDetails = datatableScheduledDetails.NewRow();

                            #region Binding Values to ScheduledDetails DataTable

                            datarowScheduledDetails["ScheduleDetailID"] = objScheduledDetails.ScheduleDetailID;
                            datarowScheduledDetails["NumberOfScheduledUsers"] = objScheduledDetails.NumberOfScheduledUsers;
                            datarowScheduledDetails["NumberOfSubmittedUsers"] = objScheduledDetails.NumberOfSubmittedUsers;

                            #endregion Binding Values to ScheduledDetails DataTable

                            datatableScheduledDetails.Rows.Add(datarowScheduledDetails);
                        }
                        dataAdapterScheduleDetails.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterScheduleDetails.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterScheduleDetails.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.OutputParameters;
                        dataAdapterScheduleDetails.UpdateBatchSize = 50;
                        dataAdapterScheduleDetails.InsertCommand.Connection = connectionScheduleDetails;
                        dataAdapterScheduleDetails.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterScheduleDetails.InsertCommand.CommandText = "SynchronizeCandidates";

                        #region Binding Command Parameters to stored procedure from DataTable

                        dataAdapterScheduleDetails.InsertCommand.Parameters.Add("@ScheduleDetailID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleDetailID");
                        dataAdapterScheduleDetails.InsertCommand.Parameters.Add(BuildSqlParameter("@NumberOfScheduledUsers", System.Data.SqlDbType.Int, sizeof(Int32), "NumberOfScheduledUsers", 0, System.Data.ParameterDirection.Output));
                        dataAdapterScheduleDetails.InsertCommand.Parameters.Add(BuildSqlParameter("@NumberOfSubmittedUsers", System.Data.SqlDbType.Int, sizeof(Int32), "NumberOfSubmittedUsers", 0, System.Data.ParameterDirection.Output));

                        #endregion Binding Command Parameters to stored procedure from DataTable

                        dataAdapterScheduleDetails.Update(datatableScheduledDetails);
                        for (int i = 0; i < ListScheduledDetails.Count; i++)
                        {
                            #region Binding Values to ScheduledDetails DataTable

                            if (Convert.ToInt64(datatableScheduledDetails.Rows[i]["ScheduleDetailID"]) == ListScheduledDetails[i].ScheduleDetailID)
                            {
                                if (datatableScheduledDetails.Rows[i]["NumberOfScheduledUsers"] == null || datatableScheduledDetails.Rows[i]["NumberOfScheduledUsers"].ToString() == "")
                                    ListScheduledDetails[i].NumberOfScheduledUsers = 0;
                                else
                                    ListScheduledDetails[i].NumberOfScheduledUsers = Convert.ToInt32(datatableScheduledDetails.Rows[i]["NumberOfScheduledUsers"]);
                                if (datatableScheduledDetails.Rows[i]["NumberOfSubmittedUsers"] == null || datatableScheduledDetails.Rows[i]["NumberOfSubmittedUsers"].ToString() == "")
                                    ListScheduledDetails[i].NumberOfSubmittedUsers = 0;
                                else
                                    ListScheduledDetails[i].NumberOfSubmittedUsers = Convert.ToInt32(datatableScheduledDetails.Rows[i]["NumberOfSubmittedUsers"]);
                            }

                            #endregion Binding Values to ScheduledDetails DataTable
                        }
                    }
                }
            }
            Log.LogInfo("End SynchronizeCandidates()");
            return ListScheduledDetails;
        }

        public void UpsertCandidateDetails(List<DataContracts.ScheduledDetails> ListScheduledDetails)
        {
            Log.LogInfo("Begin UpsertCandidateDetails() - ListScheduledDetails Count : " + (ListScheduledDetails != null && ListScheduledDetails.Count > 0 ? ListScheduledDetails.Count.ToString() : "0"));
            if (ListScheduledDetails != null && ListScheduledDetails.Count > 0)
            {
                #region Building ScheduledDetails DataTable

                System.Data.DataTable datatableScheduledDetails = new System.Data.DataTable();
                datatableScheduledDetails.Columns.Add("ScheduleDetailID");
                datatableScheduledDetails.Columns.Add("NumberOfScheduledUsers");
                datatableScheduledDetails.Columns.Add("NumberOfSubmittedUsers");

                #endregion Building ScheduledDetails DataTable

                using (System.Data.SqlClient.SqlConnection connectionScheduleDetails = CommonDAL.GetConnectionforReportingServer())
                {
                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterScheduleDetails = new System.Data.SqlClient.SqlDataAdapter("UpsertCandidateDetails", connectionScheduleDetails))
                    {
                        System.Data.DataRow datarowScheduledDetails;
                        foreach (DataContracts.ScheduledDetails objScheduledDetails in ListScheduledDetails)
                        {
                            datarowScheduledDetails = datatableScheduledDetails.NewRow();

                            #region Binding Values to ScheduledDetails DataTable

                            datarowScheduledDetails["ScheduleDetailID"] = objScheduledDetails.ScheduleDetailID;
                            datarowScheduledDetails["NumberOfScheduledUsers"] = objScheduledDetails.NumberOfScheduledUsers;
                            datarowScheduledDetails["NumberOfSubmittedUsers"] = objScheduledDetails.NumberOfSubmittedUsers;

                            #endregion Binding Values to ScheduledDetails DataTable

                            datatableScheduledDetails.Rows.Add(datarowScheduledDetails);
                        }
                        dataAdapterScheduleDetails.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterScheduleDetails.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterScheduleDetails.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterScheduleDetails.UpdateBatchSize = 50;
                        dataAdapterScheduleDetails.InsertCommand.Connection = connectionScheduleDetails;
                        dataAdapterScheduleDetails.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterScheduleDetails.InsertCommand.CommandText = "UpsertCandidateDetails";

                        #region Binding Command Parameters to stored procedure from DataTable

                        dataAdapterScheduleDetails.InsertCommand.Parameters.Add("@ScheduleDetailID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleDetailID");
                        dataAdapterScheduleDetails.InsertCommand.Parameters.Add("@NumberOfScheduledUsers", System.Data.SqlDbType.Int, sizeof(Int32), "NumberOfScheduledUsers");
                        dataAdapterScheduleDetails.InsertCommand.Parameters.Add("@NumberOfSubmittedUsers", System.Data.SqlDbType.Int, sizeof(Int32), "NumberOfSubmittedUsers");

                        #endregion Binding Command Parameters to stored procedure from DataTable

                        dataAdapterScheduleDetails.Update(datatableScheduledDetails);
                    }
                }
            }
            Log.LogInfo("End UpsertCandidateDetails()");
        }

        public void SaveScript(DataContracts.RequestScript request)
        {
            System.Data.DataTable datatableSript = new System.Data.DataTable();

            datatableSript.Columns.Add("TestCenterID");
            datatableSript.Columns.Add("Script");
            datatableSript.Columns.Add("ScriptType");
            datatableSript.Columns.Add("Status");
            using (System.Data.SqlClient.SqlConnection connectionScript = CommonDAL.GetConnection())
            {
                System.Data.DataRow datarowScript;
                using (System.Data.SqlClient.SqlDataAdapter dataAdapterScript = new System.Data.SqlClient.SqlDataAdapter("UspInsertTestCenterScriptLogs", connectionScript))
                {
                    datarowScript = datatableSript.NewRow();
                    datarowScript["TestCenterID"] = request.TestCenterID;
                    datarowScript["Script"] = (new CryptorEngine()).EncryptString(request.Script);
                    switch (request.Type.ToString())
                    {
                        case "Database": datarowScript["ScriptType"] = 1; break;
                        case "FileSystem": datarowScript["ScriptType"] = 2; break;
                    }
                    datatableSript.Rows.Add(datarowScript);

                    dataAdapterScript.InsertCommand = new System.Data.SqlClient.SqlCommand();
                    dataAdapterScript.InsertCommand.CommandTimeout = commandTimeout;
                    dataAdapterScript.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                    dataAdapterScript.UpdateBatchSize = 50;
                    dataAdapterScript.InsertCommand.Connection = connectionScript;
                    dataAdapterScript.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    dataAdapterScript.InsertCommand.CommandText = "UspInsertTestCenterScriptLogs";

                    dataAdapterScript.InsertCommand.Parameters.Add("@TestCenterID", System.Data.SqlDbType.BigInt, sizeof(Int64), "TestcenterID");
                    dataAdapterScript.InsertCommand.Parameters.Add("@Script", System.Data.SqlDbType.NVarChar, 2147483646, "Script");
                    dataAdapterScript.InsertCommand.Parameters.Add("@ScriptType", System.Data.SqlDbType.Int, sizeof(Int32), "ScriptType");
                    dataAdapterScript.InsertCommand.Parameters.Add("@Status", System.Data.SqlDbType.NVarChar, 5, "Status");

                    dataAdapterScript.Update(datatableSript);
                }
            }
        }

        public ServiceContracts.GetNonExecutedScriptsResponseType GetNonExecutedScript(ServiceContracts.GetNonExecutedScriptsRequestType request)
        {
            ServiceContracts.GetNonExecutedScriptsResponseType ScriptList = new ServiceContracts.GetNonExecutedScriptsResponseType();

            using (System.Data.SqlClient.SqlConnection sqlConnection = CommonDAL.GetConnection())
            {
                sqlConnection.Open();
                string commandText = "";
                using (System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(commandText, sqlConnection))
                {
                    sqlCommand.CommandText = "UspGetListofNonExecutedScripts";
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(CommonDAL.BuildSqlParameter("MacID", System.Data.SqlDbType.NVarChar, 999999, "MacID", request.request.MacID, System.Data.ParameterDirection.Input));
                    using (System.Data.SqlClient.SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        ScriptList.Scripts = new List<DataContracts.GetNonExecutedScriptsResponse>();
                        int Index = 0;
                        if (dataReader.Read())
                        {
                            DataContracts.GetNonExecutedScriptsResponse response = new DataContracts.GetNonExecutedScriptsResponse();

                            #region Reading data from Record Set

                            Index = dataReader.GetOrdinal("ID");
                            if (!dataReader.IsDBNull(Index))
                            {
                                response.ID = dataReader.GetInt64(Index);
                            }

                            Index = dataReader.GetOrdinal("TestcenterId");
                            if (!dataReader.IsDBNull(Index))
                            {
                                response.TestCenterID = dataReader.GetInt64(Index);
                            }

                            Index = dataReader.GetOrdinal("Script");
                            if (!dataReader.IsDBNull(Index))
                            {
                                response.Script = dataReader.GetString(Index);
                            }

                            ScriptList.Scripts.Add(response);

                            #endregion Reading data from Record Set
                        }
                    }
                }
            }
            return ScriptList;
        }

        public void UpdateScriptsData(DataContracts.UpdateScriptRequest request)
        {
            SqlParameter[] UpParam = new SqlParameter[3];
            UpParam[0] = new SqlParameter();
            UpParam[0].ParameterName = "ID"; UpParam[0].SqlDbType = SqlDbType.Int; UpParam[0].Direction = ParameterDirection.Input; UpParam[0].Value = request.ID;
            UpParam[1] = new SqlParameter();
            UpParam[1].ParameterName = "ScriptOutput"; UpParam[1].SqlDbType = SqlDbType.NVarChar; UpParam[1].Direction = ParameterDirection.Input; UpParam[1].Value = request.ScriptOutput;
            UpParam[2] = new SqlParameter();
            UpParam[2].ParameterName = "TestCenterID"; UpParam[2].SqlDbType = SqlDbType.Int; UpParam[2].Direction = ParameterDirection.Input; UpParam[2].Value = request.TestCenterID;

            using (System.Data.SqlClient.SqlConnection connection = CommonDAL.GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (System.Data.SqlClient.SqlCommand cmdUpdateScrpt = new System.Data.SqlClient.SqlCommand())
                {
                    cmdUpdateScrpt.CommandType = CommandType.StoredProcedure;
                    cmdUpdateScrpt.Connection = connection;
                    cmdUpdateScrpt.CommandText = "UspUpdateTestCenterScriptLogs";

                    for (int p = 0; p < UpParam.Length; p++)
                    {
                        cmdUpdateScrpt.Parameters.Add(UpParam[p]);
                    }
                    cmdUpdateScrpt.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public ServiceContracts.TestCenterSiteAppraisalResponse UpdateTestCenterWorkStationDetails(ServiceContracts.TestCenterSiteAppraisalRequest req)
        {
            //Load xml of software requirements and fill details into variables
            string xmlpath = AppDomain.CurrentDomain.BaseDirectory + "Resources\\TestCenterSiteAppraisal.xml";
            System.Xml.Linq.XDocument xDoc = System.Xml.Linq.XDocument.Load(xmlpath);

            SqlParameter[] UpParam = new SqlParameter[29];
            UpParam[0] = new SqlParameter() { ParameterName = "MACAddress", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.MACAddress };
            UpParam[1] = new SqlParameter() { ParameterName = "HostName", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.HostName };
            UpParam[2] = new SqlParameter() { ParameterName = "IPAddress", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.IPAddress };

            //OS Validation
            UpParam[3] = new SqlParameter() { ParameterName = "OS", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.OSVersion };
            bool isvalid = false;
            if (req.OSName != null)
                foreach (var item in xDoc.Root.Elements("OS"))
                    if (item.Value.ToUpper().IndexOf(req.OSName.ToUpper()) >= 0)
                    {
                        isvalid = true;
                        break;
                    }
            UpParam[4] = new SqlParameter() { ParameterName = "IsOSValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = isvalid };

            //RAM Validation
            UpParam[5] = new SqlParameter() { ParameterName = "RAM", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.RAM + " GB" };
            isvalid = false;
            decimal RAM = 0;
            if (xDoc.Root.Element("RAM") != null && decimal.TryParse(xDoc.Root.Element("RAM").Value.ToString(), out RAM))
                isvalid = req.RAM >= RAM;
            UpParam[6] = new SqlParameter() { ParameterName = "IsRAMValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = isvalid };

            //CPU validation
            UpParam[7] = new SqlParameter() { ParameterName = "CPU", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.CPUName };
            isvalid = false;
            decimal CPU = 0;
            if (xDoc.Root.Element("CPUSpeed") != null && decimal.TryParse(xDoc.Root.Element("CPUSpeed").Value.ToString(), out CPU))
                isvalid = req.CPUSpeed >= CPU;
            UpParam[8] = new SqlParameter() { ParameterName = "IsCPUValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = isvalid };

            //Database is required only for Test Center Server else not for workstations
            if (string.IsNullOrEmpty(req.TestCenterMacID))
            {
                if (req.DatabaseDetails != null)
                {
                    isvalid = false;
                    decimal dbversion = 0;
                    if (xDoc.Root.Element("DBVersion") != null && decimal.TryParse(xDoc.Root.Element("DBVersion").Value.ToString(), out dbversion))
                        isvalid = Version.Parse(req.DatabaseDetails.Version).Major >= dbversion;

                    UpParam[9] = new SqlParameter() { ParameterName = "DB", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.DatabaseDetails.Name + " " + req.DatabaseDetails.Version };
                    UpParam[10] = new SqlParameter() { ParameterName = "IsDBValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = isvalid };

                    //if version is valid then check for collation of database
                    if (isvalid)
                    {
                        string collation = xDoc.Root.Element("DBCollation") != null && !string.IsNullOrEmpty(xDoc.Root.Element("DBCollation").Value) ? xDoc.Root.Element("DBCollation").Value : string.Empty;
                        if (!collation.ToUpper().Equals(req.DatabaseDetails.Collation.ToUpper())) isvalid = false;
                        if (!isvalid)
                        {
                            UpParam[9] = new SqlParameter() { ParameterName = "DB", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = "Invalid collation" };
                            UpParam[10] = new SqlParameter() { ParameterName = "IsDBValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = isvalid };
                        }
                    }
                }
                else
                {
                    UpParam[9] = new SqlParameter() { ParameterName = "DB", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = "Not Installed" };
                    UpParam[10] = new SqlParameter() { ParameterName = "IsDBValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = false };
                }
            }
            else
            {
                UpParam[9] = new SqlParameter() { ParameterName = "DB", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = "Not Applicable" };
                UpParam[10] = new SqlParameter() { ParameterName = "IsDBValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = true };
            }

            //IIS is required only for Test Center Server else not for workstations
            if (string.IsNullOrEmpty(req.TestCenterMacID))
            {
                if (!string.IsNullOrEmpty(req.IISVersion))
                {
                    isvalid = false;
                    decimal IISVersion = 0;
                    if (xDoc.Root.Element("IISVersion") != null && decimal.TryParse(xDoc.Root.Element("IISVersion").Value.ToString(), out IISVersion))
                        isvalid = Version.Parse(req.IISVersion).Major >= IISVersion;

                    UpParam[11] = new SqlParameter() { ParameterName = "IIS", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = "IIS " + req.IISVersion };
                    UpParam[12] = new SqlParameter() { ParameterName = "IsIISValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = isvalid };
                }
                else
                {
                    UpParam[11] = new SqlParameter() { ParameterName = "IIS", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = "Not Installed" };
                    UpParam[12] = new SqlParameter() { ParameterName = "IsIISValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = false };
                }
            }
            else
            {
                UpParam[11] = new SqlParameter() { ParameterName = "IIS", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = "Not Applicable" };
                UpParam[12] = new SqlParameter() { ParameterName = "IsIISValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = true };
            }

            //Framework
            UpParam[13] = new SqlParameter() { ParameterName = "Framework", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.Framework.ToString() };
            isvalid = false;
            decimal FW = 0;
            if (xDoc.Root.Element("Framework") != null && decimal.TryParse(xDoc.Root.Element("Framework").Value.ToString(), out FW))
                isvalid = Version.Parse(req.Framework).Major >= FW;
            UpParam[14] = new SqlParameter() { ParameterName = "IsFrameworkValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = isvalid };

            //Hard disk space should be atleat 1 GB free space
            UpParam[15] = new SqlParameter() { ParameterName = "HDD", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.HDDFreeSpace + " GB" };
            isvalid = false;
            decimal HDD = 0;
            if (xDoc.Root.Element("HDDFreespace") != null && decimal.TryParse(xDoc.Root.Element("HDDFreespace").Value.ToString(), out HDD))
                isvalid = req.HDDFreeSpace >= HDD;
            UpParam[16] = new SqlParameter() { ParameterName = "IsHDDValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = isvalid };

            //if any one of the browser is valid then its enough
            //IE = 8 and above
            //Firefox = 25 and above
            //Chrome = 30 and above
            isvalid = false;
            int BrowserVersion = 0;
            if (xDoc.Root.Element("IEVersion") != null && int.TryParse(xDoc.Root.Element("IEVersion").Value.ToString(), out BrowserVersion))
                isvalid = Version.Parse(req.IEVersion).Major >= BrowserVersion;
            BrowserVersion = 0;
            if (!isvalid && xDoc.Root.Element("FireFoxVersion") != null && int.TryParse(xDoc.Root.Element("FireFoxVersion").Value.ToString(), out BrowserVersion))
                isvalid = Version.Parse(req.FireFoxVersion).Major >= BrowserVersion;
            BrowserVersion = 0;
            if (!isvalid && xDoc.Root.Element("ChromeVersion") != null && int.TryParse(xDoc.Root.Element("ChromeVersion").Value.ToString(), out BrowserVersion))
                isvalid = Version.Parse(req.ChromeVersion).Major >= BrowserVersion;

            UpParam[17] = new SqlParameter() { ParameterName = "IE", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.IEVersion.ToString() };
            UpParam[18] = new SqlParameter() { ParameterName = "IsIEValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = isvalid };
            UpParam[19] = new SqlParameter() { ParameterName = "Firefox", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.FireFoxVersion };
            UpParam[20] = new SqlParameter() { ParameterName = "IsFirefoxValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = isvalid };
            UpParam[21] = new SqlParameter() { ParameterName = "Chrome", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.ChromeVersion };
            UpParam[22] = new SqlParameter() { ParameterName = "IsChromeValid", SqlDbType = SqlDbType.Bit, Size = sizeof(bool), Direction = ParameterDirection.Input, Value = isvalid };

            UpParam[23] = new SqlParameter() { ParameterName = "Status", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Output, Value = string.Empty };

            //Test Center Server Mac Address
            if (req.TestCenterMacID == null || string.IsNullOrEmpty(req.TestCenterMacID))
                UpParam[24] = new SqlParameter() { ParameterName = "ServerMACAddress", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = DBNull.Value };
            else
                UpParam[24] = new SqlParameter() { ParameterName = "ServerMACAddress", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.TestCenterMacID };

            //Center Name
            if (req.CenterName != null)
                UpParam[25] = new SqlParameter() { ParameterName = "CenterName", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.CenterName };
            else
                UpParam[25] = new SqlParameter() { ParameterName = "CenterName", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = DBNull.Value };
            //Center Code
            if (req.CenterCode != null)
                UpParam[26] = new SqlParameter() { ParameterName = "CenterCode", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.CenterCode };
            else
                UpParam[26] = new SqlParameter() { ParameterName = "CenterCode", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = DBNull.Value };
            //Center Address
            if (req.CenterAddress != null)
                UpParam[27] = new SqlParameter() { ParameterName = "CenterAddress", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = req.CenterAddress };
            else
                UpParam[27] = new SqlParameter() { ParameterName = "CenterAddress", SqlDbType = SqlDbType.NVarChar, Size = 1000, Direction = ParameterDirection.Input, Value = DBNull.Value };
            //OrganizationID
            if (req.OrganizationID > 0)
                UpParam[28] = new SqlParameter() { ParameterName = "OrganizationID", SqlDbType = SqlDbType.BigInt, Size = sizeof(long), Direction = ParameterDirection.Input, Value = req.OrganizationID };
            else
                UpParam[28] = new SqlParameter() { ParameterName = "OrganizationID", SqlDbType = SqlDbType.BigInt, Size = sizeof(long), Direction = ParameterDirection.Input, Value = DBNull.Value };

            using (System.Data.SqlClient.SqlConnection connection = CommonDAL.GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (System.Data.SqlClient.SqlCommand cmdUpdateSiteAppraisal = new System.Data.SqlClient.SqlCommand())
                {
                    cmdUpdateSiteAppraisal.CommandType = CommandType.StoredProcedure;
                    cmdUpdateSiteAppraisal.Connection = connection;
                    cmdUpdateSiteAppraisal.CommandText = "UspUpsertTestCenterPrerequisite";
                    foreach (SqlParameter p in UpParam)
                        cmdUpdateSiteAppraisal.Parameters.Add(p);
                    cmdUpdateSiteAppraisal.ExecuteNonQuery();
                }

                connection.Close();
            }

            if (UpParam[23] != null && UpParam[23].Value.ToString() == "S001")
                return new ServiceContracts.TestCenterSiteAppraisalResponse() { Status = "S001", Message = "Successfully Inserted Details of : " + req.HostName };
            else
                return new ServiceContracts.TestCenterSiteAppraisalResponse() { Status = "S002", Message = "Insert failed for : " + req.HostName };
        }

        internal ServiceContracts.OrganizationsForTestCenterAppraisalResponse OrganizationsForTestCenterAppraisal(string MacAddress)
        {
            ServiceContracts.OrganizationsForTestCenterAppraisalResponse objRes = new ServiceContracts.OrganizationsForTestCenterAppraisalResponse();
            using (System.Data.SqlClient.SqlConnection connection = CommonDAL.GetConnection())
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                int Index = -1;
                long OrganizationID = 0;
                String OrganizationName = string.Empty;
                using (SqlCommand command = new SqlCommand("uspGetOrganizationsForTestCenterAppraisal", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter() { ParameterName = "MacAddress", Size = -1, Direction = ParameterDirection.Input, Value = MacAddress });
                    command.Parameters.Add(new SqlParameter() { ParameterName = "IsTestCenterRegistered", Size = -1, Direction = ParameterDirection.Output, Value = false });

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (objRes.OrganizationDetails == null) objRes.OrganizationDetails = new Dictionary<long, string>();

                            Index = reader.GetOrdinal("OrganizationID");
                            if (!reader.IsDBNull(Index))
                                OrganizationID = reader.GetInt64(Index);
                            Index = reader.GetOrdinal("OrganizationName");
                            if (!reader.IsDBNull(Index))
                                OrganizationName = reader.GetString(Index);

                            objRes.OrganizationDetails.Add(OrganizationID, OrganizationName);
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            Index = reader.GetOrdinal("CenterName");
                            if (!reader.IsDBNull(Index))
                                objRes.TestCenterName = reader.GetString(Index);
                            Index = reader.GetOrdinal("CenterCode");
                            if (!reader.IsDBNull(Index))
                                objRes.TestCenterCode = reader.GetString(Index);
                            Index = reader.GetOrdinal("Address");
                            if (!reader.IsDBNull(Index))
                                objRes.TestCenterAddress = reader.GetString(Index);
                        }
                    }

                    //Check IsTestCenterRegistered
                    objRes.IsTestCenterRegistered = bool.Parse(command.Parameters["IsTestCenterRegistered"].Value.ToString());
                }
            }
            return objRes;
        }

        public void UpdatePostInstallationStatus(System.Collections.Generic.List<DataContracts.PostInstallationComponents> listPostInstallationComponents)
        {
            Log.LogInfo("Begin UpdatePostInstallatoinComponents()");
            if (listPostInstallationComponents != null && listPostInstallationComponents.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                {
                    #region Building DailyScheduleSummaryForUser DataTable

                    System.Data.DataTable datatablePostInstallationComponents = new System.Data.DataTable();
                    datatablePostInstallationComponents.Columns.Add("TestCenterID");
                    datatablePostInstallationComponents.Columns.Add("IsCronDownloadQpack");
                    datatablePostInstallationComponents.Columns.Add("IsCronloadQpack");
                    datatablePostInstallationComponents.Columns.Add("IsCronManageSchedule");
                    datatablePostInstallationComponents.Columns.Add("IsVDMES");
                    datatablePostInstallationComponents.Columns.Add("IsVDTCDashboard");
                    datatablePostInstallationComponents.Columns.Add("IsVDTestplayer");
                    datatablePostInstallationComponents.Columns.Add("IsFileExist");
                    datatablePostInstallationComponents.Columns.Add("IsEnvironmentVerified");
                    datatablePostInstallationComponents.Columns.Add("IsFTPExists");
                    datatablePostInstallationComponents.Columns.Add("IsTCVerified");
                    datatablePostInstallationComponents.Columns.Add("ScheduledCount");
                    datatablePostInstallationComponents.Columns.Add("SubmittedCount");
                    datatablePostInstallationComponents.Columns.Add("InProgressCount");
                    datatablePostInstallationComponents.Columns.Add("ExtractedDate");
                    datatablePostInstallationComponents.Columns.Add("SystemDetails");
                    datatablePostInstallationComponents.Columns.Add("Extension1");
                    datatablePostInstallationComponents.Columns.Add("MacID");

                    #endregion Building DailyScheduleSummaryForUser DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterPostInstallationComponents = new System.Data.SqlClient.SqlDataAdapter("PersistPostInstallationStatus", ConnectionBatch))
                    {
                        foreach (DataContracts.PostInstallationComponents objPostInstallationComponents in listPostInstallationComponents)
                        {
                            System.Data.DataRow datarowPostInstallationComponents = datatablePostInstallationComponents.NewRow();

                            #region Binding Values to PostinstallationComponetns DataTable

                            datarowPostInstallationComponents["TestCenterID"] = objPostInstallationComponents.TestCenterID;
                            datarowPostInstallationComponents["IsCronDownloadQpack"] = objPostInstallationComponents.IsCronDownloadQpack;
                            datarowPostInstallationComponents["IsCronloadQpack"] = objPostInstallationComponents.IsCronloadQpack;
                            datarowPostInstallationComponents["IsCronManageSchedule"] = objPostInstallationComponents.IsCronManageSchedule;
                            datarowPostInstallationComponents["IsVDMES"] = objPostInstallationComponents.IsVDMES;
                            datarowPostInstallationComponents["IsVDTCDashboard"] = objPostInstallationComponents.IsVDTCDashboard; ;
                            datarowPostInstallationComponents["IsVDTestplayer"] = objPostInstallationComponents.IsVDTestplayer;
                            datarowPostInstallationComponents["IsFileExist"] = objPostInstallationComponents.IsFileExist;
                            datarowPostInstallationComponents["IsEnvironmentVerified"] = objPostInstallationComponents.IsEnvironmentVerified;
                            datarowPostInstallationComponents["IsFTPExists"] = objPostInstallationComponents.IsFTPExists;
                            datarowPostInstallationComponents["IsTCVerified"] = objPostInstallationComponents.IsTCVerified;
                            datarowPostInstallationComponents["ScheduledCount"] = objPostInstallationComponents.ScheduledCount;
                            datarowPostInstallationComponents["SubmittedCount"] = objPostInstallationComponents.SubmittedCount;
                            datarowPostInstallationComponents["InProgressCount"] = objPostInstallationComponents.InProgressCount;
                            if (objPostInstallationComponents.ExtractedDate != null && objPostInstallationComponents.ExtractedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPostInstallationComponents["ExtractedDate"] = System.DBNull.Value;
                            else
                                datarowPostInstallationComponents["ExtractedDate"] = objPostInstallationComponents.ExtractedDate;
                            datarowPostInstallationComponents["SystemDetails"] = objPostInstallationComponents.SystemDetails;
                            datarowPostInstallationComponents["Extension1"] = objPostInstallationComponents.Extension1;
                            datarowPostInstallationComponents["MacID"] = objPostInstallationComponents.MacID;

                            #endregion Binding Values to PostinstallationComponetns DataTable

                            datatablePostInstallationComponents.Rows.Add(datarowPostInstallationComponents);
                        }
                        dataAdapterPostInstallationComponents.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterPostInstallationComponents.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterPostInstallationComponents.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterPostInstallationComponents.UpdateBatchSize = 50;
                        dataAdapterPostInstallationComponents.InsertCommand.Connection = ConnectionBatch;
                        dataAdapterPostInstallationComponents.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterPostInstallationComponents.InsertCommand.CommandText = "PersistPostInstallationStatus";

                        #region Binding Command Parameters to stored procedure from DataTable

                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@TestCenterID", System.Data.SqlDbType.BigInt, sizeof(Int64), "TestCenterID");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@IsCronDownloadQpack", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsCronDownloadQpack");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@IsCronloadQpack", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsCronloadQpack");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@IsCronManageSchedule", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsCronManageSchedule");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@IsVDMES", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsVDMES");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@IsVDTCDashboard", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsVDTCDashboard");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@IsVDTestplayer", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsVDTestplayer");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@IsFileExist", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsFileExist");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@IsEnvironmentVerified", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsEnvironmentVerified");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@IsFTPExists", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsFTPExists");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@IsTCVerified", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsTCVerified");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@ScheduledCount", System.Data.SqlDbType.BigInt, sizeof(Int32), "ScheduledCount");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@SubmittedCount", System.Data.SqlDbType.BigInt, sizeof(Int32), "SubmittedCount");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@InProgressCount", System.Data.SqlDbType.BigInt, sizeof(Int32), "InProgressCount");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@ExtractedDate", System.Data.SqlDbType.DateTime, 200, "ExtractedDate");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@SystemDetails", System.Data.SqlDbType.VarChar, 9999, "SystemDetails");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@Extension1", System.Data.SqlDbType.VarChar, 9999, "Extension1");
                        dataAdapterPostInstallationComponents.InsertCommand.Parameters.Add("@MacID", System.Data.SqlDbType.VarChar, 9999, "MacID");

                        #endregion Binding Command Parameters to stored procedure from DataTable

                        dataAdapterPostInstallationComponents.Update(datatablePostInstallationComponents);
                    }
                }
            }
            Log.LogInfo("End UpdateBatchEndTime()");
        }

        public List<DataContracts.ScannedResponseOutput> PersistScannedResponse(List<DataContracts.ScannedResponse> listScannedResponse)
        {
            try
            {
                Log.LogInfo("--- Begin Persist Scanned Response Starts---");

                #region PersistScannedResponse

                using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                {
                    #region to build scannedresposne table

                    DataTable tblScannedResponse = new DataTable();
                    tblScannedResponse.Columns.Add("ScannedResponseId");
                    tblScannedResponse.Columns.Add("ScannedFileName");
                    tblScannedResponse.Columns.Add("ResponseString");
                    tblScannedResponse.Columns.Add("InsertedDate");
                    tblScannedResponse.Columns.Add("ISProcessed");
                    tblScannedResponse.Columns.Add("SheetID");
                    tblScannedResponse.Columns.Add("LoginName");
                    tblScannedResponse.Columns.Add("SubjectCode");
                    tblScannedResponse.Columns.Add("ISEncrypted");
                    tblScannedResponse.Columns.Add("PageNumber");
                    tblScannedResponse.Columns.Add("PageSize");
                    tblScannedResponse.Columns.Add("Extension1");
                    tblScannedResponse.Columns.Add("ISLDS");
                    using (System.Data.SqlClient.SqlDataAdapter dataAdapteScannedResponse = new System.Data.SqlClient.SqlDataAdapter("PersistScannedResponse", ConnectionBatch))
                    {
                        foreach (DataContracts.ScannedResponse objScannedResponse in listScannedResponse)
                        {
                            System.Data.DataRow datarowScannedResponse = tblScannedResponse.NewRow();

                            #region Binding Values to PostinstallationComponetns DataTable

                            if (!objScannedResponse.ISEncrypted)
                            {
                                datarowScannedResponse["ScannedResponseId"] = objScannedResponse.ScannedResponseId;
                                datarowScannedResponse["ScannedFileName"] = objScannedResponse.ScannedFileName;
                                datarowScannedResponse["ResponseString"] = objScannedResponse.ResponseString;
                                datarowScannedResponse["ISProcessed"] = objScannedResponse.ISProcessed;
                                datarowScannedResponse["SheetID"] = objScannedResponse.SheetID;
                                datarowScannedResponse["LoginName"] = objScannedResponse.LoginName;

                                if (objScannedResponse.InsertedDate != null && objScannedResponse.InsertedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowScannedResponse["InsertedDate"] = System.DBNull.Value;
                                else
                                    datarowScannedResponse["InsertedDate"] = objScannedResponse.InsertedDate;
                                datarowScannedResponse["SubjectCode"] = objScannedResponse.SubjectCode;
                                datarowScannedResponse["PageNumber"] = objScannedResponse.PageNumber;
                                datarowScannedResponse["PageSize"] = objScannedResponse.PageSize;
                                datarowScannedResponse["Extension1"] = objScannedResponse.Extension1;
                                if (objScannedResponse.ISLDS != null)
                                    datarowScannedResponse["ISLDS"] = objScannedResponse.ISLDS;
                            }
                            else
                            {
                                KeySecurity Security = new KeySecurity();
                                Log.LogInfo("Encrypted response:");
                                // datarowScannedResponse["ScannedResponseId"] = (objScannedResponse.ScannedResponseId);
                                datarowScannedResponse["ScannedFileName"] = Security.Decrypt(objScannedResponse.ScannedFileName);
                                datarowScannedResponse["ResponseString"] = Security.Decrypt(objScannedResponse.ResponseString);
                                // datarowScannedResponse["ISProcessed"] = Convert.ToBoolean(Security.Decrypt(objScannedResponse.ISProcessed.ToString()));
                                datarowScannedResponse["SheetID"] = Security.Decrypt(objScannedResponse.SheetID);
                                datarowScannedResponse["LoginName"] = Security.Decrypt(objScannedResponse.LoginName);

                                if (objScannedResponse.InsertedDate != null && objScannedResponse.InsertedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowScannedResponse["InsertedDate"] = System.DBNull.Value;
                                else
                                    datarowScannedResponse["InsertedDate"] = Convert.ToDateTime(Security.Decrypt(objScannedResponse.InsertedDate.ToString()));
                                datarowScannedResponse["SubjectCode"] = Security.Decrypt(objScannedResponse.SubjectCode);
                                datarowScannedResponse["PageNumber"] = Convert.ToInt64(Security.Decrypt(objScannedResponse.PageNumber.ToString()));
                                datarowScannedResponse["PageSize"] = Convert.ToInt64(Security.Decrypt(objScannedResponse.PageSize.ToString()));
                                datarowScannedResponse["Extension1"] = Security.Decrypt(objScannedResponse.Extension1);
                                if (objScannedResponse.ISLDS != null)
                                    datarowScannedResponse["ISLDS"] = objScannedResponse.ISLDS;
                                Log.LogInfo("Encrypted response ends:");
                            }
                            tblScannedResponse.Rows.Add(datarowScannedResponse);

                            #endregion Binding Values to PostinstallationComponetns DataTable
                        }
                        dataAdapteScannedResponse.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapteScannedResponse.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapteScannedResponse.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapteScannedResponse.UpdateBatchSize = 50;
                        dataAdapteScannedResponse.InsertCommand.Connection = ConnectionBatch;
                        dataAdapteScannedResponse.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapteScannedResponse.InsertCommand.CommandText = "PersistScannedResponse";

                        #region Binding Command Parameters to stored procedure from DataTable

                        // dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ScannedResponseId", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScannedResponseId");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ScannedFileName", System.Data.SqlDbType.NVarChar, 9999, "ScannedFileName");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ResponseString", System.Data.SqlDbType.NVarChar, 9999, "ResponseString");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@InsertedDate", System.Data.SqlDbType.DateTime, 200, "InsertedDate");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ISProcessed", System.Data.SqlDbType.Bit, sizeof(Boolean), "ISProcessed");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SheetID", System.Data.SqlDbType.NVarChar, 9999, "SheetID");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@LoginName", System.Data.SqlDbType.NVarChar, 9999, "LoginName");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SubjectCode", System.Data.SqlDbType.NVarChar, 9999, "SubjectCode");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@PageNumber", System.Data.SqlDbType.BigInt, sizeof(Int64), "PageNumber");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@PageSize", System.Data.SqlDbType.BigInt, sizeof(Int64), "PageSize");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@Extension1", System.Data.SqlDbType.NVarChar, 9999, "Extension1");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ISLDS", System.Data.SqlDbType.TinyInt, sizeof(Int16), "ISLDS");

                        #endregion Binding Command Parameters to stored procedure from DataTable

                        dataAdapteScannedResponse.Update(tblScannedResponse);
                    }

                    #endregion to build scannedresposne table
                }

                #endregion PersistScannedResponse

                Log.LogInfo("---End Persist Scanned Response ---");
                Log.LogInfo("--- Response Processing Starts For Offline---");
                DoResponseProcessForOfflineTest();

                List<DataContracts.ScannedResponseOutput> lstScannedResponse = new List<DataContracts.ScannedResponseOutput>();
                lstScannedResponse = ExtractResponseStatus();
                return lstScannedResponse;
            }
            catch (System.Exception exe)
            {
                throw exe;
            }
        }

        public List<DataContracts.ScannedResponseOutput> ExtractResponseStatus()
        {
            List<DataContracts.ScannedResponseOutput> ObjResponse = new List<DataContracts.ScannedResponseOutput>();

            using (System.Data.SqlClient.SqlConnection ResponseConnection = CommonDAL.GetConnection())
            {
                using (System.Data.SqlClient.SqlCommand ResponseCommand = new SqlCommand())
                {
                    if (ResponseConnection.State == ConnectionState.Closed)
                        ResponseConnection.Open();
                    ResponseCommand.CommandText = "SELECT LoginName,SheetID,IsProcessed  FROM [ScannedResponse] WHERE InsertedDate >=(SELECT CONVERT(VARCHAR,GETUTCDATE(),111)) ";
                    ResponseCommand.Connection = ResponseConnection;
                    System.Data.SqlClient.SqlDataReader reader = ResponseCommand.ExecuteReader();
                    ObjResponse = ReadResponseStatus(reader);
                    if (!reader.IsClosed) { reader.Close(); }
                }
            }
            return ObjResponse;
        }

        public List<DataContracts.ScannedResponseOutput> ReadResponseStatus(System.Data.SqlClient.SqlDataReader reader)
        {
            System.Collections.Generic.List<DataContracts.ScannedResponseOutput> listScannedResponse = null;
            DataContracts.ScannedResponseOutput objScannedResponse = null;
            if (reader != null && reader.HasRows)
            {
                listScannedResponse = new List<DataContracts.ScannedResponseOutput>();
                int Index;
                while (reader.Read())
                {
                    objScannedResponse = new DataContracts.ScannedResponseOutput();
                    Index = reader.GetOrdinal("LoginName");
                    if (!reader.IsDBNull(Index))
                    {
                        objScannedResponse.LoginName = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("SheetID");
                    if (!reader.IsDBNull(Index))
                    {
                        objScannedResponse.SheetID = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("IsProcessed");
                    if (!reader.IsDBNull(Index))
                    {
                        objScannedResponse.ISProcessed = reader.GetBoolean(Index);
                    }
                    listScannedResponse.Add(objScannedResponse);
                }
            }
            return listScannedResponse;
        }

        public void DoResponseProcessForOfflineTest()
        {
            try
            {
                using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                {
                    using (System.Data.SqlClient.SqlCommand Command = new SqlCommand())
                    {
                        Command.CommandText = "SELECT LoginName,ResponseString,SubjectCode FROM ScannedResponse WHERE ISNULL(IsProcessed,0)=0";
                        Command.Connection = ConnectionBatch;
                        System.Data.SqlClient.SqlDataAdapter adap = new SqlDataAdapter(Command);
                        DataTable tblResponse = new DataTable();
                        adap.Fill(tblResponse);
                        if (tblResponse.Rows.Count > 0)
                        {
                            for (int i = 0; i < tblResponse.Rows.Count; i++)
                            {
                                try
                                {
                                    string response, sheetid, scannedscheetid;
                                    int pos = tblResponse.Rows[i]["ResponseString"].ToString().IndexOf("-G");
                                    sheetid = tblResponse.Rows[i]["ResponseString"].ToString().Substring(0, pos);
                                    int pos2 = tblResponse.Rows[i]["ResponseString"].ToString().IndexOf("-XO");
                                    scannedscheetid = tblResponse.Rows[i]["ResponseString"].ToString().Substring(pos + 3, (pos2 - (pos + 3)));
                                    int responsedelimater = tblResponse.Rows[i]["ResponseString"].ToString().IndexOf("-GO");
                                    response = tblResponse.Rows[i]["ResponseString"].ToString().Substring(pos2 + 3, (responsedelimater - (pos2 + 3)));

                                    response = response.TrimStart();
                                    // response = " " + response;

                                    int noofquestion = response.Length / 4;
                                    string userresponse;
                                    int questionno = 0;
                                    //string finalresponse = sheetid + "$";
                                    string finalresponse = tblResponse.Rows[i]["LoginName"].ToString() + "$";
                                    for (int j = 0; j < (response.Length - 1); j = j + 4)
                                    {
                                        //if (i == 0)
                                        //    finalresponse += ExtractResponse(response.Substring(i, 5));
                                        //else
                                        finalresponse += ExtractResponse(response.Substring(j, 4));
                                        finalresponse += ";";
                                    }

                                    //using (System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection(GetConnectionString()))
                                    {
                                        using (System.Data.SqlClient.SqlCommand CommandProcess = new System.Data.SqlClient.SqlCommand())
                                        {
                                            if (ConnectionBatch.State == System.Data.ConnectionState.Closed)
                                                ConnectionBatch.Open();
                                            CommandProcess.Connection = ConnectionBatch;
                                            CommandProcess.CommandType = CommandType.StoredProcedure;
                                            CommandProcess.CommandText = "uspProcessofflineresponse";
                                            CommandProcess.Parameters.AddWithValue("@finalresponse", finalresponse);
                                            //CommandProcess.Parameters.AddWithValue("@Assessmentcode", tblResponse.Rows[i]["SubjectCode"].ToString());
                                            CommandProcess.ExecuteNonQuery();
                                        }
                                        ConnectionBatch.Close();
                                    }
                                    response = sheetid = scannedscheetid = null;
                                    pos = pos2 = 0;
                                }
                                catch (System.Exception exe)
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception exe)
            {
                throw exe;
            }
        }

        public List<DataContracts.OMRScannedResponseOuput> DoResponseProcessForOfflineScannedTest(string LoginName)
        {
            List<DataContracts.OMRScannedResponseOuput> listOMRScannedResponse = new List<DataContracts.OMRScannedResponseOuput>();
            DataContracts.OMRScannedResponseOuput objOMRScannedResponse = null;

            try
            {
                using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                {
                    using (System.Data.SqlClient.SqlCommand Command = new SqlCommand())
                    {
                        string sql = string.Format("SELECT LoginName,ResponseString,SubjectCode FROM ScannedResponse WHERE ISNULL(IsProcessed,0)=0 AND LoginName='{0}'", LoginName);
                        Command.CommandText = sql;
                        Command.Connection = ConnectionBatch;
                        System.Data.SqlClient.SqlDataAdapter adap = new SqlDataAdapter(Command);
                        DataTable tblResponse = new DataTable();
                        adap.Fill(tblResponse);
                        if (tblResponse.Rows.Count > 0)
                        {
                            for (int i = 0; i < tblResponse.Rows.Count; i++)
                            {
                                try
                                {
                                    //using (System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection(GetConnectionString()))
                                    {
                                        using (System.Data.SqlClient.SqlCommand CommandProcess = new System.Data.SqlClient.SqlCommand())
                                        {
                                            if (ConnectionBatch.State == System.Data.ConnectionState.Closed)
                                                ConnectionBatch.Open();
                                            CommandProcess.Connection = ConnectionBatch;
                                            CommandProcess.CommandType = CommandType.StoredProcedure;
                                            CommandProcess.CommandText = "uspProcessOfflineScannedResponse";
                                            CommandProcess.Parameters.AddWithValue("@LoginName", tblResponse.Rows[i]["LoginName"]);
                                            CommandProcess.Parameters.AddWithValue("@UserResponses", tblResponse.Rows[i]["ResponseString"]);
                                            CommandProcess.Parameters.AddWithValue("@AssessmentCode", tblResponse.Rows[i]["SubjectCode"]);
                                            //CommandProcess.Parameters.AddWithValue("@Assessmentcode", tblResponse.Rows[i]["SubjectCode"].ToString());
                                            using (System.Data.SqlClient.SqlDataAdapter sqlAdap = new SqlDataAdapter(CommandProcess))
                                            {
                                                System.Data.DataSet ds = new DataSet();
                                                sqlAdap.Fill(ds);

                                                if (tblResponse.Rows[i]["LoginName"].ToString().ToLower().Equals(LoginName.ToLower()))
                                                {
                                                    objOMRScannedResponse = new DataContracts.OMRScannedResponseOuput();
                                                    if (ds.Tables.Count >= 2)
                                                    {
                                                        objOMRScannedResponse.TotalQuestions = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalQuestions"].ToString());
                                                        objOMRScannedResponse.TotalScore = Convert.ToDouble(ds.Tables[1].Rows[0]["TotalScore"].ToString());
                                                        objOMRScannedResponse.ActualScore = Convert.ToDouble(ds.Tables[1].Rows[0]["ActualScore"].ToString());
                                                        objOMRScannedResponse.Attempted = Convert.ToInt32(ds.Tables[1].Rows[0]["Attempted"].ToString());
                                                        objOMRScannedResponse.NotAttempted = Convert.ToInt32(ds.Tables[1].Rows[0]["NotAttempted"].ToString());
                                                        objOMRScannedResponse.Correct = Convert.ToInt32(ds.Tables[1].Rows[0]["Correct"].ToString());
                                                        objOMRScannedResponse.NotCorrect = Convert.ToInt32(ds.Tables[1].Rows[0]["NotCorrect"].ToString());
                                                        objOMRScannedResponse.Status = ds.Tables[1].Rows[0]["Status"].ToString();
                                                        listOMRScannedResponse.Add(objOMRScannedResponse);
                                                    }
                                                    else
                                                    {
                                                        objOMRScannedResponse.Status = "Not Processed";
                                                    }
                                                }
                                            }
                                        }
                                        ConnectionBatch.Close();
                                    }
                                }
                                catch (System.Exception exe)
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception exe)
            {
            }
            return listOMRScannedResponse;
        }

        public string ExtractResponse(string decimalresponse)
        {
            int num;

            num = Convert.ToInt32(decimalresponse);
            int quot;

            string rem = "";

            while (num >= 1)
            {
                quot = num / 2;
                rem += (num % 2).ToString();
                num = quot;
            }
            if (rem == "")
                rem = "0000000000";
            decimalresponse = rem;
            //if (decimalresponse.Length != 10)
            //    while (decimalresponse.Length != 10)
            //    {
            //        decimalresponse += "0";
            //    }
            return decimalresponse;
        }

        public ServiceContracts.Contracts.BubbleSheetDataResponse ExtractBubbleSheetData(ServiceContracts.Contracts.BubbleSheetDataRequest request)
        {
            ServiceContracts.Contracts.BubbleSheetDataResponse ObjectBubbleSheetDataResponse = new ServiceContracts.Contracts.BubbleSheetDataResponse();
            List<DataContracts.BubbleSheetData> ListBubblesheet = null;
            Log.LogInfo("Call USPExtractBubbleSheetData");
            using (System.Data.SqlClient.SqlConnection Connection = CommonDAL.GetConnection())
            {
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();
                Log.LogInfo("Conn:" + Connection.ConnectionString);
                using (System.Data.SqlClient.SqlCommand Command = new SqlCommand())
                {
                    Command.Connection = Connection;
                    Command.CommandText = "USPExtractBubbleSheetData";
                    Command.Parameters.AddWithValue("@ScheduleUserID", request.ScheduleUserID);
                    Command.CommandType = CommandType.StoredProcedure;
                    using (System.Data.SqlClient.SqlDataReader reader = Command.ExecuteReader())
                    {
                        if (reader != null && reader.HasRows)
                        {
                            int Index;
                            ListBubblesheet = new List<DataContracts.BubbleSheetData>();
                            DataContracts.BubbleSheetData ObjectBubbleSheetData = null;
                            while (reader.Read())
                            {
                                ObjectBubbleSheetData = new DataContracts.BubbleSheetData();
                                Index = reader.GetOrdinal("Name");
                                if (!reader.IsDBNull(Index))
                                {
                                    ObjectBubbleSheetData.Name = reader.GetString(Index);
                                }
                                Index = reader.GetOrdinal("SheetNumber");
                                if (!reader.IsDBNull(Index))
                                {
                                    ObjectBubbleSheetData.SheetNumber = reader.GetInt64(Index);
                                }
                                Index = reader.GetOrdinal("IdentificationNumber");
                                if (!reader.IsDBNull(Index))
                                {
                                    ObjectBubbleSheetData.IdentificationNumber = reader.GetString(Index);
                                }
                                Index = reader.GetOrdinal("EntryTime");
                                if (!reader.IsDBNull(Index))
                                {
                                    ObjectBubbleSheetData.EntryTime = reader.GetDateTime(Index);
                                }
                                Index = reader.GetOrdinal("TestMessage");
                                if (!reader.IsDBNull(Index))
                                {
                                    ObjectBubbleSheetData.TestMessage = reader.GetString(Index);
                                }
                                Index = reader.GetOrdinal("ClassName");
                                if (!reader.IsDBNull(Index))
                                {
                                    ObjectBubbleSheetData.ClassName = reader.GetString(Index);
                                }
                                Index = reader.GetOrdinal("FacultyName");
                                if (!reader.IsDBNull(Index))
                                {
                                    ObjectBubbleSheetData.FaculatyName = reader.GetString(Index);
                                }
                                Index = reader.GetOrdinal("TestName");
                                if (!reader.IsDBNull(Index))
                                {
                                    ObjectBubbleSheetData.TestName = reader.GetString(Index);
                                }
                                Index = reader.GetOrdinal("TestMessage");
                                if (!reader.IsDBNull(Index))
                                {
                                    ObjectBubbleSheetData.TestMessage = reader.GetString(Index);
                                }
                                ListBubblesheet.Add(ObjectBubbleSheetData);
                            }
                            ObjectBubbleSheetDataResponse.ListBubbleSheetData = ListBubblesheet;
                        }
                    }
                }
            }
            return ObjectBubbleSheetDataResponse;
        }

        public List<DataContracts.OMRScannedResponseOuput> PersistOfflineScannedResponse(List<DataContracts.ScannedResponse> listScannedResponse)
        {
            List<DataContracts.OMRScannedResponseOuput> listOMRScannedResponse = new List<DataContracts.OMRScannedResponseOuput>();
            try
            {
                Log.LogInfo("--- Begin Persist Scanned Response Starts---");

                #region PersistScannedResponse

                using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                {
                    #region to build scannedresposne table

                    DataTable tblScannedResponse = new DataTable();
                    tblScannedResponse.Columns.Add("ScannedResponseId");
                    tblScannedResponse.Columns.Add("ScannedFileName");
                    tblScannedResponse.Columns.Add("ResponseString");
                    tblScannedResponse.Columns.Add("InsertedDate");
                    tblScannedResponse.Columns.Add("ISProcessed");
                    tblScannedResponse.Columns.Add("SheetID");
                    tblScannedResponse.Columns.Add("LoginName");
                    tblScannedResponse.Columns.Add("SubjectCode");
                    tblScannedResponse.Columns.Add("PageNumber");
                    tblScannedResponse.Columns.Add("PageSize");
                    tblScannedResponse.Columns.Add("Extension1");
                    tblScannedResponse.Columns.Add("ISEncrypted");
                    tblScannedResponse.Columns.Add("ISLDS");

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapteScannedResponse = new System.Data.SqlClient.SqlDataAdapter("PersistScannedResponse", ConnectionBatch))
                    {
                        foreach (DataContracts.ScannedResponse objScannedResponse in listScannedResponse)
                        {
                            System.Data.DataRow datarowScannedResponse = tblScannedResponse.NewRow();

                            #region Binding Values to PostinstallationComponetns DataTable

                            if (!objScannedResponse.ISEncrypted)
                            {
                                datarowScannedResponse["ScannedResponseId"] = objScannedResponse.ScannedResponseId;
                                datarowScannedResponse["ScannedFileName"] = objScannedResponse.ScannedFileName;
                                datarowScannedResponse["ResponseString"] = objScannedResponse.ResponseString;
                                datarowScannedResponse["ISProcessed"] = objScannedResponse.ISProcessed;
                                datarowScannedResponse["SheetID"] = objScannedResponse.SheetID;
                                datarowScannedResponse["LoginName"] = objScannedResponse.LoginName;

                                if (objScannedResponse.InsertedDate != null && objScannedResponse.InsertedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowScannedResponse["InsertedDate"] = System.DBNull.Value;
                                else
                                    datarowScannedResponse["InsertedDate"] = objScannedResponse.InsertedDate;
                                datarowScannedResponse["SubjectCode"] = objScannedResponse.SubjectCode;
                                datarowScannedResponse["PageNumber"] = objScannedResponse.PageNumber;
                                datarowScannedResponse["PageSize"] = objScannedResponse.PageSize;
                                datarowScannedResponse["Extension1"] = objScannedResponse.Extension1;
                                if (objScannedResponse.ISLDS != null)
                                    datarowScannedResponse["ISLDS"] = objScannedResponse.ISLDS;
                                tblScannedResponse.Rows.Add(datarowScannedResponse);
                            }
                            else
                            {
                                KeySecurity Security = new KeySecurity();
                                Log.LogInfo("Encrypted response:");
                                datarowScannedResponse["ScannedResponseId"] = objScannedResponse.ScannedResponseId;
                                datarowScannedResponse["ScannedFileName"] = Security.Decrypt(objScannedResponse.ScannedFileName);
                                datarowScannedResponse["ResponseString"] = Security.Decrypt(objScannedResponse.ResponseString);
                                datarowScannedResponse["ISProcessed"] = objScannedResponse.ISProcessed;
                                datarowScannedResponse["SheetID"] = Security.Decrypt(objScannedResponse.SheetID);
                                datarowScannedResponse["LoginName"] = Security.Decrypt(objScannedResponse.LoginName);

                                if (objScannedResponse.InsertedDate != null && objScannedResponse.InsertedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowScannedResponse["InsertedDate"] = System.DBNull.Value;
                                else
                                    datarowScannedResponse["InsertedDate"] = objScannedResponse.InsertedDate;
                                datarowScannedResponse["SubjectCode"] = Security.Decrypt(objScannedResponse.SubjectCode);
                                datarowScannedResponse["PageNumber"] = objScannedResponse.PageNumber;
                                datarowScannedResponse["PageSize"] = objScannedResponse.PageSize;
                                datarowScannedResponse["Extension1"] = Security.Decrypt(objScannedResponse.Extension1);
                                if (objScannedResponse.ISLDS != null)
                                    datarowScannedResponse["ISLDS"] = objScannedResponse.ISLDS;
                                tblScannedResponse.Rows.Add(datarowScannedResponse);
                                Log.LogInfo("Encrypted response ends:");
                            }

                            #endregion Binding Values to PostinstallationComponetns DataTable
                        }
                        Log.LogInfo("---End Persist Scanned Response ---");
                        dataAdapteScannedResponse.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapteScannedResponse.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapteScannedResponse.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapteScannedResponse.UpdateBatchSize = 50;
                        dataAdapteScannedResponse.InsertCommand.Connection = ConnectionBatch;
                        dataAdapteScannedResponse.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapteScannedResponse.InsertCommand.CommandText = "PersistScannedResponse";

                        #region Binding Command Parameters to stored procedure from DataTable

                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ScannedResponseId", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScannedResponseId");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ScannedFileName", System.Data.SqlDbType.NVarChar, 9999, "ScannedFileName");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ResponseString", System.Data.SqlDbType.NVarChar, 9999, "ResponseString");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@InsertedDate", System.Data.SqlDbType.DateTime, 200, "InsertedDate");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ISProcessed", System.Data.SqlDbType.Bit, sizeof(Boolean), "ISProcessed");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SheetID", System.Data.SqlDbType.NVarChar, 9999, "SheetID");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@LoginName", System.Data.SqlDbType.NVarChar, 9999, "LoginName");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SubjectCode", System.Data.SqlDbType.NVarChar, 9999, "SubjectCode");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@PageNumber", System.Data.SqlDbType.BigInt, sizeof(Int64), "PageNumber");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@PageSize", System.Data.SqlDbType.BigInt, sizeof(Int64), "PageSize");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@Extension1", System.Data.SqlDbType.NVarChar, 9999, "Extension1");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ISLDS", System.Data.SqlDbType.TinyInt, sizeof(Int16), "ISLDS");

                        #endregion Binding Command Parameters to stored procedure from DataTable

                        dataAdapteScannedResponse.Update(tblScannedResponse);
                        if (listScannedResponse[0].ISLogRequired)//checking first record to see service log required or not
                            foreach (DataContracts.ScannedResponse objScannedResponse in listScannedResponse)
                            {
                                string requestobject = "Request object:LoginName:" + objScannedResponse.LoginName + "ResponseString" + objScannedResponse.ResponseString + "Subjectcode:" + objScannedResponse.SubjectCode + "SheetID:" + objScannedResponse.SheetID;
                                using (System.Data.SqlClient.SqlConnection Connection = CommonDAL.GetConnection())
                                {
                                    using (System.Data.SqlClient.SqlCommand Command = new SqlCommand())
                                    {
                                        Command.Connection = Connection;
                                        if (Connection.State == ConnectionState.Closed)
                                            Connection.Open();
                                        Command.CommandType = CommandType.StoredProcedure;
                                        Command.CommandText = "UspUpsertWebServiceLog";
                                        Command.Parameters.AddWithValue("@Module", "SCANTRAYOMR");
                                        Command.Parameters.AddWithValue("@MethodType", "SCANTRAYOMR");
                                        Command.Parameters.AddWithValue("@StartDate", System.DateTime.UtcNow);
                                        Command.Parameters.AddWithValue("@EndDate", System.DateTime.UtcNow);
                                        Command.Parameters.AddWithValue("@RequestObject", requestobject);
                                        Command.ExecuteNonQuery();
                                        Connection.Close();
                                    }
                                }
                            }

                        #endregion to build scannedresposne table
                    }

                    #endregion PersistScannedResponse

                    Log.LogInfo("---End Persist Scanned Response ---");
                    Log.LogInfo("--- Response Processing Starts For Offline---");
                    if (listScannedResponse[0].ISLDS == 1)
                        listOMRScannedResponse = DoResponseProcessForOfflineScannedTest(listScannedResponse[0].LoginName);
                    else if (listScannedResponse[0].ISLDS == 2)
                        DOResponseProcessingFORNYCS();

                    //List<DataContracts.ScannedResponseOutput> lstScannedResponse = new List<DataContracts.ScannedResponseOutput>();
                    //lstScannedResponse = ExtractResponseStatus();
                    //return lstScannedResponse;
                }
            }
            catch (System.Exception exe)
            {
                Log.LogInfo("Error:", exe);
                throw exe;
            }
            return listOMRScannedResponse;
        }

        public ServiceContracts.Contracts.ExtractAssessmentDetailsResponse ExtractAssessmentDetails(ServiceContracts.Contracts.ExtractAssessmentDetailsRequest request)
        {
            ServiceContracts.Contracts.ExtractAssessmentDetailsResponse objResponse = null;
            try
            {
                objResponse = new ServiceContracts.Contracts.ExtractAssessmentDetailsResponse();
                objResponse.AssessmentDetailsResponse = new DataContracts.AssessmentDetailsResponse();
                SqlConnection connection = GetConnection();
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "UspgetQuestionsMaxChoice";
                command.Parameters.AddWithValue("@ScheduleUserID", request.AssessmentField.ScheduleUserID);
                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = command.ExecuteReader();

                if (reader != null && reader.HasRows)
                {
                    reader.Read();
                    int index;
                    index = reader.GetOrdinal("MaxChoice");
                    if (!reader.IsDBNull(index))
                    {
                        objResponse.AssessmentDetailsResponse.MaxChoices = reader.GetInt32(index);
                    }
                    index = reader.GetOrdinal("NoOfQuestions");
                    if (!reader.IsDBNull(index))
                    {
                        objResponse.AssessmentDetailsResponse.NoOfQuestions = reader.GetInt32(index);
                    }
                }
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
                connection.Close();
            }
            catch (Exception exe)
            {
            }
            return objResponse;
        }

        public List<DataContracts.SummaryDetailsResponse> ExtractSummaryDetails(ServiceContracts.Contracts.ExtractSummaryDetailsRequest request)
        {
            List<DataContracts.SummaryDetailsResponse> lstSummaryDetailsResponse = new List<DataContracts.SummaryDetailsResponse>();
            using (SqlConnection connection = GetConnection())
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "uspGetAllBatchStatistics";
                    command.CommandType = CommandType.StoredProcedure;
                    DataSet setSummary = new DataSet();
                    SqlDataAdapter adap = new SqlDataAdapter(command);
                    adap.Fill(setSummary);
                    DataContracts.SummaryDetailsResponse objResponse = new DataContracts.SummaryDetailsResponse();
                    objResponse.NoOfPackagesDeliveredTODCC = Convert.ToInt32(setSummary.Tables[0].Rows[0]["NoOfPackagesDelivered"]);
                    objResponse.PackagesReachedDCC = Convert.ToInt32(setSummary.Tables[0].Rows[0]["PackagesReachedDCC"]);
                    objResponse.NoOfPackagesLoadedInDCC = Convert.ToInt32(setSummary.Tables[0].Rows[0]["NoofPackagesNotYetReciveInDX"]);
                    objResponse.NoOfPackagesDelivered = Convert.ToInt32(setSummary.Tables[1].Rows[0]["NoOfPackagesDelivered"]);
                    objResponse.NoOfPackagesReachedTestCenter = Convert.ToInt32(setSummary.Tables[1].Rows[0]["NoOfPackagesDelivered"]);
                    objResponse.NoOfPackagesLoadedInTestCenter = Convert.ToInt32(setSummary.Tables[2].Rows[0]["NoOfPackagesLoadedInTestCenter"]);
                    objResponse.NoOfPackagesInProgress = Convert.ToInt32(setSummary.Tables[0].Rows[0]["NoOfPackagesLoadedInTestCenter"]);
                    objResponse.NoOfPackagesNotYetStarted = Convert.ToInt32(setSummary.Tables[0].Rows[0]["NoOfPackagesNotYetStarted"]);
                    lstSummaryDetailsResponse.Add(objResponse);
                }
            }
            return lstSummaryDetailsResponse;
        }

        public int VerifyMailID(string EmailID)
        {
            int result = 0;

            using (System.Data.SqlClient.SqlConnection Connection = GetConnection())
            {
                using (System.Data.SqlClient.SqlCommand command = new SqlCommand())
                {
                    command.CommandText = string.Format("SELECT COUNT(1) FROM [User] WHERE IsDeleted=0 AND Email='{0}'", EmailID);
                    command.Connection = Connection;
                    if (Connection.State == ConnectionState.Closed)
                        Connection.Open();
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = int.Parse(reader.GetValue(0).ToString());
                    }
                    if (!reader.IsClosed) { reader.Close(); }
                }
            }
            return result;
        }

        public int VerifyUser(string LoginName, string Password, bool IsEncrypted = true)
        {
            int Status = 0;
            string RSAPassword;
            //if ((IsEncrypted)  || Password.Length>20 )
            if ((IsEncrypted))
                RSAPassword = DecryptRSAString(Password);
            else
                RSAPassword = Password;
            Logger log = new Logger();
            log.LogInfo("ISEncrypt:" + IsEncrypted.ToString());
            log.LogInfo("Decryption of RSA:" + RSAPassword);

            using (System.Data.SqlClient.SqlConnection Connection = GetConnection())
            {
                using (System.Data.SqlClient.SqlCommand Command = new SqlCommand())
                {
                    Command.Connection = Connection;
                    string TNAPassword = Encryptstring(RSAPassword);
                    if (Connection.State == ConnectionState.Closed)
                        Connection.Open();

                    string sql = string.Format("SELECT  COUNT(1) FROM [User] INNER JOIN [Role] ON UserType=RoleID WHERE LoginName='{0}' AND Password='{1}' AND  RoleName NOT  LIKE '%Student%'", LoginName, TNAPassword);
                    Command.CommandText = sql;
                    Command.CommandType = CommandType.Text;
                    Command.Connection = Connection;
                    SqlDataReader reader = Command.ExecuteReader();
                    if (reader.Read())
                    {
                        Status = int.Parse(reader.GetValue(0).ToString());
                    }
                }
            }
            return Status;
        }

        private string DecryptRSAString(string encryptedText)
        {
            string privateKeyFileName = System.Configuration.ConfigurationManager.AppSettings["PrivateKeyFileName"] == null ? string.Empty : System.Configuration.ConfigurationManager.AppSettings["PrivateKeyFileName"];

            // Variables
            CspParameters cspParams = null;
            RSACryptoServiceProvider rsaProvider = null;
            StreamReader privateKeyFile = null;
            FileStream encryptedFile = null;
            StreamWriter plainFile = null;
            string privateKeyText = "";
            string plainText = "";
            byte[] encryptedBytes = null;
            byte[] plainBytes = null;

            try
            {
                // Select target CSP
                cspParams = new CspParameters();
                cspParams.ProviderType = 1; // PROV_RSA_FULL
                //cspParams.ProviderName; // CSP name
                rsaProvider = new RSACryptoServiceProvider(2048);

                // Read private/public key pair from file
                if (privateKeyFileName != string.Empty)
                {
                    privateKeyFile = File.OpenText(privateKeyFileName);

                    if (File.Exists(privateKeyFileName))
                    {
                        privateKeyText = privateKeyFile.ReadToEnd();
                    }
                }
                else
                {
                    privateKeyText = "<RSAKeyValue><Modulus>k/9dDsbdq9gqPbJxaaiw4Xn6HkajNKU6zQFhfk4FtFM2o/pwHZLGdQLPfTz3xs2l8W45DJY6DPwxcJdyuWrFS4TBvIuydEUb0aXcP7aX5jN7v/YCTF78AF3dk+79adgOAupn9mghKyPHn6HtGvqvgEpTmjNCOQaolWLq+wDwM5c=</Modulus><Exponent>AQAB</Exponent><P>ycz5E4ET72nfydlRq5w6hmMgTuOHuobAV8vsRTDpg6+mXzJx2UyH405fXSOZS84pqXTwhh2cptbElFXDn7CAsw==</P><Q>u78YVt8J1XMJIJu8OSkFV/fgKYJGD+R5hRxPb+/FkMV8G2sWK/Yd4u9qN89CF/YfaVsQqIzsUbLlL6YW75frjQ==</Q><DP>TADqEoF766DZi2FRFCw8Ep9E7NFfLk5QJQEF1K1uVY2TQKl0HZ5oU6ER47djphxYrpz/ddOzS1b6JNAEZKGKZw==</DP><DQ>AoJvZNneW8gJ2zG5tlniBGb/zA49uYCoTysttKVT0reRDRzFUxkbFSl2FgDjNUbI7LOW6WnYzs7BWX2y2MkDEQ==</DQ><InverseQ>Zw2ygEdd6nHCmpJpdq1SprQpS62WWFFiO4eJEUPBCq/NE+0MUjHMIsJMBPvb7ryNVK2VdJwU/iLXqqmIYCzN4g==</InverseQ><D>CxHgvgg18HKcxjygqwyJHnRnNviFoJxzR35A8peXaaOHxMAovq2J1pq9NlrnFaGwNdzOu5hJA0uhzxAk7qWD2EMUmpZFeF7jVbkewOU24mYfqvMUT4uBQLfIoi7Byd5zguYZuCQBDOywMKiL11HMijbGyx1C01JbEbiznjOiHxE=</D></RSAKeyValue>";
                }

                // Import private/public key pair
                rsaProvider.FromXmlString(privateKeyText);

                // Read encrypted text from file

                encryptedBytes = Convert.FromBase64String(encryptedText);
                //  encryptedBytes = GetBytes(encryptedText);

                // Decrypt text
                plainBytes = rsaProvider.Decrypt(encryptedBytes, false);

                // Write decrypted text to file
                //  plainText = Encoding.Unicode.GetString(plainBytes);
                //  Console.WriteLine(plainText);
                plainText = Encoding.GetEncoding("windows-1256").GetString(plainBytes);
            }
            catch (Exception ex)
            {
                Log.LogError("\nModule Name : DecryptRSAString;\nClass Name : DecryptRSAString;\nMethod Name : DecryptRSAString;\nError Message : " + ex.Message + "\nStackTrace : " + ex.StackTrace + "\n");
            }
            finally
            {
                // Do some clean up if needed
                if (privateKeyFile != null)
                {
                    privateKeyFile.Close();
                }
                if (encryptedFile != null)
                {
                    encryptedFile.Close();
                }
                if (plainFile != null)
                {
                    plainFile.Close();
                }
            }

            return plainText;
        } // Decrypt

        public string Encryptstring(string Text)
        {
            string EncryptedPassword = null;
            try
            {
                RijndaelManaged RijndaelCipher = new RijndaelManaged();
                byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(Text);
                String PasswordKey = "encryptPassword"; //System.Configuration.ConfigurationManager.AppSettings["EncryptionKey"];
                byte[] Salt = Encoding.ASCII.GetBytes(PasswordKey.Length.ToString());
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(PasswordKey, Salt);
                ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(PlainText, 0, PlainText.Length);
                cryptoStream.FlushFinalBlock();
                byte[] CipherBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                EncryptedPassword = Convert.ToBase64String(CipherBytes);
                return EncryptedPassword;
            }
            catch (Exception ex)
            {
                //objPageBase.Log.LogError("\nModule Name : EncryptDecrypt;\nClass Name : EncryptDecrypt;\nMethod Name : EncryptLogin;\nError Message : " + ex.Message + "\nStackTrace : " + ex.StackTrace + "\n");
                return null;
            }
        }

        public void PersistOMRResponse(List<DataContracts.ScannedResponse> listScannedResponse)
        {
            Log.LogInfo("Persist OMRResponse Starts");
            Log.LogInfo("Count:" + listScannedResponse.Count);
            using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
            {
                #region to build scannedresposne table

                DataTable tblScannedResponse = new DataTable();
                tblScannedResponse.Columns.Add("ScannedResponseId");
                tblScannedResponse.Columns.Add("ScannedFileName");
                tblScannedResponse.Columns.Add("ResponseString");
                tblScannedResponse.Columns.Add("InsertedDate");
                tblScannedResponse.Columns.Add("ISProcessed");
                tblScannedResponse.Columns.Add("SheetID");
                tblScannedResponse.Columns.Add("LoginName");
                tblScannedResponse.Columns.Add("SubjectCode");
                tblScannedResponse.Columns.Add("PageNumber");
                tblScannedResponse.Columns.Add("PageSize");
                tblScannedResponse.Columns.Add("Extension1");
                tblScannedResponse.Columns.Add("MarkedImagePath");
                tblScannedResponse.Columns.Add("Extension2");
                tblScannedResponse.Columns.Add("ISLDS");
                tblScannedResponse.Columns.Add("ImageName");
                using (System.Data.SqlClient.SqlDataAdapter dataAdapteScannedResponse = new System.Data.SqlClient.SqlDataAdapter("PersistScannedResponse", ConnectionBatch))
                {
                    foreach (DataContracts.ScannedResponse objScannedResponse in listScannedResponse)
                    {
                        System.Data.DataRow datarowScannedResponse = tblScannedResponse.NewRow();

                        #region Binding Values to PostinstallationComponetns DataTable

                        datarowScannedResponse["ScannedResponseId"] = objScannedResponse.ScannedResponseId;
                        datarowScannedResponse["ScannedFileName"] = objScannedResponse.ScannedFileName;
                        datarowScannedResponse["ResponseString"] = objScannedResponse.ResponseString;
                        if (objScannedResponse.ISProcessed != false)
                            datarowScannedResponse["ISProcessed"] = objScannedResponse.ISProcessed;
                        else
                            datarowScannedResponse["ISProcessed"] = System.DBNull.Value;
                        datarowScannedResponse["SheetID"] = objScannedResponse.SheetID;
                        datarowScannedResponse["LoginName"] = objScannedResponse.LoginName;

                        if (objScannedResponse.InsertedDate != null && objScannedResponse.InsertedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                            datarowScannedResponse["InsertedDate"] = System.DBNull.Value;
                        else
                            datarowScannedResponse["InsertedDate"] = objScannedResponse.InsertedDate;
                        datarowScannedResponse["SubjectCode"] = objScannedResponse.SubjectCode;
                        datarowScannedResponse["PageNumber"] = objScannedResponse.PageNumber;
                        datarowScannedResponse["PageSize"] = objScannedResponse.PageSize;
                        datarowScannedResponse["Extension1"] = objScannedResponse.Extension1;
                        datarowScannedResponse["MarkedImagePath"] = objScannedResponse.MarkedImagePath;
                        datarowScannedResponse["Extension2"] = objScannedResponse.Extension2;

                        datarowScannedResponse["ImageName"] = objScannedResponse.ImageName;
                        tblScannedResponse.Rows.Add(datarowScannedResponse);

                        #endregion Binding Values to PostinstallationComponetns DataTable
                    }
                    dataAdapteScannedResponse.InsertCommand = new System.Data.SqlClient.SqlCommand();
                    dataAdapteScannedResponse.InsertCommand.CommandTimeout = commandTimeout;
                    dataAdapteScannedResponse.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                    dataAdapteScannedResponse.UpdateBatchSize = 100;
                    dataAdapteScannedResponse.InsertCommand.Connection = ConnectionBatch;
                    dataAdapteScannedResponse.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    dataAdapteScannedResponse.InsertCommand.CommandText = "PersistScannedResponse";

                    #region Binding Command Parameters to stored procedure from DataTable

                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ScannedResponseId", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScannedResponseId");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ScannedFileName", System.Data.SqlDbType.NVarChar, 9999, "ScannedFileName");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ResponseString", System.Data.SqlDbType.NVarChar, 9999, "ResponseString");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@InsertedDate", System.Data.SqlDbType.DateTime, 200, "InsertedDate");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ISProcessed", System.Data.SqlDbType.Bit, sizeof(Boolean), "ISProcessed");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SheetID", System.Data.SqlDbType.NVarChar, 9999, "SheetID");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@LoginName", System.Data.SqlDbType.NVarChar, 9999, "LoginName");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SubjectCode", System.Data.SqlDbType.NVarChar, 9999, "SubjectCode");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@PageNumber", System.Data.SqlDbType.BigInt, sizeof(Int64), "PageNumber");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@PageSize", System.Data.SqlDbType.BigInt, sizeof(Int64), "PageSize");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@Extension1", System.Data.SqlDbType.NVarChar, 9999, "Extension1");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@MarkedImagePath", System.Data.SqlDbType.NVarChar, 9999, "MarkedImagePath");
                    //  dataAdapteScannedResponse.InsertCommand.Parameters.Add("@Extension2", System.Data.SqlDbType.VarChar, 9999, "Extension2");

                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ImageName", System.Data.SqlDbType.VarChar, 2000, "ImageName");

                    Log.LogInfo("Count2:" + tblScannedResponse.Rows.Count);

                    #endregion Binding Command Parameters to stored procedure from DataTable

                    dataAdapteScannedResponse.Update(tblScannedResponse);
                }

                #endregion to build scannedresposne table

                Log.LogInfo("Persist OMRRespone Ends");
            }
        }

        public void DoOMRResponseProcessing()
        {
            using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
            {
                using (System.Data.SqlClient.SqlCommand Command = new SqlCommand())
                {
                    Command.CommandText = "uspExtractUnProcessedOMRUser";//AND PageSize=PageNumber AND IsError=0
                    Command.Connection = ConnectionBatch;
                    Command.CommandType = CommandType.StoredProcedure;

                    System.Data.SqlClient.SqlDataAdapter adap = new SqlDataAdapter(Command);
                    DataTable tblResponse = new DataTable();
                    adap.Fill(tblResponse);

                    if (tblResponse.Rows.Count > 0)
                    {
                        int Max;
                        if (tblResponse.Rows.Count > 100)
                            Max = 100;
                        else
                            Max = tblResponse.Rows.Count;
                        for (int i = 0; i < Max; i++)
                        {
                            try
                            {
                                using (System.Data.SqlClient.SqlCommand CommandProcess = new System.Data.SqlClient.SqlCommand())
                                {
                                    if (ConnectionBatch.State == System.Data.ConnectionState.Closed)
                                        ConnectionBatch.Open();
                                    CommandProcess.Connection = ConnectionBatch;
                                    CommandProcess.CommandType = CommandType.StoredProcedure;
                                    CommandProcess.CommandText = "uspProcessOfflineScannedResponse";
                                    CommandProcess.Parameters.AddWithValue("@LoginName", tblResponse.Rows[i]["LoginName"]);
                                    CommandProcess.Parameters.AddWithValue("@UserResponses", tblResponse.Rows[i]["ResponseString"]);
                                    CommandProcess.Parameters.AddWithValue("@AssessmentCode", tblResponse.Rows[i]["SubjectCode"]);
                                    CommandProcess.Parameters.AddWithValue("@ScannedResponseID", tblResponse.Rows[i]["ScannedResponseID"]);
                                    using (System.Data.SqlClient.SqlDataAdapter sqlAdap = new SqlDataAdapter(CommandProcess))
                                    {
                                        System.Data.DataSet ds = new DataSet();
                                        sqlAdap.Fill(ds);
                                    }
                                }
                                ConnectionBatch.Close();
                            }
                            catch (System.Exception exe)
                            {
                                Log.LogError(exe.Message, exe);
                            }
                        }
                    }
                }
            }
        }

        public void PersistOMRResponseForMIIClient(List<DataContracts.ScannedResponse> listScannedResponse)
        {
            Log.LogInfo("Persist OMRResponse Starts");
            using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
            {
                #region to build scannedresposne table

                DataTable tblScannedResponse = new DataTable();
                tblScannedResponse.Columns.Add("ScannedResponseId");
                tblScannedResponse.Columns.Add("ScannedFileName");
                tblScannedResponse.Columns.Add("ResponseString");
                tblScannedResponse.Columns.Add("InsertedDate");
                tblScannedResponse.Columns.Add("ISProcessed");
                tblScannedResponse.Columns.Add("SheetID");
                tblScannedResponse.Columns.Add("LoginName");
                tblScannedResponse.Columns.Add("SubjectCode");
                using (System.Data.SqlClient.SqlDataAdapter dataAdapteScannedResponse = new System.Data.SqlClient.SqlDataAdapter("PersistScannedResponse", ConnectionBatch))
                {
                    foreach (DataContracts.ScannedResponse objScannedResponse in listScannedResponse)
                    {
                        System.Data.DataRow datarowScannedResponse = tblScannedResponse.NewRow();

                        #region Binding Values to PostinstallationComponetns DataTable

                        datarowScannedResponse["ScannedResponseId"] = objScannedResponse.ScannedResponseId;
                        datarowScannedResponse["ScannedFileName"] = objScannedResponse.ScannedFileName;
                        datarowScannedResponse["ResponseString"] = objScannedResponse.ResponseString;
                        datarowScannedResponse["ISProcessed"] = objScannedResponse.ISProcessed;
                        datarowScannedResponse["SheetID"] = objScannedResponse.SheetID;
                        datarowScannedResponse["LoginName"] = objScannedResponse.LoginName;

                        if (objScannedResponse.InsertedDate != null && objScannedResponse.InsertedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                            datarowScannedResponse["InsertedDate"] = System.DBNull.Value;
                        else
                            datarowScannedResponse["InsertedDate"] = objScannedResponse.InsertedDate;
                        datarowScannedResponse["SubjectCode"] = objScannedResponse.SubjectCode;
                        tblScannedResponse.Rows.Add(datarowScannedResponse);

                        #endregion Binding Values to PostinstallationComponetns DataTable
                    }
                    dataAdapteScannedResponse.InsertCommand = new System.Data.SqlClient.SqlCommand();
                    dataAdapteScannedResponse.InsertCommand.CommandTimeout = commandTimeout;
                    dataAdapteScannedResponse.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                    dataAdapteScannedResponse.UpdateBatchSize = 50;
                    dataAdapteScannedResponse.InsertCommand.Connection = ConnectionBatch;
                    dataAdapteScannedResponse.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    dataAdapteScannedResponse.InsertCommand.CommandText = "PersistScannedResponseForMII";

                    #region Binding Command Parameters to stored procedure from DataTable

                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ScannedResponseId", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScannedResponseId");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ScannedFileName", System.Data.SqlDbType.NVarChar, 9999, "ScannedFileName");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ResponseString", System.Data.SqlDbType.NVarChar, 9999, "ResponseString");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@InsertedDate", System.Data.SqlDbType.DateTime, 200, "InsertedDate");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ISProcessed", System.Data.SqlDbType.Bit, sizeof(Boolean), "ISProcessed");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SheetID", System.Data.SqlDbType.NVarChar, 9999, "SheetID");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@LoginName", System.Data.SqlDbType.NVarChar, 9999, "LoginName");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SubjectCode", System.Data.SqlDbType.NVarChar, 9999, "SubjectCode");

                    #endregion Binding Command Parameters to stored procedure from DataTable

                    dataAdapteScannedResponse.Update(tblScannedResponse);
                }

                #endregion to build scannedresposne table

                Log.LogInfo("Persist OMRRespone Ends");
            }
        }

        public void DoOMRResponseProcessingForMIIClient()
        {
            using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
            {
                using (System.Data.SqlClient.SqlCommand Command = new SqlCommand())
                {
                    Command.CommandText = "SELECT LoginName,ResponseString,SubjectCode FROM ScannedResponse WHERE ISNULL(IsProcessed,0)=0";
                    Command.Connection = ConnectionBatch;
                    System.Data.SqlClient.SqlDataAdapter adap = new SqlDataAdapter(Command);
                    DataTable tblResponse = new DataTable();
                    adap.Fill(tblResponse);

                    if (tblResponse.Rows.Count > 0)
                    {
                        int Max = 0;
                        if (tblResponse.Rows.Count > 100)
                            Max = 100;
                        else
                            Max = tblResponse.Rows.Count;
                        for (int i = 0; i < Max; i++)
                        {
                            try
                            {
                                using (System.Data.SqlClient.SqlCommand CommandProcess = new System.Data.SqlClient.SqlCommand())
                                {
                                    if (ConnectionBatch.State == System.Data.ConnectionState.Closed)
                                        ConnectionBatch.Open();
                                    CommandProcess.Connection = ConnectionBatch;
                                    CommandProcess.CommandType = CommandType.StoredProcedure;
                                    CommandProcess.CommandText = "uspProcessOfflineScannedResponseForMII";
                                    CommandProcess.Parameters.AddWithValue("@LoginName", tblResponse.Rows[i]["LoginName"]);
                                    CommandProcess.Parameters.AddWithValue("@UserResponses", tblResponse.Rows[i]["ResponseString"]);
                                    CommandProcess.Parameters.AddWithValue("@AssessmentCode", tblResponse.Rows[i]["SubjectCode"]);
                                    //CommandProcess.Parameters.AddWithValue("@Assessmentcode", tblResponse.Rows[i]["SubjectCode"].ToString());
                                    using (System.Data.SqlClient.SqlDataAdapter sqlAdap = new SqlDataAdapter(CommandProcess))
                                    {
                                        System.Data.DataSet ds = new DataSet();
                                        sqlAdap.Fill(ds);
                                    }
                                }
                                ConnectionBatch.Close();
                            }
                            catch (System.Exception exe)
                            {
                                Log.LogError(exe.Message, exe);
                            }
                        }
                    }
                }
            }
        }

        public void UpsertAttachmentTransferredState(List<DataContracts.AttachmentDetails> lstAttachmentDetails)
        {
            Log.LogInfo("Upsert of AttachmentDetails Starts()");
            using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
            {
                #region to build scannedresposne table

                DataTable tblAttachmentDetails = new DataTable();
                tblAttachmentDetails.Columns.Add("AttachmentFileName");
                tblAttachmentDetails.Columns.Add("IsProcessed");
                tblAttachmentDetails.Columns.Add("Extension");

                using (System.Data.SqlClient.SqlDataAdapter dataAdapteScannedResponse = new System.Data.SqlClient.SqlDataAdapter("upsertAttachmentDetails", ConnectionBatch))
                {
                    foreach (DataContracts.AttachmentDetails objAttachmentDetails in lstAttachmentDetails)
                    {
                        System.Data.DataRow datarowAttachmentDetails = tblAttachmentDetails.NewRow();

                        #region Binding Values to PostinstallationComponetns DataTable

                        datarowAttachmentDetails["AttachmentFileName"] = objAttachmentDetails.AttachmentFileName;
                        datarowAttachmentDetails["IsProcessed"] = objAttachmentDetails.IsProcessed;
                        datarowAttachmentDetails["Extension"] = objAttachmentDetails.Extension;
                        tblAttachmentDetails.Rows.Add(datarowAttachmentDetails);

                        #endregion Binding Values to PostinstallationComponetns DataTable
                    }
                    dataAdapteScannedResponse.InsertCommand = new System.Data.SqlClient.SqlCommand();
                    dataAdapteScannedResponse.InsertCommand.CommandTimeout = commandTimeout;
                    dataAdapteScannedResponse.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                    dataAdapteScannedResponse.UpdateBatchSize = 50;
                    dataAdapteScannedResponse.InsertCommand.Connection = ConnectionBatch;
                    dataAdapteScannedResponse.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    dataAdapteScannedResponse.InsertCommand.CommandText = "UspUpdateEmailAttachment";

                    #region Binding Command Parameters to stored procedure from DataTable

                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 9999, "AttachmentFileName");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@IsProcessed", System.Data.SqlDbType.Bit, sizeof(Boolean), "IsProcessed");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@Extension", System.Data.SqlDbType.NVarChar, 9999, "Extension");

                    #endregion Binding Command Parameters to stored procedure from DataTable

                    dataAdapteScannedResponse.Update(tblAttachmentDetails);
                }

                #endregion to build scannedresposne table

                Log.LogInfo("Upsert of AttachmentDetails Ends()");
            }
        }

        public void UpsertAttachmentProcessedDetails(List<DataContracts.AttachmentProcessedDetails> lstAttachmentProcessDetails)
        {
            Log.LogInfo("Upsert of AttachmentProcessedDetails Starts()");
            using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
            {
                #region to build scannedresposne table

                DataTable tblAttachmentProcessDetails = new DataTable();
                tblAttachmentProcessDetails.Columns.Add("AttachmentFileName");
                // tblAttachmentProcessDetails.Columns.Add("AttachmentID");
                tblAttachmentProcessDetails.Columns.Add("ImageName");
                tblAttachmentProcessDetails.Columns.Add("ImageConvertedStatus");
                tblAttachmentProcessDetails.Columns.Add("ImageTransferredstatus");
                tblAttachmentProcessDetails.Columns.Add("ImageTransferedDate");
                tblAttachmentProcessDetails.Columns.Add("ImageProcessedStatus");
                tblAttachmentProcessDetails.Columns.Add("ImageProcessedDate");
                tblAttachmentProcessDetails.Columns.Add("AttachmentDetailStatus");

                using (System.Data.SqlClient.SqlDataAdapter dataAdapteScannedResponse = new System.Data.SqlClient.SqlDataAdapter("upsertAttachmentDetails", ConnectionBatch))
                {
                    foreach (DataContracts.AttachmentProcessedDetails objAttachmentProcessDetails in lstAttachmentProcessDetails)
                    {
                        System.Data.DataRow datarowAttachmentDetails = tblAttachmentProcessDetails.NewRow();

                        #region Binding Values to PostinstallationComponetns DataTable

                        datarowAttachmentDetails["AttachmentFileName"] = objAttachmentProcessDetails.AttachmentFileName;
                        // datarowAttachmentDetails["AttachmentID"] = objAttachmentProcessDetails.AttachmentID;
                        datarowAttachmentDetails["ImageName"] = objAttachmentProcessDetails.ImageName;
                        datarowAttachmentDetails["ImageConvertedStatus"] = objAttachmentProcessDetails.ImageConvertedStatus;

                        datarowAttachmentDetails["ImageTransferredstatus"] = objAttachmentProcessDetails.ImageTransferredStatus;
                        if (objAttachmentProcessDetails.ImageTransferedDate != null && objAttachmentProcessDetails.ImageTransferedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                            datarowAttachmentDetails["ImageTransferedDate"] = System.DBNull.Value;
                        else
                            datarowAttachmentDetails["ImageTransferedDate"] = objAttachmentProcessDetails.ImageTransferedDate;
                        datarowAttachmentDetails["ImageProcessedStatus"] = objAttachmentProcessDetails.ImageProcessedStatus;
                        if (objAttachmentProcessDetails.ImageProcessedDate != null && objAttachmentProcessDetails.ImageProcessedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                            datarowAttachmentDetails["ImageProcessedDate"] = System.DBNull.Value;
                        else
                            datarowAttachmentDetails["ImageProcessedDate"] = objAttachmentProcessDetails.ImageProcessedDate;
                        datarowAttachmentDetails["AttachmentDetailStatus"] = objAttachmentProcessDetails.AttachmentDetailStatus;
                        tblAttachmentProcessDetails.Rows.Add(datarowAttachmentDetails);

                        #endregion Binding Values to PostinstallationComponetns DataTable
                    }
                    dataAdapteScannedResponse.InsertCommand = new System.Data.SqlClient.SqlCommand();
                    dataAdapteScannedResponse.InsertCommand.CommandTimeout = commandTimeout;
                    dataAdapteScannedResponse.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                    dataAdapteScannedResponse.UpdateBatchSize = 50;
                    dataAdapteScannedResponse.InsertCommand.Connection = ConnectionBatch;
                    dataAdapteScannedResponse.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    dataAdapteScannedResponse.InsertCommand.CommandText = "UspUpsertEmailAttachmentDetails";

                    #region Binding Command Parameters to stored procedure from DataTable

                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@AttachmentFileName", System.Data.SqlDbType.NVarChar, 9999, "AttachmentFileName");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ImageConvertedStatus", System.Data.SqlDbType.TinyInt, sizeof(Byte), "ImageConvertedStatus");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ImageName", System.Data.SqlDbType.NVarChar, 9999, "ImageName");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ImageTransferredstatus", System.Data.SqlDbType.TinyInt, sizeof(Byte), "ImageTransferredstatus");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ImageTransferedDate", System.Data.SqlDbType.DateTime, 20, "ImageTransferedDate");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ImageProcessedstatus", System.Data.SqlDbType.TinyInt, sizeof(Byte), "ImageProcessedstatus");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ImageProcessedDate", System.Data.SqlDbType.DateTime, 20, "ImageProcessedDate");
                    dataAdapteScannedResponse.InsertCommand.Parameters.Add("@AttachmentDetailStatus", System.Data.SqlDbType.NVarChar, 9999, "AttachmentDetailStatus");

                    #endregion Binding Command Parameters to stored procedure from DataTable

                    dataAdapteScannedResponse.Update(tblAttachmentProcessDetails);
                }

                #endregion to build scannedresposne table

                Log.LogInfo("Upsert of AttachmentDetails Ends()");
            }
        }

        public void UpdateMobileBasedOMRPath(System.Data.DataTable tblOMR)
        {
            using (System.Data.SqlClient.SqlConnection Connection = GetConnection())
            {
                using (System.Data.SqlClient.SqlCommand Command = new SqlCommand())
                {
                    Command.CommandText = "UspUpdateOMRPath";
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Connection = Connection;
                    if (Connection.State == ConnectionState.Closed)
                    {
                        Connection.Open();
                    }

                    SqlParameter paraMeter = new SqlParameter();
                    paraMeter.ParameterName = "@tblOMR";
                    paraMeter.SqlDbType = SqlDbType.Structured;
                    paraMeter.Value = tblOMR;
                    Command.Parameters.Add(paraMeter);
                    Command.ExecuteNonQuery();
                    Connection.Close();
                }
            }
        }

        public ServiceContracts.CourseResponse UpsertCourse(List<DataContracts.Course> lstCourse)
        {
            Log.LogInfo("Begin CreateCustom User Count : " + (lstCourse == null ? "0" : lstCourse.Count.ToString()));
            ServiceContracts.CourseResponse response = new ServiceContracts.CourseResponse();
            try
            {
                if (lstCourse != null && lstCourse.Count > 0)
                {
                    using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                    {
                        #region Building DailyScheduleSummaryForUser DataTable

                        System.Data.DataTable datatableBatch = new System.Data.DataTable();
                        datatableBatch.Columns.Add("CourseID");
                        datatableBatch.Columns.Add("CourseName");
                        datatableBatch.Columns.Add("Coursetype");
                        datatableBatch.Columns.Add("Coursecode");
                        datatableBatch.Columns.Add("CreatedDate");
                        datatableBatch.Columns.Add("LastModifiedDate");

                        #endregion Building DailyScheduleSummaryForUser DataTable

                        using (System.Data.SqlClient.SqlDataAdapter dataAdapterBatch = new System.Data.SqlClient.SqlDataAdapter("Integration.uspPersistCourse", ConnectionBatch))
                        {
                            foreach (DataContracts.Course objCourse in lstCourse)
                            {
                                System.Data.DataRow datarowBatch = datatableBatch.NewRow();

                                #region Binding Values to Batch DataTable

                                datarowBatch["CourseID"] = objCourse.CourseID;
                                datarowBatch["CourseName"] = objCourse.CourseName;
                                datarowBatch["Coursetype"] = objCourse.CourseType;
                                datarowBatch["Coursecode"] = objCourse.CourseCode;

                                if (objCourse.CreatedDate != null && objCourse.CreatedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["CreatedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["CreatedDate"] = objCourse.CreatedDate;

                                if (objCourse.LastModifiedDate != null && objCourse.LastModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["LastModifiedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["LastModifiedDate"] = objCourse.LastModifiedDate;

                                #endregion Binding Values to Batch DataTable

                                datatableBatch.Rows.Add(datarowBatch);
                            }
                            dataAdapterBatch.InsertCommand = new System.Data.SqlClient.SqlCommand();
                            dataAdapterBatch.InsertCommand.CommandTimeout = commandTimeout;
                            dataAdapterBatch.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                            dataAdapterBatch.UpdateBatchSize = 50;
                            dataAdapterBatch.InsertCommand.Connection = ConnectionBatch;
                            dataAdapterBatch.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            dataAdapterBatch.InsertCommand.CommandText = "Integration.uspPersistCourse";

                            #region Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.InsertCommand.Parameters.Add("@CourseID", System.Data.SqlDbType.NVarChar, 2147483646, "CourseID");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@CourseName", System.Data.SqlDbType.NVarChar, 2147483646, "CourseName");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@CourseType", System.Data.SqlDbType.NVarChar, 2147483646, "CourseType");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@Coursecode", System.Data.SqlDbType.NVarChar, 2147483646, "Coursecode");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime, 200, "CreatedDate");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@LastmodifiedDate", System.Data.SqlDbType.DateTime, 200, "LastModifiedDate");

                            #endregion Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.Update(datatableBatch);
                        }
                    }
                }
                response.Status = "Success";
            }
            catch (System.Exception exe)
            {
                response.Status = "Failure: " + exe.Message;
            }

            Log.LogInfo("End UpdateBatchEndTime()");
            return response;
        }

        public ServiceContracts.CourseBlockResponse UpsertCourseBlock(List<DataContracts.CourseBlock> lstCourseBlock)
        {
            Log.LogInfo("Begin UpsertCourse Count : " + (lstCourseBlock == null ? "0" : lstCourseBlock.Count.ToString()));
            ServiceContracts.CourseBlockResponse response = new ServiceContracts.CourseBlockResponse();
            try
            {
                if (lstCourseBlock != null && lstCourseBlock.Count > 0)
                {
                    using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                    {
                        #region Building DailyScheduleSummaryForUser DataTable

                        System.Data.DataTable datatableBatch = new System.Data.DataTable();
                        datatableBatch.Columns.Add("CourseBlockID");
                        datatableBatch.Columns.Add("Courseid");
                        datatableBatch.Columns.Add("CourseBlockName");
                        datatableBatch.Columns.Add("instructorname");
                        datatableBatch.Columns.Add("location");
                        datatableBatch.Columns.Add("StartDate");
                        datatableBatch.Columns.Add("EndDate");
                        datatableBatch.Columns.Add("CreatedDate");
                        datatableBatch.Columns.Add("LastmodifiedDate");

                        #endregion Building DailyScheduleSummaryForUser DataTable

                        using (System.Data.SqlClient.SqlDataAdapter dataAdapterBatch = new System.Data.SqlClient.SqlDataAdapter("Integration.uspPersistCourseblock", ConnectionBatch))
                        {
                            foreach (DataContracts.CourseBlock objCourseBlock in lstCourseBlock)
                            {
                                System.Data.DataRow datarowBatch = datatableBatch.NewRow();

                                #region Binding Values to Batch DataTable

                                datarowBatch["CourseBlockID"] = objCourseBlock.CourseBlockID;
                                datarowBatch["CourseBlockName"] = objCourseBlock.CourseBlockName;
                                datarowBatch["Courseid"] = objCourseBlock.CourseID;
                                datarowBatch["InstructorName"] = objCourseBlock.InstructorName;
                                datarowBatch["Location"] = objCourseBlock.Location;
                                if (objCourseBlock.StartDate != null && objCourseBlock.StartDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["StartDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["StartDate"] = objCourseBlock.StartDate;

                                if (objCourseBlock.EndDate != null && objCourseBlock.EndDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["EndDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["EndDate"] = objCourseBlock.LastModifiedDate;

                                if (objCourseBlock.CreatedDate != null && objCourseBlock.CreatedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["CreatedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["CreatedDate"] = objCourseBlock.CreatedDate;

                                if (objCourseBlock.LastModifiedDate != null && objCourseBlock.LastModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["LastModifiedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["LastModifiedDate"] = objCourseBlock.LastModifiedDate;

                                #endregion Binding Values to Batch DataTable

                                datatableBatch.Rows.Add(datarowBatch);
                            }
                            dataAdapterBatch.InsertCommand = new System.Data.SqlClient.SqlCommand();
                            dataAdapterBatch.InsertCommand.CommandTimeout = commandTimeout;
                            dataAdapterBatch.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                            dataAdapterBatch.UpdateBatchSize = 50;
                            dataAdapterBatch.InsertCommand.Connection = ConnectionBatch;
                            dataAdapterBatch.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            dataAdapterBatch.InsertCommand.CommandText = "Integration.uspPersistCourseblock";

                            #region Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.InsertCommand.Parameters.Add("@CourseBlockID", System.Data.SqlDbType.NVarChar, 2147483646, "CourseBlockID");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@CourseID", System.Data.SqlDbType.NVarChar, 2147483646, "Courseid");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@CourseBlockName", System.Data.SqlDbType.NVarChar, 2147483646, "CourseBlockName");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@InstructorName", System.Data.SqlDbType.NVarChar, 2147483646, "InstructorName");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@Location", System.Data.SqlDbType.NVarChar, 2147483646, "Location");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime, 200, "StartDate");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime, 200, "EndDate");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime, 200, "CreatedDate");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@LastmodifiedDate", System.Data.SqlDbType.DateTime, 200, "LastModifiedDate");

                            #endregion Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.Update(datatableBatch);
                        }
                    }
                }
                response.Status = "Success";
            }
            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
                response.Status = "Failure: " + exe.Message;
            }

            Log.LogInfo("End UpdateBatchEndTime()");
            return response;
        }

        public ServiceContracts.ClassResponse UpsertClass(List<DataContracts.Class> lstClass)
        {
            Log.LogInfo("Begin UpsertClass Count : " + (lstClass == null ? "0" : lstClass.Count.ToString()));
            ServiceContracts.ClassResponse response = new ServiceContracts.ClassResponse();
            try
            {
                if (lstClass != null && lstClass.Count > 0)
                {
                    using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                    {
                        #region Building DailyScheduleSummaryForUser DataTable

                        System.Data.DataTable datatableBatch = new System.Data.DataTable();
                        datatableBatch.Columns.Add("ClassID");
                        datatableBatch.Columns.Add("Name");
                        datatableBatch.Columns.Add("Instructors");
                        datatableBatch.Columns.Add("instructorname");
                        datatableBatch.Columns.Add("StartDate");
                        datatableBatch.Columns.Add("EndDate");
                        datatableBatch.Columns.Add("Courseblockid");
                        datatableBatch.Columns.Add("Eventid");
                        datatableBatch.Columns.Add("Eventname");
                        datatableBatch.Columns.Add("Eventcode");
                        datatableBatch.Columns.Add("Trainingpartner");
                        datatableBatch.Columns.Add("CreatedDate");
                        datatableBatch.Columns.Add("LastmodifiedDate");

                        #endregion Building DailyScheduleSummaryForUser DataTable

                        using (System.Data.SqlClient.SqlDataAdapter dataAdapterBatch = new System.Data.SqlClient.SqlDataAdapter("Integration.uspPersistCourseClass", ConnectionBatch))
                        {
                            foreach (DataContracts.Class objClass in lstClass)
                            {
                                System.Data.DataRow datarowBatch = datatableBatch.NewRow();

                                #region Binding Values to Batch DataTable

                                datarowBatch["ClassID"] = objClass.ClassID;
                                datarowBatch["Name"] = objClass.Name;
                                datarowBatch["Instructors"] = objClass.Instructors;
                                datarowBatch["CourseBlockid"] = objClass.CourseBlockid;
                                datarowBatch["EventID"] = objClass.EventID;
                                datarowBatch["EventName"] = objClass.EventName;
                                datarowBatch["EventCode"] = objClass.EventCode;
                                datarowBatch["TrainingPartner"] = objClass.TrainingPartner;
                                if (objClass.StartDate != null && objClass.StartDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["StartDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["StartDate"] = objClass.StartDate;

                                if (objClass.EndDate != null && objClass.EndDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["EndDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["EndDate"] = objClass.EndDate;

                                if (objClass.CreatedDate != null && objClass.CreatedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["CreatedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["CreatedDate"] = objClass.CreatedDate;

                                if (objClass.LastModifiedDate != null && objClass.LastModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["LastModifiedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["LastModifiedDate"] = objClass.LastModifiedDate;

                                #endregion Binding Values to Batch DataTable

                                datatableBatch.Rows.Add(datarowBatch);
                            }
                            dataAdapterBatch.InsertCommand = new System.Data.SqlClient.SqlCommand();
                            dataAdapterBatch.InsertCommand.CommandTimeout = commandTimeout;
                            dataAdapterBatch.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                            dataAdapterBatch.UpdateBatchSize = 50;
                            dataAdapterBatch.InsertCommand.Connection = ConnectionBatch;
                            dataAdapterBatch.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            dataAdapterBatch.InsertCommand.CommandText = "Integration.uspPersistCourseClass";

                            #region Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.InsertCommand.Parameters.Add("@ClassID", System.Data.SqlDbType.NVarChar, 2147483646, "ClassID");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@ClassName", System.Data.SqlDbType.NVarChar, 2147483646, "ClassName");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@Instructor", System.Data.SqlDbType.NVarChar, 2147483646, "Instructors");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime, 200, "StartDate");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime, 200, "EndDate");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@CourseBlockID", System.Data.SqlDbType.NVarChar, 2147483646, "CourseBlockID");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@EventID", System.Data.SqlDbType.NVarChar, 2147483646, "EventID");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@EventName", System.Data.SqlDbType.NVarChar, 2147483646, "EventName");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@EventCode", System.Data.SqlDbType.NVarChar, 2147483646, "EventCode");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@TrainingPartner", System.Data.SqlDbType.NVarChar, 2147483646, "TrainingPartner");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime, 200, "CreatedDate");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@LastmodifiedDate", System.Data.SqlDbType.DateTime, 200, "LastModifiedDate");

                            #endregion Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.Update(datatableBatch);
                        }
                    }
                }
                response.Status = "Success";
            }
            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
                response.Status = "Failure: " + exe.Message;
            }

            Log.LogInfo("End UpdateBatchEndTime()");
            return response;
        }

        public ServiceContracts.CustomUserResponse UpsertCustomUser(List<DataContracts.CustomUser> lstUser)
        {
            Log.LogInfo("Begin UpsertCustomUser Count : " + (lstUser == null ? "0" : lstUser.Count.ToString()));
            ServiceContracts.CustomUserResponse response = new ServiceContracts.CustomUserResponse();
            try
            {
                if (lstUser != null && lstUser.Count > 0)
                {
                    using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                    {
                        #region Building DailyScheduleSummaryForUser DataTable

                        System.Data.DataTable datatableBatch = new System.Data.DataTable();
                        datatableBatch.Columns.Add("UserID");
                        datatableBatch.Columns.Add("FirstName");
                        datatableBatch.Columns.Add("LastName");
                        datatableBatch.Columns.Add("MiddleName");
                        datatableBatch.Columns.Add("Emailid");
                        datatableBatch.Columns.Add("Status");
                        datatableBatch.Columns.Add("LEP");
                        datatableBatch.Columns.Add("ELL");
                        datatableBatch.Columns.Add("LD");
                        datatableBatch.Columns.Add("PreferedLanguage");
                        datatableBatch.Columns.Add("LanguageFluentin");
                        datatableBatch.Columns.Add("CreatedDate");
                        datatableBatch.Columns.Add("LastmodifiedDate");

                        #endregion Building DailyScheduleSummaryForUser DataTable

                        using (System.Data.SqlClient.SqlDataAdapter dataAdapterBatch = new System.Data.SqlClient.SqlDataAdapter("Integration.uspPersistUser", ConnectionBatch))
                        {
                            foreach (DataContracts.CustomUser objUser in lstUser)
                            {
                                System.Data.DataRow datarowBatch = datatableBatch.NewRow();

                                #region Binding Values to Batch DataTable

                                datarowBatch["UserID"] = objUser.UserID;
                                datarowBatch["FirstName"] = objUser.FirstName;
                                datarowBatch["LastName"] = objUser.LastName;
                                datarowBatch["MiddleName"] = objUser.MiddleName;
                                datarowBatch["Emailid"] = objUser.Emailid;
                                datarowBatch["Status"] = objUser.Status;
                                datarowBatch["LEP"] = objUser.LEP;
                                datarowBatch["ELL"] = objUser.ELL;
                                datarowBatch["LD"] = objUser.Ld;
                                datarowBatch["PreferedLanguage"] = objUser.PreferedLanguage;
                                datarowBatch["LanguageFluentin"] = objUser.LanguageFluentin;

                                if (objUser.CreatedDate != null && objUser.CreatedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["CreatedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["CreatedDate"] = objUser.CreatedDate;

                                if (objUser.LastModifiedDate != null && objUser.LastModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["LastModifiedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["LastModifiedDate"] = objUser.LastModifiedDate;

                                #endregion Binding Values to Batch DataTable

                                datatableBatch.Rows.Add(datarowBatch);
                            }
                            dataAdapterBatch.InsertCommand = new System.Data.SqlClient.SqlCommand();
                            dataAdapterBatch.InsertCommand.CommandTimeout = commandTimeout;
                            dataAdapterBatch.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                            dataAdapterBatch.UpdateBatchSize = 50;
                            dataAdapterBatch.InsertCommand.Connection = ConnectionBatch;
                            dataAdapterBatch.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            dataAdapterBatch.InsertCommand.CommandText = "Integration.uspPersistUser";

                            #region Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.InsertCommand.Parameters.Add("@UserID", System.Data.SqlDbType.NVarChar, 2147483646, "UserID");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar, 2147483646, "FirstName");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar, 2147483646, "LastName");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@MiddleName", System.Data.SqlDbType.NVarChar, 2147483646, "MiddleName");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@EmailID", System.Data.SqlDbType.NVarChar, 2147483646, "EmailID");

                            dataAdapterBatch.InsertCommand.Parameters.Add("@Status", System.Data.SqlDbType.NVarChar, 2147483646, "Status");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@LEP", System.Data.SqlDbType.Bit, sizeof(Boolean), "LEP");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@ELL", System.Data.SqlDbType.NVarChar, 2147483646, "ELL");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@LD", System.Data.SqlDbType.NVarChar, 2147483646, "LD");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@PreferedLanguage", System.Data.SqlDbType.NVarChar, 2147483646, "PreferedLanguage");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@LanguageFluentin", System.Data.SqlDbType.NVarChar, 2147483646, "LanguageFluentin");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime, 200, "CreatedDate");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@LastmodifiedDate", System.Data.SqlDbType.DateTime, 200, "LastModifiedDate");

                            #endregion Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.Update(datatableBatch);
                        }
                    }
                }
                response.Status = "Success";
            }
            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
                response.Status = "Failure: " + exe.Message;
            }

            Log.LogInfo("End UpdateBatchEndTime()");
            return response;
        }

        public ServiceContracts.InstructorResponse UpsertInstructor(List<DataContracts.Instructor> lstInstructor)
        {
            Log.LogInfo("Begin UpsertInstructor Count : " + (lstInstructor == null ? "0" : lstInstructor.Count.ToString()));
            ServiceContracts.InstructorResponse response = new ServiceContracts.InstructorResponse();
            try
            {
                if (lstInstructor != null && lstInstructor.Count > 0)
                {
                    using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                    {
                        #region Building DailyScheduleSummaryForUser DataTable

                        System.Data.DataTable datatableBatch = new System.Data.DataTable();
                        datatableBatch.Columns.Add("InstructorID");
                        datatableBatch.Columns.Add("FirstName");
                        datatableBatch.Columns.Add("LastName");
                        datatableBatch.Columns.Add("MiddleName");
                        datatableBatch.Columns.Add("Emailid");
                        datatableBatch.Columns.Add("CreatedDate");
                        datatableBatch.Columns.Add("LastmodifiedDate");

                        #endregion Building DailyScheduleSummaryForUser DataTable

                        using (System.Data.SqlClient.SqlDataAdapter dataAdapterBatch = new System.Data.SqlClient.SqlDataAdapter("Integration.uspPersistInstructor", ConnectionBatch))
                        {
                            foreach (DataContracts.Instructor objInstructor in lstInstructor)
                            {
                                System.Data.DataRow datarowBatch = datatableBatch.NewRow();

                                #region Binding Values to Batch DataTable

                                datarowBatch["InstructorID"] = objInstructor.InstructorID;
                                datarowBatch["FirstName"] = objInstructor.FirstName;
                                datarowBatch["LastName"] = objInstructor.LastName;
                                datarowBatch["MiddleName"] = objInstructor.MiddleName;
                                datarowBatch["Emailid"] = objInstructor.EmailID;
                                if (objInstructor.CreatedDate != null && objInstructor.CreatedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["CreatedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["CreatedDate"] = objInstructor.CreatedDate;

                                if (objInstructor.LastModifiedDate != null && objInstructor.LastModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["LastModifiedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["LastModifiedDate"] = objInstructor.LastModifiedDate;

                                #endregion Binding Values to Batch DataTable

                                datatableBatch.Rows.Add(datarowBatch);
                            }
                            dataAdapterBatch.InsertCommand = new System.Data.SqlClient.SqlCommand();
                            dataAdapterBatch.InsertCommand.CommandTimeout = commandTimeout;
                            dataAdapterBatch.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                            dataAdapterBatch.UpdateBatchSize = 50;
                            dataAdapterBatch.InsertCommand.Connection = ConnectionBatch;
                            dataAdapterBatch.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            dataAdapterBatch.InsertCommand.CommandText = "Integration.uspPersistInstructor";

                            #region Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.InsertCommand.Parameters.Add("@InstructorID", System.Data.SqlDbType.NVarChar, 2147483646, "InstructorID");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar, 2147483646, "FirstName");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar, 2147483646, "LastName");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@MiddleName", System.Data.SqlDbType.NVarChar, 2147483646, "MiddleName");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@EmailID", System.Data.SqlDbType.NVarChar, 2147483646, "EmailID");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime, 200, "CreatedDate");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@LastmodifiedDate", System.Data.SqlDbType.DateTime, 200, "LastModifiedDate");

                            #endregion Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.Update(datatableBatch);
                        }
                    }
                }
                response.Status = "Success";
            }
            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
                response.Status = "Failure: " + exe.Message;
            }

            Log.LogInfo("End UpdateBatchEndTime()");
            return response;
        }

        public ServiceContracts.RosterResponse UpsertRoster(List<DataContracts.Roster> lstRoster)
        {
            Log.LogInfo("Begin UpsertInstructor Count : " + (lstRoster == null ? "0" : lstRoster.Count.ToString()));
            ServiceContracts.RosterResponse response = new ServiceContracts.RosterResponse();
            try
            {
                if (lstRoster != null && lstRoster.Count > 0)
                {
                    using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                    {
                        #region Building DailyScheduleSummaryForUser DataTable

                        System.Data.DataTable datatableBatch = new System.Data.DataTable();
                        datatableBatch.Columns.Add("RosterID");
                        datatableBatch.Columns.Add("ClassID");
                        datatableBatch.Columns.Add("Status");
                        datatableBatch.Columns.Add("Userid");
                        datatableBatch.Columns.Add("CreatedDate");
                        datatableBatch.Columns.Add("LastmodifiedDate");

                        #endregion Building DailyScheduleSummaryForUser DataTable

                        using (System.Data.SqlClient.SqlDataAdapter dataAdapterBatch = new System.Data.SqlClient.SqlDataAdapter("Integration.uspPersistRoster", ConnectionBatch))
                        {
                            foreach (DataContracts.Roster objRoster in lstRoster)
                            {
                                System.Data.DataRow datarowBatch = datatableBatch.NewRow();

                                #region Binding Values to Batch DataTable

                                datarowBatch["RosterID"] = objRoster.RosterID;
                                datarowBatch["ClassID"] = objRoster.ClassID;
                                datarowBatch["Status"] = objRoster.Status;
                                datarowBatch["Userid"] = objRoster.UserID;

                                if (objRoster.CreatedDate != null && objRoster.CreatedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["CreatedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["CreatedDate"] = objRoster.CreatedDate;

                                if (objRoster.LastModifiedDate != null && objRoster.LastModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowBatch["LastModifiedDate"] = System.DBNull.Value;
                                else
                                    datarowBatch["LastModifiedDate"] = objRoster.LastModifiedDate;

                                #endregion Binding Values to Batch DataTable

                                datatableBatch.Rows.Add(datarowBatch);
                            }
                            dataAdapterBatch.InsertCommand = new System.Data.SqlClient.SqlCommand();
                            dataAdapterBatch.InsertCommand.CommandTimeout = commandTimeout;
                            dataAdapterBatch.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                            dataAdapterBatch.UpdateBatchSize = 50;
                            dataAdapterBatch.InsertCommand.Connection = ConnectionBatch;
                            dataAdapterBatch.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            dataAdapterBatch.InsertCommand.CommandText = "Integration.uspPersistRoster";

                            #region Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.InsertCommand.Parameters.Add("@RosterID", System.Data.SqlDbType.NVarChar, 2147483646, "RosterID");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@UserID", System.Data.SqlDbType.NVarChar, 2147483646, "UserID");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@Status", System.Data.SqlDbType.NVarChar, 2147483646, "Status");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@ClassID", System.Data.SqlDbType.NVarChar, 2147483646, "ClassID");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime, 200, "CreatedDate");
                            dataAdapterBatch.InsertCommand.Parameters.Add("@LastmodifiedDate", System.Data.SqlDbType.DateTime, 200, "LastModifiedDate");

                            #endregion Binding Command Parameters to stored procedure from DataTable

                            dataAdapterBatch.Update(datatableBatch);
                        }
                    }
                }
                response.Status = "Success";
            }
            catch (System.Exception exe)
            {
                Log.LogError(exe.Message, exe);
                response.Status = "Failure: " + exe.Message;
            }

            Log.LogInfo("End UpdateBatchEndTime()");
            return response;
        }

        public void DOResponseProcessingFORNYCS()
        {
            using (System.Data.SqlClient.SqlConnection Connection = CommonDAL.GetConnection())
            {
                System.Data.SqlClient.SqlCommand Command = new System.Data.SqlClient.SqlCommand();
                Command.Connection = Connection;
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();
                Command.CommandTimeout = 0;
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "UspProcessOMRResponseForNYCS";
                Command.ExecuteNonQuery();
                Connection.Close();
            }
        }

        public void ETLAppErrorLogs(ServiceContracts.Contracts.ETLAppErrorLogsRequest request)
        {
            using (System.Data.SqlClient.SqlConnection Connection = CommonDAL.GetConnection())
            {
                System.Data.SqlClient.SqlCommand Command = new System.Data.SqlClient.SqlCommand();
                Command.Connection = Connection;
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();
                Command.CommandTimeout = 0;
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "USPInsertErrorLogs";
                Command.Parameters.Add(new SqlParameter("@LogDatetime", request.logDate));
                Command.Parameters.Add(new SqlParameter("@LogLevel", request.logLevel));
                Command.Parameters.Add(new SqlParameter("@Logger", request.logger));
                Command.Parameters.Add(new SqlParameter("@Message", request.message));
                Command.Parameters.Add(new SqlParameter("@Exception", request.exception));
                Command.Parameters.Add(new SqlParameter("@SessionID", request.sessionid));
                Command.Parameters.Add(new SqlParameter("@NodeID", request.nodeid));
                Command.Parameters.Add(new SqlParameter("@UserID", request.userid));
                Command.Parameters.Add(new SqlParameter("@MACID", request.MACID));

                Command.ExecuteNonQuery();
                Connection.Close();
            }
        }

        public void MSIInstallationResult(ServiceContracts.Contracts.MSIInstallationResultRequest request)
        {
            using (System.Data.SqlClient.SqlConnection Connection = CommonDAL.GetConnection())
            {
                System.Data.SqlClient.SqlCommand Command = new System.Data.SqlClient.SqlCommand();
                Command.Connection = Connection;
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();
                Command.CommandTimeout = 0;
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "UspInsertTestCenterInstallationDetails";
                Command.Parameters.Add(new SqlParameter("@MACID", request.macId));
                Command.Parameters.Add(new SqlParameter("@IsDeleted", request.isDeleted));
                Command.Parameters.Add(new SqlParameter("@IsInstalled", request.isInstalled));
                Command.Parameters.Add(new SqlParameter("@Extension1", request.extnMessage));
                Command.Parameters.Add(new SqlParameter("@ISFullversionMSI", request.isFullMSI));
                Command.ExecuteNonQuery();
                Connection.Close();
            }
        }

        public void SaveMediaPackageLoadedDetails(ServiceContracts.Contracts.MediaPackageInfoRequest request)
        {
            using (System.Data.SqlClient.SqlConnection Connection = CommonDAL.GetConnection())
            {
                System.Data.SqlClient.SqlCommand Command = new System.Data.SqlClient.SqlCommand();
                Command.Connection = Connection;
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();
                Command.CommandTimeout = 0;
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = "UspInsertLoadedMediaPackageDetails";
                Command.Parameters.Add(new SqlParameter("@MACID", request.macId));
                Command.Parameters.Add(new SqlParameter("@Extension1", request.extnMessage));
                Command.Parameters.Add(new SqlParameter("@MediaPackageName", request.mediaPkgName));
                Command.Parameters.Add(new SqlParameter("@TestTypeMasterID", request.TestTypeMasterID));
                Command.ExecuteNonQuery();
                Connection.Close();
            }
        }

        public List<DataContracts.OMRScannedResponseOuput> PersistOfflineScannedResponseForNYCS(List<DataContracts.ScannedResponse> listScannedResponse)
        {
            #region to insert to log table

            try
            {
                if (listScannedResponse[0].ISLogRequired)//checking first record to see service log required or not
                {
                    string requestobject, responseobject;
                    foreach (DataContracts.ScannedResponse objScannedResponse in listScannedResponse)
                    {
                        requestobject = "Request object:LoginName:" + objScannedResponse.LoginName + "ResponseString" + objScannedResponse.ResponseString + "Subjectcode:" + objScannedResponse.SubjectCode + "SheetID:" + objScannedResponse.SheetID;

                        using (System.Data.SqlClient.SqlConnection Connection = CommonDAL.GetConnection())
                        {
                            using (System.Data.SqlClient.SqlCommand Command = new SqlCommand())
                            {
                                Command.Connection = Connection;
                                if (Connection.State == ConnectionState.Closed)
                                    Connection.Open();
                                Command.CommandType = CommandType.StoredProcedure;
                                Command.CommandText = "UspUpsertWebServiceLog";
                                Command.Parameters.AddWithValue("@Module", "SCANTRAYOMR");
                                Command.Parameters.AddWithValue("@MethodType", "SCANTRAYOMR");
                                Command.Parameters.AddWithValue("@StartDate", System.DateTime.UtcNow);
                                Command.Parameters.AddWithValue("@EndDate", System.DateTime.UtcNow);
                                Command.Parameters.AddWithValue("@RequestObject", requestobject);
                                Command.ExecuteNonQuery();
                                Connection.Close();
                            }
                        }
                    }
                }
            }
            catch (System.Exception exe)
            {
                Log.LogInfo("Error:", exe);
            }

            #endregion to insert to log table

            #region to update omr data

            List<DataContracts.OMRScannedResponseOuput> listOMRScannedResponse = new List<DataContracts.OMRScannedResponseOuput>();
            try
            {
                Log.LogInfo("--- Begin Persist Scanned Response Starts---");

                #region PersistScannedResponse

                using (System.Data.SqlClient.SqlConnection ConnectionBatch = CommonDAL.GetConnection())
                {
                    #region to build scannedresposne table

                    DataTable tblScannedResponse = new DataTable();
                    tblScannedResponse.Columns.Add("ScannedResponseId");
                    tblScannedResponse.Columns.Add("ScannedFileName");
                    tblScannedResponse.Columns.Add("ResponseString");
                    tblScannedResponse.Columns.Add("InsertedDate");
                    tblScannedResponse.Columns.Add("ISProcessed");
                    tblScannedResponse.Columns.Add("SheetID");
                    tblScannedResponse.Columns.Add("LoginName");
                    tblScannedResponse.Columns.Add("SubjectCode");
                    tblScannedResponse.Columns.Add("PageNumber");
                    tblScannedResponse.Columns.Add("PageSize");
                    tblScannedResponse.Columns.Add("Extension1");
                    tblScannedResponse.Columns.Add("ISEncrypted");
                    tblScannedResponse.Columns.Add("ISLDS");
                    tblScannedResponse.Columns.Add("HoldingDate");
                    tblScannedResponse.Columns.Add("SSN");
                    tblScannedResponse.Columns.Add("BatchNumber");
                    tblScannedResponse.Columns.Add("SequenceNumber");

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapteScannedResponse = new System.Data.SqlClient.SqlDataAdapter("PersistScannedResponse", ConnectionBatch))
                    {
                        foreach (DataContracts.ScannedResponse objScannedResponse in listScannedResponse)
                        {
                            System.Data.DataRow datarowScannedResponse = tblScannedResponse.NewRow();

                            #region Binding Values to PostinstallationComponetns DataTable

                            if (!objScannedResponse.ISEncrypted)
                            {
                                datarowScannedResponse["ScannedResponseId"] = objScannedResponse.ScannedResponseId;
                                datarowScannedResponse["ScannedFileName"] = objScannedResponse.ScannedFileName;
                                datarowScannedResponse["ResponseString"] = objScannedResponse.ResponseString;
                                datarowScannedResponse["ISProcessed"] = objScannedResponse.ISProcessed;
                                datarowScannedResponse["SheetID"] = objScannedResponse.SheetID;
                                datarowScannedResponse["LoginName"] = objScannedResponse.LoginName;

                                if (objScannedResponse.InsertedDate != null && objScannedResponse.InsertedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowScannedResponse["InsertedDate"] = System.DBNull.Value;
                                else
                                    datarowScannedResponse["InsertedDate"] = objScannedResponse.InsertedDate;
                                datarowScannedResponse["SubjectCode"] = objScannedResponse.SubjectCode;
                                datarowScannedResponse["PageNumber"] = objScannedResponse.PageNumber;
                                datarowScannedResponse["PageSize"] = objScannedResponse.PageSize;
                                datarowScannedResponse["Extension1"] = objScannedResponse.Extension1;
                                if (objScannedResponse.ISLDS != null)
                                    datarowScannedResponse["ISLDS"] = objScannedResponse.ISLDS;
                                datarowScannedResponse["BatchNumber"] = objScannedResponse.BatchNumber;
                                datarowScannedResponse["SequenceNumber"] = objScannedResponse.SequenceNumber;
                                datarowScannedResponse["SSN"] = objScannedResponse.SSN;
                                datarowScannedResponse["HoldingDate"] = objScannedResponse.ExamSeriesDate;

                                tblScannedResponse.Rows.Add(datarowScannedResponse);
                            }
                            else
                            {
                                KeySecurity Security = new KeySecurity();
                                Log.LogInfo("Encrypted response:");
                                datarowScannedResponse["ScannedResponseId"] = objScannedResponse.ScannedResponseId;
                                datarowScannedResponse["ScannedFileName"] = Security.Decrypt(objScannedResponse.ScannedFileName);
                                datarowScannedResponse["ResponseString"] = Security.Decrypt(objScannedResponse.ResponseString);
                                datarowScannedResponse["ISProcessed"] = objScannedResponse.ISProcessed;
                                datarowScannedResponse["SheetID"] = Security.Decrypt(objScannedResponse.SheetID);
                                datarowScannedResponse["LoginName"] = Security.Decrypt(objScannedResponse.LoginName);

                                if (objScannedResponse.InsertedDate != null && objScannedResponse.InsertedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowScannedResponse["InsertedDate"] = System.DBNull.Value;
                                else
                                    datarowScannedResponse["InsertedDate"] = objScannedResponse.InsertedDate;
                                datarowScannedResponse["SubjectCode"] = Security.Decrypt(objScannedResponse.SubjectCode);
                                datarowScannedResponse["PageNumber"] = objScannedResponse.PageNumber;
                                datarowScannedResponse["PageSize"] = objScannedResponse.PageSize;
                                datarowScannedResponse["Extension1"] = Security.Decrypt(objScannedResponse.Extension1);
                                if (objScannedResponse.ISLDS != null)
                                    datarowScannedResponse["ISLDS"] = objScannedResponse.ISLDS;
                                datarowScannedResponse["BatchNumber"] = objScannedResponse.BatchNumber;
                                datarowScannedResponse["SequenceNumber"] = objScannedResponse.SequenceNumber;
                                datarowScannedResponse["SSN"] = objScannedResponse.SSN;
                                if (objScannedResponse.ExamSeriesDate != null && objScannedResponse.ExamSeriesDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                    datarowScannedResponse["HoldingDate"] = System.DBNull.Value;
                                else
                                    datarowScannedResponse["HoldingDate"] = objScannedResponse.ExamSeriesDate;
                                tblScannedResponse.Rows.Add(datarowScannedResponse);
                                Log.LogInfo("Encrypted response ends:");
                            }

                            #endregion Binding Values to PostinstallationComponetns DataTable
                        }
                        Log.LogInfo("---End Persist Scanned Response ---");
                        dataAdapteScannedResponse.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapteScannedResponse.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapteScannedResponse.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapteScannedResponse.UpdateBatchSize = 50;
                        dataAdapteScannedResponse.InsertCommand.Connection = ConnectionBatch;
                        dataAdapteScannedResponse.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapteScannedResponse.InsertCommand.CommandText = "PersistScannedResponse";

                        #region Binding Command Parameters to stored procedure from DataTable

                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ScannedResponseId", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScannedResponseId");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ScannedFileName", System.Data.SqlDbType.NVarChar, 9999, "ScannedFileName");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ResponseString", System.Data.SqlDbType.NVarChar, 9999, "ResponseString");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@InsertedDate", System.Data.SqlDbType.DateTime, 200, "InsertedDate");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ISProcessed", System.Data.SqlDbType.Bit, sizeof(Boolean), "ISProcessed");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SheetID", System.Data.SqlDbType.NVarChar, 9999, "SheetID");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@LoginName", System.Data.SqlDbType.NVarChar, 9999, "LoginName");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SubjectCode", System.Data.SqlDbType.NVarChar, 9999, "SubjectCode");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@PageNumber", System.Data.SqlDbType.BigInt, sizeof(Int64), "PageNumber");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@PageSize", System.Data.SqlDbType.BigInt, sizeof(Int64), "PageSize");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@Extension1", System.Data.SqlDbType.NVarChar, 9999, "Extension1");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@ISLDS", System.Data.SqlDbType.TinyInt, sizeof(Int16), "ISLDS");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@BatchNumber", System.Data.SqlDbType.SmallInt, sizeof(Int64), "BatchNumber");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SequenceNumber", System.Data.SqlDbType.BigInt, sizeof(Int64), "SequenceNumber");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@SSN", System.Data.SqlDbType.NVarChar, 999, "SSN");
                        dataAdapteScannedResponse.InsertCommand.Parameters.Add("@HoldingDate", System.Data.SqlDbType.DateTime, 200, "HoldingDate");

                        #endregion Binding Command Parameters to stored procedure from DataTable

                        dataAdapteScannedResponse.Update(tblScannedResponse);

                        #endregion to build scannedresposne table
                    }

                    #endregion PersistScannedResponse

                    Log.LogInfo("---End Persist Scanned Response ---");
                    Log.LogInfo("--- Response Processing Starts For Offline---");
                    if (listScannedResponse[0].ISLDS == 1)
                        listOMRScannedResponse = DoResponseProcessForOfflineScannedTest(listScannedResponse[0].LoginName);
                    else if (listScannedResponse[0].ISLDS == 2)
                        DOResponseProcessingFORNYCS();
                }
            }

            #endregion to update omr data

            catch (System.Exception exe)
            {
                Log.LogInfo("Error:", exe);
                throw exe;
            }
            return listOMRScannedResponse;
        }

        public ServiceContracts.GetLogResponseType FetchLogRequest(string macid)
        {
            ServiceContracts.GetLogResponseType responseType = new ServiceContracts.GetLogResponseType();
            responseType.Response = new DataContracts.GetListOfLogResponse();

            using (System.Data.SqlClient.SqlConnection sqlConnection = CommonDAL.GetConnection())
            {
                sqlConnection.Open();
                string commandText = "uspGetTestCenterLogDetails";
                using (System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(commandText, sqlConnection))
                {
                    sqlCommand.CommandText = "uspGetTestCenterLogDetails";
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(CommonDAL.BuildSqlParameter("MacID", System.Data.SqlDbType.NVarChar, 999999, "MacID", macid, System.Data.ParameterDirection.Input));
                    using (System.Data.SqlClient.SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        //ScriptList.Scripts = new List<DataContracts.GetNonExecutedScriptsResponse>();
                        int Index = 0;
                        if (dataReader.Read())
                        {
                            //DataContracts.GetNonExecutedScriptsResponse response = new DataContracts.GetNonExecutedScriptsResponse();

                            #region Reading data from Record Set

                            Log.LogInfo("TestCenterLogDetailID");
                            Index = dataReader.GetOrdinal("TestCenterLogDetailID");
                            if (!dataReader.IsDBNull(Index))
                            {
                                responseType.Response.TestCenterLogDetailID = dataReader.GetInt64(Index);
                            }
                            Log.LogInfo("TestCenterCode");
                            Index = dataReader.GetOrdinal("TestCenterCode");
                            if (!dataReader.IsDBNull(Index))
                            {
                                responseType.Response.TestCenterCode = dataReader.GetString(Index);
                            }
                            Log.LogInfo("FromDate");
                            Index = dataReader.GetOrdinal("FromDate");
                            if (!dataReader.IsDBNull(Index))
                            {
                                responseType.Response.FromDate = dataReader.GetDateTime(Index);
                            }
                            Log.LogInfo("todate");
                            Index = dataReader.GetOrdinal("ToDate");
                            if (!dataReader.IsDBNull(Index))
                            {
                                responseType.Response.TODate = dataReader.GetDateTime(Index);
                            }
                            Log.LogInfo("IsSystemLogsRequired");
                            Index = dataReader.GetOrdinal("IsSystemLogsRequired");
                            if (!dataReader.IsDBNull(Index))
                            {
                                responseType.Response.ISSystemLogRequired = dataReader.GetBoolean(Index);
                            }
                            Log.LogInfo("IsapplicationLogsRequired");
                            Index = dataReader.GetOrdinal("IsApplicationLogRequired");
                            if (!dataReader.IsDBNull(Index))
                            {
                                responseType.Response.ISApplicationLogRequired = dataReader.GetBoolean(Index);
                            }
                            Log.LogInfo("IsCronLogRsRequired");
                            Index = dataReader.GetOrdinal("IsCronLogRsRequired");
                            if (!dataReader.IsDBNull(Index))
                            {
                                responseType.Response.ISCronLogRequired = dataReader.GetBoolean(Index);
                            }

                            #endregion Reading data from Record Set
                        }
                    }
                }
            }

            return responseType;
        }

        public string UpsertLogResposne(ServiceContracts.LogoutputRequestType requestType)
        {
            using (System.Data.SqlClient.SqlConnection Connection = GetConnection())
            {
                using (System.Data.SqlClient.SqlCommand command = new SqlCommand())
                {
                    command.Connection = Connection;
                    if (Connection.State == ConnectionState.Closed)
                        Connection.Open();
                    command.Parameters.AddWithValue("TestCenterLogDetailID", requestType.Request.TestCenterLogDetailID);
                    command.Parameters.AddWithValue("LogFilePath", requestType.Request.LogFilePath);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "uspUpdateTestCenterLogDetails";
                    command.ExecuteNonQuery();
                }
            }
            return "S001";
        }
    }

    public class TestCenterFactory : Generic
    {
        private int commandTimeout = 0;

        public TestCenterFactory()
        {
            commandTimeout = 2147483647;
        }

        public void UpdateTestCenters(System.Collections.Generic.List<DataContracts.TestCenter> listTestCenter)
        {
            Log.LogInfo("Begin UpdateTestCenters()" + " - Test Center Count: " + ((listTestCenter == null) ? "null" : listTestCenter.Count.ToString()));
            if (listTestCenter != null && listTestCenter.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionTestCenter = CommonDAL.GetConnection())
                {
                    #region Building TestCenter DataTable

                    System.Data.DataTable datatableTestCenter = new System.Data.DataTable();
                    datatableTestCenter.Columns.Add("MACID");
                    datatableTestCenter.Columns.Add("CenterName");
                    datatableTestCenter.Columns.Add("CenterCode");
                    datatableTestCenter.Columns.Add("IsAvailable");
                    datatableTestCenter.Columns.Add("AddressID");
                    datatableTestCenter.Columns.Add("CreatedBy");
                    datatableTestCenter.Columns.Add("CreatedDate");
                    datatableTestCenter.Columns.Add("ModifiedBy");
                    datatableTestCenter.Columns.Add("ModifiedDate");
                    datatableTestCenter.Columns.Add("ParentID");
                    datatableTestCenter.Columns.Add("LocationID");
                    datatableTestCenter.Columns.Add("ID");
                    datatableTestCenter.Columns.Add("OrganizationID");
                    datatableTestCenter.Columns.Add("Status");
                    datatableTestCenter.Columns.Add("IsDeleted");
                    datatableTestCenter.Columns.Add("CenterTypeId");

                    #endregion Building TestCenter DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterTestCenter = new System.Data.SqlClient.SqlDataAdapter("PersistTestCenter", connectionTestCenter))
                    {
                        foreach (DataContracts.TestCenter objTestCenter in listTestCenter)
                        {
                            System.Data.DataRow datarowTestCenter = datatableTestCenter.NewRow();

                            #region Binding Values to TestCenter DataTable

                            datarowTestCenter["ID"] = objTestCenter.ID;
                            datarowTestCenter["MACID"] = objTestCenter.MacID;
                            datarowTestCenter["CenterName"] = objTestCenter.CenterName;
                            datarowTestCenter["CenterCode"] = objTestCenter.CenterCode;
                            datarowTestCenter["IsAvailable"] = objTestCenter.IsAvailable;
                            //if (objTestCenter.AddressID <= 0)
                            datarowTestCenter["AddressID"] = System.DBNull.Value;
                            //else
                            //  datarowTestCenter["AddressID"] = objTestCenter.AddressID;
                            //if (objTestCenter.CreatedBy <= 0)
                            //  datarowTestCenter["CreatedBy"] = System.DBNull.Value;
                            //else
                            datarowTestCenter["CreatedBy"] = 1;// objTestCenter.CreatedBy;
                            if (objTestCenter.CreatedDate != null && objTestCenter.CreatedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowTestCenter["CreatedDate"] = System.DBNull.Value;
                            else
                                datarowTestCenter["CreatedDate"] = objTestCenter.CreatedDate;
                            //if (objTestCenter.ModifiedBy <= 0)
                            //datarowTestCenter["ModifiedBy"] = System.DBNull.Value;
                            //else
                            datarowTestCenter["ModifiedBy"] = 1;// objTestCenter.ModifiedBy;
                            if (objTestCenter.ModifiedDate != null && objTestCenter.ModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowTestCenter["ModifiedDate"] = System.DBNull.Value;
                            else
                                datarowTestCenter["ModifiedDate"] = objTestCenter.ModifiedDate;
                            if (objTestCenter.ParentID <= 0)
                                datarowTestCenter["ParentID"] = System.DBNull.Value;
                            else
                                datarowTestCenter["ParentID"] = objTestCenter.ParentID;
                            if (objTestCenter.LocationID <= 0)
                                datarowTestCenter["LocationID"] = System.DBNull.Value;
                            else
                                datarowTestCenter["LocationID"] = objTestCenter.LocationID;
                            datarowTestCenter["OrganizationID"] = objTestCenter.OrganizationID;
                            datarowTestCenter["IsDeleted"] = objTestCenter.IsDeleted;
                            datarowTestCenter["CenterTypeId"] = objTestCenter.CenterTypeId;
                            datatableTestCenter.Rows.Add(datarowTestCenter);

                            #endregion Binding Values to TestCenter DataTable
                        }
                        dataAdapterTestCenter.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterTestCenter.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterTestCenter.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.OutputParameters;
                        dataAdapterTestCenter.UpdateBatchSize = 50;
                        dataAdapterTestCenter.InsertCommand.CommandText = "PersistTestCenter";
                        dataAdapterTestCenter.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterTestCenter.InsertCommand.Connection = connectionTestCenter;

                        #region Binding Command Parameters to stored procedure from TestCenter DataTable

                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@ID", System.Data.SqlDbType.BigInt, sizeof(System.Int64), "ID");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@MACID", System.Data.SqlDbType.NVarChar, 2147483646, "MACID");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@CenterName", System.Data.SqlDbType.NVarChar, 2147483646, "CenterName");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@CenterCode", System.Data.SqlDbType.NVarChar, 2147483646, "CenterCode");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@IsAvailable", System.Data.SqlDbType.Bit, sizeof(bool), "IsAvailable");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@AddressID", System.Data.SqlDbType.BigInt, sizeof(Int64), "AddressID");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@CreatedBy", System.Data.SqlDbType.BigInt, sizeof(Int64), "CreatedBy");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime, 20, "CreatedDate");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@ModifiedBy", System.Data.SqlDbType.BigInt, sizeof(Int64), "ModifiedBy");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@ModifiedDate", System.Data.SqlDbType.DateTime, 20, "ModifiedDate");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@ParentID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ParentID");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@LocationID", System.Data.SqlDbType.BigInt, sizeof(Int64), "LocationID");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@OrganizationID", System.Data.SqlDbType.BigInt, sizeof(Int64), "OrganizationID");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@IsDeleted", System.Data.SqlDbType.Bit, sizeof(bool), "IsDeleted");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@CenterTypeId", System.Data.SqlDbType.Int, sizeof(Int32), "CenterTypeId");
                        dataAdapterTestCenter.InsertCommand.Parameters.Add("@Status", System.Data.SqlDbType.NVarChar, -1, "Status").Direction = System.Data.ParameterDirection.Output;

                        #endregion Binding Command Parameters to stored procedure from TestCenter DataTable

                        dataAdapterTestCenter.RowUpdated += new System.Data.SqlClient.SqlRowUpdatedEventHandler(dataAdapterTestCenter_RowUpdated);
                        dataAdapterTestCenter.Update(datatableTestCenter);
                    }
                    if (datatableTestCenter != null && datatableTestCenter.Rows != null && datatableTestCenter.Rows.Count > 0)
                    {
                        for (int i = 0; i < datatableTestCenter.Rows.Count; i++)
                        {
                            Log.LogInfo("Test Center Name - " + listTestCenter[i].CenterName + ", Test Center MACID - " + listTestCenter[i].MacID + ", Status - " + listTestCenter[i].Status);
                            listTestCenter[i].Status = datatableTestCenter.Rows[i]["Status"].ToString();
                        }
                    }
                }
            }
            Log.LogInfo("End UpdateTestCenters()");
        }

        private void dataAdapterTestCenter_RowUpdated(object sender, System.Data.SqlClient.SqlRowUpdatedEventArgs e)
        {
            if (e.Command.Parameters["@Status"] != null)
            {
            }
        }

        public void UpdateTagUserToTestCenter(DataContracts.TagTestCenterToUser TestCenter)
        {
            Log.LogInfo("Begin UpdateTagUserToTestCenter()" + " - UserID : " + TestCenter.UserID + "TestCenterID" + TestCenter.TestCenterID);

            using (System.Data.SqlClient.SqlConnection connectionTestCenter = CommonDAL.GetConnection())
            {
                #region Building TestCenter DataTable

                System.Data.DataTable datatableTestCenter = new System.Data.DataTable();
                datatableTestCenter.Columns.Add("ID");
                datatableTestCenter.Columns.Add("TestCenterID");
                datatableTestCenter.Columns.Add("UserID");
                datatableTestCenter.Columns.Add("IsDeleted");
                datatableTestCenter.Columns.Add("IsTransfered");

                #endregion Building TestCenter DataTable

                using (System.Data.SqlClient.SqlDataAdapter dataAdapterTestCenter = new System.Data.SqlClient.SqlDataAdapter("UpsertTestCenterUserMapping", connectionTestCenter))
                {
                    System.Data.DataRow datarowTestCenter = datatableTestCenter.NewRow();

                    #region Binding Values to TestCenter DataTable

                    datarowTestCenter["ID"] = TestCenter.ID;
                    datarowTestCenter["TestCenterID"] = TestCenter.TestCenterID;
                    datarowTestCenter["UserID"] = TestCenter.UserID;
                    datarowTestCenter["IsDeleted"] = TestCenter.IsDeleted;
                    datarowTestCenter["IsTransfered"] = TestCenter.IsTransfered;

                    datatableTestCenter.Rows.Add(datarowTestCenter);

                    #endregion Binding Values to TestCenter DataTable

                    dataAdapterTestCenter.InsertCommand = new System.Data.SqlClient.SqlCommand();
                    dataAdapterTestCenter.InsertCommand.CommandTimeout = commandTimeout;
                    dataAdapterTestCenter.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.OutputParameters;
                    dataAdapterTestCenter.UpdateBatchSize = 50;
                    dataAdapterTestCenter.InsertCommand.CommandText = "UpsertTestCenterUserMapping";
                    dataAdapterTestCenter.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    dataAdapterTestCenter.InsertCommand.Connection = connectionTestCenter;

                    #region Binding Command Parameters to stored procedure from TestCenter DataTable

                    dataAdapterTestCenter.InsertCommand.Parameters.Add("@ID", System.Data.SqlDbType.BigInt, sizeof(System.Int64), "ID");
                    dataAdapterTestCenter.InsertCommand.Parameters.Add("@TestCenterID", System.Data.SqlDbType.BigInt, sizeof(System.Int64), "TestCenterID");
                    dataAdapterTestCenter.InsertCommand.Parameters.Add("@UserID", System.Data.SqlDbType.BigInt, sizeof(System.Int64), "UserID");
                    dataAdapterTestCenter.InsertCommand.Parameters.Add("@IsDeleted", System.Data.SqlDbType.Int, sizeof(System.Int16), "IsDeleted");
                    dataAdapterTestCenter.InsertCommand.Parameters.Add("@IsTransfered", System.Data.SqlDbType.Int, sizeof(System.Int16), "IsTransfered");

                    #endregion Binding Command Parameters to stored procedure from TestCenter DataTable

                    dataAdapterTestCenter.RowUpdated += new System.Data.SqlClient.SqlRowUpdatedEventHandler(dataAdapterTestCenter_RowUpdated);
                    dataAdapterTestCenter.Update(datatableTestCenter);
                }
            }
            Log.LogInfo("End UpdateTagUserToTestCenter()");
        }

        public void ResetQPackTransfer(string MacID, List<Int64> ScheduleIDs)
        {
            Log.LogInfo("Begin ResetQPackTransfer() - TestCenter MACID : " + MacID + " ScheduleIDs :" + ScheduleIDs.Count);
            using (System.Data.SqlClient.SqlConnection connectionResetQPackTransfer = CommonDAL.GetConnection())
            {
                using (System.Data.SqlClient.SqlCommand commandResetQPackTransfer = new System.Data.SqlClient.SqlCommand("UspResetQpacktransfer", connectionResetQPackTransfer))
                {
                    commandResetQPackTransfer.CommandType = System.Data.CommandType.StoredProcedure;
                    commandResetQPackTransfer.CommandTimeout = commandTimeout;
                    commandResetQPackTransfer.Parameters.Add(CommonDAL.BuildSqlParameter("@MacId", System.Data.SqlDbType.NVarChar, 2147483646, "@MacId", MacID, System.Data.ParameterDirection.Input));
                    commandResetQPackTransfer.Parameters.Add(CommonDAL.BuildSqlParameter("@ScheduleIds", System.Data.SqlDbType.NVarChar, 2147483646, "@ScheduleIds", string.Join(",", ScheduleIDs.ToArray()), System.Data.ParameterDirection.Input));
                    connectionResetQPackTransfer.Open();
                    commandResetQPackTransfer.ExecuteNonQuery();
                    connectionResetQPackTransfer.Close();
                }
            }
            Log.LogInfo("End ResetQPackTransfer() - TestCenter MACID : " + MacID + " ScheduleIDs :" + ScheduleIDs.Count);
        }
    }

    public class PackageStatisticsFactory : Generic
    {
        private int commandTimeout = 0;

        public PackageStatisticsFactory()
        {
            commandTimeout = 2147483647;
        }

        public System.Collections.Generic.List<DataContracts.PackageStatistics> SearchPackages(bool LoadPackagesFromDCDToDCC, string MacID, string ServerType, string PackageType)
        {
            Log.LogInfo("Begin SearchPackages()" + " - MacID: " + MacID);
            Log.LogInfo("Begin SearchPackages()" + " - ServerType: " + ServerType);
            Log.LogInfo("Begin SearchPackages()" + " - PackageType: " + PackageType);
            List<DataContracts.PackageStatistics> objPackageStatistics = null;
            using (System.Data.SqlClient.SqlConnection connectionPackageStatistics = CommonDAL.GetConnection())
            {
                connectionPackageStatistics.Open();
                using (System.Data.SqlClient.SqlCommand commandPackageStatistics = new System.Data.SqlClient.SqlCommand())
                {
                    commandPackageStatistics.CommandTimeout = commandTimeout;
                    commandPackageStatistics.Connection = connectionPackageStatistics;
                    commandPackageStatistics.CommandText = "SearchPackages";
                    commandPackageStatistics.CommandType = System.Data.CommandType.StoredProcedure;
                    commandPackageStatistics.Parameters.Add(CommonDAL.BuildSqlParameter("@LoadPackagesFromDCDToDCC", System.Data.SqlDbType.Bit, sizeof(bool), "LoadPackagesFromDCDToDCC", LoadPackagesFromDCDToDCC, System.Data.ParameterDirection.Input));
                    commandPackageStatistics.Parameters.Add(CommonDAL.BuildSqlParameter("@MacID", System.Data.SqlDbType.NVarChar, 2147483646, "MacID", MacID, System.Data.ParameterDirection.Input));
                    commandPackageStatistics.Parameters.Add(CommonDAL.BuildSqlParameter("@ServerType", System.Data.SqlDbType.NVarChar, 2147483646, "ServerType", ServerType, System.Data.ParameterDirection.Input));
                    commandPackageStatistics.Parameters.Add(CommonDAL.BuildSqlParameter("@PackageType", System.Data.SqlDbType.NVarChar, 2147483646, "PackageType", PackageType, System.Data.ParameterDirection.Input));
                    using (System.Data.SqlClient.SqlDataReader dataReaderPackageStatistics = commandPackageStatistics.ExecuteReader())
                    {
                        if (dataReaderPackageStatistics != null && dataReaderPackageStatistics.HasRows)
                        {
                            //  To read PackageStatistics details
                            objPackageStatistics = ReadPackageStatistics(dataReaderPackageStatistics);
                        }
                    }
                }
            }
            Log.LogInfo("End SearchPackages()" + " - Package(s) count: " + (objPackageStatistics == null ? "0" : objPackageStatistics.Count.ToString()));
            return objPackageStatistics;
        }

        public System.Collections.Generic.List<DataContracts.PackageStatistics> SearchPackageStatistics(List<System.Int64> ListPackageStatisticsIDs)
        {
            Log.LogInfo("Begin SearchPackageStatistics() - Package(s) Count : " + (ListPackageStatisticsIDs == null ? "0" : ListPackageStatisticsIDs.Count.ToString()));
            List<DataContracts.PackageStatistics> ListPackageStatistics = null;
            if (ListPackageStatisticsIDs != null && ListPackageStatisticsIDs.Count > 0)
            {
                string IDsList = "";
                foreach (System.Int64 entryIDs in ListPackageStatisticsIDs)
                {
                    IDsList += entryIDs.ToString() + ",";
                }
                using (System.Data.SqlClient.SqlConnection connectionPackageStatistics = CommonDAL.GetConnection())
                {
                    connectionPackageStatistics.Open();
                    using (System.Data.SqlClient.SqlCommand commandPackageStatistics = new System.Data.SqlClient.SqlCommand())
                    {
                        commandPackageStatistics.CommandTimeout = commandTimeout;
                        commandPackageStatistics.Connection = connectionPackageStatistics;
                        commandPackageStatistics.CommandText = "SearchPackageStatistics";
                        commandPackageStatistics.CommandType = System.Data.CommandType.StoredProcedure;
                        commandPackageStatistics.Parameters.Add(CommonDAL.BuildSqlParameter("@IDs", System.Data.SqlDbType.NVarChar, 2147483646, "IDs", IDsList, System.Data.ParameterDirection.Input));
                        using (System.Data.SqlClient.SqlDataReader dataReaderPackageStatistics = commandPackageStatistics.ExecuteReader())
                        {
                            if (dataReaderPackageStatistics != null && dataReaderPackageStatistics.HasRows)
                            {
                                //  To read PackageStatistics details
                                ListPackageStatistics = ReadPackageStatistics(dataReaderPackageStatistics);
                            }
                        }
                    }
                }
            }
            Log.LogInfo("End SearchPackageStatistics() - Package(s) Count : " + (ListPackageStatisticsIDs == null ? "0" : ListPackageStatisticsIDs.Count.ToString()));
            return ListPackageStatistics;
        }

        public System.Collections.Generic.List<DataContracts.PackageStatistics> ReadPackageStatistics(System.Data.SqlClient.SqlDataReader reader)
        {
            Log.LogInfo("Begin ReadPackageStatistics()");
            System.Collections.Generic.List<DataContracts.PackageStatistics> listPackageStatistics = null;
            DataContracts.PackageStatistics objPackageStatistics = null;
            if (reader != null && reader.HasRows)
            {
                listPackageStatistics = new System.Collections.Generic.List<DataContracts.PackageStatistics>();
                int Index = 0;
                while (reader.Read())
                {
                    objPackageStatistics = new DataContracts.PackageStatistics();

                    #region Reading the package Statistics

                    Index = reader.GetOrdinal("ID");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.ID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("ScheduleID");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.ScheduleID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("TestCenterID");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.TestCenterID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("PackageType");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.PackageType = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("GeneratedDate");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.GeneratedDate = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToDataExchangeServer");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.TransferredToDataExchangeServer = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToDataExchangeServerOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.TransferredToDataExchangeServerOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToTestCenter");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.TransferredToTestCenter = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToTestCenterOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.TransferredToTestCenterOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToDataCenterDistributed");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.TransferredToDataCenterDistributed = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToDataCenterDistributedOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.TransferredToDataCenterDistributedOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToDataCenterCentralized");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.TransferredToDataCenterCentralized = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToDataCenterCentralizedOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.TransferredToDataCenterCentralizedOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromDataExchangeServer");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.RecievedFromDataExchangeServer = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromDataExchangeServerOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.RecievedFromDataExchangeServerOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromTestCenter");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.RecievedFromTestCenter = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromTestCenterOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.RecievedFromTestCenterOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromDataCenterDistributed");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.RecievedFromDataCenterDistributed = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromDataCenterDistributedOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.RecievedFromDataCenterDistributedOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromDataCenterCentralized");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.RecievedFromDataCenterCentralized = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromDataCenterCentralizedOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.RecievedFromDataCenterCentralizedOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("PackageName");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.PackageName = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("PackagePassword");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.PackagePassword = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("PackagePath");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.PackagePath = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("OrganizationID");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.OrganizationID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("OrganizationName");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.OrganizationName = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("DivisionID");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.DivisionID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("DivisionName");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.DivisionName = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("ProcessID");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.ProcessID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("ProcessName");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.ProcessName = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("EventID");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.EventID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("EventName");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.EventName = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("BatchID");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.BatchID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("BatchName");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.BatchName = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("TestCenterName");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.TestCenterName = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("ScheduleDate");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.ScheduleDate = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("ScheduleStartDate");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.ScheduleStartDate = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("ScheduleEndDate");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.ScheduleEndDate = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("LeadTimeForQPackDispatchInMinutes");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.LeadTimeForQPackDispatchInMinutes = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("DeleteQPackAfterExamination");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.DeleteQPackAfterExamination = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("RPackToBeSentImmediatelyAfterExamination");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.RPackToBeSentImmediatelyAfterExamination = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("RPackToBeSentAtEOD");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.RPackToBeSentAtEOD = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("DeleteRPackAfterExamination");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.DeleteRPackAfterExamination = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("DeleteRPackAtEOD");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.DeleteRPackAtEOD = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("PackageDeletedStatus");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.PackageDeletedStatus = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("IsCentralizedPackage");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.IsCentralizedPackage = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("Extension1");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.Extension1 = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("Extension2");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.Extension2 = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("Extension3");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.Extension3 = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("Extension4");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.Extension4 = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("Extension5");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.Extension5 = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("ScheduleDetailID");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.ScheduleDetailID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("LoadedDateTestCenter");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.LoadedDateTestCenter = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("IsPackageGenerated");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.IsPackageGenerated = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("IsLatest");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.IsLatest = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("LoadedDateCentralized");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.LoadedDateCentralized = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("LoadedDateDistributed");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.LoadedDateDistributed = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("AssessmentID");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.AssessmentID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("CandidateCount");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.CandidateCount = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("ProcessingStatus");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.ProcessingStatus = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("TestCenterLoadDuration");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.TestCenterLoadDuration = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("PackageGenerationType");
                    if (!reader.IsDBNull(Index)) objPackageStatistics.PackageGenerationType = reader.GetByte(Index);
                    Index = reader.GetOrdinal("IsActivated");
                    if (!reader.IsDBNull(Index)) objPackageStatistics.IsActivated = reader.GetByte(Index);
                    Index = reader.GetOrdinal("ExamTypeId");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.ExamTypeId = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("ExamVersion");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.ExamVersion = reader.GetDecimal(Index);
                    }
                    Index = reader.GetOrdinal("IsRecievedEmarkingServer");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.IsRecievedEmarkingServer = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromEmarkingServerOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.RecievedFromEmarkingServerOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("LoadedDateEmarking");
                    if (!reader.IsDBNull(Index))
                    {
                        objPackageStatistics.LoadedDateEmarking = reader.GetDateTime(Index);
                    }

                    #endregion Reading the package Statistics

                    listPackageStatistics.Add(objPackageStatistics);
                }
            }
            Log.LogInfo("End ReadPackageStatistics() - Package(s) Count : " + (listPackageStatistics == null ? "0" : listPackageStatistics.Count.ToString()));
            return listPackageStatistics;
        }

        public void UpdatePackageStatistics(System.Collections.Generic.List<DataContracts.PackageStatistics> listPackageStatistics)
        {
            Log.LogInfo("Begin UpdatePackageStatistics() - Package(s) Count : " + (listPackageStatistics == null ? "0" : listPackageStatistics.Count.ToString()));
            if (listPackageStatistics != null && listPackageStatistics.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionPackageStatistics = CommonDAL.GetConnection())
                {
                    #region Building PackageStatistics DataTable

                    System.Data.DataTable datatablePackageStatistics = new System.Data.DataTable();
                    datatablePackageStatistics.Columns.Add("ID");
                    datatablePackageStatistics.Columns.Add("ScheduleID");
                    datatablePackageStatistics.Columns.Add("TestCenterID");
                    datatablePackageStatistics.Columns.Add("PackageType");
                    datatablePackageStatistics.Columns.Add("GeneratedDate");
                    datatablePackageStatistics.Columns.Add("TransferredToDataExchangeServer");
                    datatablePackageStatistics.Columns.Add("TransferredToDataExchangeServerOn");
                    datatablePackageStatistics.Columns.Add("TransferredToTestCenter");
                    datatablePackageStatistics.Columns.Add("TransferredToTestCenterOn");
                    datatablePackageStatistics.Columns.Add("TransferredToDataCenterDistributed");
                    datatablePackageStatistics.Columns.Add("TransferredToDataCenterDistributedOn");
                    datatablePackageStatistics.Columns.Add("TransferredToDataCenterCentralized");
                    datatablePackageStatistics.Columns.Add("TransferredToDataCenterCentralizedOn");
                    datatablePackageStatistics.Columns.Add("RecievedFromDataExchangeServer");
                    datatablePackageStatistics.Columns.Add("RecievedFromDataExchangeServerOn");
                    datatablePackageStatistics.Columns.Add("RecievedFromTestCenter");
                    datatablePackageStatistics.Columns.Add("RecievedFromTestCenterOn");
                    datatablePackageStatistics.Columns.Add("RecievedFromDataCenterDistributed");
                    datatablePackageStatistics.Columns.Add("RecievedFromDataCenterDistributedOn");
                    datatablePackageStatistics.Columns.Add("RecievedFromDataCenterCentralized");
                    datatablePackageStatistics.Columns.Add("RecievedFromDataCenterCentralizedOn");
                    datatablePackageStatistics.Columns.Add("PackageName");
                    datatablePackageStatistics.Columns.Add("PackagePassword");
                    datatablePackageStatistics.Columns.Add("PackagePath");
                    datatablePackageStatistics.Columns.Add("OrganizationID");
                    datatablePackageStatistics.Columns.Add("OrganizationName");
                    datatablePackageStatistics.Columns.Add("DivisionID");
                    datatablePackageStatistics.Columns.Add("DivisionName");
                    datatablePackageStatistics.Columns.Add("ProcessID");
                    datatablePackageStatistics.Columns.Add("ProcessName");
                    datatablePackageStatistics.Columns.Add("EventID");
                    datatablePackageStatistics.Columns.Add("EventName");
                    datatablePackageStatistics.Columns.Add("BatchID");
                    datatablePackageStatistics.Columns.Add("BatchName");
                    datatablePackageStatistics.Columns.Add("TestCenterName");
                    datatablePackageStatistics.Columns.Add("ScheduleDate");
                    datatablePackageStatistics.Columns.Add("ScheduleStartDate");
                    datatablePackageStatistics.Columns.Add("ScheduleEndDate");
                    datatablePackageStatistics.Columns.Add("LeadTimeForQPackDispatchInMinutes");
                    datatablePackageStatistics.Columns.Add("DeleteQPackAfterExamination");
                    datatablePackageStatistics.Columns.Add("RPackToBeSentImmediatelyAfterExamination");
                    datatablePackageStatistics.Columns.Add("RPackToBeSentAtEOD");
                    datatablePackageStatistics.Columns.Add("DeleteRPackAfterExamination");
                    datatablePackageStatistics.Columns.Add("DeleteRPackAtEOD");
                    datatablePackageStatistics.Columns.Add("PackageDeletedStatus");
                    datatablePackageStatistics.Columns.Add("IsCentralizedPackage");
                    datatablePackageStatistics.Columns.Add("Extension1");
                    datatablePackageStatistics.Columns.Add("Extension2");
                    datatablePackageStatistics.Columns.Add("Extension3");
                    datatablePackageStatistics.Columns.Add("Extension4");
                    datatablePackageStatistics.Columns.Add("Extension5");
                    datatablePackageStatistics.Columns.Add("ScheduleDetailID");
                    datatablePackageStatistics.Columns.Add("LoadedDateTestCenter");
                    datatablePackageStatistics.Columns.Add("IsPackageGenerated");
                    datatablePackageStatistics.Columns.Add("IsLatest");
                    datatablePackageStatistics.Columns.Add("LoadedDateCentralized");
                    datatablePackageStatistics.Columns.Add("LoadedDateDistributed");
                    datatablePackageStatistics.Columns.Add("AssessmentID");
                    datatablePackageStatistics.Columns.Add("CandidateCount");
                    datatablePackageStatistics.Columns.Add("ProcessingStatus");
                    datatablePackageStatistics.Columns.Add("TestCenterLoadDuration");
                    datatablePackageStatistics.Columns.Add("PackageGenerationType");
                    datatablePackageStatistics.Columns.Add("IsActivated");
                    datatablePackageStatistics.Columns.Add("ExamTypeId");
                    datatablePackageStatistics.Columns.Add("ExamVersion");
                    datatablePackageStatistics.Columns.Add("IsRecievedEmarkingServer");
                    datatablePackageStatistics.Columns.Add("RecievedFromEmarkingServerOn");
                    datatablePackageStatistics.Columns.Add("LoadedDateEmarking");

                    #endregion Building PackageStatistics DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterPackageStatistics = new System.Data.SqlClient.SqlDataAdapter("UpdatePackageStatistics", connectionPackageStatistics))
                    {
                        foreach (DataContracts.PackageStatistics objPackageStatistics in listPackageStatistics)
                        {
                            System.Data.DataRow datarowPackageStatistics = datatablePackageStatistics.NewRow();

                            #region Binding Values to PackageStatistics DataTable

                            datarowPackageStatistics["ID"] = objPackageStatistics.ID;
                            datarowPackageStatistics["ScheduleID"] = objPackageStatistics.ScheduleID;
                            datarowPackageStatistics["TestCenterID"] = objPackageStatistics.TestCenterID;
                            datarowPackageStatistics["PackageType"] = objPackageStatistics.PackageType;
                            if (objPackageStatistics.GeneratedDate != null && objPackageStatistics.GeneratedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["GeneratedDate"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["GeneratedDate"] = objPackageStatistics.GeneratedDate;
                            datarowPackageStatistics["TransferredToDataExchangeServer"] = objPackageStatistics.TransferredToDataExchangeServer;
                            if (objPackageStatistics.TransferredToDataExchangeServerOn != null && objPackageStatistics.TransferredToDataExchangeServerOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["TransferredToDataExchangeServerOn"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["TransferredToDataExchangeServerOn"] = objPackageStatistics.TransferredToDataExchangeServerOn;
                            datarowPackageStatistics["TransferredToTestCenter"] = objPackageStatistics.TransferredToTestCenter;
                            if (objPackageStatistics.TransferredToTestCenterOn != null && objPackageStatistics.TransferredToTestCenterOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["TransferredToTestCenterOn"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["TransferredToTestCenterOn"] = objPackageStatistics.TransferredToTestCenterOn;
                            datarowPackageStatistics["TransferredToDataCenterDistributed"] = objPackageStatistics.TransferredToDataCenterDistributed;
                            if (objPackageStatistics.TransferredToDataCenterDistributedOn != null && objPackageStatistics.TransferredToDataCenterDistributedOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["TransferredToDataCenterDistributedOn"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["TransferredToDataCenterDistributedOn"] = objPackageStatistics.TransferredToDataCenterDistributedOn;
                            datarowPackageStatistics["TransferredToDataCenterCentralized"] = objPackageStatistics.TransferredToDataCenterCentralized;
                            if (objPackageStatistics.TransferredToDataCenterCentralizedOn != null && objPackageStatistics.TransferredToDataCenterCentralizedOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["TransferredToDataCenterCentralizedOn"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["TransferredToDataCenterCentralizedOn"] = objPackageStatistics.TransferredToDataCenterCentralizedOn;
                            if (objPackageStatistics.RecievedFromDataExchangeServer <= 0)
                                datarowPackageStatistics["RecievedFromDataExchangeServer"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["RecievedFromDataExchangeServer"] = objPackageStatistics.RecievedFromDataExchangeServer;
                            if (objPackageStatistics.RecievedFromDataExchangeServerOn != null && objPackageStatistics.RecievedFromDataExchangeServerOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["RecievedFromDataExchangeServerOn"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["RecievedFromDataExchangeServerOn"] = objPackageStatistics.RecievedFromDataExchangeServerOn;
                            if (objPackageStatistics.RecievedFromTestCenter <= 0)
                                datarowPackageStatistics["RecievedFromTestCenter"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["RecievedFromTestCenter"] = objPackageStatistics.RecievedFromTestCenter;
                            if (objPackageStatistics.RecievedFromTestCenterOn != null && objPackageStatistics.RecievedFromTestCenterOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["RecievedFromTestCenterOn"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["RecievedFromTestCenterOn"] = objPackageStatistics.RecievedFromTestCenterOn;
                            if (objPackageStatistics.RecievedFromDataCenterDistributed <= 0)
                                datarowPackageStatistics["RecievedFromDataCenterDistributed"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["RecievedFromDataCenterDistributed"] = objPackageStatistics.RecievedFromDataCenterDistributed;
                            if (objPackageStatistics.RecievedFromDataCenterDistributedOn != null && objPackageStatistics.RecievedFromDataCenterDistributedOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["RecievedFromDataCenterDistributedOn"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["RecievedFromDataCenterDistributedOn"] = objPackageStatistics.RecievedFromDataCenterDistributedOn;
                            if (objPackageStatistics.RecievedFromDataCenterCentralized <= 0)
                                datarowPackageStatistics["RecievedFromDataCenterCentralized"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["RecievedFromDataCenterCentralized"] = objPackageStatistics.RecievedFromDataCenterCentralized;
                            if (objPackageStatistics.RecievedFromDataCenterCentralizedOn != null && objPackageStatistics.RecievedFromDataCenterCentralizedOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["RecievedFromDataCenterCentralizedOn"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["RecievedFromDataCenterCentralizedOn"] = objPackageStatistics.RecievedFromDataCenterCentralizedOn;
                            datarowPackageStatistics["PackageName"] = objPackageStatistics.PackageName;
                            datarowPackageStatistics["PackagePassword"] = objPackageStatistics.PackagePassword;
                            datarowPackageStatistics["PackagePath"] = objPackageStatistics.PackagePath;
                            if (objPackageStatistics.OrganizationID <= 0)
                                datarowPackageStatistics["OrganizationID"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["OrganizationID"] = objPackageStatistics.OrganizationID;
                            datarowPackageStatistics["OrganizationName"] = objPackageStatistics.OrganizationName;
                            if (objPackageStatistics.DivisionID <= 0)
                                datarowPackageStatistics["DivisionID"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["DivisionID"] = objPackageStatistics.DivisionID;
                            datarowPackageStatistics["DivisionName"] = objPackageStatistics.DivisionName;
                            if (objPackageStatistics.ProcessID <= 0)
                                datarowPackageStatistics["ProcessID"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["ProcessID"] = objPackageStatistics.ProcessID;
                            datarowPackageStatistics["ProcessName"] = objPackageStatistics.ProcessName;
                            if (objPackageStatistics.EventID <= 0)
                                datarowPackageStatistics["EventID"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["EventID"] = objPackageStatistics.EventID;
                            datarowPackageStatistics["EventName"] = objPackageStatistics.EventName;
                            if (objPackageStatistics.BatchID <= 0)
                                datarowPackageStatistics["BatchID"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["BatchID"] = objPackageStatistics.BatchID;
                            datarowPackageStatistics["BatchName"] = objPackageStatistics.BatchName;
                            datarowPackageStatistics["TestCenterName"] = objPackageStatistics.TestCenterName;
                            if (objPackageStatistics.ScheduleDate != null && objPackageStatistics.ScheduleDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["ScheduleDate"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["ScheduleDate"] = objPackageStatistics.ScheduleDate;
                            if (objPackageStatistics.ScheduleStartDate != null && objPackageStatistics.ScheduleStartDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["ScheduleStartDate"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["ScheduleStartDate"] = objPackageStatistics.ScheduleStartDate;
                            if (objPackageStatistics.ScheduleEndDate != null && objPackageStatistics.ScheduleEndDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["ScheduleEndDate"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["ScheduleEndDate"] = objPackageStatistics.ScheduleEndDate;
                            datarowPackageStatistics["LeadTimeForQPackDispatchInMinutes"] = objPackageStatistics.LeadTimeForQPackDispatchInMinutes;
                            datarowPackageStatistics["DeleteQPackAfterExamination"] = objPackageStatistics.DeleteQPackAfterExamination;
                            datarowPackageStatistics["RPackToBeSentImmediatelyAfterExamination"] = objPackageStatistics.RPackToBeSentImmediatelyAfterExamination;
                            datarowPackageStatistics["RPackToBeSentAtEOD"] = objPackageStatistics.RPackToBeSentAtEOD;
                            datarowPackageStatistics["DeleteRPackAfterExamination"] = objPackageStatistics.DeleteRPackAfterExamination;
                            datarowPackageStatistics["DeleteRPackAtEOD"] = objPackageStatistics.DeleteRPackAtEOD;
                            datarowPackageStatistics["PackageDeletedStatus"] = objPackageStatistics.PackageDeletedStatus;
                            datarowPackageStatistics["IsCentralizedPackage"] = objPackageStatistics.IsCentralizedPackage;
                            datarowPackageStatistics["Extension1"] = objPackageStatistics.Extension1;
                            datarowPackageStatistics["Extension2"] = objPackageStatistics.Extension2;
                            datarowPackageStatistics["Extension3"] = objPackageStatistics.Extension3;
                            datarowPackageStatistics["Extension4"] = objPackageStatistics.Extension4;
                            datarowPackageStatistics["Extension5"] = objPackageStatistics.Extension5;
                            datarowPackageStatistics["ScheduleDetailID"] = objPackageStatistics.ScheduleDetailID;
                            if (objPackageStatistics.LoadedDateTestCenter != null && objPackageStatistics.LoadedDateTestCenter == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["LoadedDateTestCenter"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["LoadedDateTestCenter"] = objPackageStatistics.LoadedDateTestCenter;
                            datarowPackageStatistics["IsPackageGenerated"] = objPackageStatistics.IsPackageGenerated;
                            datarowPackageStatistics["IsLatest"] = objPackageStatistics.IsLatest;
                            if (objPackageStatistics.LoadedDateCentralized != null && objPackageStatistics.LoadedDateCentralized == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["LoadedDateCentralized"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["LoadedDateCentralized"] = objPackageStatistics.LoadedDateCentralized;
                            if (objPackageStatistics.LoadedDateDistributed != null && objPackageStatistics.LoadedDateDistributed == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["LoadedDateDistributed"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["LoadedDateDistributed"] = objPackageStatistics.LoadedDateDistributed;
                            datarowPackageStatistics["AssessmentID"] = objPackageStatistics.AssessmentID;
                            datarowPackageStatistics["CandidateCount"] = objPackageStatistics.CandidateCount;
                            datarowPackageStatistics["ProcessingStatus"] = objPackageStatistics.ProcessingStatus;
                            datarowPackageStatistics["TestCenterLoadDuration"] = objPackageStatistics.TestCenterLoadDuration;
                            datarowPackageStatistics["PackageGenerationType"] = objPackageStatistics.PackageGenerationType;
                            datarowPackageStatistics["IsActivated"] = objPackageStatistics.IsActivated;
                            datarowPackageStatistics["ExamTypeId"] = objPackageStatistics.ExamTypeId;
                            datarowPackageStatistics["ExamVersion"] = objPackageStatistics.ExamVersion;
                            datarowPackageStatistics["IsRecievedEmarkingServer"] = objPackageStatistics.IsRecievedEmarkingServer;
                            if (objPackageStatistics.RecievedFromEmarkingServerOn != null && objPackageStatistics.RecievedFromEmarkingServerOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["RecievedFromEmarkingServerOn"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["RecievedFromEmarkingServerOn"] = objPackageStatistics.IsRecievedEmarkingServer;

                            if (objPackageStatistics.LoadedDateEmarking != null && objPackageStatistics.LoadedDateEmarking == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageStatistics["LoadedDateEmarking"] = System.DBNull.Value;
                            else
                                datarowPackageStatistics["LoadedDateEmarking"] = objPackageStatistics.LoadedDateEmarking;
                            datatablePackageStatistics.Rows.Add(datarowPackageStatistics);

                            #endregion Binding Values to PackageStatistics DataTable
                        }

                        dataAdapterPackageStatistics.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterPackageStatistics.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterPackageStatistics.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterPackageStatistics.UpdateBatchSize = 50;
                        dataAdapterPackageStatistics.InsertCommand.CommandText = "UpdatePackageStatistics";
                        dataAdapterPackageStatistics.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterPackageStatistics.InsertCommand.Connection = connectionPackageStatistics;

                        #region Binding Command Parameters to stored procedure from PackageStatistics DataTable

                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@ID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ID");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@ScheduleID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleID");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@TestCenterID", System.Data.SqlDbType.BigInt, sizeof(Int64), "TestCenterID");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@PackageType", System.Data.SqlDbType.NVarChar, 2147483646, "PackageType");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@GeneratedDate", System.Data.SqlDbType.DateTime, 20, "GeneratedDate");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@TransferredToDataExchangeServer", System.Data.SqlDbType.Bit, sizeof(bool), "TransferredToDataExchangeServer");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@TransferredToDataExchangeServerOn", System.Data.SqlDbType.DateTime, 20, "TransferredToDataExchangeServerOn");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@TransferredToTestCenter", System.Data.SqlDbType.Bit, sizeof(bool), "TransferredToTestCenter");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@TransferredToTestCenterOn", System.Data.SqlDbType.DateTime, 20, "TransferredToTestCenterOn");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@TransferredToDataCenterDistributed", System.Data.SqlDbType.Bit, sizeof(bool), "TransferredToDataCenterDistributed");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@TransferredToDataCenterDistributedOn", System.Data.SqlDbType.DateTime, 20, "TransferredToDataCenterDistributedOn");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@TransferredToDataCenterCentralized", System.Data.SqlDbType.Bit, sizeof(bool), "TransferredToDataCenterCentralized");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@TransferredToDataCenterCentralizedOn", System.Data.SqlDbType.DateTime, 20, "TransferredToDataCenterCentralizedOn");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@RecievedFromDataExchangeServer", System.Data.SqlDbType.Int, sizeof(Int32), "RecievedFromDataExchangeServer");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@RecievedFromDataExchangeServerOn", System.Data.SqlDbType.DateTime, 20, "RecievedFromDataExchangeServerOn");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@RecievedFromTestCenter", System.Data.SqlDbType.Int, sizeof(Int32), "RecievedFromTestCenter");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@RecievedFromTestCenterOn", System.Data.SqlDbType.DateTime, 20, "RecievedFromTestCenterOn");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@RecievedFromDataCenterDistributed", System.Data.SqlDbType.Int, sizeof(Int32), "RecievedFromDataCenterDistributed");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@RecievedFromDataCenterDistributedOn", System.Data.SqlDbType.DateTime, 20, "RecievedFromDataCenterDistributedOn");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@RecievedFromDataCenterCentralized", System.Data.SqlDbType.Int, sizeof(Int32), "RecievedFromDataCenterCentralized");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@RecievedFromDataCenterCentralizedOn", System.Data.SqlDbType.DateTime, 20, "RecievedFromDataCenterCentralizedOn");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@PackageName", System.Data.SqlDbType.NVarChar, 2147483646, "PackageName");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@PackagePassword", System.Data.SqlDbType.NVarChar, 2147483646, "PackagePassword");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@PackagePath", System.Data.SqlDbType.NVarChar, 2147483646, "PackagePath");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@OrganizationID", System.Data.SqlDbType.BigInt, sizeof(Int64), "OrganizationID");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@OrganizationName", System.Data.SqlDbType.NVarChar, 2147483646, "OrganizationName");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@DivisionID", System.Data.SqlDbType.BigInt, sizeof(Int64), "DivisionID");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@DivisionName", System.Data.SqlDbType.NVarChar, 2147483646, "DivisionName");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@ProcessID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ProcessID");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@ProcessName", System.Data.SqlDbType.NVarChar, 2147483646, "ProcessName");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@EventID", System.Data.SqlDbType.BigInt, sizeof(Int64), "EventID");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@EventName", System.Data.SqlDbType.NVarChar, 2147483646, "EventName");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@BatchID", System.Data.SqlDbType.BigInt, sizeof(Int64), "BatchID");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@BatchName", System.Data.SqlDbType.NVarChar, 2147483646, "BatchName");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@TestCenterName", System.Data.SqlDbType.NVarChar, 2147483646, "TestCenterName");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@ScheduleDate", System.Data.SqlDbType.DateTime, 20, "ScheduleDate");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@ScheduleStartDate", System.Data.SqlDbType.DateTime, 20, "ScheduleStartDate");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@ScheduleEndDate", System.Data.SqlDbType.DateTime, 20, "ScheduleEndDate");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@LeadTimeForQPackDispatchInMinutes", System.Data.SqlDbType.Int, sizeof(Int32), "LeadTimeForQPackDispatchInMinutes");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@DeleteQPackAfterExamination", System.Data.SqlDbType.Bit, sizeof(bool), "DeleteQPackAfterExamination");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@RPackToBeSentImmediatelyAfterExamination", System.Data.SqlDbType.Bit, sizeof(bool), "RPackToBeSentImmediatelyAfterExamination");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@RPackToBeSentAtEOD", System.Data.SqlDbType.Bit, sizeof(bool), "RPackToBeSentAtEOD");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@DeleteRPackAfterExamination", System.Data.SqlDbType.Bit, sizeof(bool), "DeleteRPackAfterExamination");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@DeleteRPackAtEOD", System.Data.SqlDbType.Bit, sizeof(bool), "DeleteRPackAtEOD");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@PackageDeletedStatus", System.Data.SqlDbType.Bit, sizeof(bool), "PackageDeletedStatus");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@IsCentralizedPackage", System.Data.SqlDbType.Bit, sizeof(bool), "IsCentralizedPackage");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@Extension1", System.Data.SqlDbType.NVarChar, 2147483646, "Extension1");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@Extension2", System.Data.SqlDbType.NVarChar, 2147483646, "Extension2");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@Extension3", System.Data.SqlDbType.NVarChar, 2147483646, "Extension3");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@Extension4", System.Data.SqlDbType.NVarChar, 2147483646, "Extension4");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@Extension5", System.Data.SqlDbType.NVarChar, 2147483646, "Extension5");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@ScheduleDetailID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleDetailID");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@LoadedDateTestCenter", System.Data.SqlDbType.DateTime, 20, "LoadedDateTestCenter");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@IsPackageGenerated", System.Data.SqlDbType.Bit, sizeof(bool), "IsPackageGenerated");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@IsLatest", System.Data.SqlDbType.Bit, sizeof(bool), "IsLatest");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@LoadedDateCentralized", System.Data.SqlDbType.DateTime, 20, "LoadedDateCentralized");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@LoadedDateDistributed", System.Data.SqlDbType.DateTime, 20, "LoadedDateDistributed");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@AssessmentID", System.Data.SqlDbType.BigInt, sizeof(Int64), "AssessmentID");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@CandidateCount", System.Data.SqlDbType.Int, sizeof(Int32), "CandidateCount");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@ProcessingStatus", System.Data.SqlDbType.Bit, sizeof(bool), "ProcessingStatus");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@TestCenterLoadDuration", System.Data.SqlDbType.Int, sizeof(Int32), "TestCenterLoadDuration");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@PackageGenerationType", System.Data.SqlDbType.TinyInt, sizeof(byte), "PackageGenerationType");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@IsActivated", System.Data.SqlDbType.TinyInt, sizeof(byte), "IsActivated");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@ExamTypeId", System.Data.SqlDbType.BigInt, sizeof(Int64), "ExamTypeId");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@ExamVersion", System.Data.SqlDbType.Decimal, sizeof(Decimal), "ExamVersion");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@IsRecievedEmarkingServer", System.Data.SqlDbType.Bit, sizeof(bool), "IsRecievedEmarkingServer");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@RecievedFromEmarkingServerOn", System.Data.SqlDbType.DateTime, 20, "RecievedFromEmarkingServerOn");
                        dataAdapterPackageStatistics.InsertCommand.Parameters.Add("@LoadedDateEmarking", System.Data.SqlDbType.DateTime, 20, "LoadedDateEmarking");

                        #endregion Binding Command Parameters to stored procedure from PackageStatistics DataTable

                        dataAdapterPackageStatistics.Update(datatablePackageStatistics);
                    }
                }
            }
            Log.LogInfo("End UpdatePackageStatistics()");
        }

        public void UpdatePackageLoadedDate(ServiceContracts.UpdatePackageLoadedDateRequest request)
        {
            System.Collections.Generic.List<DataContracts.PackageLoadedDate> listPackageLoadedDate = request.ListPackageLoadedDate;

            Log.LogInfo("Begin UpdatePackageLoadedDate() - Package(s) Count : " + (listPackageLoadedDate == null ? "0" : listPackageLoadedDate.Count.ToString()));
            if (listPackageLoadedDate != null && listPackageLoadedDate.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionPackageLoadedDate = CommonDAL.GetConnection())
                {
                    #region Building PackageLoadedDate DataTable

                    System.Data.DataTable datatablePackageLoadedDate = new System.Data.DataTable();
                    datatablePackageLoadedDate.Columns.Add("ScheduleDetailID");
                    datatablePackageLoadedDate.Columns.Add("PackageType");
                    datatablePackageLoadedDate.Columns.Add("GeneratedDate");
                    datatablePackageLoadedDate.Columns.Add("LoadedDateTestCenter");
                    datatablePackageLoadedDate.Columns.Add("LoadedDateCentralized");
                    datatablePackageLoadedDate.Columns.Add("LoadedDateDistributed");
                    datatablePackageLoadedDate.Columns.Add("ServerType");
                    datatablePackageLoadedDate.Columns.Add("Extension5"); //Extension5 used for sucess/failure  status for package
                    datatablePackageLoadedDate.Columns.Add("TestCenterLoadDuration");
                    datatablePackageLoadedDate.Columns.Add("TestCenterID");

                    #endregion Building PackageLoadedDate DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterPackageLoadedDate = new System.Data.SqlClient.SqlDataAdapter("UpdatePackageLoadedDate", connectionPackageLoadedDate))
                    {
                        foreach (DataContracts.PackageLoadedDate objPackageLoadedDate in listPackageLoadedDate)
                        {
                            System.Data.DataRow datarowPackageLoadedDate = datatablePackageLoadedDate.NewRow();

                            #region Binding Values to PackageLoadedDate DataTable

                            datarowPackageLoadedDate["ScheduleDetailID"] = objPackageLoadedDate.ScheduleDetailID;
                            datarowPackageLoadedDate["PackageType"] = objPackageLoadedDate.PackageType;
                            if (objPackageLoadedDate.GeneratedDate != null && objPackageLoadedDate.GeneratedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageLoadedDate["GeneratedDate"] = System.DBNull.Value;
                            else
                                datarowPackageLoadedDate["GeneratedDate"] = objPackageLoadedDate.GeneratedDate;
                            if (objPackageLoadedDate.LoadedDateTestCenter != null && objPackageLoadedDate.LoadedDateTestCenter == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageLoadedDate["LoadedDateTestCenter"] = System.DBNull.Value;
                            else
                                datarowPackageLoadedDate["LoadedDateTestCenter"] = objPackageLoadedDate.LoadedDateTestCenter;

                            if (objPackageLoadedDate.LoadedDateCentralized != null && objPackageLoadedDate.LoadedDateCentralized == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageLoadedDate["LoadedDateCentralized"] = System.DBNull.Value;
                            else
                                datarowPackageLoadedDate["LoadedDateCentralized"] = objPackageLoadedDate.LoadedDateCentralized;
                            if (objPackageLoadedDate.LoadedDateDistributed != null && objPackageLoadedDate.LoadedDateDistributed == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowPackageLoadedDate["LoadedDateDistributed"] = System.DBNull.Value;
                            else
                                datarowPackageLoadedDate["LoadedDateDistributed"] = objPackageLoadedDate.LoadedDateDistributed;
                            datarowPackageLoadedDate["ServerType"] = (int)request.ServerType;
                            if (objPackageLoadedDate.Extension5 != null && objPackageLoadedDate.Extension5 != "")
                                datarowPackageLoadedDate["Extension5"] = objPackageLoadedDate.Extension5;
                            else
                                datarowPackageLoadedDate["Extension5"] = System.DBNull.Value;

                            datarowPackageLoadedDate["TestCenterLoadDuration"] = objPackageLoadedDate.TestCenterLoadDuration;
                            datarowPackageLoadedDate["TestCenterID"] = objPackageLoadedDate.TestCenterID;
                            datatablePackageLoadedDate.Rows.Add(datarowPackageLoadedDate);
                            Log.LogInfo("Begin UpdatePackageLoadedDate() - ScheduleDetailID: " + datarowPackageLoadedDate["ScheduleDetailID"].ToString());
                            Log.LogInfo("Begin UpdatePackageLoadedDate() - PackageType: " + datarowPackageLoadedDate["PackageType"].ToString());
                            Log.LogInfo("Begin UpdatePackageLoadedDate() - GeneratedDate: " + (objPackageLoadedDate.GeneratedDate.ToString()));
                            Log.LogInfo("Begin UpdatePackageLoadedDate() - LoadedDateTestCenter: " + (objPackageLoadedDate.LoadedDateTestCenter.ToString()));
                            Log.LogInfo("Begin UpdatePackageLoadedDate() - LoadedDateCentralized: " + (objPackageLoadedDate.LoadedDateCentralized.ToString()));
                            Log.LogInfo("Begin UpdatePackageLoadedDate() - LoadedDateDistributed: " + (objPackageLoadedDate.LoadedDateDistributed.ToString()));
                            Log.LogInfo("Begin UpdatePackageLoadedDate() - ServerType: " + datarowPackageLoadedDate["ServerType"].ToString());
                            Log.LogInfo("Begin UpdatePackageLoadedDate() - Extension5: " + datarowPackageLoadedDate["Extension5"].ToString());

                            #endregion Binding Values to PackageLoadedDate DataTable
                        }
                        dataAdapterPackageLoadedDate.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterPackageLoadedDate.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterPackageLoadedDate.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterPackageLoadedDate.UpdateBatchSize = 50;
                        dataAdapterPackageLoadedDate.InsertCommand.CommandText = "UpdatePackageLoadedDate";
                        dataAdapterPackageLoadedDate.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterPackageLoadedDate.InsertCommand.Connection = connectionPackageLoadedDate;

                        #region Binding Command Parameters to stored procedure from PackageStatistics DataTable

                        dataAdapterPackageLoadedDate.InsertCommand.Parameters.Add("@ScheduleDetailID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleDetailID");
                        dataAdapterPackageLoadedDate.InsertCommand.Parameters.Add("@PackageType", System.Data.SqlDbType.NVarChar, 2147483646, "PackageType");
                        dataAdapterPackageLoadedDate.InsertCommand.Parameters.Add("@GeneratedDate", System.Data.SqlDbType.DateTime, 200, "GeneratedDate");
                        dataAdapterPackageLoadedDate.InsertCommand.Parameters.Add("@LoadedDateTestCenter", System.Data.SqlDbType.DateTime, 200, "LoadedDateTestCenter");
                        dataAdapterPackageLoadedDate.InsertCommand.Parameters.Add("@LoadedDateCentralized", System.Data.SqlDbType.DateTime, 200, "LoadedDateCentralized");
                        dataAdapterPackageLoadedDate.InsertCommand.Parameters.Add("@LoadedDateDistributed", System.Data.SqlDbType.DateTime, 200, "LoadedDateDistributed");
                        dataAdapterPackageLoadedDate.InsertCommand.Parameters.Add("@Extension5", System.Data.SqlDbType.NVarChar, 2147483646, "Extension5");
                        dataAdapterPackageLoadedDate.InsertCommand.Parameters.Add("@ServerType", System.Data.SqlDbType.Int, sizeof(Int32), "ServerType");
                        dataAdapterPackageLoadedDate.InsertCommand.Parameters.Add("@TestCenterLoadDuration", System.Data.SqlDbType.Int, sizeof(Int32), "TestCenterLoadDuration");
                        dataAdapterPackageLoadedDate.InsertCommand.Parameters.Add("@TestCenterID", System.Data.SqlDbType.BigInt, sizeof(Int64), "TestCenterID");

                        #endregion Binding Command Parameters to stored procedure from PackageStatistics DataTable

                        dataAdapterPackageLoadedDate.Update(datatablePackageLoadedDate);
                    }
                }
            }
            Log.LogInfo("End UpdatePackageLoadedDate()");
        }

        public void PackageReTransferOrReGenerate(List<DataContracts.PackageDetails> ListPackageDetails)
        {
            Log.LogInfo("Begin PackageReTransferOrReGenerate(), Package(s) Count: " + (ListPackageDetails == null ? "0" : ListPackageDetails.Count.ToString()));
            if (ListPackageDetails != null && ListPackageDetails.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionPackageDetails = CommonDAL.GetConnection())
                {
                    #region Building PackageDetails DataTable

                    System.Data.DataTable datatablePackageDetails = new System.Data.DataTable();
                    datatablePackageDetails.Columns.Add("ScheduleDetailID");
                    datatablePackageDetails.Columns.Add("CenterID");
                    datatablePackageDetails.Columns.Add("OperationType");

                    #endregion Building PackageDetails DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterPackageDetails = new System.Data.SqlClient.SqlDataAdapter("PackageReTransferOrReGenerate", connectionPackageDetails))
                    {
                        foreach (DataContracts.PackageDetails entryPackageDetails in ListPackageDetails)
                        {
                            System.Data.DataRow datarowPackageDetails = datatablePackageDetails.NewRow();

                            #region Binding Values to PackageDetails DataTable

                            datarowPackageDetails["ScheduleDetailID"] = entryPackageDetails.ScheduleDetailID;
                            datarowPackageDetails["CenterID"] = entryPackageDetails.CenterID;
                            datarowPackageDetails["OperationType"] = (Int32)entryPackageDetails.Operation;
                            datatablePackageDetails.Rows.Add(datarowPackageDetails);

                            #endregion Binding Values to PackageDetails DataTable
                        }
                        dataAdapterPackageDetails.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterPackageDetails.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterPackageDetails.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterPackageDetails.UpdateBatchSize = 50;
                        dataAdapterPackageDetails.InsertCommand.CommandText = "PackageReTransferOrReGenerate";
                        dataAdapterPackageDetails.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterPackageDetails.InsertCommand.Connection = connectionPackageDetails;

                        #region Binding Command Parameters to stored procedure from PackageDetails DataTable

                        dataAdapterPackageDetails.InsertCommand.Parameters.Add("@ScheduleDetailID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleDetailID");
                        dataAdapterPackageDetails.InsertCommand.Parameters.Add("@CenterID", System.Data.SqlDbType.BigInt, sizeof(Int64), "CenterID");
                        dataAdapterPackageDetails.InsertCommand.Parameters.Add("@OperationType", System.Data.SqlDbType.BigInt, sizeof(Int64), "OperationType");

                        #endregion Binding Command Parameters to stored procedure from PackageDetails DataTable

                        dataAdapterPackageDetails.Update(datatablePackageDetails);
                    }
                }
            }
            Log.LogInfo("End PackageReTransferOrReGenerate()");
        }

        public List<DataContracts.PackageStatistics> SearchActivatedStatusForBatch(List<DataContracts.PackageStatistics> ListPackageStatistics)
        {
            Log.LogInfo("Begin SearchActivatedStatusForBatch():kiran - ListPackageStatistics Count : " + (ListPackageStatistics != null && ListPackageStatistics.Count > 0 ? ListPackageStatistics.Count.ToString() : "0"));
            if (ListPackageStatistics != null && ListPackageStatistics.Count > 0)
            {
                #region Building ScheduledDetails DataTable

                System.Data.DataTable datatableBatchPasswordDetails = new System.Data.DataTable();
                datatableBatchPasswordDetails.Columns.Add("ScheduleDetailID");
                datatableBatchPasswordDetails.Columns.Add("ScheduleID");
                datatableBatchPasswordDetails.Columns.Add("IsActivated");

                #endregion Building ScheduledDetails DataTable

                using (System.Data.SqlClient.SqlConnection connectionSessionAccessCodeDetails = CommonDAL.GetConnection())
                {
                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterBatchPasswordDetails = new System.Data.SqlClient.SqlDataAdapter("SearchActivatedStatusForBatch", connectionSessionAccessCodeDetails))
                    {
                        System.Data.DataRow datarowBatchPasswordDetails;
                        foreach (DataContracts.PackageStatistics objessionAccessCodeDetails in ListPackageStatistics)
                        {
                            datarowBatchPasswordDetails = datatableBatchPasswordDetails.NewRow();

                            #region Binding Values to ScheduledDetails DataTable

                            datarowBatchPasswordDetails["ScheduleDetailID"] = objessionAccessCodeDetails.ScheduleDetailID;
                            datarowBatchPasswordDetails["ScheduleID"] = objessionAccessCodeDetails.ScheduleID;
                            datarowBatchPasswordDetails["IsActivated"] = objessionAccessCodeDetails.IsActivated;

                            #endregion Binding Values to ScheduledDetails DataTable

                            datatableBatchPasswordDetails.Rows.Add(datarowBatchPasswordDetails);
                        }
                        dataAdapterBatchPasswordDetails.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterBatchPasswordDetails.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterBatchPasswordDetails.UpdateBatchSize = 50;
                        dataAdapterBatchPasswordDetails.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.OutputParameters;
                        dataAdapterBatchPasswordDetails.InsertCommand.Connection = connectionSessionAccessCodeDetails;
                        dataAdapterBatchPasswordDetails.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterBatchPasswordDetails.InsertCommand.CommandText = "SearchActivatedStatusForBatch";

                        #region Binding Command Parameters to stored procedure from DataTable

                        dataAdapterBatchPasswordDetails.InsertCommand.Parameters.Add("@ScheduleDetailID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleDetailID");
                        dataAdapterBatchPasswordDetails.InsertCommand.Parameters.Add("@ScheduleID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ScheduleID");
                        dataAdapterBatchPasswordDetails.InsertCommand.Parameters.Add(CommonDAL.BuildSqlParameter("@IsActivated", System.Data.SqlDbType.TinyInt, sizeof(short), "IsActivated", "", System.Data.ParameterDirection.Output));

                        #endregion Binding Command Parameters to stored procedure from DataTable

                        dataAdapterBatchPasswordDetails.Update(datatableBatchPasswordDetails);

                        for (int i = 0; i < ListPackageStatistics.Count; i++)
                        {
                            Log.LogInfo("listid:" + ListPackageStatistics[i].ScheduleDetailID);
                            Log.LogInfo("tblid:" + datatableBatchPasswordDetails.Rows[i]["Scheduledetailid"].ToString());

                            #region Binding Values to ScheduledDetails DataTable

                            if (Convert.ToInt64(datatableBatchPasswordDetails.Rows[i]["ScheduleDetailID"]) == ListPackageStatistics[i].ScheduleDetailID && Convert.ToInt64(datatableBatchPasswordDetails.Rows[i]["ScheduleID"]) == ListPackageStatistics[i].ScheduleID)
                            {
                                Log.LogInfo("abc:" + datatableBatchPasswordDetails.Rows[i]["isactivated"].ToString());
                                if (datatableBatchPasswordDetails.Rows[i]["IsActivated"] != null && datatableBatchPasswordDetails.Rows[i]["IsActivated"].ToString() != "" && datatableBatchPasswordDetails.Rows[i]["IsActivated"] != System.DBNull.Value)
                                {
                                    ListPackageStatistics[i].IsActivated = Convert.ToInt16(datatableBatchPasswordDetails.Rows[i]["IsActivated"].ToString());
                                }
                            }

                            #endregion Binding Values to ScheduledDetails DataTable
                        }
                    }
                }
            }
            Log.LogInfo("End SearchActivatedStatusForBatch()");
            return ListPackageStatistics;
        }
    }

    public class AssessmentStatisticsFactory : Generic
    {
        private int commandTimeout = 0;

        public AssessmentStatisticsFactory()
        {
            commandTimeout = 2147483647;
        }

        public void UpdateAssessmentStatistics(System.Collections.Generic.List<DataContracts.AssessmentStatistics> listAssessmentStatistics)
        {
            Log.LogInfo("Begin UpdateAssessmentStatistics() - Package(s) Count : " + (listAssessmentStatistics == null ? "0" : listAssessmentStatistics.Count.ToString()));
            if (listAssessmentStatistics != null && listAssessmentStatistics.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionAssessmentStatistics = CommonDAL.GetConnection())
                {
                    #region Building AssessmentStatistics DataTable

                    System.Data.DataTable datatableAssessmentStatistics = new System.Data.DataTable();
                    datatableAssessmentStatistics.Columns.Add("AssessmentID");
                    datatableAssessmentStatistics.Columns.Add("AssessmentName");
                    datatableAssessmentStatistics.Columns.Add("GeneratedDate");
                    datatableAssessmentStatistics.Columns.Add("TransferredToDX");
                    datatableAssessmentStatistics.Columns.Add("TransferredToDXOn");
                    datatableAssessmentStatistics.Columns.Add("RecievedFromDX");
                    datatableAssessmentStatistics.Columns.Add("RecievedFromDXOn");
                    datatableAssessmentStatistics.Columns.Add("PackageName");
                    datatableAssessmentStatistics.Columns.Add("PackagePassword");
                    datatableAssessmentStatistics.Columns.Add("StatusDescription");
                    datatableAssessmentStatistics.Columns.Add("IsPackageGenerated");
                    datatableAssessmentStatistics.Columns.Add("IsLatest");

                    #endregion Building AssessmentStatistics DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterAssessmentStatistics = new System.Data.SqlClient.SqlDataAdapter("UspUpsertAssessmentPackStatistics", connectionAssessmentStatistics))
                    {
                        foreach (DataContracts.AssessmentStatistics objAssessmentStatistics in listAssessmentStatistics)
                        {
                            System.Data.DataRow datarowAssessmentStatistics = datatableAssessmentStatistics.NewRow();

                            #region Binding Values to AssessmentStatistics DataTable

                            datarowAssessmentStatistics["AssessmentID"] = objAssessmentStatistics.AssessmentID;
                            datarowAssessmentStatistics["AssessmentName"] = objAssessmentStatistics.AssessmentName;
                            if (objAssessmentStatistics.GeneratedDate != null && objAssessmentStatistics.GeneratedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowAssessmentStatistics["GeneratedDate"] = System.DBNull.Value;
                            else
                                datarowAssessmentStatistics["GeneratedDate"] = objAssessmentStatistics.GeneratedDate;
                            datarowAssessmentStatistics["TransferredToDX"] = objAssessmentStatistics.TransferredToDX;
                            if (objAssessmentStatistics.TransferredToDXOn != null && objAssessmentStatistics.TransferredToDXOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowAssessmentStatistics["TransferredToDXOn"] = System.DBNull.Value;
                            else
                                datarowAssessmentStatistics["TransferredToDXOn"] = objAssessmentStatistics.TransferredToDXOn;
                            datarowAssessmentStatistics["RecievedFromDX"] = objAssessmentStatistics.RecievedFromDX;
                            if (objAssessmentStatistics.RecievedFromDXOn != null && objAssessmentStatistics.RecievedFromDXOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowAssessmentStatistics["RecievedFromDXOn"] = System.DBNull.Value;
                            else
                                datarowAssessmentStatistics["RecievedFromDXOn"] = objAssessmentStatistics.RecievedFromDXOn;
                            datarowAssessmentStatistics["PackageName"] = objAssessmentStatistics.PackageName;
                            datarowAssessmentStatistics["PackagePassword"] = objAssessmentStatistics.PackagePassword;
                            datarowAssessmentStatistics["StatusDescription"] = objAssessmentStatistics.StatusDescription;
                            datarowAssessmentStatistics["IsPackageGenerated"] = objAssessmentStatistics.IsPackageGenerated;
                            datarowAssessmentStatistics["IsLatest"] = objAssessmentStatistics.IsLatest;
                            datatableAssessmentStatistics.Rows.Add(datarowAssessmentStatistics);

                            #endregion Binding Values to AssessmentStatistics DataTable
                        }

                        dataAdapterAssessmentStatistics.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterAssessmentStatistics.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterAssessmentStatistics.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterAssessmentStatistics.UpdateBatchSize = 50;
                        dataAdapterAssessmentStatistics.InsertCommand.CommandText = "UspUpsertAssessmentPackStatistics";
                        dataAdapterAssessmentStatistics.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterAssessmentStatistics.InsertCommand.Connection = connectionAssessmentStatistics;

                        #region Binding Command Parameters to stored procedure from AssessmentStatistics DataTable

                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@AssessmentID", System.Data.SqlDbType.BigInt, sizeof(Int64), "AssessmentID");
                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@AssessmentName", System.Data.SqlDbType.NVarChar, 2147483646, "AssessmentName");
                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@GeneratedDate", System.Data.SqlDbType.DateTime, 20, "GeneratedDate");
                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@TransferredToDX", System.Data.SqlDbType.Int, sizeof(Int32), "TransferredToDX");
                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@TransferredToDXOn", System.Data.SqlDbType.DateTime, 20, "TransferredToDXOn");
                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@RecievedFromDX", System.Data.SqlDbType.Int, sizeof(Int32), "RecievedFromDX");
                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@RecievedFromDXOn", System.Data.SqlDbType.DateTime, 20, "RecievedFromDXOn");
                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@PackageName", System.Data.SqlDbType.NVarChar, 2147483646, "PackageName");
                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@PackagePassword", System.Data.SqlDbType.NVarChar, 2147483646, "PackagePassword");
                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@StatusDescription", System.Data.SqlDbType.NVarChar, 2147483646, "StatusDescription");
                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@IsPackageGenerated", System.Data.SqlDbType.Bit, sizeof(bool), "IsPackageGenerated");
                        dataAdapterAssessmentStatistics.InsertCommand.Parameters.Add("@IsLatest", System.Data.SqlDbType.Bit, sizeof(bool), "IsLatest");

                        #endregion Binding Command Parameters to stored procedure from AssessmentStatistics DataTable

                        dataAdapterAssessmentStatistics.Update(datatableAssessmentStatistics);
                    }
                }
            }
            Log.LogInfo("End UpdateAssessmentStatistics()");
        }

        public void UpdateTestCenterAssessmentPacks(System.Collections.Generic.List<DataContracts.TestCenterAssessmentPacks> listTestCenterAssessmentPacks)
        {
            Log.LogInfo("Begin UpdateTestCenterAssessmentPacks() - Package(s) Count : " + (listTestCenterAssessmentPacks == null ? "0" : listTestCenterAssessmentPacks.Count.ToString()));
            if (listTestCenterAssessmentPacks != null && listTestCenterAssessmentPacks.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionTestCenterAssessmentPacks = CommonDAL.GetConnection())
                {
                    #region Building TestCenterAssessmentPacks DataTable

                    System.Data.DataTable datatableTestCenterAssessmentPacks = new System.Data.DataTable();
                    datatableTestCenterAssessmentPacks.Columns.Add("AssessmentID");
                    datatableTestCenterAssessmentPacks.Columns.Add("TestCenterID");
                    datatableTestCenterAssessmentPacks.Columns.Add("TransferredToTC");
                    datatableTestCenterAssessmentPacks.Columns.Add("TransferredToTCOn");
                    datatableTestCenterAssessmentPacks.Columns.Add("RecievedFromTC");
                    datatableTestCenterAssessmentPacks.Columns.Add("RecievedFromTCOn");
                    datatableTestCenterAssessmentPacks.Columns.Add("StatusDescription");
                    datatableTestCenterAssessmentPacks.Columns.Add("LoadedDate");
                    datatableTestCenterAssessmentPacks.Columns.Add("LeadTimeForDispatchInMinutes");
                    datatableTestCenterAssessmentPacks.Columns.Add("ScheduleDetailID");

                    #endregion Building TestCenterAssessmentPacks DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterTestCenterAssessmentPacks = new System.Data.SqlClient.SqlDataAdapter("UspUpsertTestCenterAssessmentPacks", connectionTestCenterAssessmentPacks))
                    {
                        foreach (DataContracts.TestCenterAssessmentPacks objTestCenterAssessmentPacks in listTestCenterAssessmentPacks)
                        {
                            System.Data.DataRow datarowTestCenterAssessmentPacks = datatableTestCenterAssessmentPacks.NewRow();

                            #region Binding Values to TestCenterAssessmentPacks DataTable

                            datarowTestCenterAssessmentPacks["AssessmentID"] = objTestCenterAssessmentPacks.AssessmentID;
                            datarowTestCenterAssessmentPacks["TestCenterID"] = objTestCenterAssessmentPacks.TestCenterID;
                            datarowTestCenterAssessmentPacks["TransferredToTC"] = objTestCenterAssessmentPacks.TransferredToTC;
                            if (objTestCenterAssessmentPacks.TransferredToTCOn != null && objTestCenterAssessmentPacks.TransferredToTCOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowTestCenterAssessmentPacks["TransferredToTCOn"] = System.DBNull.Value;
                            else
                                datarowTestCenterAssessmentPacks["TransferredToTCOn"] = objTestCenterAssessmentPacks.TransferredToTCOn;
                            datarowTestCenterAssessmentPacks["RecievedFromTC"] = objTestCenterAssessmentPacks.RecievedFromTC;
                            if (objTestCenterAssessmentPacks.RecievedFromTCOn != null && objTestCenterAssessmentPacks.RecievedFromTCOn == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowTestCenterAssessmentPacks["RecievedFromTCOn"] = System.DBNull.Value;
                            else
                                datarowTestCenterAssessmentPacks["RecievedFromTCOn"] = objTestCenterAssessmentPacks.RecievedFromTCOn;
                            datarowTestCenterAssessmentPacks["StatusDescription"] = objTestCenterAssessmentPacks.StatusDescription;
                            if (objTestCenterAssessmentPacks.LoadedDate != null && objTestCenterAssessmentPacks.LoadedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowTestCenterAssessmentPacks["LoadedDate"] = System.DBNull.Value;
                            else
                                datarowTestCenterAssessmentPacks["LoadedDate"] = objTestCenterAssessmentPacks.LoadedDate;
                            datarowTestCenterAssessmentPacks["LeadTimeForDispatchInMinutes"] = objTestCenterAssessmentPacks.LeadTimeForDispatchInMinutes;
                            datarowTestCenterAssessmentPacks["ScheduleDetailID"] = objTestCenterAssessmentPacks.ScheduleDetailID;
                            datatableTestCenterAssessmentPacks.Rows.Add(datarowTestCenterAssessmentPacks);

                            #endregion Binding Values to TestCenterAssessmentPacks DataTable
                        }
                        dataAdapterTestCenterAssessmentPacks.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterTestCenterAssessmentPacks.UpdateBatchSize = 50;
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.CommandText = "UspUpsertTestCenterAssessmentPacks";
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.Connection = connectionTestCenterAssessmentPacks;

                        #region Binding Command Parameters to stored procedure from AssessmentStatistics DataTable

                        dataAdapterTestCenterAssessmentPacks.InsertCommand.Parameters.Add("@AssessmentID", System.Data.SqlDbType.BigInt, 2147483646, "AssessmentID");
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.Parameters.Add("@TestCenterID", System.Data.SqlDbType.BigInt, 20, "TestCenterID");
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.Parameters.Add("@TransferredToTC", System.Data.SqlDbType.Int, sizeof(Int32), "TransferredToTC");
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.Parameters.Add("@TransferredToTCOn", System.Data.SqlDbType.DateTime, 20, "TransferredToTCOn");
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.Parameters.Add("@RecievedFromTC", System.Data.SqlDbType.Int, sizeof(Int32), "RecievedFromTC");
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.Parameters.Add("@RecievedFromTCOn", System.Data.SqlDbType.DateTime, 20, "RecievedFromTCOn");
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.Parameters.Add("@StatusDescription", System.Data.SqlDbType.NVarChar, 2147483646, "StatusDescription");
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.Parameters.Add("@LoadedDate", System.Data.SqlDbType.DateTime, 20, "LoadedDate");
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.Parameters.Add("@LeadTimeForDispatchInMinutes", System.Data.SqlDbType.Int, sizeof(Int32), "LeadTimeForDispatchInMinutes");
                        dataAdapterTestCenterAssessmentPacks.InsertCommand.Parameters.Add("@ScheduleDetailID", System.Data.SqlDbType.BigInt, 2147483646, "ScheduleDetailID");

                        #endregion Binding Command Parameters to stored procedure from AssessmentStatistics DataTable

                        dataAdapterTestCenterAssessmentPacks.Update(datatableTestCenterAssessmentPacks);
                    }
                }
            }
            Log.LogInfo("End UpdateTestCenterAssessmentPacks()");
        }

        public System.Collections.Generic.List<DataContracts.TestCenetrAssessmentStatistics> SearchAssessmentStatistics(String MacID)
        {
            Log.LogInfo("Begin SearchAssessmentStatistics() - Package(s) Count : " + (MacID == null ? "0" : MacID));
            List<DataContracts.TestCenetrAssessmentStatistics> ListAssessmentStatistics = null;
            using (System.Data.SqlClient.SqlConnection connectionAssessmentStatistics = CommonDAL.GetConnection())
            {
                connectionAssessmentStatistics.Open();
                using (System.Data.SqlClient.SqlCommand commandAssessmentStatistics = new System.Data.SqlClient.SqlCommand())
                {
                    commandAssessmentStatistics.CommandTimeout = commandTimeout;
                    commandAssessmentStatistics.Connection = connectionAssessmentStatistics;
                    commandAssessmentStatistics.CommandText = "UspSearchAssessmentPackages";
                    commandAssessmentStatistics.CommandType = System.Data.CommandType.StoredProcedure;
                    commandAssessmentStatistics.Parameters.Add(CommonDAL.BuildSqlParameter("@MacID", System.Data.SqlDbType.NVarChar, 2147483646, "MacID", MacID, System.Data.ParameterDirection.Input));
                    using (System.Data.SqlClient.SqlDataReader dataReaderAssessmentStatistics = commandAssessmentStatistics.ExecuteReader())
                    {
                        if (dataReaderAssessmentStatistics != null && dataReaderAssessmentStatistics.HasRows)
                        {
                            //To read TestcenterAssessmentStatistics details
                            ListAssessmentStatistics = ReadAssessmentStatistics(dataReaderAssessmentStatistics);
                        }
                    }
                }
            }
            Log.LogInfo("End SearchAssessmentStatistics() - Package(s) Count : " + (MacID == null ? "0" : MacID));
            return ListAssessmentStatistics;
        }

        public System.Collections.Generic.List<DataContracts.TestCenetrAssessmentStatistics> ReadAssessmentStatistics(System.Data.SqlClient.SqlDataReader reader)
        {
            Log.LogInfo("Begin ReadAssessmentStatistics()");
            System.Collections.Generic.List<DataContracts.TestCenetrAssessmentStatistics> listAssessmentStatistics = null;
            DataContracts.TestCenetrAssessmentStatistics objAssessmentStatistics = null;
            if (reader != null && reader.HasRows)
            {
                listAssessmentStatistics = new System.Collections.Generic.List<DataContracts.TestCenetrAssessmentStatistics>();
                int Index = 0;
                while (reader.Read())
                {
                    objAssessmentStatistics = new DataContracts.TestCenetrAssessmentStatistics();

                    #region Reading the Testcenter Assessment Statistics

                    Index = reader.GetOrdinal("AssessmentID");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.AssessmentID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("AssessmentName");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.AssessmentName = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("GeneratedDate");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.GeneratedDate = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToDX");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.TransferredToDX = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToDXOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.TransferredToDXOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromDX");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.RecievedFromDX = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromDXOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.RecievedFromDXOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("PackageName");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.PackageName = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("PackagePassword");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.PackagePassword = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("LeadTimeForDispatchInMinutes");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.LeadTimeForDispatchInMinutes = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("StatusDescription");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.StatusDescription = reader.GetString(Index);
                    }
                    Index = reader.GetOrdinal("IsPackageGenerated");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.IsPackageGenerated = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("IsLatest");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.IsLatest = reader.GetBoolean(Index);
                    }
                    Index = reader.GetOrdinal("TestCenterID");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.TestCenterID = reader.GetInt64(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToTC");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.TransferredToTC = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("TransferredToTCOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.TransferredToTCOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromTC");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.RecievedFromTC = reader.GetInt32(Index);
                    }
                    Index = reader.GetOrdinal("RecievedFromTCOn");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.RecievedFromTCOn = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("LoadedDate");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.LoadedDate = reader.GetDateTime(Index);
                    }
                    Index = reader.GetOrdinal("ScheduleDetailID");
                    if (!reader.IsDBNull(Index))
                    {
                        objAssessmentStatistics.ScheduleDetailID = reader.GetInt64(Index);
                    }

                    #endregion Reading the Testcenter Assessment Statistics

                    listAssessmentStatistics.Add(objAssessmentStatistics);
                }
            }
            Log.LogInfo("End ReadAssessmentStatistics() - Package(s) Count : " + (listAssessmentStatistics == null ? "0" : listAssessmentStatistics.Count.ToString()));
            return listAssessmentStatistics;
        }
    }

    public class GenericDAL : Generic
    {
        private int commandTimeout = 0;

        public GenericDAL()
        {
            commandTimeout = 2147483647;
        }

        public string ValidateMacID(string MacID, DataContracts.ServerTypes ServerType)
        {
            string Status = "E000";
            Log.LogInfo("Begin ValidateMacID() - MacID: " + MacID);
            Log.LogInfo("Begin ValidateMacID() - Server Type: " + ServerType);
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

                    #endregion Building Parameters

                    commandValidateMacID.ExecuteNonQuery();
                    if (commandValidateMacID != null && commandValidateMacID.Parameters != null && commandValidateMacID.Parameters["@Status"] != null
                        && commandValidateMacID.Parameters["@Status"].Value != null && commandValidateMacID.Parameters["@Status"].Value != DBNull.Value)
                        Status = commandValidateMacID.Parameters["@Status"].Value.ToString();
                }
                connectionValidateMacID.Close();
            }
            Log.LogInfo("End ValidateMacID()");
            return Status;
        }

        public string ValidateMSIInstallation(string MacID)
        {
            string Status = "S000";
            Log.LogInfo("Begin ValidateMSIInstallation() " + MacID);

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

                    #endregion Building Parameters

                    commandValidateMSI.ExecuteNonQuery();
                    if (commandValidateMSI != null && commandValidateMSI.Parameters != null && commandValidateMSI.Parameters["@Status"] != null
                        && commandValidateMSI.Parameters["@Status"].Value != null && commandValidateMSI.Parameters["@Status"].Value != DBNull.Value)
                        Status = commandValidateMSI.Parameters["@Status"].Value.ToString();
                }
                conValidateMSIInstallation.Close();
            }
            Log.LogInfo("End ValidateMSIInstallation()");
            return Status;
        }

        public List<DataContracts.TcBatches> GetTestCenterBatch(ServiceContracts.Contracts.TcBatchesRequest Request)
        {
            List<DataContracts.TcBatches> lstMain = new List<DataContracts.TcBatches>();
            DataContracts.TcBatches lst = null;
            string validatetc = ValidateMacID(Request.MacID, global::LicensingAndTransfer.DataContracts.ServerTypes.TestCenter);
            if (validatetc.ToLower() != "s001")
            {
                return lstMain;
            }
            else
            {
                using (System.Data.SqlClient.SqlConnection connection = CommonDAL.GetConnection())
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                        connection.Open();
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "UspGetTestCenterBatch";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Date", Request.ScheduleDate);
                        command.Parameters.AddWithValue("@MacID", Request.MacID);
                        using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                int Index;
                                while (reader.Read())
                                {
                                    lst = new DataContracts.TcBatches();
                                    Index = reader.GetOrdinal("StartDateTime");
                                    if (!reader.IsDBNull(Index))
                                        lst.ScheduleDate = reader.GetDateTime(Index);

                                    lstMain.Add(lst);
                                }
                            }
                        }
                    }
                    connection.Close();
                }
            }
            return lstMain;
        }

        public List<DataContracts.TcVerification> FetchQpackRpackDetailsForTcVerification(ServiceContracts.Contracts.TcVerificationRequest Request)
        {
            List<DataContracts.TcVerification> lstMain = new List<DataContracts.TcVerification>();
            DataContracts.TcVerification lst = null;
            string validatetc = ValidateMacID(Request.MacID, global::LicensingAndTransfer.DataContracts.ServerTypes.TestCenter);
            if (validatetc.ToLower() != "s001")
            {
                return lstMain;
            }
            else
            {
                using (System.Data.SqlClient.SqlConnection connection = CommonDAL.GetConnection())
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                        connection.Open();
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "UspGetQpackRpackDetailsForTcDXVerification";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Date", Request.ScheduleDate);
                        command.Parameters.AddWithValue("@MacID", Request.MacID);
                        if (!Request.BatchTime.Equals("0"))

                            command.Parameters.AddWithValue("@BatchTime", Request.BatchTime);
                        using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                int Index;
                                while (reader.Read())
                                {
                                    lst = new DataContracts.TcVerification();
                                    Index = reader.GetOrdinal("PackageType");
                                    if (!reader.IsDBNull(Index))
                                        lst.PackageType = reader.GetString(Index);
                                    Index = reader.GetOrdinal("batchname");
                                    if (!reader.IsDBNull(Index))
                                        lst.Batch = reader.GetString(Index);
                                    Index = reader.GetOrdinal("RecievedFromTestCenter");
                                    if (!reader.IsDBNull(Index))
                                        lst.Rft = reader.GetInt32(Index);
                                    Index = reader.GetOrdinal("GeneratedDate");
                                    if (!reader.IsDBNull(Index))
                                        lst.GeneratedDate = reader.GetDateTime(Index);
                                    Index = reader.GetOrdinal("CandidateCount");
                                    if (!reader.IsDBNull(Index))
                                        lst.CandidateCount = reader.GetInt32(Index);

                                    Index = reader.GetOrdinal("ScheduleStartDate");
                                    if (!reader.IsDBNull(Index))
                                        lst.ScheduleStartDate = reader.GetDateTime(Index);

                                    Index = reader.GetOrdinal("ScheduleEndDate");
                                    if (!reader.IsDBNull(Index))
                                        lst.ScheduleEndDate = reader.GetDateTime(Index);

                                    Index = reader.GetOrdinal("ScheduleDetailID");
                                    if (!reader.IsDBNull(Index))
                                        lst.ScheduleDetailID = reader.GetInt64(Index);

                                    lstMain.Add(lst);
                                }
                            }
                        }
                    }
                    connection.Close();
                }
            }
            return lstMain;
        }

        public int ResetTcQpackReceivedStatus(string MacID, Int64 Sid)
        {
            int status = 0;
            string validatetc = ValidateMacID(MacID, global::LicensingAndTransfer.DataContracts.ServerTypes.TestCenter);
            if (validatetc.ToLower() != "s001")
            {
                status = -1;
            }
            else
            {
                using (System.Data.SqlClient.SqlConnection connection = CommonDAL.GetConnection())
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                        connection.Open();
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "ResetTcQpackReceivedStatusNullatDX";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ScheduleDetailID", Sid);
                        using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                int Index;
                                while (reader.Read())
                                {
                                    Index = reader.GetOrdinal("RecordStatus");
                                    if (!reader.IsDBNull(Index))
                                    {
                                        status = reader.GetInt32(Index);
                                    }
                                }
                            }
                        }
                    }
                    connection.Close();
                }
            }
            return status;
        }

        public List<ServiceContracts.LoadedMediaPackageDetailsResponse> FetchLoadedMediaDetails(string MacID)
        {
            Log.LogInfo("Begin FetchLoadedMediaDetails() " + MacID);
            List<ServiceContracts.LoadedMediaPackageDetailsResponse> lstMedia = new List<ServiceContracts.LoadedMediaPackageDetailsResponse>();
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
                                ServiceContracts.LoadedMediaPackageDetailsResponse MediaPackageObj = new ServiceContracts.LoadedMediaPackageDetailsResponse();
                                Index = reader.GetOrdinal("LoadedMediaPackageDetailID");
                                if (!reader.IsDBNull(Index))
                                {
                                    MediaPackageObj.LoadedMediaPackageDetailID = reader.GetInt64(Index);
                                }
                                Index = reader.GetOrdinal("TestCenterID");
                                if (!reader.IsDBNull(Index))
                                {
                                    MediaPackageObj.TestCenterID = reader.GetInt64(Index);
                                }
                                Index = reader.GetOrdinal("TestCenterCode");
                                if (!reader.IsDBNull(Index))
                                {
                                    MediaPackageObj.TestCenterCode = reader.GetString(Index);
                                }
                                Index = reader.GetOrdinal("TestCenterName");
                                if (!reader.IsDBNull(Index))
                                {
                                    MediaPackageObj.TestCenterName = reader.GetString(Index);
                                }
                                Index = reader.GetOrdinal("MediaPackageName");
                                if (!reader.IsDBNull(Index))
                                {
                                    MediaPackageObj.MediaPackageName = reader.GetString(Index);
                                }
                                lstMedia.Add(MediaPackageObj);
                            }
                        }
                    }
                }
                conLoadMedia.Close();
            }
            Log.LogInfo("End FetchLoadedMediaDetails()");
            return lstMedia;
        }

        public ServiceContracts.ValidateTestCenterTypeResponse ValidateTestCenterType(string MacID, DataContracts.ServerTypes ServerType)
        {
            ServiceContracts.ValidateTestCenterTypeResponse response = new ServiceContracts.ValidateTestCenterTypeResponse();
            try
            {
                string Status = "E000";
                Log.LogInfo("Begin ValidateTestCenterType() - MacID: " + MacID);
                Log.LogInfo("Begin ValidateTestCenterType() - Server Type: " + ServerType);
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
                        commandValidateMacID.Parameters.Add(CommonDAL.BuildSqlParameter("@TestCenterType", System.Data.SqlDbType.NVarChar, 2147483646, "TestCenterType", Status, System.Data.ParameterDirection.Output));

                        #endregion Building Parameters

                        commandValidateMacID.ExecuteNonQuery();
                        if (commandValidateMacID != null && commandValidateMacID.Parameters != null && commandValidateMacID.Parameters["@Status"] != null
                            && commandValidateMacID.Parameters["@Status"].Value != null && commandValidateMacID.Parameters["@Status"].Value != DBNull.Value)
                            response.StatusCode = commandValidateMacID.Parameters["@Status"].Value.ToString();
                        if (commandValidateMacID != null && commandValidateMacID.Parameters != null && commandValidateMacID.Parameters["@TestCenterType"] != null
                            && commandValidateMacID.Parameters["@TestCenterType"].Value != null && commandValidateMacID.Parameters["@TestCenterType"].Value != DBNull.Value)
                            response.TestCenterType = commandValidateMacID.Parameters["@TestCenterType"].Value.ToString();
                    }
                    connectionValidateMacID.Close();
                }
                Log.LogInfo("End ValidateTestCenterType()");
                return response;
            }
            catch (Exception ex)
            {
                Log.LogError("ValidateTestCenterType() Error:" + ex.StackTrace);
                response.StatusCode = "E000";
                return response;
            }
        }
    }

    public partial class UserFactory : Generic
    {
        private int commandTimeout = 0;

        public UserFactory()
        {
            commandTimeout = 2147483647;
        }

        public void UpsertUser(System.Collections.Generic.List<DataContracts.User> listUser)
        {
            Log.LogInfo("Begin Upsert() for User, Count :" + (listUser == null ? "0" : listUser.Count.ToString()));
            List<DataContracts.UserProfile> ListUserProfile = null;
            if (listUser != null && listUser.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionUser = CommonDAL.GetConnection())
                {
                    #region Building User DataTable

                    System.Data.DataTable datatableUser = new System.Data.DataTable();
                    datatableUser.Columns.Add("UserID");
                    datatableUser.Columns.Add("FirstName");
                    datatableUser.Columns.Add("LoginName");
                    datatableUser.Columns.Add("Password");
                    datatableUser.Columns.Add("Email");
                    datatableUser.Columns.Add("LastName");
                    datatableUser.Columns.Add("ClassID");
                    datatableUser.Columns.Add("YearID");
                    datatableUser.Columns.Add("UserType");
                    datatableUser.Columns.Add("OrganizationID");
                    datatableUser.Columns.Add("CreatedBy");
                    datatableUser.Columns.Add("CreatedDate");
                    datatableUser.Columns.Add("ModifiedBy");
                    datatableUser.Columns.Add("ModifiedDate");
                    datatableUser.Columns.Add("IsDeleted");
                    datatableUser.Columns.Add("IsOffLineAuthoring");
                    datatableUser.Columns.Add("IsLoggedIN");
                    datatableUser.Columns.Add("IsActive");
                    datatableUser.Columns.Add("IsApprove");
                    datatableUser.Columns.Add("IsAllowEdit");
                    datatableUser.Columns.Add("ManagerID");
                    datatableUser.Columns.Add("UserCode");
                    datatableUser.Columns.Add("IsFirstTimeLoggedIn");
                    datatableUser.Columns.Add("OfficeID");
                    datatableUser.Columns.Add("SectionID");
                    datatableUser.Columns.Add("IsManager");
                    datatableUser.Columns.Add("LoginCount");
                    datatableUser.Columns.Add("PasswordLastModifiedDate");
                    datatableUser.Columns.Add("AdditionalTimeInPercent");
                    datatableUser.Columns.Add("IsBlock");
                    datatableUser.Columns.Add("CourseType");
                    datatableUser.Columns.Add("Course");
                    datatableUser.Columns.Add("EnrollmentNumber");
                    datatableUser.Columns.Add("LastLoginDate");
                    datatableUser.Columns.Add("LastLogoutDate");
                    datatableUser.Columns.Add("LocationID");
                    datatableUser.Columns.Add("AnnotationSettings");

                    #endregion Building User DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterUser = new System.Data.SqlClient.SqlDataAdapter("UpsertUser", connectionUser))
                    {
                        foreach (DataContracts.User objUser in listUser)
                        {
                            System.Data.DataRow datarowUser = datatableUser.NewRow();

                            #region Binding Values to User DataTable

                            if (objUser.UserID <= 0)
                                datarowUser["UserID"] = System.DBNull.Value;
                            else
                                datarowUser["UserID"] = objUser.UserID;
                            datarowUser["FirstName"] = objUser.FirstName;
                            datarowUser["LoginName"] = objUser.LoginName;
                            datarowUser["Password"] = objUser.Password;
                            datarowUser["Email"] = objUser.Email;
                            datarowUser["LastName"] = objUser.LastName;
                            if (objUser.ClassID <= 0)
                                datarowUser["ClassID"] = System.DBNull.Value;
                            else
                                datarowUser["ClassID"] = objUser.ClassID;
                            if (objUser.YearID <= 0)
                                datarowUser["YearID"] = System.DBNull.Value;
                            else
                                datarowUser["YearID"] = objUser.YearID;
                            if (objUser.UserType <= 0)
                                datarowUser["UserType"] = System.DBNull.Value;
                            else
                                datarowUser["UserType"] = objUser.UserType;
                            if (objUser.OrganizationID <= 0)
                                datarowUser["OrganizationID"] = System.DBNull.Value;
                            else
                                datarowUser["OrganizationID"] = objUser.OrganizationID;
                            if (objUser.CreatedBy <= 0)
                                datarowUser["CreatedBy"] = System.DBNull.Value;
                            else
                                datarowUser["CreatedBy"] = objUser.CreatedBy;
                            if (objUser.CreatedDate != null && objUser.CreatedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowUser["CreatedDate"] = System.DBNull.Value;
                            else
                                datarowUser["CreatedDate"] = objUser.CreatedDate;
                            if (objUser.ModifiedBy <= 0)
                                datarowUser["ModifiedBy"] = System.DBNull.Value;
                            else
                                datarowUser["ModifiedBy"] = objUser.ModifiedBy;
                            if (objUser.ModifiedDate != null && objUser.ModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowUser["ModifiedDate"] = System.DBNull.Value;
                            else
                                datarowUser["ModifiedDate"] = objUser.ModifiedDate;
                            datarowUser["IsDeleted"] = objUser.IsDeleted;
                            datarowUser["IsOffLineAuthoring"] = objUser.IsOffLineAuthoring;
                            datarowUser["IsLoggedIN"] = objUser.IsLoggedIN;
                            datarowUser["IsActive"] = objUser.IsActive;
                            datarowUser["IsApprove"] = objUser.IsApprove;
                            datarowUser["IsAllowEdit"] = objUser.IsAllowEdit;
                            if (objUser.ManagerID <= 0)
                                datarowUser["ManagerID"] = System.DBNull.Value;
                            else
                                datarowUser["ManagerID"] = objUser.ManagerID;
                            datarowUser["UserCode"] = objUser.UserCode;
                            datarowUser["IsFirstTimeLoggedIn"] = objUser.IsFirstTimeLoggedIn;
                            if (objUser.OfficeID <= 0)
                                datarowUser["OfficeID"] = System.DBNull.Value;
                            else
                                datarowUser["OfficeID"] = objUser.OfficeID;
                            if (objUser.SectionID <= 0)
                                datarowUser["SectionID"] = System.DBNull.Value;
                            else
                                datarowUser["SectionID"] = objUser.SectionID;
                            datarowUser["IsManager"] = objUser.IsManager;
                            if (objUser.LoginCount <= 0)
                                datarowUser["LoginCount"] = System.DBNull.Value;
                            else
                                datarowUser["LoginCount"] = objUser.LoginCount;
                            if (objUser.PasswordLastModifiedDate != null && objUser.PasswordLastModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowUser["PasswordLastModifiedDate"] = System.DBNull.Value;
                            else
                                datarowUser["PasswordLastModifiedDate"] = objUser.PasswordLastModifiedDate;
                            datarowUser["AdditionalTimeInPercent"] = objUser.AdditionalTimeInPercent;
                            datarowUser["IsBlock"] = objUser.IsBlock;
                            datarowUser["CourseType"] = objUser.CourseType;
                            datarowUser["Course"] = objUser.Course;
                            datarowUser["EnrollmentNumber"] = objUser.EnrollmentNumber;
                            if (objUser.LastLoginDate != null && objUser.LastLoginDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowUser["LastLoginDate"] = System.DBNull.Value;
                            else
                                datarowUser["LastLoginDate"] = objUser.LastLoginDate;
                            if (objUser.LastLogoutDate != null && objUser.LastLogoutDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowUser["LastLogoutDate"] = System.DBNull.Value;
                            else
                                datarowUser["LastLogoutDate"] = objUser.LastLogoutDate;
                            if (objUser.LocationID <= 0)
                                datarowUser["LocationID"] = System.DBNull.Value;
                            else
                                datarowUser["LocationID"] = objUser.LocationID;
                            datarowUser["AnnotationSettings"] = objUser.AnnotationSettings;
                            datatableUser.Rows.Add(datarowUser);

                            #endregion Binding Values to User DataTable

                            //  To load the profile of a User
                            if (objUser.ListUserProfile != null && objUser.ListUserProfile.Count > 0)
                            {
                                if (ListUserProfile == null)
                                    ListUserProfile = new List<DataContracts.UserProfile>();
                                ListUserProfile.AddRange(objUser.ListUserProfile);
                            }
                        }
                        dataAdapterUser.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterUser.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterUser.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterUser.UpdateBatchSize = 50;
                        dataAdapterUser.InsertCommand.CommandText = "UpsertUser";
                        dataAdapterUser.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterUser.InsertCommand.Connection = connectionUser;

                        #region Binding Command Parameters to stored procedure from User DataTable

                        dataAdapterUser.InsertCommand.Parameters.Add("@UserID", System.Data.SqlDbType.BigInt, sizeof(Int64), "UserID");
                        dataAdapterUser.InsertCommand.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar, 2147483646, "FirstName");
                        dataAdapterUser.InsertCommand.Parameters.Add("@LoginName", System.Data.SqlDbType.NVarChar, 2147483646, "LoginName");
                        dataAdapterUser.InsertCommand.Parameters.Add("@Password", System.Data.SqlDbType.NVarChar, 2147483646, "Password");
                        dataAdapterUser.InsertCommand.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 2147483646, "Email");
                        dataAdapterUser.InsertCommand.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar, 2147483646, "LastName");
                        dataAdapterUser.InsertCommand.Parameters.Add("@ClassID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ClassID");
                        dataAdapterUser.InsertCommand.Parameters.Add("@YearID", System.Data.SqlDbType.Int, sizeof(Int32), "YearID");
                        dataAdapterUser.InsertCommand.Parameters.Add("@UserType", System.Data.SqlDbType.BigInt, sizeof(Int64), "UserType");
                        dataAdapterUser.InsertCommand.Parameters.Add("@OrganizationID", System.Data.SqlDbType.BigInt, sizeof(Int64), "OrganizationID");
                        dataAdapterUser.InsertCommand.Parameters.Add("@CreatedBy", System.Data.SqlDbType.BigInt, sizeof(Int64), "CreatedBy");
                        dataAdapterUser.InsertCommand.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime, 20, "CreatedDate");
                        dataAdapterUser.InsertCommand.Parameters.Add("@ModifiedBy", System.Data.SqlDbType.BigInt, sizeof(Int64), "ModifiedBy");
                        dataAdapterUser.InsertCommand.Parameters.Add("@ModifiedDate", System.Data.SqlDbType.DateTime, 20, "ModifiedDate");
                        dataAdapterUser.InsertCommand.Parameters.Add("@IsDeleted", System.Data.SqlDbType.Bit, sizeof(bool), "IsDeleted");
                        dataAdapterUser.InsertCommand.Parameters.Add("@IsOffLineAuthoring", System.Data.SqlDbType.Bit, sizeof(bool), "IsOffLineAuthoring");
                        dataAdapterUser.InsertCommand.Parameters.Add("@IsLoggedIN", System.Data.SqlDbType.Bit, sizeof(bool), "IsLoggedIN");
                        dataAdapterUser.InsertCommand.Parameters.Add("@IsActive", System.Data.SqlDbType.Bit, sizeof(bool), "IsActive");
                        dataAdapterUser.InsertCommand.Parameters.Add("@IsApprove", System.Data.SqlDbType.Bit, sizeof(bool), "IsApprove");
                        dataAdapterUser.InsertCommand.Parameters.Add("@IsAllowEdit", System.Data.SqlDbType.Bit, sizeof(bool), "IsAllowEdit");
                        dataAdapterUser.InsertCommand.Parameters.Add("@ManagerID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ManagerID");
                        dataAdapterUser.InsertCommand.Parameters.Add("@UserCode", System.Data.SqlDbType.NVarChar, 2147483646, "UserCode");
                        dataAdapterUser.InsertCommand.Parameters.Add("@IsFirstTimeLoggedIn", System.Data.SqlDbType.Bit, sizeof(bool), "IsFirstTimeLoggedIn");
                        dataAdapterUser.InsertCommand.Parameters.Add("@OfficeID", System.Data.SqlDbType.BigInt, sizeof(Int64), "OfficeID");
                        dataAdapterUser.InsertCommand.Parameters.Add("@SectionID", System.Data.SqlDbType.BigInt, sizeof(Int64), "SectionID");
                        dataAdapterUser.InsertCommand.Parameters.Add("@IsManager", System.Data.SqlDbType.Bit, sizeof(bool), "IsManager");
                        dataAdapterUser.InsertCommand.Parameters.Add("@LoginCount", System.Data.SqlDbType.Int, sizeof(Int32), "LoginCount");
                        dataAdapterUser.InsertCommand.Parameters.Add("@PasswordLastModifiedDate", System.Data.SqlDbType.DateTime, 20, "PasswordLastModifiedDate");
                        dataAdapterUser.InsertCommand.Parameters.Add("@AdditionalTimeInPercent", System.Data.SqlDbType.Decimal, sizeof(Decimal), "AdditionalTimeInPercent");
                        dataAdapterUser.InsertCommand.Parameters.Add("@IsBlock", System.Data.SqlDbType.Bit, sizeof(bool), "IsBlock");
                        dataAdapterUser.InsertCommand.Parameters.Add("@CourseType", System.Data.SqlDbType.NVarChar, 2147483646, "CourseType");
                        dataAdapterUser.InsertCommand.Parameters.Add("@Course", System.Data.SqlDbType.NVarChar, 2147483646, "Course");
                        dataAdapterUser.InsertCommand.Parameters.Add("@EnrollmentNumber", System.Data.SqlDbType.NVarChar, 2147483646, "EnrollmentNumber");
                        dataAdapterUser.InsertCommand.Parameters.Add("@LastLoginDate", System.Data.SqlDbType.DateTime, 20, "LastLoginDate");
                        dataAdapterUser.InsertCommand.Parameters.Add("@LastLogoutDate", System.Data.SqlDbType.DateTime, 20, "LastLogoutDate");
                        dataAdapterUser.InsertCommand.Parameters.Add("@LocationID", System.Data.SqlDbType.Int, sizeof(Int32), "LocationID");
                        dataAdapterUser.InsertCommand.Parameters.Add("@AnnotationSettings", System.Data.SqlDbType.NText, 2147483646, "AnnotationSettings");

                        #endregion Binding Command Parameters to stored procedure from User DataTable

                        dataAdapterUser.Update(datatableUser);

                        //  To load the profile of a User
                        UpsertUserProfile(ListUserProfile);
                    }
                }
            }
            Log.LogInfo("End Upsert() for  User");
        }

        public void UpsertUserProfile(System.Collections.Generic.List<DataContracts.UserProfile> listUserProfile)
        {
            Log.LogInfo("Begin UpsertUserProfile() for UserProfile, Count :" + (listUserProfile == null ? "0" : listUserProfile.Count.ToString()));
            if (listUserProfile != null && listUserProfile.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionUserProfile = CommonDAL.GetConnection())
                {
                    #region Building UserProfile DataTable

                    System.Data.DataTable datatableUserProfile = new System.Data.DataTable();
                    datatableUserProfile.Columns.Add("UserID");
                    datatableUserProfile.Columns.Add("Gender");
                    datatableUserProfile.Columns.Add("TelephoneNUM1");
                    datatableUserProfile.Columns.Add("TelephoneNUM2");
                    datatableUserProfile.Columns.Add("MobileNUM");
                    datatableUserProfile.Columns.Add("PersonalNUM");
                    datatableUserProfile.Columns.Add("PostNUM");
                    datatableUserProfile.Columns.Add("Remarks");
                    datatableUserProfile.Columns.Add("Photo1");
                    datatableUserProfile.Columns.Add("MyVideo");
                    datatableUserProfile.Columns.Add("SecreatQuestion");
                    datatableUserProfile.Columns.Add("HintAnswer");
                    datatableUserProfile.Columns.Add("TimeZone");
                    datatableUserProfile.Columns.Add("TimeFormat");
                    datatableUserProfile.Columns.Add("DateFormat");
                    datatableUserProfile.Columns.Add("EducationalLevel");
                    datatableUserProfile.Columns.Add("IsDeleted");
                    datatableUserProfile.Columns.Add("PAssetID");
                    datatableUserProfile.Columns.Add("VAssetID");
                    datatableUserProfile.Columns.Add("Language");
                    datatableUserProfile.Columns.Add("DOB");
                    datatableUserProfile.Columns.Add("TeacherCode");
                    datatableUserProfile.Columns.Add("Initial");
                    datatableUserProfile.Columns.Add("keyword");
                    datatableUserProfile.Columns.Add("AlternateEmail");
                    datatableUserProfile.Columns.Add("Designation");
                    datatableUserProfile.Columns.Add("OfficeID");
                    datatableUserProfile.Columns.Add("AssessmentsEnrolledFor");
                    datatableUserProfile.Columns.Add("EnrolledDate");
                    datatableUserProfile.Columns.Add("ManagerName");
                    datatableUserProfile.Columns.Add("OfficeName");
                    datatableUserProfile.Columns.Add("RoleName");
                    datatableUserProfile.Columns.Add("HallTicketNumber");
                    datatableUserProfile.Columns.Add("TeachingMode");
                    datatableUserProfile.Columns.Add("StreamName");
                    datatableUserProfile.Columns.Add("BranchID");
                    datatableUserProfile.Columns.Add("SemesterID");
                    datatableUserProfile.Columns.Add("VerificationPhotoID");
                    datatableUserProfile.Columns.Add("FingerPrint1");
                    datatableUserProfile.Columns.Add("FingerPrint2");

                    #endregion Building UserProfile DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterUserProfile = new System.Data.SqlClient.SqlDataAdapter("UpsertUserProfile", connectionUserProfile))
                    {
                        foreach (DataContracts.UserProfile objUserProfile in listUserProfile)
                        {
                            System.Data.DataRow datarowUserProfile = datatableUserProfile.NewRow();

                            #region Binding Values to UserProfile DataTable

                            if (objUserProfile.UserID <= 0)
                                datarowUserProfile["UserID"] = System.DBNull.Value;
                            else
                                datarowUserProfile["UserID"] = objUserProfile.UserID;
                            datarowUserProfile["Gender"] = objUserProfile.Gender;
                            datarowUserProfile["TelephoneNUM1"] = objUserProfile.TelephoneNUM1;
                            datarowUserProfile["TelephoneNUM2"] = objUserProfile.TelephoneNUM2;
                            datarowUserProfile["MobileNUM"] = objUserProfile.MobileNUM;
                            datarowUserProfile["PersonalNUM"] = objUserProfile.PersonalNUM;
                            datarowUserProfile["PostNUM"] = objUserProfile.PostNUM;
                            datarowUserProfile["Remarks"] = objUserProfile.Remarks;
                            datarowUserProfile["Photo1"] = objUserProfile.Photo1;
                            datarowUserProfile["MyVideo"] = objUserProfile.MyVideo;
                            datarowUserProfile["SecreatQuestion"] = objUserProfile.SecreatQuestion;
                            datarowUserProfile["HintAnswer"] = objUserProfile.HintAnswer;
                            datarowUserProfile["TimeZone"] = objUserProfile.TimeZone;
                            datarowUserProfile["TimeFormat"] = objUserProfile.TimeFormat;
                            datarowUserProfile["DateFormat"] = objUserProfile.DateFormat;
                            datarowUserProfile["EducationalLevel"] = objUserProfile.EducationalLevel;
                            datarowUserProfile["IsDeleted"] = objUserProfile.IsDeleted;
                            if (objUserProfile.PAssetID <= 0)
                                datarowUserProfile["PAssetID"] = System.DBNull.Value;
                            else
                                datarowUserProfile["PAssetID"] = objUserProfile.PAssetID;
                            if (objUserProfile.VAssetID <= 0)
                                datarowUserProfile["VAssetID"] = System.DBNull.Value;
                            else
                                datarowUserProfile["VAssetID"] = objUserProfile.VAssetID;
                            datarowUserProfile["Language"] = objUserProfile.Language;
                            if (objUserProfile.DOB != null && objUserProfile.DOB == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowUserProfile["DOB"] = System.DBNull.Value;
                            else
                                datarowUserProfile["DOB"] = objUserProfile.DOB;
                            if (objUserProfile.TeacherCode <= 0)
                                datarowUserProfile["TeacherCode"] = System.DBNull.Value;
                            else
                                datarowUserProfile["TeacherCode"] = objUserProfile.TeacherCode;
                            datarowUserProfile["Initial"] = objUserProfile.Initial;
                            datarowUserProfile["keyword"] = objUserProfile.Keyword;
                            datarowUserProfile["AlternateEmail"] = objUserProfile.AlternateEmail;
                            datarowUserProfile["Designation"] = objUserProfile.Designation;
                            if (objUserProfile.OfficeID <= 0)
                                datarowUserProfile["OfficeID"] = System.DBNull.Value;
                            else
                                datarowUserProfile["OfficeID"] = objUserProfile.OfficeID;
                            datarowUserProfile["AssessmentsEnrolledFor"] = objUserProfile.AssessmentsEnrolledFor;
                            if (objUserProfile.EnrolledDate != null && objUserProfile.EnrolledDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowUserProfile["EnrolledDate"] = System.DBNull.Value;
                            else
                                datarowUserProfile["EnrolledDate"] = objUserProfile.EnrolledDate;
                            datarowUserProfile["ManagerName"] = objUserProfile.ManagerName;
                            datarowUserProfile["OfficeName"] = objUserProfile.OfficeName;
                            datarowUserProfile["RoleName"] = objUserProfile.RoleName;
                            datarowUserProfile["HallTicketNumber"] = objUserProfile.HallTicketNumber;
                            datarowUserProfile["TeachingMode"] = objUserProfile.TeachingMode;
                            datarowUserProfile["StreamName"] = objUserProfile.StreamName;
                            if (objUserProfile.BranchID <= 0)
                                datarowUserProfile["BranchID"] = System.DBNull.Value;
                            else
                                datarowUserProfile["BranchID"] = objUserProfile.BranchID;
                            if (objUserProfile.SemesterID <= 0)
                                datarowUserProfile["SemesterID"] = System.DBNull.Value;
                            else
                                datarowUserProfile["SemesterID"] = objUserProfile.SemesterID;
                            if (objUserProfile.VerificationPhotoID <= 0)
                                datarowUserProfile["VerificationPhotoID"] = System.DBNull.Value;
                            else
                                datarowUserProfile["VerificationPhotoID"] = objUserProfile.VerificationPhotoID;
                            datarowUserProfile["FingerPrint1"] = objUserProfile.FingerPrint1;
                            datarowUserProfile["FingerPrint2"] = objUserProfile.FingerPrint2;
                            datatableUserProfile.Rows.Add(datarowUserProfile);

                            #endregion Binding Values to UserProfile DataTable
                        }
                        dataAdapterUserProfile.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterUserProfile.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterUserProfile.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterUserProfile.UpdateBatchSize = 50;
                        dataAdapterUserProfile.InsertCommand.CommandText = "UpsertUserProfile";
                        dataAdapterUserProfile.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterUserProfile.InsertCommand.Connection = connectionUserProfile;

                        #region Binding Command Parameters to stored procedure from UserProfile DataTable

                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@UserID", System.Data.SqlDbType.BigInt, sizeof(Int64), "UserID");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@Gender", System.Data.SqlDbType.Bit, sizeof(bool), "Gender");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@TelephoneNUM1", System.Data.SqlDbType.NVarChar, 2147483646, "TelephoneNUM1");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@TelephoneNUM2", System.Data.SqlDbType.NVarChar, 2147483646, "TelephoneNUM2");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@MobileNUM", System.Data.SqlDbType.NVarChar, 2147483646, "MobileNUM");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@PersonalNUM", System.Data.SqlDbType.NVarChar, 2147483646, "PersonalNUM");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@PostNUM", System.Data.SqlDbType.NVarChar, 2147483646, "PostNUM");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@Remarks", System.Data.SqlDbType.NVarChar, 2147483646, "Remarks");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@Photo1", System.Data.SqlDbType.NVarChar, 2147483646, "Photo1");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@MyVideo", System.Data.SqlDbType.NVarChar, 2147483646, "MyVideo");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@SecreatQuestion", System.Data.SqlDbType.NVarChar, 2147483646, "SecreatQuestion");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@HintAnswer", System.Data.SqlDbType.NVarChar, 2147483646, "HintAnswer");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@TimeZone", System.Data.SqlDbType.NVarChar, 2147483646, "TimeZone");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@TimeFormat", System.Data.SqlDbType.NVarChar, 2147483646, "TimeFormat");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@DateFormat", System.Data.SqlDbType.NVarChar, 2147483646, "DateFormat");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@EducationalLevel", System.Data.SqlDbType.NVarChar, 2147483646, "EducationalLevel");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@IsDeleted", System.Data.SqlDbType.Bit, sizeof(bool), "IsDeleted");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@PAssetID", System.Data.SqlDbType.BigInt, sizeof(Int64), "PAssetID");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@VAssetID", System.Data.SqlDbType.BigInt, sizeof(Int64), "VAssetID");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@Language", System.Data.SqlDbType.NVarChar, 2147483646, "Language");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@DOB", System.Data.SqlDbType.DateTime, 20, "DOB");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@TeacherCode", System.Data.SqlDbType.BigInt, sizeof(Int64), "TeacherCode");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@Initial", System.Data.SqlDbType.NVarChar, 2147483646, "Initial");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@keyword", System.Data.SqlDbType.NVarChar, 2147483646, "keyword");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@AlternateEmail", System.Data.SqlDbType.NVarChar, 2147483646, "AlternateEmail");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@Designation", System.Data.SqlDbType.NVarChar, 2147483646, "Designation");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@OfficeID", System.Data.SqlDbType.BigInt, sizeof(Int64), "OfficeID");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@AssessmentsEnrolledFor", System.Data.SqlDbType.NVarChar, 2147483646, "AssessmentsEnrolledFor");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@EnrolledDate", System.Data.SqlDbType.DateTime, 20, "EnrolledDate");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@ManagerName", System.Data.SqlDbType.NVarChar, 2147483646, "ManagerName");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@OfficeName", System.Data.SqlDbType.NVarChar, 2147483646, "OfficeName");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@RoleName", System.Data.SqlDbType.NVarChar, 2147483646, "RoleName");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@HallTicketNumber", System.Data.SqlDbType.NVarChar, 2147483646, "HallTicketNumber");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@TeachingMode", System.Data.SqlDbType.NVarChar, 2147483646, "TeachingMode");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@StreamName", System.Data.SqlDbType.NVarChar, 2147483646, "StreamName");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@BranchID", System.Data.SqlDbType.BigInt, sizeof(Int64), "BranchID");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@SemesterID", System.Data.SqlDbType.BigInt, sizeof(Int64), "SemesterID");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@VerificationPhotoID", System.Data.SqlDbType.BigInt, sizeof(Int64), "VerificationPhotoID");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@FingerPrint1", System.Data.SqlDbType.NVarChar, 2147483646, "FingerPrint1");
                        dataAdapterUserProfile.InsertCommand.Parameters.Add("@FingerPrint2", System.Data.SqlDbType.NVarChar, 2147483646, "FingerPrint2");

                        #endregion Binding Command Parameters to stored procedure from UserProfile DataTable

                        dataAdapterUserProfile.Update(datatableUserProfile);
                    }
                }
            }
            Log.LogInfo("End UpsertUserProfile() for UserProfile");
        }
    }

    public partial class RoleFactory : Generic
    {
        private int commandTimeout = 0;

        public RoleFactory()
        {
            commandTimeout = 2147483647;
        }

        public void UpsertRole(System.Collections.Generic.List<DataContracts.Role> listRole)
        {
            Log.LogInfo("Begin Upsert() for Role, Count :" + (listRole == null ? "0" : listRole.Count.ToString()));
            List<DataContracts.RoleToPrivilages> ListRoleToPrivilages = null;
            if (listRole != null && listRole.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionRole = CommonDAL.GetConnection())
                {
                    #region Building Role DataTable

                    System.Data.DataTable datatableRole = new System.Data.DataTable();
                    datatableRole.Columns.Add("RoleID");
                    datatableRole.Columns.Add("RoleName");
                    datatableRole.Columns.Add("RoleDescription");
                    datatableRole.Columns.Add("IsDeleted");
                    datatableRole.Columns.Add("RoleType");
                    datatableRole.Columns.Add("CustomerID");
                    datatableRole.Columns.Add("DocumentId");
                    datatableRole.Columns.Add("DocumentUrl");
                    datatableRole.Columns.Add("Code");
                    datatableRole.Columns.Add("MetadataID");

                    #endregion Building Role DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterRole = new System.Data.SqlClient.SqlDataAdapter("UpsertRole", connectionRole))
                    {
                        foreach (DataContracts.Role objRole in listRole)
                        {
                            System.Data.DataRow datarowRole = datatableRole.NewRow();

                            #region Binding Values to Role DataTable

                            if (objRole.RoleID <= 0)
                                datarowRole["RoleID"] = System.DBNull.Value;
                            else
                                datarowRole["RoleID"] = objRole.RoleID;
                            datarowRole["RoleName"] = objRole.RoleName;
                            datarowRole["RoleDescription"] = objRole.RoleDescription;
                            datarowRole["IsDeleted"] = objRole.IsDeleted;
                            if (objRole.RoleType <= 0)
                                datarowRole["RoleType"] = System.DBNull.Value;
                            else
                                datarowRole["RoleType"] = objRole.RoleType;
                            if (objRole.CustomerID <= 0)
                                datarowRole["CustomerID"] = System.DBNull.Value;
                            else
                                datarowRole["CustomerID"] = objRole.CustomerID;
                            if (objRole.DocumentId <= 0)
                                datarowRole["DocumentId"] = System.DBNull.Value;
                            else
                                datarowRole["DocumentId"] = objRole.DocumentId;
                            datarowRole["DocumentUrl"] = objRole.DocumentUrl;
                            datarowRole["Code"] = objRole.Code;
                            if (objRole.MetadataID <= 0)
                                datarowRole["MetadataID"] = System.DBNull.Value;
                            else
                                datarowRole["MetadataID"] = objRole.MetadataID;
                            datatableRole.Rows.Add(datarowRole);

                            #endregion Binding Values to Role DataTable

                            //  To load the privialges for a Role
                            if (objRole.ListRoleToPrivilages != null && objRole.ListRoleToPrivilages.Count > 0)
                            {
                                if (ListRoleToPrivilages == null)
                                    ListRoleToPrivilages = new List<DataContracts.RoleToPrivilages>();
                                ListRoleToPrivilages.AddRange(objRole.ListRoleToPrivilages);
                            }
                        }
                        dataAdapterRole.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterRole.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterRole.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterRole.UpdateBatchSize = 50;
                        dataAdapterRole.InsertCommand.CommandText = "UpsertRole";
                        dataAdapterRole.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterRole.InsertCommand.Connection = connectionRole;

                        #region Binding Command Parameters to stored procedure from Role DataTable

                        dataAdapterRole.InsertCommand.Parameters.Add("@RoleID", System.Data.SqlDbType.BigInt, sizeof(Int64), "RoleID");
                        dataAdapterRole.InsertCommand.Parameters.Add("@RoleName", System.Data.SqlDbType.NVarChar, 2147483646, "RoleName");
                        dataAdapterRole.InsertCommand.Parameters.Add("@RoleDescription", System.Data.SqlDbType.NVarChar, 2147483646, "RoleDescription");
                        dataAdapterRole.InsertCommand.Parameters.Add("@IsDeleted", System.Data.SqlDbType.Bit, sizeof(bool), "IsDeleted");
                        dataAdapterRole.InsertCommand.Parameters.Add("@RoleType", System.Data.SqlDbType.BigInt, sizeof(Int64), "RoleType");
                        dataAdapterRole.InsertCommand.Parameters.Add("@CustomerID", System.Data.SqlDbType.BigInt, sizeof(Int64), "CustomerID");
                        dataAdapterRole.InsertCommand.Parameters.Add("@DocumentId", System.Data.SqlDbType.BigInt, sizeof(Int64), "DocumentId");
                        dataAdapterRole.InsertCommand.Parameters.Add("@DocumentUrl", System.Data.SqlDbType.NVarChar, 2147483646, "DocumentUrl");
                        dataAdapterRole.InsertCommand.Parameters.Add("@Code", System.Data.SqlDbType.NVarChar, 2147483646, "Code");
                        dataAdapterRole.InsertCommand.Parameters.Add("@MetadataID", System.Data.SqlDbType.BigInt, sizeof(Int64), "MetadataID");

                        #endregion Binding Command Parameters to stored procedure from Role DataTable

                        dataAdapterRole.Update(datatableRole);
                    }
                    //  To load the privilages for the role
                    UpsertRoleToPrivilages(ListRoleToPrivilages);
                }
            }
            Log.LogInfo("End Upsert() for   Role");
        }

        public void UpsertRoleToPrivilages(System.Collections.Generic.List<DataContracts.RoleToPrivilages> listRoleToPrivilages)
        {
            Log.LogInfo("Begin Upsert() for RoleToPrivilages, Count :" + (listRoleToPrivilages == null ? "0" : listRoleToPrivilages.Count.ToString()));
            if (listRoleToPrivilages != null && listRoleToPrivilages.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionRoleToPrivilages = CommonDAL.GetConnection())
                {
                    #region Building RoleToPrivilages DataTable

                    System.Data.DataTable datatableRoleToPrivilages = new System.Data.DataTable();
                    datatableRoleToPrivilages.Columns.Add("RtoPID");
                    datatableRoleToPrivilages.Columns.Add("RoleId");
                    datatableRoleToPrivilages.Columns.Add("PrivilageId");
                    datatableRoleToPrivilages.Columns.Add("IsDeleted");

                    #endregion Building RoleToPrivilages DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterRoleToPrivilages = new System.Data.SqlClient.SqlDataAdapter("UpsertRoleToPrivilages", connectionRoleToPrivilages))
                    {
                        foreach (DataContracts.RoleToPrivilages objRoleToPrivilages in listRoleToPrivilages)
                        {
                            System.Data.DataRow datarowRoleToPrivilages = datatableRoleToPrivilages.NewRow();

                            #region Binding Values to RoleToPrivilages DataTable

                            datarowRoleToPrivilages["RtoPID"] = objRoleToPrivilages.RtoPID;
                            datarowRoleToPrivilages["RoleId"] = objRoleToPrivilages.RoleId;
                            datarowRoleToPrivilages["PrivilageId"] = objRoleToPrivilages.PrivilageId;
                            datarowRoleToPrivilages["IsDeleted"] = objRoleToPrivilages.IsDeleted;
                            datatableRoleToPrivilages.Rows.Add(datarowRoleToPrivilages);

                            #endregion Binding Values to RoleToPrivilages DataTable
                        }
                        dataAdapterRoleToPrivilages.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterRoleToPrivilages.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterRoleToPrivilages.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterRoleToPrivilages.UpdateBatchSize = 50;
                        dataAdapterRoleToPrivilages.InsertCommand.CommandText = "UpsertRoleToPrivilages";
                        dataAdapterRoleToPrivilages.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterRoleToPrivilages.InsertCommand.Connection = connectionRoleToPrivilages;

                        #region Binding Command Parameters to stored procedure from RoleToPrivilages DataTable

                        dataAdapterRoleToPrivilages.InsertCommand.Parameters.Add("@RtoPID", System.Data.SqlDbType.BigInt, sizeof(Int64), "RtoPID");
                        dataAdapterRoleToPrivilages.InsertCommand.Parameters.Add("@RoleId", System.Data.SqlDbType.BigInt, sizeof(Int64), "RoleId");
                        dataAdapterRoleToPrivilages.InsertCommand.Parameters.Add("@PrivilageId", System.Data.SqlDbType.BigInt, sizeof(Int64), "PrivilageId");
                        dataAdapterRoleToPrivilages.InsertCommand.Parameters.Add("@IsDeleted", System.Data.SqlDbType.Bit, sizeof(bool), "IsDeleted");

                        #endregion Binding Command Parameters to stored procedure from RoleToPrivilages DataTable

                        dataAdapterRoleToPrivilages.Update(datatableRoleToPrivilages);
                    }
                }
            }
            Log.LogInfo("End Upsert() for   RoleToPrivilages");
        }

        public void DeleteRolePrivilages(long RoleID)
        {
            try
            {
                using (System.Data.SqlClient.SqlConnection Connection = CommonDAL.GetConnection())
                {
                    Connection.Open();
                    using (System.Data.SqlClient.SqlCommand Command = new System.Data.SqlClient.SqlCommand("DELETE FROM ROLETOPRIVILAGES WHERE ROLEID=" + RoleID + "", Connection))
                    {
                        Command.CommandTimeout = commandTimeout;
                        Command.CommandType = System.Data.CommandType.Text;
                        Command.ExecuteNonQuery();
                    }
                    Connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Method - DeleteRolePrivilages Error Catch : " + ex.Message, ex);
            }
        }
    }

    public partial class OrganizationFactory : Generic
    {
        private int commandTimeout = 0;

        public OrganizationFactory()
        {
            commandTimeout = 2147483647;
        }

        public void Load(System.Collections.Generic.List<DataContracts.Organization> listOrganization)
        {
            Log.LogInfo("Begin Upsert() for Organization , Count :" + (listOrganization == null ? "0" : listOrganization.Count.ToString()));

            if (listOrganization != null && listOrganization.Count > 0)
            {
                using (System.Data.SqlClient.SqlConnection connectionOrganization = CommonDAL.GetConnection())
                {
                    #region Building Organization DataTable

                    System.Data.DataTable datatableOrganization = new System.Data.DataTable();
                    datatableOrganization.Columns.Add("OrganizationID");
                    datatableOrganization.Columns.Add("OrganizationName");
                    datatableOrganization.Columns.Add("CustomerNumber");
                    datatableOrganization.Columns.Add("PostCode");
                    datatableOrganization.Columns.Add("OrganizationCode");
                    datatableOrganization.Columns.Add("PhoneNo");
                    datatableOrganization.Columns.Add("EmailID");
                    datatableOrganization.Columns.Add("WebAddress");
                    datatableOrganization.Columns.Add("Notes");
                    datatableOrganization.Columns.Add("IsDeleted");
                    datatableOrganization.Columns.Add("StandardPassword");
                    datatableOrganization.Columns.Add("IsBlocked");
                    datatableOrganization.Columns.Add("ParentID");
                    datatableOrganization.Columns.Add("LocationCode");
                    datatableOrganization.Columns.Add("AllowStudentsToCreatePassword");
                    datatableOrganization.Columns.Add("AllowTeachersToCreatePassword");
                    datatableOrganization.Columns.Add("OrganizationType");
                    datatableOrganization.Columns.Add("ContactPerson");
                    datatableOrganization.Columns.Add("Logo");
                    datatableOrganization.Columns.Add("CountryID");
                    datatableOrganization.Columns.Add("LocationHead");
                    datatableOrganization.Columns.Add("CreatedDate");
                    datatableOrganization.Columns.Add("ModifiedDate");
                    datatableOrganization.Columns.Add("Theme");
                    datatableOrganization.Columns.Add("TestPlayerConcurrentUsers");
                    datatableOrganization.Columns.Add("LegalName");
                    datatableOrganization.Columns.Add("StartDate");
                    datatableOrganization.Columns.Add("EndDate");
                    datatableOrganization.Columns.Add("OrganizationTypeID");
                    datatableOrganization.Columns.Add("MetadataID");
                    datatableOrganization.Columns.Add("CreatedBy");
                    datatableOrganization.Columns.Add("ModifiedBy");
                    datatableOrganization.Columns.Add("LocationID");

                    #endregion Building Organization DataTable

                    using (System.Data.SqlClient.SqlDataAdapter dataAdapterOrganization = new System.Data.SqlClient.SqlDataAdapter("UpsertOrganization", connectionOrganization))
                    {
                        foreach (DataContracts.Organization objOrganization in listOrganization)
                        {
                            System.Data.DataRow datarowOrganization = datatableOrganization.NewRow();

                            #region Binding Values to Organization DataTable

                            if (objOrganization.OrganizationID <= 0)
                                datarowOrganization["OrganizationID"] = System.DBNull.Value;
                            else
                                datarowOrganization["OrganizationID"] = objOrganization.OrganizationID;
                            datarowOrganization["OrganizationName"] = objOrganization.OrganizationName;
                            datarowOrganization["CustomerNumber"] = objOrganization.CustomerNumber;
                            datarowOrganization["PostCode"] = objOrganization.PostCode;
                            datarowOrganization["OrganizationCode"] = objOrganization.OrganizationCode;
                            datarowOrganization["PhoneNo"] = objOrganization.PhoneNo;
                            datarowOrganization["EmailID"] = objOrganization.EmailID;
                            datarowOrganization["WebAddress"] = objOrganization.WebAddress;
                            datarowOrganization["Notes"] = objOrganization.Notes;
                            datarowOrganization["IsDeleted"] = objOrganization.IsDeleted;
                            datarowOrganization["StandardPassword"] = objOrganization.StandardPassword;
                            datarowOrganization["IsBlocked"] = objOrganization.IsBlocked;
                            if (objOrganization.ParentID <= 0)
                                datarowOrganization["ParentID"] = System.DBNull.Value;
                            else
                                datarowOrganization["ParentID"] = objOrganization.ParentID;
                            datarowOrganization["LocationCode"] = objOrganization.LocationCode;
                            datarowOrganization["AllowStudentsToCreatePassword"] = objOrganization.AllowStudentsToCreatePassword;
                            datarowOrganization["AllowTeachersToCreatePassword"] = objOrganization.AllowTeachersToCreatePassword;
                            if (objOrganization.OrganizationType <= 0)
                                datarowOrganization["OrganizationType"] = System.DBNull.Value;
                            else
                                datarowOrganization["OrganizationType"] = objOrganization.OrganizationType;
                            if (objOrganization.ContactPerson <= 0)
                                datarowOrganization["ContactPerson"] = System.DBNull.Value;
                            else
                                datarowOrganization["ContactPerson"] = objOrganization.ContactPerson;
                            if (objOrganization.Logo <= 0)
                                datarowOrganization["Logo"] = System.DBNull.Value;
                            else
                                datarowOrganization["Logo"] = objOrganization.Logo;
                            if (objOrganization.CountryID <= 0)
                                datarowOrganization["CountryID"] = System.DBNull.Value;
                            else
                                datarowOrganization["CountryID"] = objOrganization.CountryID;
                            datarowOrganization["LocationHead"] = objOrganization.LocationHead;
                            if (objOrganization.CreatedDate != null && objOrganization.CreatedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowOrganization["CreatedDate"] = System.DBNull.Value;
                            else
                                datarowOrganization["CreatedDate"] = objOrganization.CreatedDate;
                            if (objOrganization.ModifiedDate != null && objOrganization.ModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowOrganization["ModifiedDate"] = System.DBNull.Value;
                            else
                                datarowOrganization["ModifiedDate"] = objOrganization.ModifiedDate;
                            datarowOrganization["Theme"] = objOrganization.Theme;
                            if (objOrganization.TestPlayerConcurrentUsers <= 0)
                                datarowOrganization["TestPlayerConcurrentUsers"] = System.DBNull.Value;
                            else
                                datarowOrganization["TestPlayerConcurrentUsers"] = objOrganization.TestPlayerConcurrentUsers;
                            datarowOrganization["LegalName"] = objOrganization.LegalName;
                            if (objOrganization.StartDate != null && objOrganization.StartDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowOrganization["StartDate"] = System.DBNull.Value;
                            else
                                datarowOrganization["StartDate"] = objOrganization.StartDate;
                            if (objOrganization.EndDate != null && objOrganization.EndDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                                datarowOrganization["EndDate"] = System.DBNull.Value;
                            else
                                datarowOrganization["EndDate"] = objOrganization.EndDate;
                            if (objOrganization.OrganizationTypeID <= 0)
                                datarowOrganization["OrganizationTypeID"] = System.DBNull.Value;
                            else
                                datarowOrganization["OrganizationTypeID"] = objOrganization.OrganizationTypeID;
                            if (objOrganization.MetadataID <= 0)
                                datarowOrganization["MetadataID"] = System.DBNull.Value;
                            else
                                datarowOrganization["MetadataID"] = objOrganization.MetadataID;
                            if (objOrganization.CreatedBy <= 0)
                                datarowOrganization["CreatedBy"] = System.DBNull.Value;
                            else
                                datarowOrganization["CreatedBy"] = objOrganization.CreatedBy;
                            if (objOrganization.ModifiedBy <= 0)
                                datarowOrganization["ModifiedBy"] = System.DBNull.Value;
                            else
                                datarowOrganization["ModifiedBy"] = objOrganization.ModifiedBy;
                            if (objOrganization.LocationID <= 0)
                                datarowOrganization["LocationID"] = System.DBNull.Value;
                            else
                                datarowOrganization["LocationID"] = objOrganization.LocationID;
                            datatableOrganization.Rows.Add(datarowOrganization);

                            #endregion Binding Values to Organization DataTable
                        }
                        dataAdapterOrganization.InsertCommand = new System.Data.SqlClient.SqlCommand();
                        dataAdapterOrganization.InsertCommand.CommandTimeout = commandTimeout;
                        dataAdapterOrganization.InsertCommand.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dataAdapterOrganization.UpdateBatchSize = 50;
                        dataAdapterOrganization.InsertCommand.CommandText = "UpsertOrganization";
                        dataAdapterOrganization.InsertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dataAdapterOrganization.InsertCommand.Connection = connectionOrganization;

                        #region Binding Command Parameters to stored procedure from Organization DataTable

                        dataAdapterOrganization.InsertCommand.Parameters.Add("@OrganizationID", System.Data.SqlDbType.BigInt, sizeof(Int64), "OrganizationID");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@OrganizationName", System.Data.SqlDbType.NVarChar, 2147483646, "OrganizationName");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@CustomerNumber", System.Data.SqlDbType.NVarChar, 2147483646, "CustomerNumber");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@PostCode", System.Data.SqlDbType.NVarChar, 2147483646, "PostCode");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@OrganizationCode", System.Data.SqlDbType.NVarChar, 2147483646, "OrganizationCode");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@PhoneNo", System.Data.SqlDbType.NVarChar, 2147483646, "PhoneNo");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@EmailID", System.Data.SqlDbType.NVarChar, 2147483646, "EmailID");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@WebAddress", System.Data.SqlDbType.NVarChar, 2147483646, "WebAddress");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@Notes", System.Data.SqlDbType.NVarChar, 2147483646, "Notes");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@IsDeleted", System.Data.SqlDbType.Bit, sizeof(bool), "IsDeleted");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@StandardPassword", System.Data.SqlDbType.NVarChar, 2147483646, "StandardPassword");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@IsBlocked", System.Data.SqlDbType.Bit, sizeof(bool), "IsBlocked");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@ParentID", System.Data.SqlDbType.BigInt, sizeof(Int64), "ParentID");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@LocationCode", System.Data.SqlDbType.NVarChar, 2147483646, "LocationCode");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@AllowStudentsToCreatePassword", System.Data.SqlDbType.Bit, sizeof(bool), "AllowStudentsToCreatePassword");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@AllowTeachersToCreatePassword", System.Data.SqlDbType.Bit, sizeof(bool), "AllowTeachersToCreatePassword");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@OrganizationType", System.Data.SqlDbType.Int, sizeof(Int32), "OrganizationType");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@ContactPerson", System.Data.SqlDbType.BigInt, sizeof(Int64), "ContactPerson");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@Logo", System.Data.SqlDbType.BigInt, sizeof(Int64), "Logo");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@CountryID", System.Data.SqlDbType.BigInt, sizeof(Int64), "CountryID");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@LocationHead", System.Data.SqlDbType.NVarChar, 2147483646, "LocationHead");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime, 20, "CreatedDate");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@ModifiedDate", System.Data.SqlDbType.DateTime, 20, "ModifiedDate");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@Theme", System.Data.SqlDbType.NVarChar, 2147483646, "Theme");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@TestPlayerConcurrentUsers", System.Data.SqlDbType.BigInt, sizeof(Int64), "TestPlayerConcurrentUsers");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@LegalName", System.Data.SqlDbType.NVarChar, 2147483646, "LegalName");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime, 20, "StartDate");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime, 20, "EndDate");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@OrganizationTypeID", System.Data.SqlDbType.Int, sizeof(Int32), "OrganizationTypeID");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@MetadataID", System.Data.SqlDbType.BigInt, sizeof(Int64), "MetadataID");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@CreatedBy", System.Data.SqlDbType.BigInt, sizeof(Int64), "CreatedBy");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@ModifiedBy", System.Data.SqlDbType.BigInt, sizeof(Int64), "ModifiedBy");
                        dataAdapterOrganization.InsertCommand.Parameters.Add("@LocationID", System.Data.SqlDbType.BigInt, sizeof(Int64), "LocationID");

                        #endregion Binding Command Parameters to stored procedure from Organization DataTable

                        dataAdapterOrganization.Update(datatableOrganization);
                    }
                }
            }
            Log.LogInfo("End Upsert() for  Organization");
        }
    }

    public partial class ApplicationLicensingFactory : Generic
    {
        public List<DataContracts.ApplicationLicensingStatus> GetApplicationLicensingForTestCenter(string MACAddress)
        {
            List<DataContracts.ApplicationLicensingStatus> lstApplicationLicensingStatus = null;
            try
            {
                Log.LogInfo("Begin GetApplicationLicensingForTestCenter()" + " - MacID: " + MACAddress);
                using (System.Data.SqlClient.SqlConnection connectionApplicationLicensing = CommonDAL.GetConnection())
                {
                    connectionApplicationLicensing.Open();
                    using (System.Data.SqlClient.SqlCommand commandApplicationLicensing = new System.Data.SqlClient.SqlCommand())
                    {
                        commandApplicationLicensing.CommandTimeout = 0;
                        commandApplicationLicensing.Connection = connectionApplicationLicensing;
                        commandApplicationLicensing.CommandText = "UspGetApplicationLicensingStatus";
                        commandApplicationLicensing.CommandType = System.Data.CommandType.StoredProcedure;
                        commandApplicationLicensing.Parameters.Add(new SqlParameter("TestCenterMacID", MACAddress));
                        using (System.Data.SqlClient.SqlDataReader dataApplicationLicensing = commandApplicationLicensing.ExecuteReader())
                        {
                            if (dataApplicationLicensing != null && dataApplicationLicensing.HasRows)
                            {
                                //  To read PackageStatistics details
                                lstApplicationLicensingStatus = ReadApplicationLicensing(dataApplicationLicensing);
                            }
                        }
                    }
                }
                Log.LogInfo("End GetApplicationLicensingForTestCenter()" + " - Package(s) count: " + (lstApplicationLicensingStatus == null ? "0" : lstApplicationLicensingStatus.Count.ToString()));
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, ex);
            }

            return lstApplicationLicensingStatus;
        }

        private List<DataContracts.ApplicationLicensingStatus> ReadApplicationLicensing(SqlDataReader reader)
        {
            Log.LogInfo("Begin ReadApplicationLicensing()");
            List<DataContracts.ApplicationLicensingStatus> listApplicationLicensingStatus = null;
            DataContracts.ApplicationLicensingStatus objApplicationLicensingStatus = null;
            if (reader != null && reader.HasRows)
            {
                listApplicationLicensingStatus = new List<DataContracts.ApplicationLicensingStatus>();
                int Index = 0;
                while (reader.Read())
                {
                    objApplicationLicensingStatus = new DataContracts.ApplicationLicensingStatus();

                    #region Reading the package Statistics

                    Index = reader.GetOrdinal("AttemptedCount");
                    if (!reader.IsDBNull(Index))
                        objApplicationLicensingStatus.AttemptedCount = reader.GetString(Index);
                    Index = reader.GetOrdinal("FailureAttemptCount");
                    if (!reader.IsDBNull(Index))
                        objApplicationLicensingStatus.FailureAttemptCount = reader.GetString(Index);
                    Index = reader.GetOrdinal("ExpiryPeriod");
                    if (!reader.IsDBNull(Index))
                        objApplicationLicensingStatus.ExpiryPeriod = reader.GetString(Index);
                    Index = reader.GetOrdinal("CurrentStatus");
                    if (!reader.IsDBNull(Index))
                        objApplicationLicensingStatus.CurrentStatus = reader.GetString(Index);
                    Index = reader.GetOrdinal("RootOrganizationId");
                    if (!reader.IsDBNull(Index))
                        objApplicationLicensingStatus.RootOrganizationId = reader.GetInt64(Index);

                    #endregion Reading the package Statistics

                    listApplicationLicensingStatus.Add(objApplicationLicensingStatus);
                }
            }
            Log.LogInfo("End ReadApplicationLicensing() - Package(s) Count : " + (listApplicationLicensingStatus == null ? "0" : listApplicationLicensingStatus.Count.ToString()));
            return listApplicationLicensingStatus;
        }
    }
}