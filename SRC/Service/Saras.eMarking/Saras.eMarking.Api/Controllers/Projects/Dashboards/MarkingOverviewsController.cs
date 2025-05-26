using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Dashboards;
using Saras.eMarking.Domain.ViewModels.Project.Dashboards;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.Dashboards
{
    /// <summary>
    /// Marking overview controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/projects/dashboards/overview/")]
    public class MarkingOverviewsController : BaseApiController<MarkingOverviewsController>
    {

        private readonly IMarkingOverviewsService markingOverviewsService;
        readonly IAuthService AuthService;
        public MarkingOverviewsController(IMarkingOverviewsService _markingOverviewsService, ILogger<MarkingOverviewsController> _logger, AppOptions appOptions, IAuthService _authService) : base(appOptions, _logger)
        {
            markingOverviewsService = _markingOverviewsService;
            AuthService = _authService;
        }

        /// <summary>
        /// Api to get script marking overviews
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <returns></returns>

        [Route("all/{QigId}")]
        [Route("all/{QigId}/{ProjectUserRoleId}")]
        [HttpGet]
        public async Task<IActionResult> GetAllOverView(long QigId, long ProjectUserRoleId = 0)
        {
            try
            {
                logger.LogDebug($"MarkingOverviewsController > Method Name: GetAllOverView() started. ProjectId={GetCurrentProjectId()} and QigId={QigId} and ProjectUserRoleId={ProjectUserRoleId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                if (ProjectUserRoleId > 0 && (!AuthService.IsValidProject(GetCurrentProjectId(), ProjectUserRoleId)
                   || !AuthService.IsValidProjectQig(GetCurrentProjectId(), ProjectUserRoleId, QigId)))
                {
                    return new ForbidResult();
                }

                MarkingOverviewsModel result = await markingOverviewsService.GetAllOverView(QigId, ProjectUserRoleId == 0 ? GetCurrentProjectUserRoleID() : ProjectUserRoleId, GetCurrentContextTimeZone());

                logger.LogDebug($"MarkingOverviewsController > Method Name: GetAllOverView() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId} and ProjectUserRoleId={ProjectUserRoleId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkingOverviewsController > GetAllOverView()");
                throw;
            }
        }

        /// <summary>
        /// Get Qig Standardisation overview
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        [Route("{QigId}/standardisations")]
        [HttpGet]
        public async Task<IActionResult> GetStandardisationOverview(long QigId)
        {
            try
            {
                logger.LogDebug($"MarkingOverviewsController > Method Name: GetStandardisationOverview() started. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }

                StandardisationOverviewModel result = await markingOverviewsService.GetStandardisationOverview(QigId, GetCurrentProjectUserRoleID());

                logger.LogDebug($"MarkingOverviewsController > Method Name: GetStandardisationOverview() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkingOverviewsController > GetStandardisationOverview()");
                throw;
            }
        }

        /// <summary>
        /// Get standardisation approval details.
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        [Route("{QigId}/approvals")]
        [HttpGet]
        public async Task<IActionResult> GetStandardisationApprovals(long QigId)
        {
            try
            {
                logger.LogDebug($"MarkingOverviewsController > Method Name: GetStandardisationApprovals() started. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }

                StandardisationApprovalCountsModel result = await markingOverviewsService.StandardisationApprovalCounts(QigId, GetCurrentProjectUserRoleID(), GetCurrentProjectId());

                logger.LogDebug($"MarkingOverviewsController > Method Name: GetStandardisationApprovals() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkingOverviewsController > GetStandardisationApprovals()");
                throw;
            }
        }

        /// <summary>
        /// Get live pool overview.
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        [Route("{QigId}/live-pool")]
        [HttpGet]
        public async Task<IActionResult> GetLivePoolOverview(long QigId)
        {
            try
            {
                logger.LogDebug($"MarkingOverviewsController > Method Name: GetLivePoolOverview() started. ProjectId={GetCurrentProjectId()} and QigId={QigId}");
                
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }

                LiveMarkingOverviewsModel result = await markingOverviewsService.GetLivePoolOverview(QigId, GetCurrentProjectUserRoleID(), GetCurrentProjectId());

                logger.LogDebug($"MarkingOverviewsController > Method Name: GetLivePoolOverview() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkingOverviewsController > GetLivePoolOverview()");
                throw;
            }
        }
    }
}
