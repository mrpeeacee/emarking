using Saras.eMarking.RCScheduler.Jobs.DataAccess;
using Saras.eMarking.RCScheduler.Jobs.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;

namespace Saras.eMarking.RCScheduler.Jobs.BusinessLogic
{
    public class RcCheck : BaseJobScheduleManager
    {
        private readonly RcCheckDA rcCheckDA = new RcCheckDA();
        internal string GetAndMoveScriptsToRcPool(string triggerName, string schedulename)
        {
            string dbMsg = string.Empty;
            string statusMsg = "GetAndMoveScriptsToRcPool method started. Job Name : ";
            string jobname = string.Empty;
            long ProjectID = 0;
            long QigID = 0;
            int RcType = 0;
            int Duration = 0;
            string ProcessedScripts = null;
            decimal SamplingRate = 0;
            int processStatus = 0;


            if (Log.IsDebugEnabled) { Log.LogDebug("Enter into  RcCheck BL GetAndMoveScriptsToRcPool."); }
            try
            {


                jobname = triggerName;

                statusMsg += jobname;

                var SchedulerDetails = jobname.Split('_');

                ProjectID = Convert.ToInt64(SchedulerDetails[1]);
                QigID = Convert.ToInt64(SchedulerDetails[2]);
                RcType = Convert.ToInt16(SchedulerDetails[3]);
                Duration = Convert.ToInt16(SchedulerDetails[5]);

                RandomCheck scriptrcsampling = rcCheckDA.GetScriptRcSampling(ProjectID, QigID, RcType);
                JavaScriptSerializer JSONSerializer = new JavaScriptSerializer();

                if (scriptrcsampling != null)
                {
                    SamplingRate = scriptrcsampling.SamplingRate;

                    if (scriptrcsampling.Scripts != null && scriptrcsampling.Scripts.Count > 0)
                    {
                        Random rnd = new Random();
                        scriptrcsampling.Scripts = scriptrcsampling.Scripts.OrderBy(item => rnd.Next()).ToList();

                        foreach (IGrouping<long, RcScripts> grpProject in scriptrcsampling.Scripts.GroupBy(a => a.ActionBy))
                        {
                            List<RcScripts> submittedScriptByMarker = grpProject.ToList();
                            int scriptNeedToPick = FindNearestMatchingValue(submittedScriptByMarker.Count, SamplingRate);

                            if (scriptNeedToPick > 0)
                            {
                                submittedScriptByMarker = grpProject.OrderBy(scN => scN.PhaseStatusTrackingID).Take(scriptNeedToPick).ToList();

                                int percentage = (int)(SamplingRate / 100 * submittedScriptByMarker.Count);

                                submittedScriptByMarker.ForEach(rc => rc.IsJobRun = true);

                                submittedScriptByMarker.OrderBy(x => rnd.Next()).Take(percentage).ToList().ForEach(rdmsc => rdmsc.IsSelectedForRC = true);
                            }
                        }
                        scriptrcsampling.Scripts = scriptrcsampling.Scripts.Where(a => a.IsJobRun).ToList();

                        DataTable dt = ToGetDataTable(scriptrcsampling.Scripts);

                        dbMsg = rcCheckDA.SendDataToUDT(dt, RcType);

                        ProcessedScripts = JSONSerializer.Serialize(scriptrcsampling.Scripts);

                        statusMsg += ". " + scriptrcsampling.Scripts.Count + " Records Processed. DB status is " + dbMsg;
                        processStatus = 1;
                    }
                    else
                    {
                        statusMsg += ". No records Found. ";
                    }

                }
                else
                {
                    statusMsg += ". RC Script Sampling is null. ";
                }
                statusMsg += " GetAndMoveScriptsToRcPool method completed";
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error in RcCheck BL GetAndMoveScriptsToRcPool Message {0} ", ex.Message), ex);
                statusMsg += ". " + string.Format(" Error in RcCheck BL GetAndMoveScriptsToRcPool Message {0} ", ex.Message);
                processStatus = 2;
                throw;
            }
            finally
            {
                rcCheckDA.RcJobTrackingHistory(schedulename, jobname, jobname, jobname, ProjectID, QigID, RcType, Duration, SamplingRate, DateTime.UtcNow, ProcessedScripts, statusMsg, processStatus);

            }
            return dbMsg;
        }

        private int FindNearestMatchingValue(int submittedScriptCount, decimal samplingRate)
        {
            int loopcount = 1;
            int pickcount = 0;
            if (samplingRate > 0)
            {
                double nCount = (double)(100 / samplingRate);

                while (pickcount < submittedScriptCount)
                {
                    int ceilingCount = (int)Math.Ceiling(nCount * loopcount);
                    if (ceilingCount <= submittedScriptCount)
                    {
                        pickcount = ceilingCount;
                        loopcount++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                return submittedScriptCount;
            }
            return pickcount;
        }

        private DataTable ToGetDataTable(List<RcScripts> items)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("PhaseStatusTrackingID", typeof(long));
            dataTable.Columns.Add("ProjectID", typeof(long));
            dataTable.Columns.Add("QIGID", typeof(long));
            dataTable.Columns.Add("ScriptID", typeof(long));
            dataTable.Columns.Add("ActionBy", typeof(long));
            dataTable.Columns.Add("IsSelectedForRC", typeof(bool));
            if (items != null && items.Count > 0)
            {
                items.ForEach(itm =>
                {
                    dataTable.Rows.Add(itm.PhaseStatusTrackingID, itm.ProjectID, itm.Qigid, itm.ScriptID, itm.ActionBy, itm.IsSelectedForRC);
                });
            }

            return dataTable;
        }

    }
}
