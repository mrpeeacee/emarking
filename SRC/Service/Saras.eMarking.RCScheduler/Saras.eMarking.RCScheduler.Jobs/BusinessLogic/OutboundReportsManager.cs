using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.RCScheduler.Jobs.DataAccess;
using Saras.eMarking.RCScheduler.Jobs.DTO;
using Saras.eMarking.RCScheduler.Jobs.Utilities;
using Saras.SystemFramework.Core.Logging;

using static Saras.eMarking.RCScheduler.Jobs.Utilities.GenericApiCallHandler;

namespace Saras.eMarking.RCScheduler.Jobs.BusinessLogic
{
    public static class OutboundReportsManager
    {
        static readonly SarasLogger Log = new SarasLogger(typeof(SendEmailManager));
        private static OutboundReportsDataAccess syncReportDataAccess = new OutboundReportsDataAccess();


        public static string SyncReport(string triggerName, string schedulerName)
        {
            Log.LogDebug("OutboundReportsScheduler  SyncReport() Method started. triggerName : " + triggerName + ", schedulerName : " + schedulerName);
            string ApiResponse = string.Empty;
            int ProcessStatus = 0;
            try
            {
                //Check the total records in the Report sync queue.
                List<OutboundReportEntity> reportQueue = syncReportDataAccess.GetOutboundReportQueue();
                DateTime utcDateTime = DateTime.UtcNow;
                if (reportQueue != null && reportQueue.Count > 0)
                {
                    reportQueue.GroupBy(a => a.ProjectId).ToList().ForEach(que =>
                    {
                        que.OrderBy(ro => ro.RequestOrder).ToList().ForEach(async reprot =>
                        {
                            int Total_Part = que.Where(typ => typ.ReportType == reprot.ReportType).Max(mr => mr.RequestOrder);
                            SyncResponseModel response = new SyncResponseModel();
                            SyncReportModel sysReportModel = null;
                            try
                            {
                                sysReportModel = GenerateReportFile(reprot, Total_Part, utcDateTime);
                                if (sysReportModel != null)
                                {
                                    response = await UploadFileAsync(sysReportModel);

                                    Log.LogDebug(string.Format("Response status of UploadFile {0}, Request param {1}", response, JsonConvert.SerializeObject(reprot)));

                                }
                                else
                                {
                                    response.Message = "There is no record from GenerateReportFile";
                                    response.Status = SyncResponseStatus.Error;
                                    Log.LogDebug("There is no record from GenerateReportFile");
                                }
                            }
                            catch (Exception ex)
                            {
                                response.Message = "Error while processing the report files. " + ex.Message;
                                response.Status = SyncResponseStatus.Error;
                                Log.LogError("Error while processing the report files." + JsonConvert.SerializeObject(reprot), ex);
                            }
                            finally
                            {
                                syncReportDataAccess.UpdateOutboundReportQueueHistory(reprot, response, sysReportModel);
                            }

                        });
                    });
                    ApiResponse = "Api request processed successfully.";
                    ProcessStatus = 1;
                }
                else
                {
                    ApiResponse = "There is no record from GetOutboundReportQueue";
                    Log.LogDebug("There is no record from GetOutboundReportQueue");
                    ProcessStatus = 0;
                }

                Log.LogDebug("OutboundReportsScheduler  SyncReport() Method completed. triggerName : " + triggerName + ", schedulerName : " + schedulerName);
                return ApiResponse;
            }
            catch (Exception ex)
            {
                ApiResponse = "Error in OutboundReportsScheduler SyncReport method";
                Log.LogError("Error in OutboundReportsScheduler SyncReport(). triggerName: " + triggerName + ", schedulerName: " + schedulerName, ex);
            }
            finally
            {
                RcCheckDA rcCheckDA = new RcCheckDA();
                rcCheckDA.RcJobTrackingHistory(schedulerName, triggerName, triggerName, triggerName, 0, 0, 0, 0, 0, DateTime.UtcNow, null, ApiResponse, ProcessStatus);
            }
            return ApiResponse;
        }

