using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.Reports
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/reports/ems")]
    public class EmsReportController : BaseApiController<EmsReportController>
    {
        private readonly IEmsReportService emsReportService;
        private readonly IAuthService AuthService;
        public EmsReportController(IEmsReportService _emsReportService, ILogger<EmsReportController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            emsReportService = _emsReportService;
            AuthService = _authService;
        }

        /// <summary>
        /// Get ESM1 Report
        /// </summary> 
        /// <returns></returns> 
        [Route("{istext}/1/{onlydelta}")]
        [HttpGet]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetEms1Report(byte istext, byte onlydelta)
        {
            long projectId = 0;
            
            try
            {
                projectId = GetCurrentProjectId();

                logger.LogDebug($"EmsReportController > GetEms1Report() started. Project = {projectId} and IsText = {istext} and OnlyDelta = {onlydelta}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                PaginationModel<Ems1ReportModel> result = await emsReportService.GetEms1Report(projectId, istext, onlydelta, AppOptions.AppSettings.Calendar.DefaultTimeZone.TimeZoneCode, GetCurrentContextTimeZone());

                logger.LogDebug($"EMSReportController > GetEms1Report() completed. Project = {projectId} and Istext = {istext} and OnlyDelta = {onlydelta}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportController > GetEms1Report() method. Project = {projectId} and Istext = {istext} and OnlyDelta = {onlydelta}");
                throw;
            }
        }

        /// <summary>
        /// Get ESM2 Report
        /// </summary> 
        /// <returns></returns> 
        [Route("{istext}/2/{onlydelta}")]
        [HttpGet]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetEms2Report(byte istext, byte onlydelta)
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();

                logger.LogDebug($"EMSReportController > GetEms2Report() started. Project = {projectId} and Istext = {istext} and OnlyDelta = {onlydelta}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                PaginationModel<Ems2ReportModel> result = await emsReportService.GetEms2Report(projectId, istext, onlydelta, AppOptions.AppSettings.Calendar.DefaultTimeZone.TimeZoneCode, GetCurrentContextTimeZone());

                logger.LogDebug($"EMSReportController > GetEms2Report() completed. Project = {projectId} and Istext = {istext} and OnlyDelta = {onlydelta}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportController > GetEms2Report(). Project = {projectId} and Istext = {istext} and OnlyDelta = {onlydelta}");
                throw;
            }
        }

        /// <summary>
        /// Get OMS Report
        /// </summary> 
        /// <returns></returns> 
        [Route("{istext}/3/{onlydelta}")]
        [HttpGet]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetOmsReport(byte istext, byte onlydelta)
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();

                logger.LogDebug($"EMSReportController > GetOmsReport() started. Project = {projectId} and Istext = {istext} and OnlyDelta = {onlydelta}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                PaginationModel<OmsReportModel> result = await emsReportService.GetOmsReport(projectId, istext, onlydelta, AppOptions.AppSettings.Calendar.DefaultTimeZone.TimeZoneCode, GetCurrentContextTimeZone());

                logger.LogDebug($"EMSReportController > GetOmsReport() completed. Project = {projectId} and Istext = {istext} and OnlyDelta = {onlydelta}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportController > GetOmsReport() method. Project = {projectId} and Istext = {istext} and OnlyDelta = {onlydelta}");
                throw;
            }
        }

        /// <summary>
        /// Download logs.
        /// </summary>
        /// <returns></returns> 
        [Route("{correlationid}/downloadoutboundlogs/{processedon}")]
        [AllowAnonymous]
        [HttpGet]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> DownloadOutboundLogs(string correlationid, string processedon)
        {
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();
                
                logger.LogDebug($"EMSReportController > DownloadOutboundLogs() started. ProjectId = {ProjectId} and CorrelationId = {correlationid} and TimeZone = {processedon}");

                PaginationModel<DownloadOutBoundLog> result = await emsReportService.DownloadOutboundLogs(correlationid, processedon);

                logger.LogDebug($"EMSReportController > DownloadOutboundLogs() completed. ProjectId = {ProjectId} and CorrelationId = {correlationid} and TimeZone = {processedon}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportController > DownloadOutboundLogs(). ProjectId = {ProjectId} and CorrelationId = {correlationid} and TimeZone = {processedon}");
                throw;
            }
        }

        [Route("studentreportresult")]
        [HttpPost]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> StudentResultReport(StudentResultReportModel studentResultReportModel)
        {
            try
            {
                logger.LogDebug($"EMSReportController > StudentResultReport() started. List of studentResultReportModel = {studentResultReportModel}");

                studentResultReportModel.ProjectID = GetCurrentProjectId();
                studentResultReportModel.ProjectUserRoleID = GetCurrentProjectUserRoleID();

                if (!AuthService.IsValidProject(studentResultReportModel.ProjectID, studentResultReportModel.ProjectUserRoleID))
                {
                    return new ForbidResult();
                }

                var response = await emsReportService.StudentResultReport(studentResultReportModel);
                List<StudentReport> result = null;
                if (response != null)
                {
                    result = response.Items;
                }

                logger.LogDebug($"EMSReportController > StudentResultReport() completed. studentResultReportModel = {studentResultReportModel}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportController > StudentResultReport() method. studentResultReportModel = {studentResultReportModel}");
                throw;
            }

        }

        [Route("QuestionCode/{qigidval}")]
        [HttpGet]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQuestions(long qigidval)
        {
            long ProjectID = 0;
            try
            {
                ProjectID = GetCurrentProjectId();

                logger.LogDebug($"EMSReportController > GetQuestions() started. ProjectId = {ProjectID} and QigId = {qigidval}");

                List<QuestionCodeModel> result = await emsReportService.GetQuestions(ProjectID, qigidval);

                logger.LogDebug($"EMSReportController > GetQuestions() completed. ProjectId = {ProjectID} and QigId = {qigidval}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportController > GetQuestions() method. ProjectId = {ProjectID} and QigId = {qigidval}");
                throw;
            }
        }

        /// <summary>
        /// Post for Sync report
        /// </summary> 
        /// <returns></returns> 
        [Route("{istype}/{OnlyDelta}")]
        [HttpPost]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> _auditService(byte istype, byte onlydelta)
        {
            long projectId = 0;
            string result = "";
            string _pagesize = AppOptions.AppSettings.ReportSyncPageSize;
            try
            {
                projectId = GetCurrentProjectId();

                logger.LogDebug($"EmsReportController > _auditService() started. ProjectId = {projectId} and IsType = {istype} and TimeZone = {AppOptions.AppSettings.Calendar.DefaultTimeZone.TimeZoneCode} and OnlyDelta = {onlydelta}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                result = await emsReportService.SyncEmsReport(projectId, onlydelta, GetCurrentProjectUserRoleID(), istype, _pagesize, AppOptions.AppSettings.Calendar.DefaultTimeZone.TimeZoneCode);

                logger.LogDebug($"EMSReportController > _auditService() completed. ProjectId = {projectId} and IsType = {istype} and TimeZone = {AppOptions.AppSettings.Calendar.DefaultTimeZone.TimeZoneCode} and OnlyDelta = {onlydelta}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportController > _auditService(). ProjectId = {projectId} and IsType = {istype} and TimeZone = {AppOptions.AppSettings.Calendar.DefaultTimeZone.TimeZoneCode} and OnlyDelta = {onlydelta}");
                throw;
            }
            finally
            {
                string results = string.Empty;
                if (istype == 1)
                {
                    results = "EMS1 is initiated";
                }
                if (istype == 2)
                {
                    results = "EMS2 is initiated";
                }
                if (istype == 3)
                {
                    results = "OMS is initiated";

                }

                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPLOADTOIEXAM2,
                    Module = AuditTrailModule.OUTBOUNDREPORTS,
                    Entity = AuditTrailEntity.REPORT,

                    Remarks = results,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// Get OMS Report
        /// </summary> 
        /// <returns></returns> 
        [Route("outbound/logs")]
        [HttpGet]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetReportsOutboundLogs()
        {
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();
                logger.LogDebug($"EMSReportController > GetReportsOutboundLogs() started. Project = {ProjectId}");

                if (!AuthService.IsValidProject(ProjectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                List<ReportsOutboundLogsModel> result = await emsReportService.GetReportsOutboundLogs(ProjectId, GetCurrentContextTimeZone());

                logger.LogDebug($"EMSReportController > GetReportsOutboundLogs() completed. Project = {GetCurrentProjectId()}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportController > GetReportsOutboundLogs(). Project = {ProjectId}");
                throw;
            }
        }

        /// <summary>
        /// Get Oral Project Closure Details.
        /// </summary> 
        /// <returns></returns> 
        [Route("outbound/oralproject")]
        [AllowAnonymous]
        [HttpGet]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetOralProjectClosureDetails()
        {
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();
                logger.LogDebug($"EMSReportController > GetOralProjectClosureDetails() started. Project = {ProjectId}");

                GetOralProjectClosureDetailsModel result = await emsReportService.GetOralProjectClosureDetails(ProjectId);

                logger.LogDebug($"EMSReportController > GetOralProjectClosureDetails() completed. Project = {ProjectId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportController > GetOralProjectClosureDetails(). Project = {ProjectId}");
                throw;
            }
        }

        [Route("IsArchive")]
        [HttpGet]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public  IActionResult CheckISArchive()
        {
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();
                logger.LogDebug($"EMSReportController > GetOralProjectClosureDetails() started. Project = {ProjectId}");

                Boolean result =  emsReportService.CheckISArchive(ProjectId);

                logger.LogDebug($"EMSReportController > GetOralProjectClosureDetails() completed. Project = {ProjectId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportController > GetOralProjectClosureDetails(). Project = {ProjectId}");
                throw;
            }

        }
    }
}
