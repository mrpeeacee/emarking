using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.Reports
{
    /// <summary>
    /// Qig Api
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/reports")]
    [ApiVersion("1.0")]
    public class ViewSolutionController : BaseApiController<ViewSolutionController>
    {
        private readonly IViewSolutionService viewSolutionService;
        private readonly IAuthService AuthService;
        /// <summary>
        /// QIG constructor
        /// </summary>
        /// <param name="viewSolutionService"></param>
        /// <param name="_logger"></param>
        /// <param name="appOptions"></param>
        /// <param name="_authService"></param>
        public ViewSolutionController(IViewSolutionService viewSolutionService, ILogger<ViewSolutionController> _logger, AppOptions appOptions, IAuthService _authService) : base(appOptions, _logger)
        {
            this.viewSolutionService = viewSolutionService;
            AuthService = _authService;
        }

        /// <summary>
        /// Method to get all qig for specific project
        /// </summary>
        /// <returns></returns> 

        [Authorize(Roles = "EO,AO,CM")]
        [Route("solution/{UserId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetUserScheduleDetails(long UserId)
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                return Ok(await viewSolutionService.GetUserScheduleDetails(projectId, UserId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigController Page while getting all Qigs : Method Name : Get() and ProjectID ={projectId}, UserId = {UserId}");
                throw;
            }
        }

        [Authorize(Roles = "EO,AO,CM,ACM,SUPERADMIN,EM,SERVICEADMIN,MARKER,TL,ATL")]
        [Route("userresponse/{UserId}/{Isfrommarkingplayer}/{Testcetreid}/{reportstatus}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetUserResponse(long UserId, bool Isfrommarkingplayer,long Testcetreid, bool reportstatus = true)
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                return Ok(await viewSolutionService.GetUserResponse(projectId, UserId, Isfrommarkingplayer, Testcetreid, reportstatus));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigController Page while getting all Qigs : Method Name : Get() and ProjectID = {projectId}, UserId = {UserId}, Testcetreid = {UserId}, reportstatus = {reportstatus}");
                throw;
            }
        }
        [Authorize(Roles = "EO,AO,SUPERADMIN,EM,SERVICEADMIN")]
        [Route("userresponse")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetUserResponses(SearchFilterModel searchFilterModel)
        {
            int MaskingRequired = 0;
            int PreOrPostMarking = 1;
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                return Ok(await viewSolutionService.GetUserResponses(GetCurrentProjectId(), MaskingRequired, PreOrPostMarking, searchFilterModel));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ViewSolutionController Page while getting all Qigs : Method Name : GetUserResponses()");
                throw;
            }
        }

        /// <summary>
        /// Get Schools By Project ID.
        /// </summary>
        /// <returns></returns>

        [Authorize(Roles = "EO,AO,SUPERADMIN,EM,SERVICEADMIN")]
        [Route("schools")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetSchools()
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await viewSolutionService.GetSchools(GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ViewSolutionController Page while getting all Qigs : Method Name : GetSchools()");
                throw;
            }
        }
    }
}
