using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.IQigManagement;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Auth;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigManagement;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TokenLibrary.EncryptDecrypt.AES;

namespace Saras.eMarking.Api.Controllers.Projects.Setup.QigManagement
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/qig-management")]
    [ApiVersion("1.0")]
    public class QigManagementController : BaseApiController<QigManagementController>
    {
        readonly IQigManagementService QigManagementService;
        private readonly IAuthService AuthService;
        readonly IQigService qigService;

        public QigManagementController(IQigService _qigService, IAuthService _authService, IQigManagementService _qigManagementService, ILogger<QigManagementController> _logger, IAuditService _auditService, AppOptions appOptions) : base(appOptions, _logger, _auditService)
        {
            QigManagementService = _qigManagementService;
            AuthService = _authService;
            qigService = _qigService;
        }

        /// <summary>
        /// GetQigQuestions : These API is used to Get all Qig Questions.
        /// /// </summary>
        /// <returns></returns>  
        [Route("QigQuestions/{QuestionType}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQigQuestions(int QuestionType)
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                return Ok(await QigManagementService.GetQigQuestions(GetCurrentProjectId(), QuestionType));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QIG Management page while Getting Qig Questions:Method Name:GetQigQuestions()");
                throw;
            }
        }


        /// <summary>
        /// GetQigDetails : These API is used to Get QigDetails
        /// </summary>
        /// <returns></returns>  
        [Route("QigDetails/{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQigDetails(long QigId)
        {
            try
            {
                return Ok(await QigManagementService.GetQigDetails(GetCurrentProjectId(), QigId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QIG Management page while Getting Qig Details:Method Name:GetQigDetails()");
                throw;
            }
        }

        /// <summary>
        /// GetManagedQigDetails : These API is used to Get Qig's.
        /// </summary>
        /// <returns></returns>
        [Route("QigmanagedDetails")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetManagedQigDetails()
        {
            GetManagedQigListDetails getManagedQigListDetails = new GetManagedQigListDetails();
            try
            {

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                getManagedQigListDetails = await QigManagementService.GetManagedQigDetails(GetCurrentProjectId());

                var role = GetCurrentProjectUserRoleCode();

                if (role == "SERVICEADMIN")
                {
                    getManagedQigListDetails.IsResetEnable = true;
                }
                else
                {
                    getManagedQigListDetails.IsResetEnable = false;
                }

                return Ok(getManagedQigListDetails);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QIG Management page while Getting Manage Qig Details:Method Name:GetManagedQigDetails()");
                throw;
            }
        }


        /// <summary>
        /// CreateQigs : These API is used to Create new Qigs for the projects.
        /// </summary>
        /// <returns></returns>
        [Route("Qiginsertion")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> CreateQigs(CreateQigsModel createqigsModel)
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
                logger.LogDebug($"QigManagementController > CreateQigs() started. ProjectId = {projectId}");
                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await QigManagementService.CreateQigs(createqigsModel, projectId, currentprojectuserroleId);
                logger.LogDebug($"QigManagementController > CreateQigs() started. ProjectId = {projectId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigManagementController > CreateQigs() started. ProjectId = {projectId}");
                throw;
            }
            finally
            {

                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.CREATE,
                    Module = AuditTrailModule.QIGMANAGEMENT,
                    Entity = AuditTrailEntity.QIG,
                    UserId = currentuserId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = createqigsModel,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }

        }

        /// <summary>
        /// UpdateMandatoryQuestion : These API is used to Update Mandatory Question for the QIG's.
        /// </summary>
        /// <returns></returns>
        [Route("QigDetails")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UpdateMandatoryQuestion(QigDetails qigDetails)
        {
            long projectId = 0;
            long currentuserid = 0;
            long currentprojectuserroleId = 0;
            string result = string.Empty;
            try
            {
                currentuserid = GetCurrentUserId();
                projectId = GetCurrentProjectId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                qigDetails.ProjectId = projectId;
                logger.LogDebug($"QigManagementController > UpdateMandatoryQuestion() started. ProjectId = {projectId}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigDetails.QigId))
                {
                    return new ForbidResult();
                }

                result = await QigManagementService.UpdateMandatoryQuestion(qigDetails);
                logger.LogDebug($"QigManagementController > UpdateMandatoryQuestion() End. ProjectId = {projectId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigManagementController > UpdateMandatoryQuestion() started. ProjectId = {projectId}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.EDIT,
                    Module = AuditTrailModule.QIGMANAGEMENT,
                    Entity = AuditTrailEntity.QIG,
                    UserId = currentuserid,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = qigDetails,
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// GetQuestionxml : These API is used to Get Question xml for particular questions.
        /// </summary>
        /// <param name="ProjectQuestionID"></param>
        /// <param name="ProjectQigId"></param>
        /// <returns></returns>  
        [Route("Qnsxml/{ProjectQigId}/{ProjectQuestionID}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQuestionxml(long ProjectQigId, long ProjectQuestionID)
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await QigManagementService.GetQuestionxml(GetCurrentProjectId(), ProjectQigId, ProjectQuestionID));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QIG Management page while Getting Questionxml:Method Name:GetQuestionxml()");
                throw;
            }
        }


        /// <summary>
        /// GetQuestionDetails : These API is used to Get Question Details
        /// </summary>
        /// <param name="QigType"></param>
        /// <param name="ProjectQigId"></param>
        /// <param name="QnsType"></param>
        /// <returns></returns>  
        [Route("Qnsdetails/{QigType}/{ProjectQigId}/{QnsType}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQuestionDetails(long QigType, long ProjectQigId, long QnsType)
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await QigManagementService.GetQuestionDetails(QigType, ProjectQigId, GetCurrentProjectId(), QnsType));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QIG Management page while Question Details:Method Name:GetQuestionDetails()");
                throw;
            }
        }

        /// <summary>
        /// MoveorTagQIG : These API is used to Tag/Move QIG to another QIG.    
        /// </summary>
        /// <param name="tagqigdetails"></param>
        /// <returns></returns>  
        [Route("moveqig")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> MoveorTagQIG(Tagqigdetails tagqigdetails)
        {
            string result = string.Empty;
            long projectId = 0;
            long currentprojectuserroleId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                if (!AuthService.IsValidProject(projectId, currentprojectuserroleId))
                {
                    return new ForbidResult();
                }

                result = await QigManagementService.MoveorTagQIG(tagqigdetails, currentprojectuserroleId, projectId);
                logger.LogDebug($"QigManagementController > SaveQigQuestions() started. ProjectId = {projectId}");
                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QIG Management page while Moving Qig Questions:Method Name:MoveorTagQIG()");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.EDIT,
                    Module = AuditTrailModule.QIGMANAGEMENT,
                    Entity = AuditTrailEntity.QIG,
                    Remarks = tagqigdetails,
                    ResponseStatus = result == "U001" || result == "S001" || result == "U002" || result == "U003" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// GetQIGs : These API is used to Get QIG's list.
        /// </summary>
        /// <param name="Qigtype"></param>
        /// <returns></returns>  
        [Route("getqigs/{Qigtype}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<ActionResult> GetQIGs(long? Qigtype = 0)
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                return Ok(await qigService.GetQIGs(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), true, Qigtype));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QIG Management Page while fetching GetQIGs : Method Name : GetQIGs()");
                throw;
            }
        }

        /// <summary>
        /// GetBlankQuestions : These API is used to get blank questions for Non-Composition type.
        /// </summary>
        /// <param name="ParentQuestionId"></param>
        /// <returns></returns>  
        [Route("BlankQuestions/{ParentQuestionId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetBlankQuestions(long ParentQuestionId)
        {
            try
            {
                return Ok(await QigManagementService.GetBlankQuestions(GetCurrentProjectId(), ParentQuestionId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigManagement page while getting question blanks : Method Name:GetBlankQuestions()");
                throw;
            }
        }

        /// <summary>
        /// SaveQigQuestions : These API is used to Save QIG Questions for a project.
        /// </summary>
        /// <param name="remarks"></param>
        /// <returns></returns>  
        [Route("finaliseQig")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> SaveQigQuestions(FinalRemarks remarks)
        {
            string result = string.Empty;
            long projectId = 0;
            long currentprojectuserroleId = 0;
            long currentuserId = 0;

            try
            {
                projectId = GetCurrentProjectId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                currentuserId = GetCurrentUserId();
                if (!AuthService.IsValidProject(projectId, currentprojectuserroleId))
                {
                    return new ForbidResult();
                }

                result = await QigManagementService.SaveQigQuestions(projectId, currentprojectuserroleId, remarks);
                logger.LogDebug($"QigManagementController > SaveQigQuestions() started. ProjectId = {projectId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QIG Management page while saving Qig Questions:Method Name:SaveQigQuestions()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.SAVE,
                    Module = AuditTrailModule.QIGMANAGEMENT,
                    Entity = AuditTrailEntity.QIG,
                    UserId = currentuserId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = remarks,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        ///<summary>
        ///SaveQigQuestionsDetails : These API is used to Save Created QigQuestions
        ///</summary>
        ///<returns></returns>
        [Route("QigQuestions")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> SaveQigQuestionsDetails(SaveQigQuestions saveQigQuestions)
        {
            string result = string.Empty;
            long projectId = 0;
            long currentprojectuserroleId = 0;
            long userId = 0;

            try
            {
                projectId = GetCurrentProjectId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                userId = GetCurrentUserId();
                if (!AuthService.IsValidProject(projectId, currentprojectuserroleId))
                {
                    return new ForbidResult();
                }

                result = await QigManagementService.SaveQigQuestionsDetails(saveQigQuestions, projectId, currentprojectuserroleId);
                logger.LogDebug($"QigManagementController > SaveQigQuestionsDetails() started. ProjectId = {projectId}");
                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management while save qig questions : Method Name :SaveQigQuestionsDetails()");
                throw;
            }

            finally
            {
                if (saveQigQuestions.QigId == 0)
                {
                    #region Insert Audit Trail
                    _ = InsertAuditLogs(new AuditTrailData
                    {
                        Event = AuditTrailEvent.CREATE,
                        Module = AuditTrailModule.QIGMANAGEMENT,
                        Entity = AuditTrailEntity.QIG,
                        UserId = userId,
                        ProjectUserRoleID = currentprojectuserroleId,
                        Remarks = saveQigQuestions,
                        ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                        Response = JsonSerializer.Serialize(result)
                    });
                    #endregion
                }
                else
                {
                    #region Insert Audit Trail
                    _ = InsertAuditLogs(new AuditTrailData
                    {
                        Event = AuditTrailEvent.UPDATE,
                        Module = AuditTrailModule.QIGMANAGEMENT,
                        Entity = AuditTrailEntity.QIG,
                        UserId = userId,
                        ProjectUserRoleID = currentprojectuserroleId,
                        Remarks = saveQigQuestions,
                        ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                        Response = JsonSerializer.Serialize(result)
                    });
                    #endregion
                }

            }
        }


        /// <summary>
        /// GetUntaggedQuestions : These API is used to Get Un Tagged Questions details.
        /// </summary>
        /// <returns></returns>
        [Route("UntaggedQuestions")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetUntaggedQuestions()
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await QigManagementService.GetUntaggedQuestions(GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigManagement page while getting Untaggedquestion : Method Name:GetUntaggedQuestions()");
                throw;
            }
        }

        /// <summary>
        /// DeleteQig : These Post API is used to delete pparticular QIG from the project.
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        [Route("Deleteqig")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> DeleteQig(ManageQigs QigId)
        {
            string result = string.Empty;
            long currentprojectuserroleId = 0;
            long userId = 0;

            try
            {
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                userId = GetCurrentUserId();
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await QigManagementService.DeleteQig(GetCurrentProjectId(), QigId.projectqigId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigManagement page while getting Untaggedquestion : Method Name:GetUntaggedQuestions()");
                throw;
            }

            finally
            {
              
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.DELETE,
                    Module = AuditTrailModule.QIGMANAGEMENT,
                    Entity = AuditTrailEntity.QIG,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = QigId,
                    ResponseStatus = result == "D001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// QigReset : These API is used to Reset already QIG
        /// </summary>
        /// <returns></returns>
        [Route("qigreset")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> QigReset()
        {
            QigResetDetails qigreset = new QigResetDetails();
            
            string result = string.Empty;
            long ProjectId = GetCurrentProjectId();
            long currentprojectuserroleId = 0;
            long userId = 0;
            
            qigreset.projectId = ProjectId;

            try
            {
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                userId = GetCurrentUserId();
                
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await QigManagementService.QigReset(ProjectId, currentprojectuserroleId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigManagement page while Resetting the QIG's : Method Name:QigReset()");
                throw;
            }
            finally
            {
                if(result == "S001")
                {
                    qigreset.Remarks = "Qig reset done for the project.";
                }
                else
                {
                    qigreset.Remarks = "Qig reset failed.";
                }

                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.RESET,
                    Module = AuditTrailModule.QIGMANAGEMENT,
                    Entity = AuditTrailEntity.QIG,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = qigreset,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion

            }
        }


        /// <summary>
        /// SaveUserDetails : These API works for Multi factor Authentication for QIG Reset.
        /// </summary>
        /// <returns></returns>
        [Route("SaveUser")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "SERVICEADMIN")]
        public async Task<IActionResult> SaveUserDetails(AuthenticateRequestModel loginCredential)
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
                logger.LogDebug($"QigManagementController > SaveUserDetails() started. ProjectId = {projectId}");
                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                if (AppOptions.AppSettings.IsPasswordEncryptedInClient)
                {
                    EncryptDecryptAes.StrEncryptionKey = AppOptions.AppSettings.EncryptionAlgorithmKey;
                    loginCredential.Password = EncryptDecryptAes.DecryptStringAES(loginCredential.Password);
                }

                result = await QigManagementService.SaveUserDetails(loginCredential, projectId, currentprojectuserroleId);

                logger.LogDebug($"QigManagementController > SaveUserDetails() started. ProjectId = {projectId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigManagementController > SaveUserDetails() started. ProjectId = {projectId}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.AUTHENTICATE,
                    Module = AuditTrailModule.QIGMANAGEMENT,
                    Entity = AuditTrailEntity.QIG,
                    UserId = currentuserId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = loginCredential,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }

        }

    }
}
