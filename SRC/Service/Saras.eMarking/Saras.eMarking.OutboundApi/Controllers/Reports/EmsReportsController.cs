using Microsoft.AspNetCore.Mvc;
using Saras.eMarking.Api.Controllers;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.Domain.ViewModels.Report;

namespace Saras.eMarking.OutboundApi.Controllers.Reports
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/reports/ems")]
    public class EmsReportsController : BaseApiController<EmsReportsController>
    {
        private readonly IEmsReportService emsReportService;
        public EmsReportsController(IEmsReportService _emsReportService, ILogger<EmsReportsController> _logger, AppOptions appOptions) : base(appOptions, _logger)
        {
            emsReportService = _emsReportService;
        }

        /// <summary>
        /// Get ESM1 Report
        /// </summary> 
        /// <returns>
        /// ESM1 Report
        /// </returns> 
        [Route("1")]
        [HttpGet]
        [ProducesResponseType(typeof(PaginationModel<Ems1ReportModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEms1Report(string subjectCode, string paperCode, string moaCode, string examSeriesCode, string examLevelCode, long examYear, int pageSize = 0, int pageIndex = 1)
        {
            try
            {
                PaginationModel<Ems1ReportModel>? result = null;
                logger.LogInformation("Data Event", $"EMSReportController > GetEms1Report() started.");

                long projectId = await emsReportService.GetProjectId(subjectCode, paperCode, moaCode, examSeriesCode, examLevelCode, examYear);

                if (projectId > 0)
                {
                    result = await emsReportService.GetEms1Report(projectId, 1, null, pageSize, pageIndex);
                }
                logger.LogInformation("Data Event", $"EMSReportController > GetEms1Report() completed.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in EMSReportController > GetEms1Report().", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get ESM2 Report
        /// </summary> 
        /// <returns>ESM2 Report</returns> 
        [Route("2")]
        [HttpGet]
        [ProducesResponseType(typeof(PaginationModel<Ems2ReportModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEms2Report(string subjectCode, string paperCode, string moaCode, string examSeriesCode, string examLevelCode, long examYear, int pageSize = 0, int pageIndex = 1)
        {
            try
            {
                PaginationModel<Ems2ReportModel>? result = null;
                logger.LogInformation("Data Event", $"EMSReportController > GetEms2Report() started.");

                long projectId = await emsReportService.GetProjectId(subjectCode, paperCode, moaCode, examSeriesCode, examLevelCode, examYear);

                if (projectId > 0)
                {
                    result = await emsReportService.GetEms2Report(projectId, 1, null, pageSize, pageIndex);
                }

                logger.LogInformation("Data Event", $"EMSReportController > GetEms2Report() completed.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in EMSReportController > GetEms2Report().", ex.Message);
                throw;
            }
        }
        /// <summary>
        /// Get ESM1 Report
        /// </summary> 
        /// <returns>
        /// ESM1 Report
        /// </returns>  
        [Route("1")]
        [HttpPost]
        [ProducesResponseType(typeof(PaginationModel<Ems1ReportModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SyncReportToiExam(string subjectCode, string paperCode, string moaCode, string examSeriesCode, string examLevelCode, long examYear, int pageSize = 0, int pageIndex = 1)
        {
            try
            {
                Ems1ReportModel result = null;
                PaginationModel<Ems1ReportModel>? res = null;
                logger.LogInformation("Data Event", $"EMSReportController > GetEms1Report() started.");

                long projectId = await emsReportService.GetProjectId(subjectCode, paperCode, moaCode, examSeriesCode, examLevelCode, examYear);
                if (projectId > 0)
                {
                    res = await emsReportService.GetEms1Report(projectId, 2, null, pageSize, pageIndex, true);
                    
                }

                result = await emsReportService.SyncReportToiExam(res);

                logger.LogInformation("Data Event", $"EMSReportController > GetEms1Report() completed.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in EMSReportController > GetEms1Report().", ex.Message);
                throw;
            }
        }

    }
}
