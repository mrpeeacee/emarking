using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Setup.ResolutionOfCoi;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Project.Setup
{
    /// <summary>
    /// Resolution of Conflict of Interest(col) apis
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/resolution-of-coi")]
    [ApiVersion("1.0")]
    public class ResolutionOfCoiController : BaseApiController<ResolutionOfCoiController>
    {
        readonly IResolutionOfCoiService ResolutionOfCoiService;

        readonly IAuthService AuthService;
        public ResolutionOfCoiController(IResolutionOfCoiService _resolutionOfCoiService, ILogger<ResolutionOfCoiController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            ResolutionOfCoiService = _resolutionOfCoiService;

            AuthService = _authService;
        }
        /// <summary>
        /// GetResolutionCOI : This GET Api is used to get all conflict of Interest  schools
        /// </summary> 
        /// <returns></returns>
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        [Route("markers")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetResolutionCOI()
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ResolutionOfCoiService.GetResolutionCOI(GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ResolutionOfCoiController page while getting markers, default school name and Exception schools for specific Project: Method Name: GetResolutionCOI() and QIGId=");
                throw;
            }
        }
        /// <summary>
        /// GetSchoolsCOI : This GET Api is used to get all schools preset in emarking
        /// </summary> 
        /// <returns></returns>
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        [Route("schools")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetSchoolsCOI()
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ResolutionOfCoiService.GetSchoolsCOI(GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ResolutionOfCoiController page while getting schools for specific Project: Method Name: GetSchoolsCOI() and QIGId=");
                throw;
            }
        }
        /// <summary>
        /// UpdateResolutionCOI : This POST Api is used to Update Resolution of Conflict of Interest(Coi) schools
        /// </summary>
        /// <param name="COI"></param>
        /// <param name="ProjectUserRoleID"></param>
        /// <returns></returns>
        [Route("markers/{ProjectUserRoleID}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UpdateResolutionCOI(List<CoiSchoolModel> COI, long ProjectUserRoleID)
        {
            string result = string.Empty;
            try
            {

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProject(GetCurrentProjectId(), ProjectUserRoleID))
                {
                    return new ForbidResult();
                }
                result = await ResolutionOfCoiService.UpdateResolutionCOI(COI, ProjectUserRoleID, GetCurrentProjectUserRoleID(), GetCurrentProjectId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ResolutionOfCoiController page while getting markers, default school name and Exception schools for specific Project : Method Name : UpdateResolutionCOIs()");
                throw;
            }


            finally
            {
                #region Insert Audit Trail

                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.CREATE,
                    Module = AuditTrailModule.RCOI,
                    Entity = AuditTrailEntity.CoI,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = COI,
                    ResponseStatus = result == "UP001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion 
            }
        }
    }
}
