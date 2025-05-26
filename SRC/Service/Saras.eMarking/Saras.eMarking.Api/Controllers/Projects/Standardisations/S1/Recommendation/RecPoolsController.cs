using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Recommendation;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.Standardisations.S1.Recommendation
{
    /// <summary>
    ///  Standardisation recommendation pool controller
    /// </summary> 

    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/standardisation/s1/recommendation/rec-pools")]
    public class RecPoolsController : BaseApiController<RecPoolsController>
    {
        private readonly IRecPoolService _recPoolService;
        private readonly IAuthService AuthService;

        public RecPoolsController(IRecPoolService recPoolservice, ILogger<RecPoolsController> _logger, AppOptions appOptions, IAuthService _authService) : base(appOptions, _logger)
        {
            _recPoolService = recPoolservice;
            AuthService = _authService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("statistics/{qigid}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "KP")]
        public async Task<IActionResult> GetRecPoolStastics(long QIGID)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QIGID, true))
                {
                    return new ForbidResult();
                }
                return Ok(await _recPoolService.GetRecPoolStastics(GetCurrentProjectId(), QIGID, CurrentUserContext.CurrentRole));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in RecPoolController Page while getting all untagged and my scripts for QIG : Method Name: GetRecPoolStastics() and QIGID = {QIGID}");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("scripts/{QIGID}/{ScriptId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "KP")]
        public async Task<IActionResult> GetRecPoolScript(long QIGID, long ScriptId = 0)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QIGID, true)
                    || (ScriptId > 0 && !AuthService.IsValidProjectQigScript(GetCurrentProjectId(), QIGID, ScriptId)))
                {
                    return new ForbidResult();
                }
                return Ok(await _recPoolService.GetRecPoolScript(GetCurrentProjectId(), QIGID, CurrentUserContext.CurrentRole, ScriptId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in RecPoolController Page while getting all untagged and my scripts for QIG : Method Name: GetRecPoolScript() and QIGID = {QIGID}");
                throw;
            }
        }

    }
}
