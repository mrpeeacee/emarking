using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Categorisation;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Categorisation;

namespace Saras.eMarking.Api.Controllers.Projects.Standardisations.S1.Categorisation
{
    /// <summary>
    /// Standardisation Categorisation apis 
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/s1/categorisation-pools")]
    [ApiVersion("1.0")]
    // [Authorize]
    public class CategorisationPoolsController : BaseApiController<CategorisationPoolsController>
    {

        private readonly ICategorisationPoolService _categorisationService;
        private readonly IAuthService AuthService;

        /// <summary>
        /// Categorisation constructor
        /// </summary>
        /// <param name="categorisationService"></param>
        /// <param name="_logger"></param>
        /// <param name="appOptions"></param>
        /// <param name="_auditService"></param>
        /// <param name="_authService"></param>
        public CategorisationPoolsController(ICategorisationPoolService categorisationService, ILogger<CategorisationPoolsController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            _categorisationService = categorisationService;
            AuthService = _authService;
        }

        /// <summary>
        /// GetCategorisationStatistics : This GET Api is used to get script categorisation statistics
        /// </summary>
        /// <param name="qigId">Qig id</param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("{qigId}/statistics")]
        [HttpGet]
        public async Task<IActionResult> GetCategorisationStatistics(long qigId)
        {
            try
            {

                logger.LogInformation($"CategorisationPoolController > GetCategorisationStatistics() started. QigId = {qigId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigId, true))
                {
                    return new ForbidResult();
                }

                CategorisationStasticsModel stasticsModel = await _categorisationService.GetCategorisationStatistics(qigId, GetCurrentProjectUserRoleID());

                logger.LogInformation($"CategorisationPoolController > GetCategorisationStatistics() completed. QigId = {qigId}");

                return Ok(stasticsModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolController > GetCategorisationStatistics(). QigId = {qigId}");
                throw;
            }
        }

        /// <summary>
        /// GetCategorisationScripts : This GET Api is used to get trial marked script for categorisation and filter the same
        /// </summary>
        /// <param name="qigId">Qig Id</param>
        /// <param name="poolTypes">Pool type for filter</param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("scripts/{qigId}")]
        [Route("scripts/{qigId}/filter")]
        [Route("scripts/{qigId}/{searchValue}")]
        [Route("scripts/{qigId}/{searchValue}/filter")]
        [HttpGet]
        public async Task<IActionResult> GetCategorisationScripts(long qigId,string searchValue ="" ,[FromQuery] int[] poolTypes = null)
        {
            try
            {
                logger.LogInformation($"CategorisationPoolController > GetCategorisationScripts() started. QigId = {qigId}, poolTypes = {poolTypes}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigId, true))
                {
                    return new ForbidResult();

                }

                IList<CategorisationModel> res = await _categorisationService.GetCategorisationScripts(qigId,GetCurrentProjectUserRoleID(), GetCurrentProjectUserRoleCode(), searchValue, poolTypes);

                logger.LogInformation($"CategorisationPoolController > GetCategorisationScripts() completed. QigId = {qigId}, poolTypes = {poolTypes}");

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolController > GetCategorisationScripts(). QigId = {qigId}, poolTypes = {poolTypes}");
                throw;
            }
        }

        /// <summary>
        /// GetTrialMarkedScript : This GET Api is used to get Trial marked response for specific script
        /// </summary>
        /// <param name="scriptId">Script Id</param>
        /// <param name="qigId">Qig Id</param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("{qigId}/trial-marked-script/{scriptId}/{UserScriptMarkingRefID}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrialMarkedScript(long scriptId, long qigId, long UserScriptMarkingRefID = 0)
        {
            try
            {
                logger.LogInformation($"CategorisationPoolController > GetTrialMarkedScript() started. ScriptId = {scriptId} and QigId = {qigId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigId, true)
                    || !AuthService.IsValidProjectQigScript(GetCurrentProjectId(), qigId, scriptId))
                {
                    return new ForbidResult();
                }

                CategorisationTrialMarkModel1 res = await _categorisationService.GetTrialMarkedScript(scriptId, GetCurrentProjectUserRoleID(), qigId, UserScriptMarkingRefID);

                logger.LogInformation($"CategorisationPoolController > GetTrialMarkedScript() completed. ScriptId = {scriptId}");


                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolController > GetTrialMarkedScript(). ScriptId = {scriptId} and QigId = {qigId}");
                throw;
            }

        }

        /// <summary>
        /// IsQigInQualifying : This Method is used to validate script is in Qualifying assessment of not
        /// </summary>
        /// <param name="qigId"> qig Id </param>
        /// <param name="scriptId">script Id</param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("qualified/{qigId}/{scriptId}")]
        [HttpGet]
        public async Task<IActionResult> IsQigInQualifying(long qigId, long scriptId)
        {
            try
            {
                logger.LogInformation($"CategorisationPoolController > IsQigInQualifying() started. QigId = {qigId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigId, true)
                    || !AuthService.IsValidProjectQigScript(GetCurrentProjectId(), qigId, scriptId))
                {
                    return new ForbidResult();
                }

                bool IsQigInQualified = await _categorisationService.IsQigInQualifying(qigId, GetCurrentProjectUserRoleID(), scriptId);

                logger.LogInformation($"CategorisationPoolController > IsQigInQualifying() completed. QigId = {qigId}");

                return Ok(IsQigInQualified);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolController > IsQigInQualifying(). QigId = {qigId}");
                throw;
            }
        }

        /// <summary>
        /// CatagoriseScript : This PATCH Api is used to Categorise the script
        /// </summary>
        /// <param name="categoriseModel">categorise Model</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("categorise")]
        [HttpPatch]
        public async Task<IActionResult> CatagoriseScript(CategoriseAsModel categoriseModel)
        {
            string status = string.Empty;
            try
            {
                if (categoriseModel is not null)
                {
                    logger.LogInformation($"CategorisationPoolController > CatagoriseScript() started. CategoriseAsModel = {categoriseModel}");

                    if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), categoriseModel.QigId, true) || !AuthService.IsValidProjectQigScript(GetCurrentProjectId(), categoriseModel.QigId, categoriseModel.ScriptId))
                    {
                        return new ForbidResult();
                    }

                    status = await _categorisationService.CatagoriseScript(categoriseModel, GetCurrentProjectUserRoleID());

                    logger.LogInformation($"CategorisationPoolController > CatagoriseScript() completed. CategoriseAsModel = {categoriseModel}");
                }
                else
                {
                    logger.LogDebug($"CategorisationPoolController > CatagoriseScript() categoriseModel is null.");
                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolController > CatagoriseScript(). CategoriseAsModel = {categoriseModel}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.SAVE,
                    Entity = AuditTrailEntity.STANDARDISATION,
                    Module = AuditTrailModule.CATEGORISATION,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = categoriseModel,
                    ResponseStatus = status == "SU001" || status == "UNCATS" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(status)
                });
                #endregion
            }
        }

