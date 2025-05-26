using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.ViewModels.AuditModels.Modules.Standardisation;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using System.Text.Json;
using Saras.eMarking.Domain.ViewModels.AuditModels;

namespace Saras.eMarking.Api.Controllers.Projects.Standardisations.S1.Setup
{
    /// <summary>
    ///  Standardisation one setup key personnel controller
    /// </summary> 
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/s1/setup/key-personnels")]
    public class KeyPersonnelsController : BaseApiController<KeyPersonnelsController>
    {
        private readonly IKeyPersonnelService _keyPersonnelService;
        
        private readonly IAuthService AuthService;

        public KeyPersonnelsController(IKeyPersonnelService keyPersonnelservice, IAuthService _authService, ILogger<KeyPersonnelsController> _logger, AppOptions appOptions, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            _keyPersonnelService = keyPersonnelservice;
           
            AuthService = _authService;
        }

        /// <summary>
        /// ProjectKps : This GET Api is used to get the project kps
        /// </summary>
        /// <param name="QigId">Qig Id</param>
        /// <returns></returns>       
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO, CM")]
        [Route("{QigId}")]
        public async Task<IActionResult> ProjectKps(long QigId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId, true))
                {
                    return new ForbidResult();
                }

                return Ok(await _keyPersonnelService.ProjectKps(GetCurrentProjectId(), QigId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in KeyPersonnelController page while getting ProjectKp's for specific Project : Method Name : ProjectKps()");
                throw;
            }
        }

        /// <summary>
        /// UpdateKeyPersonnels : This POST Api is used to update the the key peronnels
        /// </summary>
        /// <param name="objKeyPersonnelModel">objKeyPersonnelModel</param> 
        /// <param name="QigId">Qig Id</param> 
        /// <returns>result</returns>      
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO, CM")]
        [ValidateAntiForgeryToken]
        [Route("{QigId}")]
        public async Task<IActionResult> UpdateKeyPersonnels(List<KeyPersonnelModel> objKeyPersonnelModel, long QigId)
        {
            string result = string.Empty;
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId, true))
                {
                    return new ForbidResult();
                }
                result = await _keyPersonnelService.UpdateKeyPersonnels(objKeyPersonnelModel, GetCurrentProjectUserRoleID(), GetCurrentProjectId(), QigId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in KeyPersonnelController page while updating Key Personnels : Method Name : UpdateKeyPersonnels()");
                throw;
            }
            finally
            {
                List<StandardisationSetUpAction> lt = new List<StandardisationSetUpAction>();

                foreach (var item in objKeyPersonnelModel)
                {
                    StandardisationSetUpAction ssua = new StandardisationSetUpAction();
                    ssua.QigId = QigId;
                    
                    ssua.RoleName = item.RoleName;
                    ssua.RoleCode = item.RoleCode;
                    ssua.IsKP = item.IsKP;
                    lt.Add(ssua);
                }
                 #region  Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.SETUP,
                    Entity = AuditTrailEntity.STANDARDISATION,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "UP002" ? AuditTrailResponseStatus.keypersonalupdateCompleted : AuditTrailResponseStatus.Keypersonalupdatefailed,
                    Remarks = lt,
                    Response = JsonSerializer.Serialize(result)
                    #endregion
                });
            }
        }

    }
}
