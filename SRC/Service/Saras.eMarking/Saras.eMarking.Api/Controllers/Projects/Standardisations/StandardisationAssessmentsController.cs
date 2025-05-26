using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;

namespace Saras.eMarking.Api.Controllers.Project.Standardisation
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/standardisation/assessments")]
    public class StandardisationAssessmentsController : BaseApiController<StandardisationAssessmentsController>
    {
        private readonly IStdAssessmentService _stdAssessmentService;
        private readonly IAuthService AuthService;
        public StandardisationAssessmentsController(IStdAssessmentService stdAssessmentService, ILogger<StandardisationAssessmentsController> _logger, IAuthService _authService, AppOptions appOptions) : base(appOptions, _logger)
        {
            _stdAssessmentService = stdAssessmentService;
            AuthService = _authService;
        }


        [Authorize(Roles = "ATL,TL")]
        [Route("s2/is-enable/{QigID}")]
        [ApiVersion("1.0")]
        [HttpGet]
        public async Task<IActionResult> IsS2PracticeEnable(long QigID)
        {
            try
            {
                return await IsPracticeQualifyingEnable(QigID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in  StdAssessmentController page while getting  practice enable details remarks : Method Name : IsS2PracticeEnable() and project : ProjectID=" + GetCurrentProjectId() + ", QigID=" + QigID.ToString());
                throw;
            }
        }

        private async Task<IActionResult> IsPracticeQualifyingEnable(long qigID)
        {
            if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigID))
            {
                return new ForbidResult();
            }
            return Ok(await _stdAssessmentService.IsPracticeQualifyingEnable(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigID));
        }

        [Authorize(Roles = "ATL,TL")]
        [Route("s2/status/{QigID}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> S2AssessmentStatus(long QigID)
        {
            try
            {
                return await GetAssessmentStatus(QigID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in StdAssessmentController page while getting  Assessment Status  remarks : Method Name : S2AssessmentStatus() and project : ProjectID=" + GetCurrentProjectId() + ", ProjectUserRoleID=" + GetCurrentProjectUserRoleID() + ",  QigID=" + QigID.ToString());
                throw;
            }
        }

        private async Task<IActionResult> GetAssessmentStatus(long qigID, long ProjectId = 0, long ProjectUserRoleID = 0)
        {

            if (ProjectId == 0)
            {
                ProjectId = GetCurrentProjectId();
            }
            if (ProjectUserRoleID > 0)
            {
                if (!AuthService.IsValidProjectQigUser(ProjectId, ProjectUserRoleID, qigID))
                {
                    return new ForbidResult();
                }
            }
            else 
            {
                ProjectUserRoleID = GetCurrentProjectUserRoleID();
                if (!AuthService.IsValidProjectQig(ProjectId, ProjectUserRoleID, qigID))
                {
                    return new ForbidResult();
                }
            }

            return Ok(await _stdAssessmentService.AssessmentStatus(ProjectId, ProjectUserRoleID, qigID));
        }

        [Authorize(Roles = "MARKER")]
        [Route("s3/is-enable/{QigID}")]
        [ApiVersion("1.0")]
        [HttpGet]
        public async Task<IActionResult> IsS3PracticeEnable(long QigID)
        {
            try
            {
                return await IsPracticeQualifyingEnable(QigID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in  StdAssessmentController page while getting  practice enable details remarks : Method Name : IsS3PracticeEnable() and project : ProjectID=" + GetCurrentProjectId() + ", QigID=" + QigID.ToString());
                throw;
            }
        }

        [Authorize(Roles = "MARKER")]
        [Route("s3/status/{QigID}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> S3AssessmentStatus(long QigID)
        {
            try
            {
                return await GetAssessmentStatus(QigID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in StdAssessmentController page while getting  Assessment Status  remarks : Method Name : S3AssessmentStatus() and project : ProjectID=" + GetCurrentProjectId() + ", ProjectUserRoleID=" + GetCurrentProjectUserRoleID() + ",  QigID=" + QigID.ToString());
                throw;
            }
        }

        [Route("s3/status/assessmentstatus/{QigID}/{ProjectUserRoleId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> AssessmentStatus(long QigID, long ProjectUserRoleID)
        {
            try
            {
                return await GetAssessmentStatus(QigID, 0, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in StdAssessmentController page while getting  Assessment Status  remarks : Method Name : S3AssessmentStatus() and project : ProjectID=" + GetCurrentProjectId() + ", ProjectUserRoleID=" + GetCurrentProjectUserRoleID() + ",  QigID=" + QigID.ToString());
                throw;
            }
        }
    }
}
