
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;

using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using System;
using System.Threading.Tasks;


namespace Saras.eMarking.Api.Controllers.Reports
{
    [Authorize]
    [ApiController]
    [Route("/api/public/v{version:apiVersion}/reports/testcentrewiseresponse")]
    [ApiVersion("1.0")]
    public class TestcenterwiseresponseReportController : BaseApiController<TestcenterwiseresponseReportController>
    {
        private readonly ITestcenterReportService TestcenterReportService;

        public TestcenterwiseresponseReportController(ITestcenterReportService _TestcenterReportService, ILogger<TestcenterwiseresponseReportController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger)
        {
            TestcenterReportService = _TestcenterReportService;
        }


        [Route("projectcenters")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> ProjectCenters()
        {
            try
            {
                return Ok(await TestcenterReportService.ProjectCenters(GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TestcenterReportController page while getting Centers for specific Project : Method Name : ProjectCenters()");
                throw;
            }
        }
        [Route("getquestiondetails/{questionid}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> getquestiondetails(long questionid)
        {
            try
            {
                return Ok(await TestcenterReportService.getquestiondetails(questionid, GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TestcenterReportController page while getting Centers for specific Project : Method Name : getquestiondetails()");
                throw;
            }
        }
    }
}