        /// <summary>
        /// ReCategoriseScript : This PATCH Api is used to re categorise the scripts
        /// </summary>
        /// <param name="categoriseModel">categorise Model</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("re-categorise")]
        [HttpPatch]
        public async Task<IActionResult> ReCategoriseScript(CategoriseAsModel categoriseModel)
        {
            string status = string.Empty;
            try
            {
                logger.LogInformation($"CategorisationPoolController > ReCategoriseScript() started. CategoriseAsModel = {categoriseModel}");

                if (categoriseModel is not null)
                {
                    if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), categoriseModel.QigId, true) || !AuthService.IsValidProjectQigScript(GetCurrentProjectId(), categoriseModel.QigId, categoriseModel.ScriptId))
                    {
                        return new ForbidResult();
                    }

                    status = await _categorisationService.ReCategoriseScript(categoriseModel, GetCurrentProjectUserRoleID());

                    logger.LogInformation($"CategorisationPoolController > ReCategoriseScript() completed. CategoriseAsModel = {categoriseModel}");
                }
                else
                {
                    logger.LogDebug($"CategorisationPoolController > ReCategoriseScript() categoriseModel is null.");
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolController > ReCatagoriseScript(). CategoriseAsModel = {categoriseModel}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.RECATEGORISATION,
                    Entity = AuditTrailEntity.STANDARDISATION,
                    Module = AuditTrailModule.CATEGORISATION,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = categoriseModel,
                    ResponseStatus = status == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(status)
                });
                #endregion
            }
        }

        /// <summary>
        /// IsScriptCategorised : This GET Api is used to Check the script is categorsed or not
        /// </summary>
        /// <param name="qigId">qig Id</param>
        /// <param name="scriptid">script id</param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("{qigId}/categorised/{scriptid}")]
        [HttpGet]
        public async Task<IActionResult> IsScriptCategorised(long qigId, long scriptid)
        {
            try
            {
                logger.LogInformation($"CategorisationPoolController > IsScriptCategorised() started. QigId = {qigId}, scriptid = {scriptid}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigId, true)
                    || !AuthService.IsValidProjectQigScript(GetCurrentProjectId(), qigId, scriptid))
                {
                    return new ForbidResult();
                }

                bool IsQigInQualified = await _categorisationService.IsScriptCategorised(qigId, scriptid, GetCurrentProjectUserRoleID());

                logger.LogInformation($"CategorisationPoolController > IsScriptCategorised() completed. QigId = {qigId}, scriptid = {scriptid}");

                return Ok(IsQigInQualified);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolController > IsScriptCategorised(). QigId = {qigId}, scriptid = {scriptid}");
                throw;
            }
        }
    }

}
