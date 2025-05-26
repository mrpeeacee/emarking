using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Business.Account;
using Saras.eMarking.Business;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Qig
{
    /// <summary>
    /// Qig Api
    /// </summary>
    [ApiController]
    [Route("/api/public/v{version:apiVersion}/qig-configurations")]
    [ApiVersion("1.0")]
    public class QigConfigController : BaseApiController<QigConfigController>
    {
        private readonly IQigConfigService QigConfigService;
        private readonly IAuthService AuthService;

        /// <summary>
        /// QIG constructor
        /// </summary>
        /// <param name="qigService"></param>
        /// <param name="_logger"></param>
        /// <param name="appOptions"></param> 
        /// <param name="_authService"></param>
        /// <param name="_auditService"></param>
        public QigConfigController(IQigConfigService qigService, ILogger<QigConfigController> _logger, AppOptions appOptions, IAuthService _authService, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            QigConfigService = qigService;
            AuthService = _authService;
        }

        /// <summary>
        /// GetQuestions : This GET Api is used to get the Questions tagged to Qig
        /// </summary> 
        /// <param name="qigid"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/qig/qig/{qigid}/question")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]

        public async Task<IActionResult> GetQuestions(long qigid)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigid))
                {
                    return new ForbidResult();
                }
                return Ok(await QigConfigService.GetAllQigQuestions(GetCurrentProjectId(), qigid));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Setup Page while getting Qig Questions: Method Name: GetQuestions() and ProjectID = " + GetCurrentProjectId() + ", QigId = " + qigid.ToString());
                throw;
            }
        }

        /// <summary>
        /// GetQigQuestionandMarks : This GET Api is used to get the all the questions and marks for specific project and Qig
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/qig/qigquestionsandmarks/{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQigQuestionandMarks(long QigId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await QigConfigService.GetQigQuestionandMarks(QigId, GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration page while getting QIG Questions and Total marks :Method Name: GetQigQuestionandMarks() and ProjectID=" + GetCurrentProjectId());
                throw;
            }
        }

        /// <summary>
        /// Getavailablemarkschemes : This GET Api is used to get the all the available mark schemes
        /// </summary>
        /// <param name="Maxmarks"></param>
        /// <param name="markschemeType"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/qig/availablemarkschemes/{Maxmarks}/{markschemeType}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> Getavailablemarkschemes(decimal Maxmarks, int? markschemeType = null)
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await QigConfigService.Getavailablemarkschemes(Maxmarks, GetCurrentProjectId(), markschemeType));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration page while getting QIG Questions and Total marks :Method Name: Getavailablemarkschemes() and ProjectID=" + GetCurrentProjectId());
                throw;
            }
        }

        [Route("/api/public/v{version:apiVersion}/qig/availableScoringLibrary/{Maxmarks}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetavailableScoringLibrary( decimal Maxmarks)
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await QigConfigService.GetavailableScoringLibrary(GetCurrentProjectId(), Maxmarks));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration page while getting QIG Questions and Total marks :Method Name: Getavailablemarkschemes() and ProjectID=" + GetCurrentProjectId());
                throw;
            }
        }
        /// <summary>
        /// TagAvailableMarkScheme : This POST Api is used to tag the available mark schme
        /// </summary>
        /// <param name="ObQigQuestionModel"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/qig/tagavailablemarkscheme")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> TagAvailableMarkScheme(QigQuestionModel ObQigQuestionModel)
        {
            string status = string.Empty;
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                status = await QigConfigService.TagAvailableMarkScheme(ObQigQuestionModel, GetCurrentProjectUserRoleID(), GetCurrentProjectId());
                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration page while tagging available mark schemes for specific Project : Method Name : TagAvailableMarkScheme()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.TAG,
                    Entity = AuditTrailEntity.QIG,
                    Module = AuditTrailModule.QIGSETUP,
                    Remarks = ObQigQuestionModel,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = status == "UP001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(status)
                });
                #endregion 
            }
        }


		[Route("/api/public/v{version:apiVersion}/qig/SaveScoringComponentLibrary")]
		[HttpPost]
		[ApiVersion("1.0")]
		[Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
		public async Task<IActionResult> SaveScoringComponentLibrary(QigQuestionModel ObQigQuestionModel)
		{
			string status = string.Empty;
			try
			{
				if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
				{
					return new ForbidResult();
				}
				status = await QigConfigService.SaveScoringComponentLibrary(ObQigQuestionModel, GetCurrentProjectUserRoleID(), GetCurrentProjectId());
				return Ok(status);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error in Qig Configuration page while tagging available mark schemes for specific Project : Method Name : TagAvailableMarkScheme()");
				throw;
			}
			finally
			{
				#region Insert Audit Trail
				_ = InsertAuditLogs(new AuditTrailData
				{
					Event = AuditTrailEvent.TAG,
					Entity = AuditTrailEntity.QIG,
					Module = AuditTrailModule.QIGSETUP,
					Remarks = ObQigQuestionModel,
					UserId = GetCurrentUserId(),
					ProjectUserRoleID = GetCurrentProjectUserRoleID(),
					ResponseStatus = status == "UP001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
					Response = JsonSerializer.Serialize(status)
				});
				#endregion
			}
		}

		/// <summary>
		/// GetSetupStatus : This GET Api is used to get the set up status for specific project
		/// </summary>
		/// <returns></returns>
		[Route("/api/public/v{version:apiVersion}/qig/setupstatus")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetSetupStatus()
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await QigConfigService.GetSetupStatus(GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration page while getting QIG Questions and Total marks :Method Name: GetSetupStatus() and ProjectID=" + GetCurrentProjectId());
                throw;
            }
        }

        /// <summary>
        /// UpdateMaxMarks : This POST Api is used to update the max marks for specific project and projectquestionid
        /// </summary>
        /// <param name="projectQuestionId"></param>
        /// <param name="questionMaxmarks"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/qig/max-marks")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UpdateMaxMarks(QigQuestionModel ObQigQuestionModel)
        {
            string result = string.Empty;
            long projectQuestionId = (long)ObQigQuestionModel.ProjectQuestionID;
            long questionMaxmarks = (long)ObQigQuestionModel.MaxMark;
            var QuestionCode = ObQigQuestionModel.QuestionCode;
            try
            {

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await QigConfigService.UpdateMaxMarks(projectQuestionId, questionMaxmarks, GetCurrentProjectUserRoleID(), GetCurrentProjectId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration page while Updating  Question Marks :Method Name: UpdateMaxMarks() and ProjectID=" + GetCurrentProjectId());
                throw;
            }
            finally
            {

                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Entity = AuditTrailEntity.QIG,
                    Module = AuditTrailModule.QIGSETUP,
                    Remarks = new UpdatedQigQuestions { projectQuestionId = projectQuestionId, questionMaxmarks = questionMaxmarks, QuestionCode = QuestionCode },
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion 
            }
        }

        /// <summary>
        /// GetQIGConfigDetails : This GET Api is used to get the details of qig config
        /// </summary>
        /// <param name="qigid"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/qig/qig-config-details/{qigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQIGConfigDetails(long qigid)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigid))
                {
                    return new ForbidResult();
                }
                return Ok(await QigConfigService.GetQIGConfigDetails(GetCurrentProjectId(), qigid));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Setup Page while getting QIG Config Details: Method Name: GetQIGConfigDetails() and ProjectID = " + GetCurrentProjectId() + ", QigId = " + qigid.ToString());
                throw;
            }
        }



		/// <summary>
		/// GetQIGConfigDetails : This GET Api is used to check ISCBP project
		/// </summary>
		
		/// <returns></returns>
		[Route("/api/public/v{version:apiVersion}/qig/qig/IsCBPproject")]
		[HttpGet]
		[ApiVersion("1.0")]
		[Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
		public async Task<IActionResult> IsCBPproject()
		{
            bool IsCBPproject = false;
			try
			{
				IsCBPproject = await(QigConfigService.IsCBPproject(GetCurrentProjectId()));           
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error in Qig Setup Page while getting QIG Config Details: Method Name: GetQIGConfigDetails() and ProjectID = " + GetCurrentProjectId() );
				throw;
			}
            return Ok(IsCBPproject);
		}
	}
}
