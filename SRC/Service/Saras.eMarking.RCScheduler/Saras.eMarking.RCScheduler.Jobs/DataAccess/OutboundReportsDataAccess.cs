using Newtonsoft.Json;
using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.RCScheduler.Jobs.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Saras.eMarking.RCScheduler.Jobs.DataAccess
{
    public class OutboundReportsDataAccess : BaseJobScheduleManager
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["SchedulerDB"].ConnectionString.Trim();

        public List<OutboundReportEntity> GetOutboundReportQueue()
        {
            List<OutboundReportEntity> OutboundRequests = null;
            Log.LogDebug("OutboundReportsDataAccess  GetOutboundReportQueue() Method started.");
            try
            {
                SqlConnection sqlCon = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("[Marking].[USPGetOutboundAPIDetails]", sqlCon)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                OutboundRequests = (from DataRow dr in dt.Rows
                                    select new OutboundReportEntity()
                                    {
                                        RequestedId = Convert.ToInt64(dr["RequestID"]),
                                        ProjectId = Convert.ToInt64(dr["ProjectID"] == DBNull.Value ? 0 : dr["ProjectID"]),
                                        RequestBy = Convert.ToInt64(dr["RequestBy"] == DBNull.Value ? 0 : dr["RequestBy"]),
                                        RequestGuid = Convert.ToString(dr["RequestGUID"] == DBNull.Value ? string.Empty : dr["RequestGUID"]),
                                        ReportType = Convert.ToInt16(dr["ReportType"] == DBNull.Value ? 0 : dr["ReportType"]),
                                        PageNo = Convert.ToInt32(dr["PageNo"] == DBNull.Value ? 0 : dr["PageNo"]),
                                        PageSize = Convert.ToInt32(dr["PageSize"] == DBNull.Value ? 0 : dr["PageSize"]),
                                        RequestOrder = Convert.ToInt16(dr["RequestOrder"] == DBNull.Value ? 0 : dr["RequestOrder"]),
                                        //FileName = Convert.ToString(dr["FileName"] == DBNull.Value ? string.Empty : dr["FileName"]),
                                        //FilePath = Convert.ToString(dr["FilePath"] == DBNull.Value ? string.Empty : dr["FilePath"]),
                                        RequestDate = Convert.ToDateTime(dr["RequestDate"])
                                    }).ToList();

                if (sqlCon.State != ConnectionState.Closed)
                {
                    sqlCon.Close();
                }

                Log.LogDebug("OutboundReportsDataAccess  GetOutboundReportQueue() Method completed.");
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error in OutboundReportsDataAccess GetOutboundReportQueue Message {0} ", ex.Message), ex);
                throw;
            }
            return OutboundRequests;
        }

        public TextReportModel GetReportsData(OutboundReportEntity outboundReportEntity, int total_Part, DateTime utcDateTime)
        {
            TextReportModel results = new TextReportModel();
            try
            {
                SqlConnection sqlCon = new SqlConnection(connectionString);

                SqlCommand sqlCmd = new SqlCommand("[Marking].[USPGetOutboundEMSAPIDetails]", sqlCon)
                {
                    CommandType = CommandType.StoredProcedure
                };
                sqlCmd.Parameters.Add("@RequestGUID", SqlDbType.NVarChar).Value = outboundReportEntity.RequestGuid;

                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    results.Items = new List<string>();
                    while (reader.Read())
                    {
                        if (reader["Results"] != DBNull.Value)
                        {
                            results.Items.Add(Convert.ToString(reader["Results"]));
                        }
                    }
                }
                reader.NextResult();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        results.PageSize = outboundReportEntity.PageSize == 0 ? Convert.ToInt64(reader["Row_Count"]) : outboundReportEntity.PageSize;
                        results.PageIndex = outboundReportEntity.PageNo;
                        results.Count = Convert.ToInt64(reader["Row_Count"] == DBNull.Value ? 0 : reader["Row_Count"]);
                    }
                }

                DateTime curntdate = ConvertUtcToSgt(utcDateTime);
                results.ProcessDate = curntdate;

                string curentdateHeader = string.Concat(curntdate.ToString("ddMMyyyy"), curntdate.ToString("HH:mm:ss"));
                string curentdatePage = string.Concat(curntdate.ToString("ddMMyyyy"), '_', curntdate.ToString("HHmmss"));

                reader.NextResult();
                while (reader.Read())
                {
                    string firstLine = string.Empty;
                    if (outboundReportEntity.ReportType == 3)
                    {
                        firstLine = Convert.ToString(reader["FILE_NAME_"] == DBNull.Value ? 0 : reader["FILE_NAME_"]) + "             " + curentdateHeader;
                    }
                    else
                    {
                        firstLine = Convert.ToString(reader["FILE_NAME_"] == DBNull.Value ? 0 : reader["FILE_NAME_"]) + "            " + curentdateHeader;
                    }
                    results.Items.Insert(0, firstLine);

                    results.Items.Add(Convert.ToString(results.Count));
                    results.Count = (int)(results.Count % 10000);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    results.FileName = GetFormattedFileName(reader, curentdatePage, results.PageIndex, total_Part);
                    results.ExamYear = Convert.ToInt32(reader["ExamYear"]);
                }

                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                if (sqlCon.State != ConnectionState.Closed)
                {
                    sqlCon.Close();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error in RcCheck DA GetScriptRcSampling Message {0} ", ex.Message), ex);
                throw;
            }
            return results;
        }

        private static string GetFormattedFileName(SqlDataReader reader, string curntdate, long PageIndex, long total_Part)
        {
            StringBuilder _fileName = new StringBuilder();
            _fileName.Append(Convert.ToString(reader["XE"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["FILE_NAME_"] == DBNull.Value ? "EMS" : reader["FILE_NAME_"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["ExamLevelCode"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["SUBJECTCODE"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["PAPERCODE"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["MOACode"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["ExamSeriesName"]));
            _fileName.Append('_');
            _fileName.Append(PageIndex);
            _fileName.Append('_');
            _fileName.Append(total_Part);
            _fileName.Append('_');
            _fileName.Append(curntdate);

            return _fileName.ToString();
        }

        private static DateTime ConvertUtcToSgt(DateTime utcTime)
        {
            string TimeZoneName = ConfigurationManager.AppSettings.Get("TimeZoneName");
            TimeZoneInfo sgtTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
            DateTime sgtTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, sgtTimeZone);
            return sgtTime;
        }

        internal void UpdateOutboundReportQueueHistory(OutboundReportEntity reprot, SyncResponseModel response, SyncReportModel sysReportModel)
        {
            try
            {
                SqlConnection sqlCon = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("[Marking].[USPInsertOutboundAPIHistoryDetails]", sqlCon)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@RequestID", SqlDbType.BigInt).Value = reprot.RequestedId;
                cmd.Parameters.Add("@IsRequestServed", SqlDbType.Bit).Value = response.Status == SyncResponseStatus.Success;
                if (sysReportModel != null)
                {
                    cmd.Parameters.Add("@FilePath", SqlDbType.NVarChar).Value = sysReportModel.RootFolder + @"\\" + sysReportModel.FileId + "~" + sysReportModel.FileName;
                    cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = sysReportModel.FileName;
                }
                else
                {
                    cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = string.Empty;
                    cmd.Parameters.Add("@FilePath", SqlDbType.NVarChar).Value = string.Empty;
                }
                cmd.Parameters.Add("@IsProcessed", SqlDbType.Bit).Value = 1;
                cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(response);
                cmd.Parameters.Add("@RequestServedDate", SqlDbType.DateTime).Value = sysReportModel.ProcessDate;

                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                cmd.ExecuteNonQuery();
                if (sqlCon.State != ConnectionState.Closed)
                {
                    sqlCon.Close();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error in RcCheck DA RcJobTrackingHistory Message {0} ", ex.Message), ex);
            }
        }
    }
}