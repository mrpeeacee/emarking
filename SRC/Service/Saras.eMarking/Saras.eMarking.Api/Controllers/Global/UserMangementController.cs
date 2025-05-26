using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using System.Threading.Tasks;
using System;
using Saras.eMarking.Domain.Interfaces.GlobalBusinessInterface;
using Saras.eMarking.Domain.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System.Text.Json;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using Saras.eMarking.Domain.ViewModels.Auth;
using Nest;

namespace Saras.eMarking.Api.Controllers.Global
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/global/user-management")]
    public class UserMangementController : BaseApiController<UserMangementController>
    {
        private readonly IUserMangementService userMangementService;
        public UserMangementController(IUserMangementService _userMangementService, ILogger<UserMangementController> _logger, IAuditService _auditService, AppOptions appOptions) : base(appOptions, _logger, _auditService)
        {
            userMangementService = _userMangementService;
        }

        /// <summary>
        /// GetAllUsers:this api is to Get All users at global level
        /// </summary>
        /// <returns></returns>       
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("allusers")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<IActionResult> GetAllUsers(SearchFilterModel searchFilterModel)
        {
            String auditStatus = "";
            logger.LogDebug($"UserMangementController > Method Name: GetAllUsers() started. ProjectId={GetCurrentProjectId()} and Search filter details={searchFilterModel}");
            try
            {
                logger.LogDebug($"UserMangementController > Method Name: GetAllUsers() completed. ProjectId={GetCurrentProjectId()} and Search filter details={searchFilterModel}");

                return Ok(await userMangementService.GetAllUsers(searchFilterModel, GetCurrentUserId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting All users list");
                auditStatus = "ERROR";

				throw;
            }
            finally
            {
                if(searchFilterModel.SearchText==null && searchFilterModel.navigate==0)
                { 
				#region Insert Audit Trail
				_ = InsertAuditLogs(new AuditTrailData
				{
					Event = AuditTrailEvent.PAGENAVIGATION,
					Module = AuditTrailModule.APPLICATIONUSERMANAGEMENT,
					Entity = AuditTrailEntity.USER,
					Remarks = searchFilterModel,
					UserId = GetCurrentUserId(),
					ResponseStatus = auditStatus == "ERROR" ? AuditTrailResponseStatus.Error : AuditTrailResponseStatus.Success,
					//Response = JsonSerializer.Serialize(status),
					SessionId = GetCurrentSessionKey()
				});
                    #endregion
                }
            }
		}

        /// <summary>
        /// GetCreateEditUserdetails:this api is to Get Role School and user details
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>       
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("roleschool/{UserId}")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<IActionResult> GetCreateEditUserdetails(long UserId)
        {
            logger.LogDebug($"UserMangementController > Method Name: GetCreateEditUserdetails() started. ProjectId={GetCurrentProjectId()} and UserId={UserId}");
            try
            {
                logger.LogDebug($"UserMangementController > Method Name: GetCreateEditUserdetails() complete. ProjectId={GetCurrentProjectId()} and UserId={UserId}");

                return Ok(await userMangementService.GetCreateEditUserdetails(UserId, GetCurrentUserId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting Role School details");
                throw;
            }
        }

        ///<summary>
        ///CreateEditUser:this api is to Create and Edit a user
        ///</summary>
        ///<returns></returns> 
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("CreateEditUser")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<IActionResult> CreateEditUser(CreateEditUser model)
        {
            string status = string.Empty;
            logger.LogDebug($"UserMangementController > Method Name: CreateEditUser() started. ProjectId={GetCurrentProjectId()} and UserID={model.UserId}");

            try
            {
                status = await userMangementService.CreateEditUser(model, GetCurrentUserId());

                logger.LogDebug($"UserManagementController > Method Name: CreateEditUser() completed. ProjectId={GetCurrentProjectId()} and UserID = {model.UserId}");

                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Edit user:Method Name:CreateEditUser()");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                AuditTrailEvent auditTrailEvent;
                if (model.UserId == 0)
                {
                    auditTrailEvent = AuditTrailEvent.CREATE;
                }
                else
                {
                    auditTrailEvent = AuditTrailEvent.UPDATE;
                }
                if (model.SchooolCode == null)
                {
                    model.SchooolCode = "NA";
                }
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = auditTrailEvent,
                    Module = AuditTrailModule.APPLICATIONUSERMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    Remarks = model,
                    UserId = GetCurrentUserId(),
                    ResponseStatus = status == "I001" || status == "U001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(status),
                    SessionId = GetCurrentSessionKey()
                });
                #endregion
            }
        }

        ///<summary>
        ///Resetpwd:this api is to Reset user password
        ///</summary>
        ///<returns></returns> 
        [HttpPost]
        [Route("Resetpwd")]
        [ApiVersion("1.0")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<ActionResult<SendMailRequestModel>> Resetpwd(CreateEditUser UserId)
        {
            SendMailRequestModel sendMailRequestModel = new SendMailRequestModel();

            logger.LogDebug($"UserMangementController > Method Name: Resetpwd() started. ProjectId={GetCurrentProjectId()} and UserID={UserId}");

            try
            {

                SendMailRequestModel user = userMangementService.Resetpwd(UserId.UserId, GetCurrentUserId());

                if (user != null)
                {
                    sendMailRequestModel.Status = user.Status;
                    sendMailRequestModel.QueueID = user.QueueID;
                    sendMailRequestModel.IsMailSent = user.IsMailSent;

                    if (user.Status == "SUCC001")
                    {
                        sendMailRequestModel.QueueID = user.QueueID;
                        sendMailRequestModel.Status = user.Status;
                        sendMailRequestModel.IsMailSent = user.IsMailSent;
                    }

                }
                else
                {
                    logger.LogDebug("Login failed for {Loginname}: User not found.");
                    return Unauthorized();
                }
                await Task.CompletedTask;

                logger.LogDebug($"UserMangementController > Method Name: Resetpwd() completed. ProjectId={GetCurrentProjectId()} and UserID={UserId}");

                return Ok(sendMailRequestModel);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while Resetting User Pwd:Method Name:Resetpwd()");
                //throw
                return Unauthorized();
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.RESET,
                    Module = AuditTrailModule.APPLICATIONUSERMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    Remarks = UserId,
                    UserId = GetCurrentUserId(),
                    ResponseStatus = sendMailRequestModel.Status == "SUCC001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(sendMailRequestModel),
                    SessionId = GetCurrentSessionKey()
                });
                #endregion
            }


        }

        ///<summary>
        ///UserCreations:this api is to Create Bulk Users
        ///</summary>
        ///<returns></returns>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("UserCreation/{type}")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<IActionResult> UserCreations(int type)
        {
            UserCreations ltuserCreations = new UserCreations();

            logger.LogDebug($"UserMangementController > Method Name: UserCreations() started. ProjectId={GetCurrentProjectId()} and UserID={type}");

            try
            {
                IFormFile file = Request.Form.Files[0];
                ltuserCreations = await userMangementService.UserCreation(file, GetCurrentUserId(), type);

                logger.LogDebug($"UserMangementController > Method Name: UserCreations() completed. ProjectId={GetCurrentProjectId()} and UserID={type}");

                return Ok(ltuserCreations);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while creating bulk users : Method Name:UserCreation()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                if (type == 2)
                {
                    _ = InsertAuditLogs(new AuditTrailData
                    {
                        Event = AuditTrailEvent.IMPORT,
                        Module = AuditTrailModule.APPLICATIONUSERMANAGEMENT,
                        Entity = AuditTrailEntity.USER,
                        Remarks = "Imported successfully",
                        UserId = GetCurrentUserId(),
                        ResponseStatus = ltuserCreations.status == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                        Response = JsonSerializer.Serialize(ltuserCreations),
                    });
                }

                if(type == 1)
                {
                    _ = InsertAuditLogs(new AuditTrailData
                    {
                        Event = AuditTrailEvent.UPLOAD,
                        Module = AuditTrailModule.APPLICATIONUSERMANAGEMENT,
                        Entity = AuditTrailEntity.USER,
                        Remarks = ltuserCreations.users,
                        UserId = GetCurrentUserId(),
                        ResponseStatus = ltuserCreations.status == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                        Response = JsonSerializer.Serialize(ltuserCreations),
                    });
                }
                #endregion
            }

        }

        /// <summary>
        /// GetBlockedUsers:this api is to get used to GetBlockedUsers
        /// </summary>
        /// <returns></returns>    
        [HttpGet]
        [Route("BlockedUsers")]
        [ApiVersion("1.0")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<IActionResult> GetBlockedUsers()
        {
            logger.LogDebug($"UserMangementController > Method Name: GetBlockedUsers() started. ProjectId={GetCurrentProjectId()}");

            try
            {
                logger.LogDebug($"UserMangementController > Method Name: GetBlockedUsers() completed. ProjectId={GetCurrentProjectId()}");

                return Ok(await userMangementService.GetBlockedUsers());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Blocked User Management page while getting All users list");
                throw;
            }
        }

        /// <summary>
        /// unblockUser:this api is to unblock  Users
        /// <param name="objunblockUsers"></param>
        /// </summary>
        /// <returns></returns>  
        [HttpPost]
        [Route("unblockUser")]
        [ApiVersion("1.0")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<IActionResult> unblockUser(List<GetAllUsersModel> objunblockUsers)
        {
            logger.LogDebug($"UserMangementController > Method Name: unblockUser() started. ProjectId={GetCurrentProjectId()} and UnblockUserList={objunblockUsers}");
            string result = string.Empty;
            try
            {
                logger.LogDebug($"UserMangementController > Method Name: unblockUser() completed. ProjectId={GetCurrentProjectId()} and UnblockUserList={objunblockUsers}");

                result = await userMangementService.unblockUser(objunblockUsers);

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Blocked User Management page while unblocking the user");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UNBLOCKUSER,
                    Module = AuditTrailModule.APPLICATIONUSERMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    UserId = GetCurrentUserId(),
                    Remarks = objunblockUsers,
                    ResponseStatus = result == "S002" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result),
                    SessionId = GetCurrentSessionKey()
                });
                #endregion
            }
        }

        /// <summary>
        /// GetPassPhrase: this Get Api used to Get a passphrase     
        /// <returns></returns>
        /// </summary>
        [HttpGet]
        [Route("PassPhrase")]
        [ApiVersion("1.0")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<IActionResult> GetPassPhrase()
        {
            logger.LogDebug($"UserMangementController > Method Name: GetPassPhrase() started. ProjectId={GetCurrentProjectId()}");
            try
            {
                logger.LogDebug($"UserMangementController > Method Name: GetPassPhrase() completed. ProjectId={GetCurrentProjectId()}");

                return Ok(await userMangementService.GetPassPhrase(GetCurrentUserId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in PassPhrase User Management page while getting All PassPhrase list");
                throw;
            }
        }

        /// <summary>
        /// AddPassPhrase:this api is used to add and update a passphrase
        /// <param name="passPhraseObject"></param>
        /// </summary>
        /// <returns></returns>  
        [HttpPost]
        [Route("AddPassPhrase")]
        [ApiVersion("1.0")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<IActionResult> AddPassPhrase(PassPharseModel passPhraseObject)
        {
            string result = string.Empty;
            logger.LogDebug($"UserMangementController > Method Name: GetPassPhrase() started. ProjectId={GetCurrentProjectId()} and Pass phrase object={passPhraseObject}");
            try
            {

                result = await userMangementService.AddPassPhrase(passPhraseObject, GetCurrentUserId());

                logger.LogDebug($"UserMangementController > Method Name: GetPassPhrase() completed. ProjectId={GetCurrentProjectId()} and Pass phrase object={passPhraseObject}");
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in PassPhrase User Management page while inserting PassPhrase ");
                throw;
            }
            finally
            {

                #region Insert Audit Trail

                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.CREATE,
                    Module = AuditTrailModule.PASSPHRASE,
                    Entity = AuditTrailEntity.USER,
                    UserId = GetCurrentUserId(),
                    Remarks = passPhraseObject,
                    ResponseStatus = result == "success" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result),
                    SessionId = GetCurrentSessionKey()
                });
                #endregion
            }
        }

        /// <summary>
        /// ValidateRemoveUser:this api is to validate the user is mapped with any  project
        /// <param name="UserId"></param>
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [Route("RemoveUser/{UserId}")]
        [ApiVersion("1.0")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<IActionResult> ValidateRemoveUser(long UserId)
        {
            logger.LogDebug($"UserMangementController > Method Name: ValidateRemoveUser() started. ProjectId={GetCurrentProjectId()} and UserId={UserId}");
            try
            {
                logger.LogDebug($"UserMangementController > Method Name: ValidateRemoveUser() completed. ProjectId={GetCurrentProjectId()} and UserId={UserId}");

                return Ok(await userMangementService.ValidateRemoveUser(UserId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Change Status Users page while ValidateRemoveUser");
                throw;
            }
        }

        /// <summary>
        /// ScriptExists:this api is to validate for a particular users as a script
        /// <param name="StatusId"></param>
        /// <param name="UserId"></param>
        /// </summary>
        /// <returns></returns>  
        [HttpGet]
        [Route("ScriptExists/{UserId}/{StatusId}")]
        [ApiVersion("1.0")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<IActionResult> ScriptExists(int StatusId, long UserId)
        {
            logger.LogDebug($"UserMangementController > Method Name: ScriptExists() started. ProjectId={GetCurrentProjectId()} and StatusId={StatusId} and UserId={UserId}");
            try
            {
                logger.LogDebug($"UserMangementController > Method Name: ScriptExists() completed. ProjectId={GetCurrentProjectId()} and StatusId={StatusId} and UserId={UserId}");

                return Ok(await userMangementService.ScriptExists(StatusId, UserId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Change Status Users page while  Script is exists");
                throw;
            }
        }

      

        /// /// <summary>
        /// ChangeStatusUsers:this api is to changestatus of particular user
        /// <param name="StatusId"></param>
        /// <param name="UserId"></param>
        /// <param name="loginName"></param>
        /// </summary>
        /// <returns></returns> 
        [HttpPost]
        [Route("ChangeStatusUsers/{UserId}/{StatusId}/{loginName}")]
        [ApiVersion("1.0")]
        [Authorize(Roles = "SUPERADMIN,SERVICEADMIN,EO,EM")]
        public async Task<ActionResult<SendMailRequestModel>> ChangeStatusUsers(int StatusId, long UserId, string loginName)
        {
            logger.LogDebug($"UserMangementController > Method Name: ChangeStatusUsers() started. ProjectId={GetCurrentProjectId()} and StatusId={StatusId} and UserId={UserId}");

            SendMailRequestModel sendMailRequestModel = new SendMailRequestModel();
            try
            {
                SendMailRequestModel user = await userMangementService.ChangeStatusUsers(StatusId, UserId, GetCurrentUserId());

                if (user != null)
                {
                    sendMailRequestModel.Status = user.Status;
                    sendMailRequestModel.IsMailSent = user.IsMailSent;

                    if (user.Status == "EN001")
                    {
                        sendMailRequestModel.Status = user.Status;
                        sendMailRequestModel.IsMailSent = user.IsMailSent;
                    }

                }
                else
                {
                    logger.LogDebug("Login failed for {Loginname}: User not found.");
                    return Unauthorized();
                }
                await Task.CompletedTask;

                logger.LogDebug($"UserMangementController > Method Name: ChangeStatusUsers() completed. ProjectId={GetCurrentProjectId()} and StatusId={StatusId} and UserId={UserId}");

                return Ok(sendMailRequestModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Change Status Users page while getting All users list");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                InsertChangeStatusUsersAudit(StatusId, sendMailRequestModel, UserId, loginName);
                #endregion
            }
        }

        private void InsertChangeStatusUsersAudit(int StatusId, SendMailRequestModel sendMailRequestModel, long UserId, string loginName)
        {
            modelName modObj = new modelName();
            modObj.loginName = loginName;
            modObj.UserId = UserId;

            if (StatusId == 6)
            {
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.ENABLE,
                    Module = AuditTrailModule.APPLICATIONUSERMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    UserId = GetCurrentUserId(),
                    Remarks = modObj,
                    ResponseStatus = AuditTrailResponseStatus.Success,
                    Response = JsonSerializer.Serialize(sendMailRequestModel),
                    SessionId = GetCurrentSessionKey()
                });
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.ACTIVE,
                    Module = AuditTrailModule.APPLICATIONUSERMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    UserId = GetCurrentUserId(),
                    Remarks = modObj,
                    ResponseStatus = AuditTrailResponseStatus.Success,
                    Response = JsonSerializer.Serialize(sendMailRequestModel),
                    SessionId = GetCurrentSessionKey()
                });
            }
            else
            {

                AuditTrailEvent auditTrailEvent;
                if (StatusId == 2)
                {
                    auditTrailEvent = AuditTrailEvent.INACTIVE;
                }
                else if (StatusId == 3)
                {
                    auditTrailEvent = AuditTrailEvent.DISABLE;
                }
                else if (StatusId == 4)
                {
                    auditTrailEvent = AuditTrailEvent.ENABLE;
                }
                else if (StatusId == 5)
                {
                    auditTrailEvent = AuditTrailEvent.DELETE;
                }
                else
                {
                    auditTrailEvent = AuditTrailEvent.ACTIVE;
                }

                var auditTrailData = new AuditTrailData
                {
                    Event = auditTrailEvent,
                    Module = AuditTrailModule.APPLICATIONUSERMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    UserId = GetCurrentUserId(),
                    Remarks = modObj,
                    ResponseStatus = sendMailRequestModel.Status == "ERR01" || sendMailRequestModel.Status == "E001"
                    || sendMailRequestModel.Status == "EM002" || sendMailRequestModel.Status == " " ? AuditTrailResponseStatus.Error : AuditTrailResponseStatus.Success,
                    Response = JsonSerializer.Serialize(sendMailRequestModel),
                    SessionId = GetCurrentSessionKey()
                };

                _ = InsertAuditLogs(auditTrailData);
            }
        }

        /// <summary>
        /// GetMyProfileDetails:this api is to Get user for headers
        /// </summary> 
        /// <returns></returns>
        [Route("my-profile")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetMyProfileDetails()
        {
            logger.LogDebug($"UserMangementController > Method Name: GetMyProfileDetails() started. ProjectId={GetCurrentProjectId()}");

            try
            {
                logger.LogDebug($"UserMangementController > Method Name: GetMyProfileDetails() completed. ProjectId={GetCurrentProjectId()}");

                return Ok(await userMangementService.GetMyProfileDetails(GetCurrentUserId(), GetCurrentProjectUserRoleID()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthenticateController page while getting My Profile Details: Method Name: GetMyProfileDetails()");
                throw;
            }
        }

        /// <summary>
        /// getMyprofileDetailsProject:this api is Get user for headers
        /// </summary> 
        /// <returns></returns>
        [Route("my-profile/{ProjectuserroleId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> getMyprofileDetailsProject(long? ProjectuserroleId = null)
        {
            logger.LogDebug($"UserMangementController > Method Name: getMyprofileDetailsProject() started. ProjectId={GetCurrentProjectId()} and (Optional)Project user roleId={ProjectuserroleId}");

            try
            {
                if (ProjectuserroleId > 0)
                {
                    ProjectuserroleId = GetCurrentProjectUserRoleID();
                }
                else
                {
                    ProjectuserroleId = 0;
                }

                logger.LogDebug($"UserMangementController > Method Name: getMyprofileDetailsProject() completed. ProjectId={GetCurrentProjectId()} and (Optional)Project user roleId={ProjectuserroleId}");

                return Ok(await userMangementService.GetMyProfileDetails(GetCurrentUserId(), ProjectuserroleId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthenticateController page while getting My Profile Details: Method Name: GetMyProfileDetails()");
                throw;
            }
        }

        /// <summary>
        /// GetusermaagemetCompleteReport:this api is to Get User  Management Complete Report
        /// </summary> 
        /// <returns></returns> 
        [Route("usermanagement-complete-report")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetusermaagemetCompleteReport()
        {
            logger.LogDebug($"UserMangementController > Method Name: GetusermaagemetCompleteReport() started. ProjectId={GetCurrentProjectId()}");

            GetAllApplicationUsersModel Alluserslst = new();
            SearchFilterModel searchFilterModel = new SearchFilterModel();
            searchFilterModel.PageNo = 0;
            searchFilterModel.PageSize = 0;
            searchFilterModel.SearchText = string.Empty;
            try
            {
                logger.LogInformation($"UserManagementController > GetusermaagemetCompleteReport() started.");

                Alluserslst = await userMangementService.GetAllUsers(searchFilterModel, GetCurrentUserId());
                DataTable dt = ToGetUserDataTable(Alluserslst.getAllUsersModel);

                using (XLWorkbook wb = new XLWorkbook())
                {

                    wb.Worksheets.Add(dt, "Application user list");
                    wb.Worksheet("Application user list").Columns("1").Width = 30;
                    wb.Worksheet("Application user list").Columns("2").Width = 30;
                    wb.Worksheet("Application user list").Columns("3").Width = 30;
                    wb.Worksheet("Application user list").Columns("4").Width = 30;
                    wb.Worksheet("Application user list").Columns("5").Width = 30;
                    wb.Worksheet("Application user list").Columns("6").Width = 30;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        logger.LogDebug($"UserMangementController > Method Name: GetusermaagemetCompleteReport() completed. ProjectId={GetCurrentProjectId()}");

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in UserManagementController > GetusermaagemetCompleteReport().");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.EXPORT,
                    Module = AuditTrailModule.APPLICATIONUSERMANAGEMENT,
                    Entity = AuditTrailEntity.USER,
                    Remarks = "Exported successfully",
                    UserId = GetCurrentUserId(),
                    ResponseStatus = AuditTrailResponseStatus.Success,
                    Response = JsonSerializer.Serialize(Alluserslst),
                });
                #endregion
            }
        }

        private static DataTable ToGetUserDataTable(List<GetAllUsersModel> userdatalist)
        {
            ////DataTable dt1 = new DataTable();
            DataTable dt = InitializeDataTable();

            if (userdatalist.Count > 0)
            {
                userdatalist.ForEach(uc =>
                {
                    string roleName = uc.RoleName == "Assessment Officer" ? "Chief Examiner" : uc.RoleName;
                    string status = GetStatus(uc);

                    dt.Rows.Add(uc.Name, uc.LoginName, roleName, uc.SchooolName, uc.Phone, status);
                });
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static DataTable InitializeDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("LoginName", typeof(string));
            dt.Columns.Add("Role", typeof(string));
            dt.Columns.Add("SchoolName", typeof(string));
            dt.Columns.Add("PhoneNumber", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            return dt;
        }

        /// <summary>
        /// To get the Status of IsActive and IsDisabled
        /// </summary>
        /// <param name="uc"></param>
        /// <returns></returns>
        private static string GetStatus(GetAllUsersModel uc)
        {
            if (uc.isDisable == true)
            {
                return "Disabled";
            }
            else if (uc.isactive == true)
            {
                return "Active";
            }
            else // Assuming uc.IsActive is false
            {
                return "Inactive";
            }
        }

        /// <summary>
        /// GetApplicationLevelUserRoles:this api is to Get Role School details
        /// </summary>
        /// <returns></returns>       
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("applicationlevel-user-role")]
        //  [Authorize(Roles = "SUPERADMIN,SERVICEADMIN")]
        public async Task<IActionResult> GetApplicationLevelUserRoles()
        {
            logger.LogDebug($"UserMangementController > Method Name: GetApplicationLevelUserRoles() started. ProjectId={GetCurrentProjectId()}");
            try
            {
                logger.LogDebug($"UserMangementController > Method Name: GetApplicationLevelUserRoles() completed. ProjectId={GetCurrentProjectId()}");

                return Ok(await userMangementService.GetApplicationLevelUserRoles());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while getting User Roles details");
                throw;
            }
        }
    }
}
