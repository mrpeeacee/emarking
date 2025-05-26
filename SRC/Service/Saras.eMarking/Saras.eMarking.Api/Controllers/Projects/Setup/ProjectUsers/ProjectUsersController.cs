using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.ProjectUsers;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.Setup.ProjectUsers
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/project-users")]
    [ApiVersion("1.0")]
    public class ProjectUsersController : BaseApiController<ProjectUsersController>
    {
        readonly IProjectUsersService ProjectUsersService;
        private readonly IAuthService AuthService;
        public ProjectUsersController(IAuthService _authService, IProjectUsersService _projectUsersService, ILogger<ProjectUsersController> _logger, IAuditService _auditService, AppOptions appOptions) : base(appOptions, _logger, _auditService)
        {
            ProjectUsersService = _projectUsersService;
            AuthService = _authService;
        }

        /// <summary>
        /// Userscount : This API method is to Get Project users data view
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>      
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("{QigId}")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> Userscount(long? QigId)
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();

                logger.LogDebug($"ProjectUsersController > Userscount() started. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                if (QigId > 0 && !AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), (long)QigId))
                {
                    return new ForbidResult();
                }

                logger.LogDebug($"ProjectUsersController > Userscount() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                return Ok(await ProjectUsersService.Userscount(projectId, QigId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting Project user count for specific project:Method Name:Projectusercount() projectId = {projectId}");
                throw;
            }
        }

        /// <summary>
        /// GetProjectUserslist : This API method is to Get Project users list
        /// </summary>
        /// <returns></returns>       
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("project/userview")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetProjectUserslist()
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();

                logger.LogDebug($"ProjectUsersController > Userscount() started. ProjectId={projectId}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                logger.LogDebug($"ProjectUsersController > Userscount() completed. ProjectId={projectId}");

                return Ok(await ProjectUsersService.GetProjectUserslist(projectId, GetCurrentContextTimeZone()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting Project users data view for specific project:Method Name:GetProjectUserslist() projectId = {projectId}");
                throw;
            }
        }

        /// <summary>
        /// GetQiguserDatalist : This API method is to Get Qig users data list
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="FilteredBy"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("dataorHierarchy_view")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQiguserDatalist(QiguserDataviewModel qiguserdataviewmodel, string FilteredBy = "All")
        {
            List<QiguserDataviewModel> result = null;
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();

                logger.LogDebug($"ProjectUsersController > Userscount() started. ProjectId={projectId} and QigId={qiguserdataviewmodel.QIGId} and (Optional)FilteredBy={FilteredBy} and (Optional)ProjectUserRoleId={qiguserdataviewmodel.ProjectUserRoleID}");

                if (!AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), qiguserdataviewmodel.QIGId))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.GetQiguserDatalist(projectId, qiguserdataviewmodel.QIGId, qiguserdataviewmodel.ProjectUserRoleID, FilteredBy, GetCurrentContextTimeZone());

                logger.LogDebug($"ProjectUsersController > Userscount() completed. ProjectId={projectId} and QigId={qiguserdataviewmodel.QIGId} and (Optional)FilteredBy={FilteredBy} and (Optional)ProjectUserRoleId={qiguserdataviewmodel.ProjectUserRoleID}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting Qig users Data view for specific Qig:Method Name:GetQiguserDatalist() and ProjectID: ProjectID = {projectId}");
                throw;
            }

            finally
            {
                ModelQig newModel = new ModelQig
                {
                    QigId = qiguserdataviewmodel.QIGId
                };
                if (qiguserdataviewmodel.ProjectUserRoleID != 0)
                {
                    #region Insert Audit Trail
                    _ = InsertAuditLogs(new AuditTrailData
                    {
                        Event = AuditTrailEvent.DOWNLOAD,
                        Module = AuditTrailModule.QIGTEAMMANAGEMENT,
                        Entity = AuditTrailEntity.USER,
                        Remarks = qiguserdataviewmodel,
                        ResponseStatus = (result != null && result.Count != 0) ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                        Response = JsonSerializer.Serialize(result)
                    });
                    #endregion
                }


            }

        }

       

        /// <summary>
        /// QigUsersImportFile : This API method is to Qig users import file
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>

        [Route("qig/file_upload/{QigId}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> QigUsersImportFile(long QigId, string ProjectName)
        {
            List<QigUserModel> result = null;
            try
            {
                logger.LogDebug($"ProjectUsersController > Userscount() started. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                IFormFile file = Request.Form.Files[0];
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.QigUsersImportFile(file, QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID());

                logger.LogDebug($"ProjectUsersController > Userscount() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Importing Qig users:Method Name:QigUsersImportFile().  QigId = {QigId}");
                throw;
            }
            finally
            {
                importModel impMod = new importModel();
                {                    
                    impMod.QigId = QigId;
                    impMod.ProjectName = ProjectName;
                };
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.IMPORT,
                    Module = AuditTrailModule.QIGTEAMMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    Remarks = impMod,
                    ResponseStatus = (result != null && result.Count == 0) ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// GetQiguserDataById : This API method is to Get Qig User Details by Id
        /// </summary>
        /// <returns></returns>  
        [Route("qig/{QigId}/{ProjectQIGTeamHierarchyID}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQiguserDataById(long QigId, long ProjectQIGTeamHierarchyID)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }

                return Ok(await ProjectUsersService.GetQiguserDataById(GetCurrentProjectId(), QigId, ProjectQIGTeamHierarchyID));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Getting Qig users:Method Name:GetQiguserDataById() QigId = {QigId}, ProjectQIGTeamHierarchyID = {ProjectQIGTeamHierarchyID}");
                throw;
            }
        }

        /// <summary>
        /// UpdateQiguserDataById : This API method is to Update Qig User Details by Id
        /// </summary>
        /// <returns></returns>  
        [Route("qig/{QigId}/{ProjectQIGTeamHierarchyID}/{ReportingToId}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UpdateQiguserDataById(long QigId, long ProjectQIGTeamHierarchyID, long ReportingToId)
        {
            string result = string.Empty;
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.UpdateQiguserDataById(GetCurrentProjectUserRoleID(), ProjectQIGTeamHierarchyID, ReportingToId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Getting Qig users: Method Name:UpdateQiguserDataById() QigId = {QigId}, ProjectQIGTeamHierarchyID = {ProjectQIGTeamHierarchyID}, ReportingToId = {ReportingToId}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.QIGTEAMMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    Remarks = new UpdateQiguserDataByIdModel { QigId = QigId, ProjectQIGTeamHierarchyID = ProjectQIGTeamHierarchyID, ReportingToId = ReportingToId },
                    ResponseStatus = result == "U001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// DeleteUsers : This API method is to User Delete.
        /// </summary>
        /// <returns></returns>  
        [Route("qig/delete-users/{UserRoleId}/{QigId}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> DeleteUsers(long UserRoleId, long QigId)
        {
            long projectId = 0;
            projectId = GetCurrentProjectId();
            string result = string.Empty;
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }

                result = await ProjectUsersService.DeleteUsers(UserRoleId, QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID());
                logger.LogDebug($"QigManagementController > UpdateMandatoryQuestion() End. ProjectId = {projectId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Deleting users:Method Name:DeleteUsers() and ProjectID: QigId = {QigId}");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.DELETE,
                    Module = AuditTrailModule.PROJECTSETUP,
                    Entity = AuditTrailEntity.USER,
                    Remarks = UserRoleId,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }

        }


        /// <summary>
        /// Get Qig has marking team 
        /// </summary>
        /// <returns></returns>  
        [Route("qig/BlankQigs")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetBlankQigIds()
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.GetBlankQigIds(GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Getting Blank QigIds :Method Name:GetBlankQigIds()");
                throw;
            }
        }

        /// <summary>
        /// UsersRoles : This API method is to get Roles List
        /// </summary>
        /// <returns></returns>  
        [Route("qig/user-roles")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UsersRoles()
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.UsersRoles(GetCurrentProjectUserRoleID()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting users roles:Method Name:UsersRoles()");
                throw;
            }
        }

        /// <summary>
        /// CreateUser : This API method is to create Single User
        /// </summary>
        /// <returns></returns>  
        [Route("qig/createuser")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> CreateUser(CreateUserModel createUserModel)
        {
            long projectId = 0;
            string result = string.Empty;
            try
            {
                projectId = GetCurrentProjectId();
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.CreateUser(createUserModel, projectId, GetCurrentUserId(), GetCurrentProjectUserRoleID());
                logger.LogDebug($"ProjectUsersController > CreateUser() completed. ProjectId = {projectId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while cteating user:Method Name:CreateUser()");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.CREATE,
                    Module = AuditTrailModule.PROJECTSETUP,
                    Entity = AuditTrailEntity.USER,
                    Remarks = createUserModel,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }


        /// <summary>
        /// Move MarkingTeam from Existing Qig to EmptyQig
        /// </summary>
        /// <returns></returns>  
        [Route("qig/MoveMarkingTeamToEmptyQig")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> MoveMarkingTeamToEmptyQig(MoveMarkingTeamToEmptyQig moveMarkingTeamToEmptyQig)
        {
            long projectId = 0;
            string result = string.Empty;
            try
            {
                projectId = GetCurrentProjectId();
                moveMarkingTeamToEmptyQig.ProjectID = GetCurrentProjectId();
                moveMarkingTeamToEmptyQig.ProjectUserRoleId = GetCurrentProjectUserRoleID();

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), moveMarkingTeamToEmptyQig.FromQigId)
                    || !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), moveMarkingTeamToEmptyQig.ToQigId))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.MoveMarkingTeamToEmptyQig(moveMarkingTeamToEmptyQig);
                logger.LogDebug($"QigManagementController > MoveMarkingTeamToEmptyQig() End. ProjectId = {projectId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while  page while move marking team to empty qig :Method Name:MoveMarkingTeamToEmptyQig()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.COPY,
                    Module = AuditTrailModule.QIGTEAMMANAGEMENT,
                    Entity = AuditTrailEntity.QIG,
                    Remarks = moveMarkingTeamToEmptyQig,
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion

            }

        }

        /// <summary>
        /// Is S1 Started Or LiveMarkingEnabled for specific QIg
        /// </summary>
        /// <returns></returns>  
        [Route("qig/s1orlivestarted/{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> CheckS1StartedOrLiveMarkingEnabled(long QigId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.CheckS1StartedOrLiveMarkingEnabled(QigId, GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while checking S1StartedOrLiveMarkingEnabled :Method Name:CheckS1StartedOrLiveMarkingEnabled()");
                throw;
            }
        }


        /// <summary>
        /// Get Qig has marking team 
        /// </summary>
        /// <returns></returns>  
        [Route("qig/EmptyQigs")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetEmptyQigIds()
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.GetEmptyQigIds(GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Getting Blank QigIds :Method Name:GetBlankQigIds()");
                throw;
            }
        }

        /// <summary>
        /// Move MarkingTeam from Existing Qig to EmptyQig
        /// </summary>
        /// <returns></returns>  
        [Route("qig/MoveMarkingTeamToEmptyQigIds")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> MoveMarkingTeamToEmptyQigs(MoveMarkingTeamToEmptyQigs moveMarkingTeamToEmptyQig)
        {
            string result = string.Empty;
            try
            {
                moveMarkingTeamToEmptyQig.ProjectID = GetCurrentProjectId();
                moveMarkingTeamToEmptyQig.ProjectUserRoleId = GetCurrentProjectUserRoleID();


                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), moveMarkingTeamToEmptyQig.FromQigId))

                {
                    return new ForbidResult();
                }

                result = await ProjectUsersService.MoveMarkingTeamToEmptyQigs(moveMarkingTeamToEmptyQig);
                logger.LogDebug($"ProjectUsersService > MoveMarkingTeamToEmptyQigs() End. ProjectId = {moveMarkingTeamToEmptyQig}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while move marking team to empty qig :Method Name:MoveMarkingTeamToEmptyQigs()");
                throw;
            }
            finally
            {

                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.MOVE,
                    Module = AuditTrailModule.QIGTEAMMANAGEMENT,
                    Entity = AuditTrailEntity.QIG,
                    Remarks = moveMarkingTeamToEmptyQig,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }


        }

        /// <summary>
        /// Unblock project Users.
        /// </summary>
        /// <returns></returns>  
        [Route("Unblocking/{UserId}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UnBlockUsers(long UserId)
        {
            long currentprojectuserroleId = 0;
            string result = string.Empty;
            try
            {
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.UnBlockUsers(currentprojectuserroleId, UserId);
                logger.LogDebug($"ProjectUsersController > UnBlockUsers() started. UserId = {UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Getting UnBlocking Users:Method Name:UnBlockUsers()");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.BLOCKUSER,
                    Module = AuditTrailModule.PROJECTSETUP,
                    Entity = AuditTrailEntity.USER,
                    UserId = UserId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = "blockeduser: " + UserId,
                    ResponseStatus = result == "U001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("mappedusers")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> MappedUsers(SearchFilterModel searchFilterModel)
        {
            try
            {
                string currentloginrolecode = GetCurrentProjectUserRoleCode();
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.MappedUsers(searchFilterModel, GetCurrentProjectId(), currentloginrolecode, GetCurrentContextTimeZone()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while getting MappedUsers in the Projects. :Method Name:MappedUsers()");
                throw;
            }

        }

        /// <summary>
        /// For Edit a particular User
        /// </summary>
        /// <returns></returns>
        [Route("selectedmappedusers/{UserId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetSelectedMappedUsers(long UserId)
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.GetSelectedMappedUsers(UserId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while getting MappedUsers in the Projects. :Method Name:MappedUsers()");
                throw;
            }
        }


        /// <summary>
        /// List of UnMapded Users. 
        /// </summary>
        /// <returns></returns>
        [Route("Unmappedusers")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UnMappedUsers(SearchFilterModel searchFilterModel)
        {
            long ProjectId = 0;
            try
            {
                string currentloginrolecode = GetCurrentProjectUserRoleCode();
                ProjectId = GetCurrentProjectId();
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                return Ok(await ProjectUsersService.UnMappedUsers(searchFilterModel, currentloginrolecode, GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while getting UnMappedUsers in the Projects. :Method Name:UnMappedUsers()");
                throw;
            }


        }

        /// <summary>
        /// Save UnMapped User.
        /// </summary>
        /// <returns></returns>
        [Route("push-mappedusers")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> SaveUnMappedUsers(MappedUsersModel mappedUsersModel)
        {
            long ProjectId = 0;
            string result = string.Empty;
            long UserId = 0;
            long currentprojectuserroleId = GetCurrentProjectUserRoleID();
            try
            {
                ProjectId = GetCurrentProjectId();
                UserId = GetCurrentUserId();
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.SaveUnMappedUsers(mappedUsersModel, ProjectId, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while saving UnMapped User for a project.:Method Name:SaveUnMappedUsers()");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.MAP,
                    Module = AuditTrailModule.PROJECTUSERMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    UserId = UserId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = mappedUsersModel,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }


        /// <summary>
        /// Save UnMapped User.
        /// </summary>
        /// <returns></returns>
        [Route("unmapped-ao")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> SaveUnMappedAO(MappedUsersModel mappedUsersModel)
        {
            long UserId = 0;
            long currentprojectuserroleId = 0;
            string result = string.Empty;

            try
            {
                currentprojectuserroleId = GetCurrentProjectId();
                UserId = GetCurrentUserId();
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.SaveUnMappedAO(mappedUsersModel, currentprojectuserroleId, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while saving MapAO User for a project.:Method Name:SaveUnMappedAO()");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.MAP,
                    Module = AuditTrailModule.PROJECTUSERMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    UserId = UserId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = mappedUsersModel,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }

        }

        /// <summary>
        /// UnMapping Users.  
        /// </summary>
        /// <returns></returns>
        [Route("UnMappingParticularUsers")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UnMappingParticularUsers(UnMapAodetails UnmapAodetail)
        {
            long ProjectId = 0;
            long UserId = 0;
            long currentprojectuserroleId = 0;
            string result = string.Empty;

            try
            {
                ProjectId = GetCurrentProjectId();
                UserId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();

                result = await ProjectUsersService.UnMappingParticularUsers(UnmapAodetail, ProjectId, currentprojectuserroleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while getting UnMappedUsers in the Projects. :Method Name:UnMappingParticularUsers()");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UNMAP,
                    Module = AuditTrailModule.PROJECTUSERMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    UserId = UserId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = UnmapAodetail,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }


        /// <summary>
        /// Get Roles
        /// </summary>
        /// <returns></returns>
        [Route("Roles")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.GetRoles());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while fetching the Roles User for a project.:Method Name:GetRoles()");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("blockorunblock/{UserRoleId}/{QigId}/{Isactive}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> BlockorUnblockUserQig(long UserRoleId, long QigId, bool Isactive)
        {
            long currentprojectuserroleId = 0;
            long userId = 0;
            string result = string.Empty;

            try
            {
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                userId = GetCurrentUserId();
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.BlockorUnblockUserQig(QigId, currentprojectuserroleId, UserRoleId, Isactive);
                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Blocking or UnBlocking Users:Method Name:BlockorUnblockUserQig()");
                throw;
            }

            finally
            {
                if (Isactive)
                {
                    #region Insert Audit Trail
                    _ = InsertAuditLogs(new AuditTrailData
                    {
                        Event = AuditTrailEvent.BLOCKUSER,
                        Module = AuditTrailModule.PROJECTSETUP,
                        Entity = AuditTrailEntity.USER,
                        UserId = userId,
                        ProjectUserRoleID = currentprojectuserroleId,
                        Remarks = new BlockorUnblockUserQig { UserRoleId = UserRoleId, QigId = QigId, Isactive = Isactive },
                        ResponseStatus = result == "BLK001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                        Response = JsonSerializer.Serialize(result)
                    });
                }
                else
                {
                    _ = InsertAuditLogs(new AuditTrailData
                    {
                        Event = AuditTrailEvent.SAVE,
                        Module = AuditTrailModule.PROJECTSETUP,
                        Entity = AuditTrailEntity.USER,
                        UserId = userId,
                        ProjectUserRoleID = currentprojectuserroleId,
                        Remarks = new BlockorUnblockUserQig { UserRoleId = UserRoleId, QigId = QigId, Isactive = Isactive },
                        ResponseStatus = result == "BLK002" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                        Response = JsonSerializer.Serialize(result)
                    });
                }
                #endregion
            }
        }

        /// <summary>
        /// GetWithDrawUsers : This API is used to get the users.
        /// </summary>
        /// <param name="objectSearch"></param>
        /// <returns></returns>
        [Route("GetWithDrawUsers")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetWithDrawUsers(SearchFilterModel objectSearch)
        {
            try
            {

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.GetWithDrawUsers(GetCurrentProjectId(), objectSearch));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while fetching the WithDrawUsers User for a project.:Method Name:GetWithDrawUsers()");
                throw;
            }
        }



        /// <summary>
        ///  WithDrawUsers : This POST Api is used to Withdraw the users.
        /// </summary>
        /// <param name="ObjectUserWithdraw"></param>
        /// <returns></returns>
        [Route("WithDrawUsers")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> WithDrawUsers(List<UserWithdrawModel> ObjectUserWithdraw)
        {
            long projectId = 0;
            long currentprojectuserroleId = 0;
            long currentuserId = 0;
            string result = string.Empty;
            try
            {

                projectId = GetCurrentProjectId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                currentuserId = GetCurrentUserId();

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.WithDrawUsers(ObjectUserWithdraw, GetCurrentProjectUserRoleID());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while fetching the WithDrawUsers User for a project.:Method Name:WithDrawUsers()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.WITHDRAW,
                    Module = AuditTrailModule.CANDIDATEWITHDRAW,
                    Entity = AuditTrailEntity.USER,
                    UserId = currentuserId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = ObjectUserWithdraw,
                    ResponseStatus = result == "success" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result),
                    SessionId = GetCurrentSessionKey()
                });
                #endregion
            }
        }

        /// <summary>
        ///  Get Suspend Users
        /// </summary>
        /// <returns></returns>
        [Route("qig/suspend-users/{currentprojectuserroleid}/{SuspendOrUnmap}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetSuspendUserDetails(long currentprojectuserroleid, int SuspendOrUnmap)
        {
            try
            {

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.GetSuspendUserDetails(currentprojectuserroleid, SuspendOrUnmap));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while fetching the GetSuspendUserDetails.:Method Name:GetSuspendUserDetails()");
                throw;
            }
        }

        /// <summary>
        ///  Un Tag Qig User
        /// </summary>
        /// <returns></returns>
        [Route("qig/un-tag-qig-users")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UnTagQigUser(SuspendUserDetails suspendUserDetails)
        {
            string result = string.Empty;
            try
            {
                suspendUserDetails.ProjectId = GetCurrentProjectId();
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), suspendUserDetails.QigId))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.UnTagQigUser(suspendUserDetails, GetCurrentProjectUserRoleID());
                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while untaggingh qig users:Method Name:UnTagQigUser()");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UNTAGUSERS,
                    Module = AuditTrailModule.QIGTEAMMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    Remarks = suspendUserDetails,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result),
                    SessionId = GetCurrentSessionKey()
                });
                #endregion
            }
        }

        /// <summary>
        ///  Get Upper Hierarchy Role
        /// </summary>
        /// <returns></returns>
        [Route("qig/un-tag-qig-users/{RoleId}/{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetUpperHierarchyRole(long RoleId, long QigId)
        {
            try
            {

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.GetUpperHierarchyRole(RoleId, QigId, GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while fetching the GetUpperHierarchyRole.:Method Name:GetUpperHierarchyRole()");
                throw;
            }
        }

        /// <summary>
        ///  Re Tag Qig User
        /// </summary>
        /// <returns></returns>
        [Route("qig/re-tag-qig-users/{RoleId}/{QigId}/{ReportingTo}/{RoleCode}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> ReTagQigUser(long RoleId, long QigId, long ReportingTo, string RoleCode)
        {
            string result = string.Empty;
            try
            {

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                result = await ProjectUsersService.ReTagQigUser(RoleId, QigId, ReportingTo, RoleCode, GetCurrentProjectId(), GetCurrentProjectUserRoleID());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while untaggingh qig users:Method Name:UnTagQigUser()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.RETAG,
                    Module = AuditTrailModule.QIGTEAMMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = new ReTag { RoleId = RoleId, QigId = QigId, ReportingTo = ReportingTo, RoleCode = RoleCode },
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result),

                });
                #endregion

            }
        }

        /// <summary>
        ///  Get Re Tag Upper Hierarchy Role
        /// </summary>
        /// <returns></returns>
        [Route("qig/re-tag-qig-users/{RoleId}/{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetReTagUpperHierarchyRole(long RoleId, long QigId)
        {
            try
            {

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.GetReTagUpperHierarchyRole(RoleId, QigId, GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while fetching the GetReTagUpperHierarchyRole.:Method Name:GetReTagUpperHierarchyRole()");
                throw;
            }
        }

        /// <summary>
        ///  Get Reporting To Hierarchy
        /// </summary>
        /// <returns></returns>
        [Route("qig/re-tag-reporting/{RoleId}/{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetReportingToHierarchy(long RoleId, long QigId)
        {
            try
            {

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.GetReportingToHierarchy(RoleId, QigId, GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while fetching the GetUpperHierarchyRole.:Method Name:GetUpperHierarchyRole()");
                throw;
            }
        }

        /// <summary>
        ///  Untaguserhaschildusers
        /// </summary>
        /// <returns></returns>
        [Route("qig/tag-qig-users-exits/{RoleId}/{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,EO,,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> Untaguserhaschildusers(long RoleId, long QigId)
        {
            try
            {

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.Untaguserhaschildusers(RoleId, QigId, GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while fetching the Untaguserhaschildusers.:Method Name:Untaguserhaschildusers()");
                throw;
            }
        }


        /// <summary>
        ///  To identify the Marking Personnel is done any activity or not in a Marking Project to Change Role Functionality
        /// </summary>
        /// <returns></returns>
        [Route("Checkactivity/{ProjectUserRoleId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN")]
        public async Task<IActionResult> CheckActivityOfMP(long ProjectUserRoleId)
        {
            try
            {

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectUsersService.CheckActivityOfMP(ProjectUserRoleId,GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while checking the Activity of MP.:Method Name:CheckActivityOfMP()");
                throw;
            }
        }

        /// <summary>
        ///  This SP is used to upgrade/downgrade the ProjectUsers role
        /// </summary>
        /// <returns></returns>
        [Route("Checkactivity/userrolechange")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN")]
        public async Task<IActionResult> CreateEditProjectUserRoleChange(CreateEditProjectUserRoleChange model)
        {
            string Status = string.Empty;
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                Status = await ProjectUsersService.CreateEditProjectUserRoleChange(model, GetCurrentProjectId(), GetCurrentProjectUserRoleID());
                return Ok(Status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while upgrade/downgrade the ProjectUsers role.:Method Name:CreateEditProjectUserRoleChange()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.PROJECTUSERMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    UserId = GetCurrentUserId(),
                    Remarks = model,
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),                   
                    ResponseStatus = Status == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = Status

                });
                #endregion
            }

        }
    }
}

