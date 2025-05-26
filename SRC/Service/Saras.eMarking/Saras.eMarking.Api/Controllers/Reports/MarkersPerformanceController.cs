using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Reports
{
    /// <summary>
    /// Markers Performance report Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/reports/markers-performance")]
    public class MarkersPerformanceController : BaseApiController<MarkersPerformanceController>
    {

        private readonly IMarkersPerformanceService markersPerformanceService;
        private readonly IQigService qigService;

        public MarkersPerformanceController(IMarkersPerformanceService _markersPerformanceService, IQigService qigService, ILogger<MarkersPerformanceController> _logger, AppOptions appOptions) : base(appOptions, _logger)
        {
            markersPerformanceService = _markersPerformanceService;
            this.qigService = qigService;
        }

        /// <summary>
        /// Get school list
        /// </summary> 
        /// <returns></returns> 
        [Route("SchoolList/{ProjectId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetSchoolDetails(long ProjectId)
        {
            try
            {
                logger.LogInformation($"MarkersPerformanceController > GetSchoolDetails() started. Project = {ProjectId}");

                List<SchoolDetails> result = await markersPerformanceService.GetSchoolDetails(ProjectId);

                logger.LogInformation($"MarkersPerformanceController > GetSchoolDetails() completed. Project = {ProjectId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkersPerformanceController > GetSchoolDetails(). Project = {ProjectId}");
                throw;
            }
        }
        /// <summary>
        /// Get count of marker perfomance details
        /// </summary> 
        /// <returns></returns> 
        [Route("MarkerSchoolReport")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetMarkerPerformanceDetails(MarkerDetails markerDetails)
        {
            try
            {
                logger.LogInformation($"MarkersPerformanceController > GetMarkerPerformanceDetails() started. ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");

                MarkerPerformanceStatistics result = await markersPerformanceService.GetMarkerPerformanceDetails(markerDetails);

                logger.LogInformation($"MarkersPerformanceController > GetMarkerPerformanceDetails() completed. ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkersPerformanceController > GetMarkerPerformanceDetails(). ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");
                throw;
            }
        }

        /// <summary>
        /// Get marker performance list
        /// </summary> 
        /// <returns></returns> 
        [Route("MarkerDetailsList")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetMarkerPerformance(MarkerDetails markerDetails)
        {
            try
            {
                logger.LogInformation($"MarkersPerformanceController > GetMarkerPerformance() started. ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");

                List<MarkerPerformance> result1 = await markersPerformanceService.GetMarkerPerformance(markerDetails);

                logger.LogInformation($"MarkersPerformanceController > GetMarkerPerformance() completed. ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");

                return Ok(result1);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkersPerformanceController > GetMarkerPerformance(). ProjectId = {markerDetails.ProjectId} and QigId = {markerDetails.QigID} and ExamYear = {markerDetails.ExamYear} and MarkerName = {markerDetails.MarkerName} and SchoolCode = {markerDetails.SchoolCode}");
                throw;
            }
        }

        [Route("Qigs/{ProjectId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ActionResult> GetQIGs(long ProjectId)
        {
            try
            {
                bool? iskp = null;

                return Ok(await qigService.GetQIGs(ProjectId, GetCurrentProjectUserRoleID(), iskp, 1));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigController Page while fetching GetQIGs : Method Name : GetQIGs()");
                throw;
            }
        }
    }
}
