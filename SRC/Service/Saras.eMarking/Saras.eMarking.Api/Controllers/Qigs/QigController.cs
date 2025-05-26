using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Qig
{
    /// <summary>
    /// Qig Api
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/public/v{version:apiVersion}/qig")]
    [ApiVersion("1.0")]
    public class QigController : BaseApiController<QigController>
    {
        private readonly IQigService qigService;
        private readonly IAuthService AuthService;
        /// <summary>
        /// QIG constructor
        /// </summary>
        /// <param name="qigService"></param>
        /// <param name="_logger"></param>
        /// <param name="appOptions"></param>
        /// <param name="_authService"></param>
        /// <param name="_auditService"></param>
        public QigController(IQigService qigService, ILogger<QigController> _logger, AppOptions appOptions, IAuthService _authService, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            this.qigService = qigService;
            AuthService = _authService;
        }

        /// <summary>
        /// Method to get all qig for specific project
        /// </summary>
        /// <returns></returns> 
        [Route("/api/public/v{version:apiVersion}/qig")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> Get()
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                return Ok(await qigService.GetAllQIGs(GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigController Page while getting all Qigs : Method Name : Get() and ProjectID = " + GetCurrentProjectId());
                throw;
            }
        }

        /// <summary>
        /// GetQIGs : This GET Api is used to get Qigs
        /// </summary>
        /// <param name="iskp"></param>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        [Route("/api/v{version:apiVersion}/project/qig-tab/{iskp?}/{ProjectId?}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ActionResult> GetQIGs(bool? iskp = null, long ProjectId = 0)
        {
            try
            {
                if (!AuthService.IsValidProject(ProjectId != 0 ? ProjectId : GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                return Ok(await qigService.GetQIGs(ProjectId != 0 ? ProjectId : GetCurrentProjectId(), GetCurrentProjectUserRoleID(), iskp, 0));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigController Page while fetching GetQIGs : Method Name : GetQIGs()");
                throw;
            }
        }


        /// <summary>
        /// GetQuestions : This GET Api is used to get the Questions tagged to Qig
        /// </summary> 
        /// <param name="qigid"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/qig/{qigid}/question")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetQuestions(long qigid)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigid))
                {
                    return new ForbidResult();
                }
                return Ok(await qigService.GetAllQigQuestions(GetCurrentProjectId(), qigid));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigController Page while getting Qig Questions : Method Name : GetQuestions() and ProjectID = " + GetCurrentProjectId() + ", QigId = " + qigid.ToString());
                throw;
            }
        }

        /// <summary>
        /// UpdateQigSetting : This POST Api is used to update the qig setting
        /// </summary>
        /// <param name="objQigModel"></param> 
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPatch]
        [ApiVersion("1.0")]
        [Route("/api/public/v{version:apiVersion}/qig")]
        public async Task<IActionResult> UpdateQigSetting(QigModel objQigModel)
        {
            bool Status = false;
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), objQigModel.QigId))
                {
                    return new ForbidResult();
                }
                Status = await qigService.UpdateQigSetting(objQigModel, GetCurrentProjectId(), GetCurrentProjectUserRoleID());
                return Ok(Status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigController Page while update Qig Settings : Method Name : UpdateQigSetting() and ProjectID = " + GetCurrentProjectId());
                throw;
            }

        }

        /// <summary>
        /// Getqigworkflowtracking : This GET Api is used to get the qig workflow tracking
        /// </summary>
        /// <param name="entityid"></param>
        /// <param name="entitytype"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/qig/workflowtracking/{entityid}/{entitytype}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> Getqigworkflowtracking(long entityid, EnumAppSettingEntityType entitytype)
        {
            try
            {
                if (entityid != 0 && !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), entityid))
                {
                    return new ForbidResult();

                }

                long projectId = GetCurrentProjectId();
                return Ok(await qigService.GetQigWorkflowTracking(projectId, entityid, entitytype));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigController while getting qigworkflowstatus : Method Name : GetQigWorkflowTracking() and  EntityID = " + entityid.ToString() + "EntityType = " + entitytype);
                throw;
            }
        }

    }
}
