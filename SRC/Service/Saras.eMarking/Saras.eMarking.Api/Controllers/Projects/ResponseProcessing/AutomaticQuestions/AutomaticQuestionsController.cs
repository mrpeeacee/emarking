using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.ResponseProcessing.AutomaticQuestions;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.ResponseProcessing;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.ResponseProcessing.AutomaticQuestions
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/response-processing/automatic")]
    [ApiVersion("1.0")]
    public class AutomaticQuestionsController : BaseApiController<AutomaticQuestionsController>
    {
        readonly IAutomaticQuestionsService automaticQuestionsService;
        readonly IAuthService AuthService;
        public AutomaticQuestionsController(IAutomaticQuestionsService _automaticQuestionsService, ILogger<AutomaticQuestionsController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            automaticQuestionsService = _automaticQuestionsService;
            AuthService = _authService;
        }

        /// <summary>
        /// Get Automatic Questions
        /// </summary> 
        /// <returns></returns>
        [Authorize(Roles = "AO,CM")]
        [Route("questions")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetViewAllAutomaticQuestions()
        {
            try
            {
                logger.LogDebug($"AutomaticQuestionsController > GetViewAllAutomaticQuestions() started. ProjectId={GetCurrentProjectId()}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                logger.LogDebug($"AutomaticQuestionsController > GetViewAllAutomaticQuestions() completed. ProjectId={GetCurrentProjectId()}");

                return Ok(await automaticQuestionsService.GetViewAllAutomaticQuestions(GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AutomaticQuestionsController page while getting Automatic Questions for specific Project: Method Name: GetViewAllAutomaticQuestions()");
                throw;
            }
        }

        /// <summary>
        /// Get view moderate automatic questions
        /// </summary>
        /// <param name="parentQuestionId"></param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM")]
        [Route("questions/{parentQuestionId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetViewModerateAutomaticQuestions(long? parentQuestionId = null)
        {
            try
            {
                logger.LogDebug($"AutomaticQuestionsController > GetViewModerateAutomaticQuestions() started. ProjectId={GetCurrentProjectId()} and (Optional)ParentQuestionId={parentQuestionId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                logger.LogDebug($"AutomaticQuestionsController > GetViewModerateAutomaticQuestions() completed. ProjectId={GetCurrentProjectId()} and (Optional)ParentQuestionId={parentQuestionId}");

                return Ok(await automaticQuestionsService.GetViewAllAutomaticQuestions(GetCurrentProjectId(), parentQuestionId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AutomaticQuestionsController page while getting Automatic Questions for specific Project: Method Name: GetViewAllAutomaticQuestions() and parentQuestionId = {parentQuestionId}");
                throw;
            }
        }

        /// <summary>
        /// Update Moderate Score in Automatic Questions
        /// </summary>
        /// <param name="CandidatesAnswer"></param>
        /// <returns></returns>
        [Route("moderate-score")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM")]
        public async Task<IActionResult> UpdateModerateScore(CandidatesAnswerModel CandidatesAnswer)
        {
            string result = string.Empty;
            long projectId = 0;
            long userId = 0;
            long currentprojectuserroleId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                userId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();

                logger.LogDebug($"AutomaticQuestionsController > UpdateModerateScore() started. ProjectId={GetCurrentProjectId()} and CandidatesAnswer={CandidatesAnswer}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await automaticQuestionsService.UpdateModerateScore(CandidatesAnswer, currentprojectuserroleId, projectId);

                logger.LogDebug($"AutomaticQuestionsController > UpdateModerateScore() completed. ProjectId={GetCurrentProjectId()} and CandidatesAnswer={CandidatesAnswer}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AutomatocQuestionsController page while updating Moderate Marks for specific Project : Method Name : UpdateModerateScore()");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.AUTOMATIC,
                    Entity = AuditTrailEntity.RESPONSE,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = CandidatesAnswer,
                    ResponseStatus = result == "P001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }
    }
}
