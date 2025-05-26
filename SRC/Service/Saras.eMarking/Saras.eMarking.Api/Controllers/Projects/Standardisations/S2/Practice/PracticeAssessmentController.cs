using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Project.Standardisation.Practice
{
    /// <summary>
    /// Standardisation practice assessment controller
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/standardisation/practice")]
    [ApiVersion("1.0")]
    public class PracticeAssessment1Controller : BaseApiController<PracticeAssessment1Controller>
    {
        readonly IPracticeAssessmentService _PracticeAssessmentService;
        private readonly IAppCache AppCache;
        readonly IAuthService AuthService;

        public PracticeAssessment1Controller(IPracticeAssessmentService practiceassessmentService, ILogger<PracticeAssessment1Controller> _logger, AppOptions appOptions, IAppCache _appCache, IAuthService _authService) : base(appOptions, _logger)
        {
            AppCache = _appCache;
            _PracticeAssessmentService = practiceassessmentService;
            AuthService = _authService;
        }

        /// <summary>
        /// Get Get S2 Practice Script
        /// </summary> 
        ///<param name="QigID"></param>
        /// <returns></returns>
        [Route("s2/script/{QigID}")]
        [Authorize(Roles = "TL,ATL")]
        [HttpGet]
        public async Task<IActionResult> GetS2PracticeScript(long QigID)
        {
            logger.LogInformation($"PracticeAssessment1Controller > GetS2PracticeScript() started. QigId = {QigID}");

            var result = await GetPracticeScript(QigID);
             
            logger.LogInformation($"PracticeAssessment1Controller > GetS2PracticeScript() completed. QigId = {QigID}");

            return result;
        }

        /// <summary>
        /// Get S3 Practice Script
        /// </summary> 
        ///<param name="QigID"></param>
        /// <returns></returns>
        [Route("s3/script/{QigID}")]
        [Authorize(Roles = "MARKER")]
        [HttpGet]
        public async Task<IActionResult> GetS3PracticeScript(long QigID)
        {
            logger.LogInformation($"PracticeAssessment1Controller > GetS3PracticeScript() started. QigId = {QigID}");

            var result = await GetPracticeScript(QigID);

            logger.LogInformation($"PracticeAssessment1Controller > GetS3PracticeScript() Completed. QigId = {QigID}");

            return result;
        }

        private async Task<IActionResult> GetPracticeScript(long QigID)
        {
            try
            {
                logger.LogInformation($"PracticeAssessment1Controller > GetPracticeScript() started. QigId = {QigID}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigID, false))
                {
                    return new ForbidResult();
                }

                var result = await _PracticeAssessmentService.GetStandardisationScript(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigID, AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Practice, EnumWorkflowType.Script));

                logger.LogInformation($"PracticeAssessment1Controller > GetPracticeScript() completed. QigId = {QigID}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in  PracticeAssessment1Controller page while getting  standardisation script  remarks : Method Name : GetS3PracticeScript() and project: ProjectID=" + GetCurrentProjectId() + ", ProjectUserRoleID=" + GetCurrentProjectUserRoleID() + ", QigID=" + QigID.ToString() + ", WorkflowStatusID =" + EnumWorkflowStatus.Practice + "");
                throw;
            }
        }

        /// <summary>
        /// S2 Practice Question Details
        /// </summary>
        /// <param name="QigID"></param>
        /// <param name="ScriptID"></param>
        /// <param name="iscompleted"></param>
        /// <param name="userRoleId(Optional)"></param> 
        /// <returns></returns>
        //[Authorize(Roles = "AO,TL,ATL")]
        [Route("s2/{QigID}/{ScriptID}/questions/{iscompleted}/{userRoleId}")]
        [HttpGet]
        public async Task<IActionResult> S2PracticeQuestionDetails(long QigID, long ScriptID, bool iscompleted, long userRoleId = 0)
        {
            logger.LogInformation($"PracticeAssessment1Controller > S2PracticeQuestionDetails() started. QigId = {QigID} and ScriptID = {ScriptID} and IsCompleted = {iscompleted} and UserRoleId = {userRoleId}");

            if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigID)
                    || (userRoleId > 0 && !AuthService.IsValidProjectQig(GetCurrentProjectId(), userRoleId, QigID)))
            {
                return new ForbidResult();
            }

            var result = await PracticeQuestionDetails(QigID, ScriptID, iscompleted, userRoleId);

            logger.LogInformation($"PracticeAssessment1Controller > S2PracticeQuestionDetails() completed. QigId = {QigID} and ScriptID = {ScriptID} and IsCompleted = {iscompleted} and UserRoleId = {userRoleId}");

            return result;
        }

        /// <summary>
        /// Get S3 Practice Question Details
        /// </summary>
        /// <param name="QigID"></param>
        /// <param name="ScriptID"></param>
        /// <param name="iscompleted"></param>
        /// <returns></returns>
        //[Authorize(Roles = "AO,MARKER")]
        [Route("s3/{QigID}/{ScriptID}/questions/{iscompleted}")]
        [HttpGet]
        public async Task<IActionResult> S3PracticeQuestionDetails(long QigID, long ScriptID, bool iscompleted)
        {
            logger.LogInformation($"PracticeAssessment1Controller > S3PracticeQuestionDetails() started. QigId = {QigID} and ScriptID = {ScriptID} and IsCompleted = {iscompleted}");

            if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigID))
            {
                return new ForbidResult();
            }

            var result = await PracticeQuestionDetails(QigID, ScriptID, iscompleted);

            logger.LogInformation($"PracticeAssessment1Controller > S3PracticeQuestionDetails() started. QigId = {QigID} and ScriptID = {ScriptID} and IsCompleted = {iscompleted}");

            return result;
        }



        private async Task<IActionResult> PracticeQuestionDetails(long QigID, long ScriptID, bool iscompleted, long userRoleId = 0)
        {
            try
            {
                logger.LogInformation($"PracticeAssessment1Controller > PracticeQuestionDetails() started. QigId = {QigID} and ScriptID = {ScriptID} and IsCompleted = {iscompleted} and UserRoleId = {userRoleId}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), userRoleId <= 0 ? GetCurrentProjectUserRoleID() : userRoleId, QigID, false)
                    || (userRoleId > 0 && !AuthService.IsValidProjectQig(GetCurrentProjectId(), userRoleId, QigID)))
                {
                    return new ForbidResult();
                }

                var result = await _PracticeAssessmentService.GetPracticeQuestionDetails(GetCurrentProjectId(), userRoleId <= 0 ? GetCurrentProjectUserRoleID() : userRoleId, QigID, ScriptID, iscompleted);

                logger.LogInformation($"PracticeAssessment1Controller > PracticeQuestionDetails() started. QigId = {QigID} and ScriptID = {ScriptID} and IsCompleted = {iscompleted} and UserRoleId = {userRoleId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in  PracticeAssessment1Controller page while getting  practice questions details remarks : Method Name : S3PracticeQuestionDetails() and project: ProjectID=" + GetCurrentProjectId() + ", ProjectUserRoleID=" + GetCurrentProjectUserRoleID() + ", QigID=" + QigID.ToString() + ", ScriptID =" + ScriptID.ToString() + "");
                throw;
            }
        }
    }
}
