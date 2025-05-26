using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Dashboard;
using Saras.eMarking.Domain.ViewModels.Dashboard;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Dashboard
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/dashboard")]
    public class DashboardController : BaseApiController<DashboardController>
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> _logger, AppOptions appOptions) : base(appOptions, _logger)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get project stastics for specific marker
        /// </summary> 
        /// <returns></returns> 
        [Route("statistics")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> Statistics()
        {
            logger.LogDebug($"ErrorController > Method Name: ErrorAsync() started. ProjectId={GetCurrentProjectId()}");

            try
            {
                logger.LogDebug($"ErrorController > Method Name: ErrorAsync() completed. ProjectId={GetCurrentProjectId()}");

                return Ok(await _dashboardService.GetProjectStatistics(GetCurrentUserId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while Get project stastics for specific marker in Home page. Method : GetProjectStatistics()");
                throw;
            }
        }

        /// <summary>
        /// List the exam year
        /// </summary>
        /// <returns></returns>
        [Route("examyear")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> ListAllExamYear()
        {
            logger.LogDebug($"ErrorController > Method Name: ListAllExamYear() started. ProjectId={GetCurrentProjectId()}");

            try
            {
                logger.LogDebug($"ErrorController > Method Name: ListAllExamYear() completed. ProjectId={GetCurrentProjectId()}");

                return Ok(await _dashboardService.ListAllExamYear());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while get all Exam years. Method : ListAllExamYear()");
                throw;
            }
        }

        /// <summary>
        /// List the projects assigned for specific marker
        /// </summary> 
        /// <returns></returns>
        [Route("projects/{Year?}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> ListProjects(int Year = 0)
        {
            logger.LogDebug($"ErrorController > Method Name: ListProjects() started. ProjectId={GetCurrentProjectId()} and Year={Year} ");

            try
            {
                long ProjectId = GetCurrentProjectId();
               
                logger.LogDebug($"ErrorController > Method Name: ListProjects() completed. ProjectId={GetCurrentProjectId()} and Year={Year} ");
                //CurrentUserContext.

                return Ok(await _dashboardService.GetAll(GetCurrentUserId(), GetCurrentContextTimeZone(), Year, ProjectId,GetCurrentProjectUserRoleCode()));
          
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while get all activities created in Home page. Method : GetAll() and TokenID : {GetCurrentUserId()} and {GetCurrentContextTimeZone()} and {Year}");
                throw;
            }
        }

        /// <summary>
        /// To Get Project Status For MARKINGPERSONNEL
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        [Route("ProjectStatus/{ProjectId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> ProjectStatus(int ProjectId)
        {
            ProjectStatusDetails ProjectStatus = new ProjectStatusDetails();
            logger.LogDebug($"ErrorController>MethodName:ProjectStatus() started.ProjectId={GetCurrentProjectId()}");
            try
            {
                logger.LogDebug($"ErrorController>Method Name:ProjectStatus() completed.ProjectId={GetCurrentProjectId()}");

                ProjectStatus = _dashboardService.GetProjectStatus(ProjectId, GetCurrentContextTimeZone());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while get ProjectStatus. Method : ProjectStatus() and ProjectId={GetCurrentProjectId()}");

                throw;
            }
            await Task.CompletedTask;
            return Ok(ProjectStatus);
        }
    }
}
