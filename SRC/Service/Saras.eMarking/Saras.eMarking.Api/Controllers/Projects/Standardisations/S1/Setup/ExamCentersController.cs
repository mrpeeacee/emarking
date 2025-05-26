using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.Standardisations.S1.Setup
{

    /// <summary>
    ///  Standardisation setup exam center controller
    /// </summary> 

    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/s1/setup/exam-centers")]
    public class ExamCentersController : BaseApiController<ExamCentersController>
    {
        private readonly IExamCenterService _examCenterService;
        private readonly IAuthService AuthService;

        public ExamCentersController(IExamCenterService examCenterservice, IAuthService _authService, ILogger<ExamCentersController> _logger, AppOptions appOptions, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            _examCenterService = examCenterservice;
            AuthService = _authService;
        }

        /// <summary>
        /// ProjectCenters : This GET Api is used to get the project centers
        /// </summary> 
        /// <param name="QigId">Qig
        /// Id</param>
        /// <returns></returns>
        [Route("{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO, CM")]
        public async Task<IActionResult> ProjectCenters(long QigId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId, true))
                {
                    return new ForbidResult();
                }
                return Ok(await _examCenterService.ProjectCenters(GetCurrentProjectId(), QigId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ExamCenterController page while getting Centers for specific Project : Method Name : ProjectCenters()");
                throw;
            }
        }

        /// <summary>
        /// UpdateProjectCenters : This POST Api is used to Update the Project Centers
        /// </summary>
        /// <param name="objExamCenterModel">objExamCenter Model</param> 
        /// <param name="QigId">Qig Id</param> 
        /// <returns></returns>
        [Route("{QigId}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO, CM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProjectCenters(List<ExamCenterModel> objExamCenterModel, long QigId)
        {
            string status = string.Empty;
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Required fields cannot not be empty");
                    return BadRequest(ModelState);
                }
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId, true))
                {
                    return new ForbidResult();
                }
                status = await _examCenterService.UpdateProjectCenters(objExamCenterModel, GetCurrentProjectUserRoleID(), GetCurrentProjectId(), QigId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ExamCenterController page while updating ProjectCenters : Method Name : UpdateProjectCenters()");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.SETUP,
                    Entity = AuditTrailEntity.STANDARDISATION,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = new ExamCenterActionModel { ExamCenterModel = objExamCenterModel, QigId = QigId },
                    ResponseStatus = status == "UP001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(status)
                });
                #endregion
            }
        }
    }
}
