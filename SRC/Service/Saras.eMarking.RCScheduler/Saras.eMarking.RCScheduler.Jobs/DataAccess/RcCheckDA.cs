using Saras.eMarking.RCScheduler.Jobs.DTO;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Saras.eMarking.RCScheduler.Jobs.DataAccess
{
    public class RcCheckDA : BaseJobScheduleManager
    {
        readonly static string connectionString = ConfigurationManager.ConnectionStrings["SchedulerDB"].ConnectionString.Trim();

        readonly SqlConnection con = new SqlConnection(connectionString);
        public RandomCheck GetScriptRcSampling(long ProjectID, long QigID, long RcType)
        {
            RandomCheck scripts = new RandomCheck();
            try
            {

                SqlCommand cmd = new SqlCommand("[Marking].[UspGetScriptsforRCSampling]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                cmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigID;
                cmd.Parameters.Add("@RC", SqlDbType.BigInt).Value = RcType;
                cmd.Parameters.Add("@SamplingRate", SqlDbType.Decimal, 20).Direction = ParameterDirection.Output;

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                scripts.Scripts = (from DataRow dr in dt.Rows
                                   select new RcScripts()
                                   {
                                       ProjectID = Convert.ToInt64(dr["ProjectID"]),
                                       Qigid = Convert.ToInt64(dr["Qigid"].ToString()),
                                       ScriptID = Convert.ToInt64(dr["ScriptID"].ToString()),
                                       ActionBy = Convert.ToInt64(dr["ActionBy"].ToString()),
                                       IsSelectedForRC = false,
                                       PhaseStatusTrackingID = Convert.ToInt64(dr["PhaseStatusTrackingID"].ToString())
                                   }).ToList();
                var smprate = cmd.Parameters["@SamplingRate"].Value;
                if (smprate != null && smprate != DBNull.Value)
                {
                    scripts.SamplingRate = Convert.ToDecimal(cmd.Parameters["@SamplingRate"].Value);
                }
                else
                {
                    Log.LogDebug(string.Format("SamplingRate is null in RcCheck DA GetScriptRcSampling ProjectID {0} ", ProjectID));
                }
                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error in RcCheck DA GetScriptRcSampling Message {0} ", ex.Message), ex);
                throw;
            }
            return scripts;
        }

        public string SendDataToUDT(DataTable dt, int RcType)
        {
            string status;
            SqlCommand cmd = new SqlCommand("Marking.USPInsertRCScriptDetails", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@UDTRCScriptInfo", SqlDbType.Structured).Value = dt;
            cmd.Parameters.Add("@RCType", SqlDbType.TinyInt).Value = RcType;
            cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            cmd.ExecuteNonQuery();

            status = Convert.ToString(cmd.Parameters["@Status"].Value);

            if (con.State != ConnectionState.Closed)
            {
                con.Close();
            }

            return status;

        }


        public void RcJobTrackingHistory(string schedulername, string jobName, string jobGuid, string jobGroup, long projectID, long qigID, int rcType, int duration, decimal samplingRate, DateTime dateTime, string ProcessedScripts, string statusMsg, int processStatus)
        {
            string JobTrackingLogType = ConfigurationManager.AppSettings.Get("JobTrackingLogType");

            if (JobTrackingLogType == "1")
            {
                Log.LogDebug(string.Format("RcJobTrackingHistory. schedulername : {0}, jobName : {1}, jobGuid : {2}, jobGroup : {3}, projectID : {4}, qigID : {5} , rcType : {6}, duration : {7}, samplingRate : {8}, dateTime : {9}, ProcessedScripts : {10}, processStatus : {11}, statusMsg : {12} ",
                       schedulername, jobName, jobGuid, jobGroup, projectID, qigID, rcType, duration, samplingRate, dateTime, ProcessedScripts, processStatus, statusMsg));
            }
            else
              if (JobTrackingLogType == "2")
            {
                LogJobTracking(schedulername, jobName, jobGuid, jobGroup, projectID, qigID, rcType, duration, samplingRate, dateTime, ProcessedScripts, statusMsg, processStatus);

            }
            else if (JobTrackingLogType == "3")
            {
                Log.LogDebug(string.Format("RcJobTrackingHistory. schedulername : {0}, jobName : {1}, jobGuid : {2}, jobGroup : {3}, projectID : {4}, qigID : {5} , rcType : {6}, duration : {7}, samplingRate : {8}, dateTime : {9}, ProcessedScripts : {10}, processStatus : {11}, statusMsg : {12} ",
                      schedulername, jobName, jobGuid, jobGroup, projectID, qigID, rcType, duration, samplingRate, dateTime, ProcessedScripts, processStatus, statusMsg));

                LogJobTracking(schedulername, jobName, jobGuid, jobGroup, projectID, qigID, rcType, duration, samplingRate, dateTime, ProcessedScripts, statusMsg, processStatus);
            }
        }

        private void LogJobTracking(string schedulername, string jobName, string jobGuid, string jobGroup, long projectID, long qigID, int rcType, int duration, decimal samplingRate, DateTime dateTime, string ProcessedScripts, string statusMsg, int processStatus)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Quartz.UspInsertQuartzJobTrackingDetails", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@SCHED_NAME", SqlDbType.NVarChar).Value = schedulername;
                cmd.Parameters.Add("@JOB_NAME", SqlDbType.NVarChar).Value = jobName;

                cmd.Parameters.Add("@JobGUID", SqlDbType.NVarChar).Value = jobGuid;
                cmd.Parameters.Add("@JOB_GROUP", SqlDbType.NVarChar).Value = jobGroup;
                cmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectID;
                cmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = qigID;
                cmd.Parameters.Add("@RCType", SqlDbType.Int).Value = rcType;
                cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = duration;
                cmd.Parameters.Add("@SamplingRate", SqlDbType.Decimal).Value = samplingRate;
                cmd.Parameters.Add("@JobRunDateTime", SqlDbType.DateTime).Value = dateTime;
                cmd.Parameters.Add("@ProcessedScripts", SqlDbType.NVarChar).Value = ProcessedScripts;
                cmd.Parameters.Add("@JobStatus", SqlDbType.Int).Value = processStatus;
                cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = statusMsg;

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                cmd.ExecuteNonQuery();

                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error in RcCheck DA RcJobTrackingHistory Message {0} ", ex.Message), ex);
            }
        }
    }
}
