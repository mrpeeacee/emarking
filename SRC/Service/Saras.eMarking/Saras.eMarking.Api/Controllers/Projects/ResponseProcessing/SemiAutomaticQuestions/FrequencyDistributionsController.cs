using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.ResponseProcessing.SemiAutomaticQuestions;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigManagement;
using Saras.eMarking.Domain.ViewModels.ResponseProcessing;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.ResponseProcessing.SemiAutomaticQuestions
{

    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/response-processing/semi-automatic/frequency-distributions")]
    [ApiVersion("1.0")]
    public class FrequencyDistributionsController : BaseApiController<FrequencyDistributionsController>
    {
        readonly IFrequencyDistributionsService frequencyDistributionsService;
        readonly IAuthService AuthService;

        public FrequencyDistributionsController(IFrequencyDistributionsService _frequencyDistributionsService, ILogger<FrequencyDistributionsController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            frequencyDistributionsService = _frequencyDistributionsService;
            AuthService = _authService;

        }

        /// <summary>
        /// GetAllViewQuestions : This GET Api is used to get all semi automatic and sore finger question type
        /// </summary> 
        /// <returns></returns>
        [Authorize(Roles = "AO,CM")]
        [Route("allviewquestion")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetAllViewQuestions()
        {
            try
            {
                logger.LogDebug($"FrequencyDistributionsController > GetAllViewQuestions() started. ProjectId={GetCurrentProjectId()}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                logger.LogDebug($"FrequencyDistributionsController > GetAllViewQuestions() completed. ProjectId={GetCurrentProjectId()}");

                return Ok(await frequencyDistributionsService.GetAllViewQuestions(GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsController page while getting Frequency Distribution for specific Project: Method Name: GetAllViewQuestions()");
                throw;
            }
        }
        
        /// <summary>
        /// GetFrequencyDistribution : This GET Api used to get semi automatic question type questions
        /// </summary> 
        /// <param name="QuestionId"></param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM")]
        [Route("view-frequency-distributions/{QuestionId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetFrequencyDistribution(long QuestionId)
        {
            try
            {
                logger.LogDebug($"FrequencyDistributionsController > GetFrequencyDistribution() started. ProjectId={GetCurrentProjectId()} and QuestionId={QuestionId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                logger.LogDebug($"FrequencyDistributionsController > GetFrequencyDistribution() completed. ProjectId={GetCurrentProjectId()} and QuestionId={QuestionId}");

                return Ok(await frequencyDistributionsService.GetFrequencyDistribution(GetCurrentProjectId(), QuestionId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsController page while getting Frequency Distribution for specific Project: Method Name: GetFrequencyDistribution() QuestionId = {QuestionId}");
                throw;
            }
        }

        /// <summary>
        /// UpdateModerateMarks: This POST Api used to moderate the particular semiautmatic responses
        /// </summary>
        /// <param name="CandidatesAnswer"></param>
        /// <returns></returns>
        [Route("moderate-marks")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM")]
        public async Task<IActionResult> UpdateModerateMarks(CandidatesAnswerModel CandidatesAnswer)
        {
            string result = string.Empty;
            try
            {
                logger.LogDebug($"FrequencyDistributionsController > UpdateModerateMarks() started. ProjectId={GetCurrentProjectId()} and CandidatesAnswer={CandidatesAnswer}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await frequencyDistributionsService.UpdateModerateMarks(CandidatesAnswer, GetCurrentProjectUserRoleID(), GetCurrentProjectId());

                logger.LogDebug($"FrequencyDistributionsController > UpdateModerateMarks() completed. ProjectId={GetCurrentProjectId()} and CandidatesAnswer={CandidatesAnswer}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsController page while updating Moderate Marks for specific Project : Method Name : UpdateModerateMarks() CandidatesAnswer = {CandidatesAnswer}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.SEMIAUTOMATIC,
                    Entity = AuditTrailEntity.RESPONSE,
                    Remarks = CandidatesAnswer,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion


            }
        }

        /// <summary>
        /// UpdateOverallModerateMarks : This POST Api used to moderate the all the responses present in the selected QIG 
        /// </summary>
        /// <param name="ProjectQuestionId"></param>
        /// <returns></returns>
        [Route("moderate-marks/{ProjectQuestionId}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM")]
        public async Task<IActionResult> UpdateOverallModerateMarks(long ProjectQuestionId)
        {
            string result = string.Empty;
            try
            {
                logger.LogDebug($"FrequencyDistributionsController > UpdateOverallModerateMarks() started. ProjectId={GetCurrentProjectId()} and ProjectQuestionId={ProjectQuestionId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await frequencyDistributionsService.UpdateOverallModerateMarks(ProjectQuestionId, GetCurrentProjectUserRoleID(), GetCurrentProjectId());

                logger.LogDebug($"FrequencyDistributionsController > UpdateOverallModerateMarks() started. ProjectId={GetCurrentProjectId()} and ProjectQuestionId={ProjectQuestionId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsController page while updating Moderate Marks for specific Project : Method Name : UpdateOverallModerateMarks() ProjectQuestionId = {ProjectQuestionId}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.SEMIAUTOMATIC,
                    Entity = AuditTrailEntity.RESPONSE,
                    Remarks = "Marking  as zero for all pending response for projectQuestion:" + ProjectQuestionId,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// UpdateManualMarkig : This POST Api used for marking manually for particular Qig
        /// </summary>
        /// <param name="ManualMarkig"></param>
        /// <returns></returns>
        [Route("enable-manual-markig")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM")]
        public async Task<IActionResult> UpdateManualMarkig(EnableManualMarkigModel ManualMarkig)
        {
            string result = string.Empty;
            try
            {
                logger.LogDebug($"FrequencyDistributionsController > UpdateManualMarkig() started. ProjectId={GetCurrentProjectId()} and ManualMarkig={ManualMarkig}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await frequencyDistributionsService.UpdateManualMarkig(ManualMarkig, GetCurrentProjectUserRoleID(), GetCurrentProjectId());

                logger.LogDebug($"FrequencyDistributionsController > UpdateManualMarkig() completed. ProjectId={GetCurrentProjectId()} and ManualMarkig={ManualMarkig}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsController page while updating Manual Markig for specific Project : Method Name : UpdateManualMarkig() ManualMarkig ={ManualMarkig}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.SEMIAUTOMATIC,
                    Entity = AuditTrailEntity.RESPONSE,
                    Remarks = ManualMarkig,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }
        
        /// <summary>
        /// GetAllBlankSummary : This GET Api used to get all blank summary for all qig's present in the semi automatic questions
        /// </summary> 
        /// <param name="ParentQuestionId"></param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM")]
        [Route("view-all-blank-summary/{ParentQuestionId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetAllBlankSummary(long ParentQuestionId)
        {
            try
            {
                logger.LogDebug($"FrequencyDistributionsController > GetAllBlankSummary() started. ProjectId={GetCurrentProjectId()} and ParentQuestionId={ParentQuestionId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                logger.LogDebug($"FrequencyDistributionsController > GetAllBlankSummary() completed. ProjectId={GetCurrentProjectId()} and ParentQuestionId={ParentQuestionId}");

                return Ok(await frequencyDistributionsService.GetAllBlankSummary(GetCurrentProjectId(), ParentQuestionId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsController page while getting All blank summary for specific Project: Method Name: GetAllBlankSummary() and ParentQuestionId = {ParentQuestionId}");
                throw;
            }
        }

        /// <summary>
        /// GetDiscrepancyReportFIB : This GET Api used to get Discrepancy data for particular blanks
        /// </summary> 
        /// <param name="candidateResponse"></param>
        /// <param name="ProjectQuestionId"></param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM")]
        [Route("discrepancy-report-fib/{candidateResponse}/{ProjectQuestionId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetDiscrepancyReportFIB(string candidateResponse, long ProjectQuestionId)
        {
            try
            {
                logger.LogDebug($"FrequencyDistributionsController > GetDiscrepancyReportFIB() started. ProjectId={GetCurrentProjectId()} and candidateResponse={candidateResponse} and ProjectQuestionId={ProjectQuestionId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                logger.LogDebug($"FrequencyDistributionsController > GetDiscrepancyReportFIB() started. ProjectId={GetCurrentProjectId()} and candidateResponse={candidateResponse} and ProjectQuestionId={ProjectQuestionId}");

                return Ok(await frequencyDistributionsService.GetDiscrepancyReportFIB(GetCurrentProjectId(), candidateResponse, ProjectQuestionId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsController page while getting All blank summary for specific Project: Method Name: GetDiscrepancyReportFIB() and candidateResponse = {candidateResponse}, ProjectQuestionId = {ProjectQuestionId}");
                throw;
            }
        }

        /// <summary>
        /// UpdateNormaliseScore: This POST Api used to update the normalized score to particular response
        /// </summary>
        /// <param name="ObjFibReport"></param>
        /// <returns></returns>
        [Route("normalise-score")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM")]
        public async Task<IActionResult> UpdateNormaliseScore(DiscrepencyNormalizeScoreModel ObjFibReport)
        {
            string result = string.Empty;
            try
            {
                logger.LogDebug($"FrequencyDistributionsController > UpdateNormaliseScore() started. ProjectId={GetCurrentProjectId()} and ObjFibReport={ObjFibReport}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await frequencyDistributionsService.UpdateNormaliseScore(ObjFibReport, GetCurrentProjectUserRoleID(), GetCurrentProjectId());

                logger.LogDebug($"FrequencyDistributionsController > UpdateNormaliseScore() completed. ProjectId={GetCurrentProjectId()} and ObjFibReport={ObjFibReport}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsController page while updating Manual Markig for specific Project : Method Name : UpdateNormaliseScore() ObjFibReport = {ObjFibReport}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.SEMIAUTOMATIC,
                    Entity = AuditTrailEntity.RESPONSE,
                    Remarks = ObjFibReport,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// UpdateManualMarkig : This POST Api used for marking manually for particular Qig
        /// </summary>
        /// <param name="ParentQuestionId"></param>
        /// <returns></returns>
        [Route("all-responses-to-manual-markig/{ParentQuestionId}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM")]
        public async Task<IActionResult> UpdateAllResponsestoManualMarkig(long ParentQuestionId)
        {
            string result = string.Empty;
            try
            {
                logger.LogDebug("FrequencyDistributionsController > UpdateAllResponsestoManualMarkig() started. ProjectId={GetCurrentProjectId()}, ParentQuestionId={ParentQuestionId}", GetCurrentProjectId(), ParentQuestionId);

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await frequencyDistributionsService.UpdateAllResponsestoManualMarkig(ParentQuestionId, GetCurrentProjectUserRoleID(), GetCurrentProjectId());

                logger.LogDebug("FrequencyDistributionsController > UpdateAllResponsestoManualMarkig() started. ProjectId={GetCurrentProjectId()}, ParentQuestionId={ParentQuestionId}", GetCurrentProjectId(), ParentQuestionId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in FrequencyDistributionsController page while updating Manual Markig for specific Project : Method Name : UpdateAllResponsestoManualMarkig().ProjectId={GetCurrentProjectId()}, ParentQuestionId ={ParentQuestionId}", GetCurrentProjectId(), ParentQuestionId);
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.SEMIAUTOMATIC,
                    Entity = AuditTrailEntity.RESPONSE,
                    Remarks = ParentQuestionId,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// UpdateAcceptDescrepancy: This POST Api used to update the accept-descrepancy to particular response
        /// </summary>
        /// <param name="ObjFibReport"></param>
        /// <returns></returns>
        [Route("accept-descrepancy")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM")]
        public async Task<IActionResult> UpdateAcceptDescrepancy(DiscrepencyNormalizeScoreModel ObjFibReport)
        {
            string result = string.Empty;
            try
            {
                logger.LogDebug($"FrequencyDistributionsController > UpdateAcceptDescrepancy() started. ProjectId={GetCurrentProjectId()} and ObjFibReport={ObjFibReport}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await frequencyDistributionsService.UpdateAcceptDescrepancy(ObjFibReport, GetCurrentProjectUserRoleID(), GetCurrentProjectId());

                logger.LogDebug($"FrequencyDistributionsController > UpdateAcceptDescrepancy() completed. ProjectId={GetCurrentProjectId()} and ObjFibReport={ObjFibReport}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsController page while updating Manual Markig for specific Project : Method Name : UpdateAcceptDescrepancy() ObjFibReport = {ObjFibReport}");
                throw;
            }
            finally
            {
                ////#region Insert Audit Trail
                ////_ = InsertAuditLogs(new AuditTrailData
                ////{
                ////    Event = AuditTrailEvent.UPDATE,
                ////    Module = AuditTrailModule.SEMIAUTOMATIC,
                ////    Entity = AuditTrailEntity.RESPONSE,
                ////    Remarks = ObjFibReport,
                ////    UserId = GetCurrentUserId(),
                ////    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                ////    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                ////    Response = JsonSerializer.Serialize(result)
                ////});
                ////#endregion
            }
        }

    }
}
