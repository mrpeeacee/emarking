using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Privilege;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Project.Privilege
{
    /// <summary>
    /// Project privileges api controller
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/privileges/")]
    public class PrivilegesController : BaseApiController<PrivilegesController>
    {
        private readonly IPrivilegeService _privilegeService;

        public PrivilegesController(IPrivilegeService privilegeService, ILogger<PrivilegesController> _logger, AppOptions appOptions) : base(appOptions, _logger)
        {
            _privilegeService = privilegeService;
        }

        [Route("{Type}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize]
        public async Task<ActionResult> GetUserPrivileges(int Type = 1)
        {
            logger.LogDebug($"PrivilegesController > GetUserPrivileges() started. ProjectId={GetCurrentProjectId()} and Type={Type}");

            try
            {
                logger.LogDebug($"PrivilegesController > GetUserPrivileges() completed. ProjectId={GetCurrentProjectId()} and Type={Type}");

                return Ok(await _privilegeService.GetUserPrivileges(Type, GetCurrentProjectUserRoleID(), GetCurrentUserId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Trial Marking Page while fetching GetUserPrivileges : Method Name: GetUserPrivileges()");
                throw;
            }
        }
    }
}
