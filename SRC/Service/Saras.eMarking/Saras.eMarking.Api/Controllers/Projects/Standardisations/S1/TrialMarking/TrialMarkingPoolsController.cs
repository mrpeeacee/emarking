using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.TrialMarking;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.TrialMarking;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.Standardisations.S1.TrialMarking
{
    /// <summary>
    /// Standardisation trial marking pool controller
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/s1/trial-marking-pools")]
    public class TrialMarkingPoolsController : BaseApiController<TrialMarkingPoolsController>
    {
        private readonly ITrialMarkingPoolService _trialMarkingPoolService;

        public TrialMarkingPoolsController(ITrialMarkingPoolService trialMarkingPoolService, ILogger<TrialMarkingPoolsController> _logger, AppOptions appOptions, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            _trialMarkingPoolService = trialMarkingPoolService;
        }

        [Route("script/{qigid}/{filterby}/{searchValue?}")]
        [Authorize(Roles = "KP")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ActionResult> GetQIGScriptForTrialMark(long qigid, string filterby,string searchValue="")
        {
            try
            {
                return Ok(await _trialMarkingPoolService.GetQIGScriptForTrialMark(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigid, filterby,searchValue, CurrentUserContext.CurrentRole));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrialMarkingPoolController Page while fetching GetQIGScriptForTrialMark : Method Name : GetQIGScriptForTrialMark()");
                throw;
            }
        }


        [ValidateAntiForgeryToken]
        [Route("trailmark")]
        [Authorize(Roles = "KP")]
        [HttpPost]
        [ApiVersion("1.0")]
       // public async Task<ActionResult> UpdateTrialMarkWorkFlowStatus(long ScriptId)
        public async Task<ActionResult> UpdateTrialMarkWorkFlowStatus(TrailmarkingModel TrialMarkedScripts)
        {
            string result = string.Empty;
            try
            {
                result = await _trialMarkingPoolService.UpdateTrialMarkWorkFlowStatus(GetCurrentProjectId(), TrialMarkedScripts);
                ////result = await _trialMarkingPoolService.UpdateTrialMarkWorkFlowStatus(GetCurrentProjectId(), ScriptId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrialMarkingPoolController Page while updating UpdateTrialMarkWorkFlowStatus : Method Name : UpdateTrialMarkWorkFlowStatus()");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.SAVE,
                    Entity = AuditTrailEntity.STANDARDISATION,
                    Module = AuditTrailModule.TRIALMRKNG,
                    Remarks = TrialMarkedScripts,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "P001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        [Route("script/bands/{scriptid}")]
        [Authorize(Roles = "KP")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ActionResult> GetScriptQuestionBandInformation(long ScriptId)
        {
            try
            {
                return Ok(await _trialMarkingPoolService.GetScriptQuestionBandInformation(GetCurrentProjectId(), ScriptId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrialMarkingPoolController Page while fetching getScriptQuestionBandInformation : Method Name : getScriptQuestionBandInformation()");
                throw;
            }
        }
    }
}