        private static SyncReportModel GenerateReportFile(OutboundReportEntity outboundReportEntity, int total_Part, DateTime utcDateTime)
        {
            SyncReportModel syncReportModel = null;
            string ApiUrl = string.Empty;

            try
            {
                string ReportsRootFolder = ConfigurationManager.AppSettings.Get("ReportsRootFolder");
                TextReportModel ReportData = syncReportDataAccess.GetReportsData(outboundReportEntity, total_Part, utcDateTime);
                if (ReportData != null && ReportData.Items != null && ReportData.Items.Count > 0)
                {
                    syncReportModel = new SyncReportModel
                    {
                        ReportType = outboundReportEntity.ReportType,
                        FileName = ReportData.FileName + ".txt",
                        FileId = outboundReportEntity.RequestGuid,
                        RootFolder = ReportsRootFolder,
                        Year = ReportData.ExamYear,
                        ProcessDate = ReportData.ProcessDate,

                    };

                    WriteReportFileToText(syncReportModel, ReportData.Items);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error in SendEmailManager CallSendEmail Url : {0} and Exception Message : {1}", ApiUrl, ex.Message), ex);
            }

            return syncReportModel;
        }
        private static string WriteReportFileToText(SyncReportModel syncReportModel, List<string> Items)
        {
            string Status = string.Empty;
            try
            {

                //string boundary = "----------------------------" + DateTime.UtcNow.Ticks;

                //adding file data
                StringBuilder sb = new StringBuilder();
                //sb.Append(boundary);
                //sb.Append(Environment.NewLine);
                //sb.Append("Content-Disposition: form-data; name=\"file\"; filename=\"");
                //sb.Append(syncReportModel.FileName);
                //sb.Append("\"");
                //sb.Append(Environment.NewLine);
                //sb.Append("Content-Type: text/plain");
                //sb.Append(Environment.NewLine);
                //List<string> updatedStrings = Items.Take(Items.Count - 1) // Take the first element without modifying it
                //.Concat(Items.Select(s => s + Environment.NewLine)) // Add new lines to the middle elements
                //.Concat(Items.Skip(Items.Count - 1)) // Take the last element without modifying it
                //.ToList();
                //foreach (string s in updatedStrings)
                //{
                //    sb.Append(s);
                //}

                //sb.Append(Environment.NewLine);
                //sb.Append(boundary);
                //sb.Append("--");


                for (int i = 0; i < Items.Count - 1; i++)
                {
                    sb.AppendLine(Items[i]);
                }

                sb.Append(Items[Items.Count - 1]);

                using (TextWriter tw = new StreamWriter(syncReportModel.RootFolder + @"\\" + syncReportModel.FileId + "~" + syncReportModel.FileName))
                {
                    tw.Write(sb);
                }
                Status = "S001";
            }
            catch (Exception ex)
            {
                Status = "ER001";
                Log.LogError("Error in WriteReportFileToText", ex);
            }

            return Status;
        }
        private static async Task<SyncResponseModel> UploadFileAsync(SyncReportModel syncReportModel)
        {
            SyncResponseModel ApiResponse = new SyncResponseModel();
            string OutboundApiUrl = string.Empty;
            try
            {
                OutboundApiUrl = ConfigurationManager.AppSettings.Get("MarkingOutboundApiUrl");
                var settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.All;
                var ApiRequestObject = JsonConvert.SerializeObject(syncReportModel, Formatting.Indented, settings);
                string Content = HttpClientRequestHandler.PostApiMultiFormHandler(OutboundApiUrl, ApiRequestObject, syncReportModel.RootFolder + @"\\" + syncReportModel.FileId + "~" + syncReportModel.FileName);

                if (!string.IsNullOrEmpty(Content))
                {
                    ApiResponse = JsonConvert.DeserializeObject<SyncResponseModel>(Content);
                }
                else
                {
                    ApiResponse.Status = SyncResponseStatus.Error;
                    ApiResponse.Message = "Error in UploadFile method";
                }
                return ApiResponse;
            }
            catch (Exception ex)
            {
                ApiResponse.Status = SyncResponseStatus.Error;
                ApiResponse.Message = "Error in UploadFile method. Error : " + ex.Message;

                Log.LogError(string.Format("Error in SendEmailManager CallSendEmail Url : {0} and Exception Message : {1}", OutboundApiUrl, ex.Message), ex);
            }

            return ApiResponse;
        }
    }
}
