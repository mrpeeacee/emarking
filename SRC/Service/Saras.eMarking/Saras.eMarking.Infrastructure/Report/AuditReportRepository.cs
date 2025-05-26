using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Report
{
    ////    public class AuditReportRepository : BaseRepository<AuditReportRepository>, IAuditReportRepository
    ////    {
    ////        private readonly ApplicationDbContext context;
    ////        public AuditReportRepository(ILogger<AuditReportRepository> _logger, ApplicationDbContext context) : base(_logger)
    ////        {
    ////            this.context = context;
    ////        }

    ////        /// <summary>
    ////        /// GetAuditReport
    ////        /// </summary>
    ////        /// <param name="userId"></param>
    ////        /// <param name="projectuserroleID"></param>
    ////        /// <returns></returns>
    ////        public async Task<List<LoginReportModel>> GetAuditReport(AuditReportModel objaudit, long projectuserroleID, string LoginId, UserTimeZone TimeZone)

    ////        {
    ////            List<AuditReportModel> result = null;
    ////            List<LoginReportModel> finalresult = new List<LoginReportModel>();
    ////            LoginReportModel objlogin = null;

    ////            var startdateVal = DateTime.Parse(objaudit.StartDate);
    ////            var enddateval = DateTimeOffset.Parse(objaudit.EndDate).Date;
    ////            try
    ////            {
    ////                if (objaudit != null)
    ////                {
    ////                    await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
    ////                    using SqlCommand sqlCmd = new("[Marking].[UspGetEventAuditDetails]", sqlCon);
    ////                    sqlCmd.CommandType = CommandType.StoredProcedure;
    ////                    sqlCmd.Parameters.Add("@USERID", SqlDbType.BigInt).Value = 0;
    ////                    sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = 0;
    ////                    sqlCmd.Parameters.Add("@ModuleCode", SqlDbType.NVarChar).Value = objaudit.ModuleCode;
    ////                    sqlCmd.Parameters.Add("@EntityCode", SqlDbType.NVarChar).Value = objaudit.EntityCode;
    ////                    sqlCmd.Parameters.Add("@EventCode", SqlDbType.NVarChar).Value = objaudit.EventCode;
    ////                    sqlCmd.Parameters.Add("@LoginID", SqlDbType.NVarChar).Value = objaudit.LoginId;
    ////                    sqlCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = Convert.ToDateTime(startdateVal.ToString("yyyy-MM-dd"));
    ////                    sqlCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = Convert.ToDateTime(enddateval.ToString("yyyy-MM-dd")).AddHours(23).AddMinutes(59);
    ////                    sqlCon.Open();
    ////                    SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();

    ////                    if (reader.HasRows)
    ////                    {
    ////                        result = new List<AuditReportModel>();
    ////                        while (reader.Read())
    ////                        {
    ////                            AuditReportModel objqm = new AuditReportModel();

    ////                            objqm.FirstName = reader["FirstName"].ToString();
    ////                            objqm.LoginId = reader["LoginID"].ToString();
    ////                            objqm.EventId = Convert.ToInt64(reader["EventID"]);
    ////                            objqm.EventCode = reader["EventCode"].ToString();
    ////                            objqm.EntityId = Convert.ToInt64(reader["EntityID"]);
    ////                            objqm.EntityCode = reader["EntityCode"].ToString();
    ////                            objqm.EventDescription = reader["Description"].ToString();
    ////                            objqm.EntityDescription = reader["EntityDescription"].ToString();
    ////                            objqm.EventDateTime = reader["EventDateTime"] == DBNull.Value ? null : Convert.ToDateTime(reader["EventDateTime"]);
    ////                            objqm.IPAddress = reader["IPAddress"].ToString();
    ////                            objqm.Remarks = reader["Remarks"].ToString();
    ////                            objqm.Status = Convert.ToInt32(reader["Status"]);
    ////                            objqm.SessionId = reader["SessionID"].ToString();
    ////                            objqm.ModuleCode = reader["ModuleCode"].ToString();

    ////                            result.Add(objqm);
    ////                        }
    ////                    }

    ////                    if (!reader.IsClosed)
    ////                    {
    ////                        reader.Close();
    ////                    }

    ////                    if (sqlCon.State != ConnectionState.Closed)
    ////                    {
    ////                        sqlCon.Close();
    ////                    }

    ////                    if (result != null)
    ////                    {
    ////                        if (objaudit.ModuleCode == null || objaudit.ModuleCode == string.Empty)
    ////                        {
    ////                            var groupbyres = result.Where(ad => !string.IsNullOrEmpty(ad.SessionId)).Where(a => (a.ModuleCode == "USERLOGIN" && a.EventDateTime != null)).GroupBy(a => a.SessionId).ToList();
    ////                            foreach (var item in groupbyres)
    ////                            {
    ////                                objlogin = new LoginReportModel();
    ////                                objlogin.UserName = item.FirstOrDefault().LoginId; /*((DateTime)smpst.GracePeriodEndDateTime).UtcToProfileDateTime(userTimeZone)*/
    ////                                objlogin.LogInDateTime = (item.FirstOrDefault(a => a.EventCode == "LOGIN")?.EventDateTime).UtcToProfileDateTime(TimeZone);
    ////                                objlogin.LogOutDateTime = item.FirstOrDefault(a => a.EventCode == "LOGOUT")?.EventDateTime.UtcToProfileDateTime(TimeZone);
    ////                                //objlogin.IPAddress = item.FirstOrDefault()?.IPAddress
    ////                                objlogin.Status = item.FirstOrDefault().Status;
    ////                                objlogin.ModuleCode = item.FirstOrDefault().ModuleCode;
    ////                                objlogin.EventId = item.FirstOrDefault().EventId;
    ////                                objlogin.Remarks = item.FirstOrDefault().Remarks;
    ////                                if (objlogin.LogOutDateTime != null && objlogin.LogInDateTime != null)
    ////                                {
    ////                                    objlogin.Duration = (DateTime.Parse(objlogin.LogOutDateTime.ToString()).Subtract(DateTime.Parse(objlogin.LogInDateTime.ToString()))).ToString(@"hh\:mm\:ss");
    ////                                }
    ////                                else
    ////                                {
    ////                                    objlogin.Duration = null;
    ////                                }

    ////                                finalresult.Add(objlogin);
    ////                            }
    ////                            finalresult = finalresult.OrderBy(a => a.UserName).ToList();
    ////                            int i = 1;
    ////                            finalresult.GroupBy(a => a.UserName).ToList().ForEach(item =>
    ////                            {
    ////                                i = 1;
    ////                                foreach (var val in item)
    ////                                {
    ////                                    val.SlNo = i++;
    ////                                }
    ////                            });
    ////                        }
    ////                        else
    ////                        {
    ////                            var groupbyress = result.GroupBy(a => a.SessionId).ToList();

    ////                            foreach (var item in groupbyress)
    ////                            {
    ////                                objlogin = new LoginReportModel();
    ////                                objlogin.ActivitiesperformedModels = item.Where((a => a.EventCode == "LOGIN" || a.EventCode == "LOGOUT" || a.EventCode == "CREATE" ||
    ////                                a.EventCode == "DELETE" || a.EventCode == "UPDATE" || a.EventCode == "SAVE" || a.EventCode == "CHANGEPASSWORD" || a.EventCode == "FORGOTPASSWORD"))
    ////                                    .Select(a => new LoginReportModel
    ////                                    {
    ////                                        UserName = a.LoginId,
    ////                                        EventDateTime = a.EventDateTime,
    ////                                        //IPAddress = a.IPAddress,
    ////                                        Status = a.Status,
    ////                                        ModuleCode = a.ModuleCode,
    ////                                        EventCode = a.EventCode,
    ////                                        Remarks = a.Remarks,
    ////                                        EventId = a.EventId
    ////                                    }).OrderBy(a => a.EventDateTime).ToList();
    ////                                objlogin.UserName = item.FirstOrDefault().LoginId; /*((DateTime)smpst.GracePeriodEndDateTime).UtcToProfileDateTime(userTimeZone)*/
    ////                                objlogin.LogInDateTime = item.FirstOrDefault(a => a.EventCode == "LOGIN")?.EventDateTime.UtcToProfileDateTime(TimeZone);
    ////                                objlogin.LogOutDateTime = item.FirstOrDefault(a => a.EventCode == "LOGOUT")?.EventDateTime.UtcToProfileDateTime(TimeZone);
    ////                                //objlogin.IPAddress = item.FirstOrDefault()?.IPAddress
    ////                                objlogin.Remarks = item.FirstOrDefault().Remarks;
    ////                                objlogin.Testdata = null;

    ////                                StringBuilder sbAmout = new StringBuilder();

    ////                                var tqigname = "";
    ////                                var tmandatoryquestions = "";
    ////                                var tquestiontype = "";
    ////                                for (int k = 0; k < objlogin.ActivitiesperformedModels.Count; k++)
    ////                                {
    ////                                    var jsonchild = JsonConvert.DeserializeObject<object>((objlogin.ActivitiesperformedModels[k].Remarks != null ? objlogin.ActivitiesperformedModels[k].Remarks.ToString() : String.Empty));
    ////                                    var jsonchild1 = JsonConvert.DeserializeObject<JObject>((jsonchild != null ? jsonchild.ToString() : String.Empty));

    ////                                    foreach (KeyValuePair<string, JToken> keyValuePair in jsonchild1)
    ////                                    {

    ////                                        if (objlogin.ActivitiesperformedModels[k].EventCode == "DELETE" && objlogin.ActivitiesperformedModels[k].SessionId == objlogin.SessionId)
    ////                                        {

    ////                                            if (keyValuePair.Key == "QigName")
    ////                                            {
    ////                                                tqigname = " ' " + keyValuePair.Value.ToString() + " ' ";

    ////                                            }
    ////                                            if (keyValuePair.Key == "QigType" && (int)keyValuePair.Value == 2)
    ////                                            {
    ////                                                tquestiontype = "Composition";
    ////                                                sbAmout.Append(tqigname + " QIG deleted of type " + " ' " + tquestiontype + " ' "  + " at " + objlogin.ActivitiesperformedModels[k].EventDateTime.UtcToProfileDateTime(TimeZone) + ".");
    ////                                                sbAmout.Append("<br />");
    ////                                            }
    ////                                            else if (keyValuePair.Key == "QigType" && (int)keyValuePair.Value == 3)
    ////                                            {
    ////                                                tquestiontype = "Non Composition";
    ////                                                sbAmout.Append(tqigname + " QIG deleted of type " + " ' " + tquestiontype + " ' " + " at " + objlogin.ActivitiesperformedModels[k].EventDateTime.UtcToProfileDateTime(TimeZone) + ".");
    ////                                                sbAmout.Append("<br />");
    ////                                            }
    ////                                        }

    ////                                        else if (objlogin.ActivitiesperformedModels[k].EventCode == "SAVE" && objlogin.ActivitiesperformedModels[k].SessionId == objlogin.SessionId)
    ////                                        {
    ////                                            sbAmout.Append(" User finalised QIG Setup successfully </span> " + objlogin.ActivitiesperformedModels[k].EventDateTime.UtcToProfileDateTime(TimeZone) + ".");
    ////                                        }

    ////                                        else if (objlogin.ActivitiesperformedModels[k].EventCode == "CREATE" && objlogin.ActivitiesperformedModels[k].SessionId == objlogin.SessionId)
    ////                                        {

    ////                                            if (keyValuePair.Key == "QigName")
    ////                                            {
    ////                                                tqigname = " ' " + keyValuePair.Value.ToString() + " ' ";

    ////                                            }
    ////                                            if (keyValuePair.Key == "ManadatoryQuestions")
    ////                                            {
    ////                                                tmandatoryquestions = keyValuePair.Value.ToString() + " " + keyValuePair.Key;
    ////                                            }
    ////                                            if (keyValuePair.Key == "QigMarkingType" && (int)keyValuePair.Value == 2)
    ////                                            {
    ////                                                tquestiontype = "Composition";
    ////                                                sbAmout.Append(" QIG Created as " + tqigname + "of type " + " ' " + tquestiontype + " ' " + " with " + tmandatoryquestions + " at " + objlogin.ActivitiesperformedModels[k].EventDateTime.UtcToProfileDateTime(TimeZone) + ".");
    ////                                                sbAmout.Append("<br />");
    ////                                            }
    ////                                            else if (keyValuePair.Key == "QigMarkingType" && (int)keyValuePair.Value == 3)
    ////                                            {
    ////                                                tquestiontype = "Non Composition";
    ////                                                sbAmout.Append(" QIG Created as " + tqigname + "of type " + " ' " + tquestiontype + " ' " + " with " + tmandatoryquestions  +  " at " + objlogin.ActivitiesperformedModels[k].EventDateTime.UtcToProfileDateTime(TimeZone) + ".");
    ////                                                sbAmout.Append("<br />");
    ////                                            }

    ////                                        }
    ////                                        else if (objlogin.ActivitiesperformedModels[k].EventCode == "UPDATE" && objlogin.ActivitiesperformedModels[k].SessionId == objlogin.SessionId)
    ////                                        {
    ////                                            if (keyValuePair.Key == "QigName")
    ////                                            {
    ////                                                tqigname = " ' " + keyValuePair.Value.ToString() + " ' ";
    ////                                            }
    ////                                            if (keyValuePair.Key == "ManadatoryQuestions")
    ////                                            {
    ////                                                tmandatoryquestions = keyValuePair.Value.ToString() + " " + keyValuePair.Key;
    ////                                            }
    ////                                            if (keyValuePair.Key == "QigMarkingType" && (int)keyValuePair.Value == 2)
    ////                                            {
    ////                                                tquestiontype = "Composition";
    ////                                                sbAmout.Append(" QIG Updated as " + tqigname + "of type " + " ' " + tquestiontype  + " ' " +" with " + tmandatoryquestions + " at " + objlogin.ActivitiesperformedModels[k].EventDateTime.UtcToProfileDateTime(TimeZone) + ".");
    ////                                                sbAmout.Append("<br />");
    ////                                            }
    ////                                            else if (keyValuePair.Key == "QigMarkingType" && (int)keyValuePair.Value == 3)
    ////                                            {
    ////                                                tquestiontype = "Non Composition";
    ////                                                sbAmout.Append(" QIG Updated as " + tqigname + "of type " + " ' " + tquestiontype + " ' " + " with " + tmandatoryquestions  + " at " + objlogin.ActivitiesperformedModels[k].EventDateTime.UtcToProfileDateTime(TimeZone) + ".");
    ////                                                sbAmout.Append("<br />");
    ////                                            }
    ////                                        }
    ////                                        if (objlogin.ActivitiesperformedModels[k].SessionId == objlogin.SessionId)
    ////                                        {
    ////                                            bool isChangePasswordEvent = objlogin.ActivitiesperformedModels[k].EventCode == "CHANGEPASSWORD" && keyValuePair.Key == "Oldpassword" && keyValuePair.Key != "CaptchaText";
    ////                                            bool isForgotPasswordEvent = objlogin.ActivitiesperformedModels[k].EventCode == "FORGOTPASSWORD" && keyValuePair.Key == "CaptchaText";

    ////                                            if (isChangePasswordEvent || isForgotPasswordEvent)
    ////                                            {
    ////                                                if (objlogin.ActivitiesperformedModels[k].Status == 5)
    ////                                                {
    ////                                                    string action = isChangePasswordEvent ? "Changed password" : "Forgot password changed";
    ////                                                    sbAmout.Append(action + " successfully at </span><b> " + objlogin.ActivitiesperformedModels[k].EventDateTime.UtcToProfileDateTime(TimeZone) + "</b>.");
    ////                                                }
    ////                                                else
    ////                                                {
    ////                                                    string action = isChangePasswordEvent ? "Changed password" : "Forgot password";
    ////                                                    sbAmout.Append(action + " failed at </span><b> " + objlogin.ActivitiesperformedModels[k].EventDateTime.UtcToProfileDateTime(TimeZone) + "</b>.");
    ////                                                }
    ////                                            }
    ////                                        }

    ////                                    }
    ////                                }
    ////                                objlogin.Testdata += " " + sbAmout.ToString();
    ////                                objlogin.Status = item.FirstOrDefault().Status;
    ////                                objlogin.ModuleCode = item.FirstOrDefault().ModuleCode;
    ////                                objlogin.EventId = item.FirstOrDefault().EventId;

    ////                                if (objlogin.LogOutDateTime != null && objlogin.LogInDateTime != null)
    ////                                {
    ////                                    objlogin.Duration = (DateTime.Parse(objlogin.LogOutDateTime.ToString()).Subtract(DateTime.Parse(objlogin.LogInDateTime.ToString()))).ToString(@"hh\:mm\:ss");
    ////                                }
    ////                                else
    ////                                {
    ////                                    objlogin.Duration = null;
    ////                                }

    ////                                finalresult.Add(objlogin);
    ////                            }
    ////                            finalresult = finalresult.OrderBy(a => a.UserName).ToList();
    ////                            int i = 1;
    ////                            finalresult.GroupBy(a => a.UserName).ToList().ForEach(item =>
    ////                            {
    ////                                i = 1;
    ////                                foreach (var val in item)
    ////                                {
    ////                                    val.SlNo = i++;
    ////                                }
    ////                            });
    ////                        }
    ////                    }

    ////                    else
    ////                    {
    ////                        return null;
    ////                    }
    ////                }
    ////                else
    ////                {
    ////                    return null;
    ////                }
    ////            }
    ////            catch (Exception ex)
    ////            {
    ////                logger.LogError(ex, "Error in Audit Report page while getting Audit Report Log:Method Name:GetAuditReport()");
    ////                throw;
    ////            }

    ////            return finalresult.OrderBy(a => a.EventDateTime).ThenBy(a => a.SessionId).ToList();
    ////        }


    ////    }
    ////}
 

    public class AuditReportRepository : BaseRepository<AuditReportRepository>, IAuditReportRepository
    {
        private readonly ApplicationDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditReportRepository"/> class.
        /// </summary>
        /// <param name="_logger">The _logger.</param>
        /// <param name="context">The context.</param>
        public AuditReportRepository(ILogger<AuditReportRepository> _logger, ApplicationDbContext context) : base(_logger)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the app modules.
        /// </summary>
        /// <returns>A Task.</returns>
        public async Task<List<ApplicationModuleModel>> GetAppModules()
        {
            logger.LogDebug("AuditReportRepository GetAppModules() method started. ");
            List<ApplicationModuleModel> moduleData = null;
            try
            {
                moduleData = await (from appModule in context.ModuleMasters
                                    where appModule.IsDeleted == false
                                    select new ApplicationModuleModel
                                    {
                                        ModuleCode = appModule.ModuleCode,
                                        ModuleName = appModule.ModuleName
                                    }).ToListAsync();

                logger.LogDebug("AuditReportRepository GetAppModules() method completed. ");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Audit Report page while getting application modules : Method Name : GetAppModules()");
                throw;
            }
            return moduleData;
        }

        /// <summary>
        /// Get Audit Report data access
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectuserroleID"></param>
        /// <returns></returns>
        public async Task<List<AuditReportModel>> GetAuditReport(AuditReportRequestModel objaudit)
        {
            logger.LogDebug("AuditReportRepository GetAuditReport() method started.");
            List<AuditReportModel> result = null;
            var startdateVal = DateTime.Parse(objaudit.StartDate);
            var enddateval = DateTimeOffset.Parse(objaudit.EndDate).Date;
            try
            {
                ////if (objaudit != null)
                ////{
                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[UspGetEventAuditDetails]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure; 
                sqlCmd.Parameters.Add("@ModuleCode", SqlDbType.NVarChar).Value = objaudit.ModuleCodes;
                sqlCmd.Parameters.Add("@LoginID", SqlDbType.NVarChar).Value = objaudit.LoginId;
                sqlCmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = Convert.ToDateTime(startdateVal.ToString("yyyy-MM-dd"));
                sqlCmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = Convert.ToDateTime(enddateval.ToString("yyyy-MM-dd")).AddHours(23).AddMinutes(59);
                sqlCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = objaudit.PageNo;
                sqlCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = objaudit.PageSize;
                sqlCon.Open();

                SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    ////result = new List<AuditReportModel>();
                    ////while (reader.Read())
                    ////{
                    ////    AuditReportModel objqm = new()
                    ////    {
                    ////        FirstName = Convert.ToString(reader["FirstName"]),
                    ////        LoginId = Convert.ToString(reader["LoginID"]),
                    ////        EventId = Convert.ToInt64(reader["EventID"]),
                    ////        EventCode = Convert.ToString(reader["EventCode"]),
                    ////        EntityId = Convert.ToInt64(reader["EntityID"]),
                    ////        EntityCode = Convert.ToString(reader["EntityCode"]),
                    ////        EventDescription = Convert.ToString(reader["Description"]),
                    ////        EntityDescription = Convert.ToString(reader["EntityDescription"]),
                    ////        EventDateTime = reader["EventDateTime"] == DBNull.Value ? null : Convert.ToDateTime(reader["EventDateTime"]),
                    ////        IPAddress = Convert.ToString(reader["IPAddress"]),
                    ////        Remarks = Convert.ToString(reader["Remarks"]),
                    ////        Status = Convert.ToInt32(reader["Status"]),
                    ////        SessionId = Convert.ToString(reader["SessionID"]),
                    ////        ProjectName = Convert.ToString(reader["ProjectName"]),
                    ////        ModuleCode = Convert.ToString(reader["ModuleCode"]),
                    ////        TotalRows = Convert.ToInt32(reader["TotalRows"]),
                    ////    };

                    ////    result.Add(objqm);
                    ////}

                    ////if (result.Count > 0)
                    ////{
                    ////    result = result.OrderBy(a => a.EventDateTime).ThenBy(a => a.SessionId).ToList();
                    ////}

                    result = BuildAuditReport(reader);
                }

                if (!reader.IsClosed) { reader.Close(); }
                if (sqlCon.State == ConnectionState.Open) { sqlCon.Close(); }
                ////}
                logger.LogDebug("AuditReportRepository GetAuditReport() method completed. ");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Audit Report page while getting Audit Report Log : Method Name:GetAuditReport()");
                throw;
            }

            return result;
        }

        /// <summary>
        /// Builds the audit report.
        /// </summary>
        /// <param name="reader"> sql reader object</param>
        /// <returns>A list of AuditReportModels.</returns>
        private static List<AuditReportModel> BuildAuditReport(SqlDataReader reader)
        {
            List<AuditReportModel> result = new();
            while (reader.Read())
            {
                AuditReportModel objqm = new()
                {
                    FirstName = Convert.ToString(reader["FirstName"]),
                    LoginId = Convert.ToString(reader["LoginID"]),
                    EventId = Convert.ToInt64(reader["EventID"]),
                    EventCode = Convert.ToString(reader["EventCode"]),
                    EntityId = Convert.ToInt64(reader["EntityID"]),
                    EntityCode = Convert.ToString(reader["EntityCode"]),
                    EventDescription = Convert.ToString(reader["Description"]),
                    EntityDescription = Convert.ToString(reader["EntityDescription"]),
                    EventDateTime = reader["EventDateTime"] == DBNull.Value ? null : Convert.ToDateTime(reader["EventDateTime"]),
                    IPAddress = Convert.ToString(reader["IPAddress"]),
                    Remarks = Convert.ToString(reader["Remarks"]),
                    Status = Convert.ToInt32(reader["Status"]),
                    SessionId = Convert.ToString(reader["SessionID"]),
                    ProjectName = Convert.ToString(reader["ProjectName"]),
                    ModuleCode = Convert.ToString(reader["ModuleCode"]),
                    TotalRows = Convert.ToInt32(reader["TotalRows"]),
                };

                result.Add(objqm);
            }

            if (result.Count > 0)
            {
                 result = result.OrderBy(a => a.EventDateTime).ThenBy(a => a.EventDateTime).ToList();
                //result = result.OrderByDescending(x => x.EventDateTime).ToList();
            }

            return result;
        }
    }
}