using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.ViewModels.Auth;
using Microsoft.AspNetCore.Http;
using Saras.eMarking.Domain.Entities.Security;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Business;
using System.Text.Json;
using TokenLibrary.EncryptDecrypt.AES;
using Nest;
using Microsoft.AspNetCore.Http.HttpResults;
using DocumentFormat.OpenXml.Bibliography;
using Saras.eMarking.Domain.ViewModels;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Saras.eMarking.Api.Controllers
{
    /// <summary>
    /// Authenticate apis 
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("/api/public/v{version:apiVersion}/authenticate")]
    [ApiVersion("1.0")]
    public class AuthenticateController : BaseApiController<AuthenticateController>
    {
        private readonly JwtOptions jwtOptions;
        readonly IAuthService AccountService;
        readonly IAuditService AuditService;
        /// <summary>
        /// AuthenticateController default constructor
        /// </summary>
        public AuthenticateController(IAuthService accountService, ILogger<AuthenticateController> _logger, AppOptions appOptions, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            AccountService = accountService;
            AuditService = _auditService;
            jwtOptions = appOptions.JwtOptions;
        }

        /// <summary>
        /// Logs in the user with the given user ID and sets the JWT token in the response header.
        /// </summary>
        /// <param name="loginModel">The login model.</param>
        /// <response code="400">The login model is invalid.</response>
        /// <response code="401">Login failed.</response> 
        [HttpPost, MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [Route("/api/public/v{version:apiVersion}/authenticate/login")]
        public async Task<ActionResult<AuthenticateResponseModel>> LoginAsync(AuthenticateRequestModel loginModel)
        {         
            AuthenticateResponseModel authenticateResponseModel = null;
            string loginname = loginModel?.Loginname?.Trim().ToLowerInvariant();
            bool isSso = false;
            using (logger.BeginScope("Authentication", Request.Headers))
            {
                if (string.IsNullOrEmpty(loginname))
                {
                    logger.LogDebug("Login failed: Login Name is required.");
                    return BadRequest("Login Name is required.");
                }
                if (string.IsNullOrWhiteSpace(loginModel.Password))
                {
                    logger.LogDebug("Login failed for {Loginname}: Password is required.", loginname);
                    return BadRequest("Password is required.");
                }

                try
                {
                    // Decrypts the encrypted login details received from the client and compares them with the values stored in the database.
                    loginModel.SessionKey = Guid.NewGuid().ToString();
                    if (AppOptions.AppSettings.IsPasswordEncryptedInClient)
                    {
                        EncryptDecryptAes.StrEncryptionKey = AppOptions.AppSettings.EncryptionAlgorithmKey;
                        loginModel.Password = EncryptDecryptAes.DecryptStringAES(loginModel.Password);
                    }

                    //Authenticates the login details with DB and genrate jwt token with user details
                    AuthenticateResponseModel user = AccountService.Authenticate(loginModel, IpAddress(), isSso);

                    if (user != null)
                    {                    
                        if (user.Status == "E003" || user.Status == "E002" || user.Status == "E005")
                        {
                            authenticateResponseModel = new AuthenticateResponseModel
                            {
                                Status = user.Status,
                                UserId = user.UserId,
                                LoginId = user.LoginId,
                                LoginCount = user.LoginCount,
                                NoOfAttempts = AppOptions.AppSettings.ChangePasswords.NoOfAttemps
                            };
                        }
                        else if(user.Status== "E006")
                            {
                            authenticateResponseModel = new AuthenticateResponseModel
                            {
                                Status = user.Status,
                                UserId = user.UserId,
                                LoginId = user.LoginId,
                                LoginCount = user.LoginCount,
                                NoOfAttempts = AppOptions.AppSettings.ChangePasswords.NoOfAttemps,
                                Timeleft = user.Timeleft,
                                //IscaptchaRequired=true,
                            };                        
                        }
                        else if(user.Status== "INVALCAP")
                        {
                            authenticateResponseModel = new AuthenticateResponseModel
                            {
                                Status = user.Status,
                                UserId = user.UserId,
                                LoginId = user.LoginId,
                                LoginCount = user.LoginCount,
                            };
                        }
                        else
                        {
                            authenticateResponseModel = new AuthenticateResponseModel
                            {
                                IsFirstTimeLoggedIn = user.IsFirstTimeLoggedIn,
                                LoginId = user.LoginId,
                                UserId = user.UserId
                            };

                            //Set jwt token to response header
                            Utilities.InsertStringToCookie(HttpContext, "ACCESS-TOKEN", user.Token, jwtOptions.TokenValidityInMinutes);
                            Utilities.InsertStringToCookie(HttpContext, "REFRESH-TOKEN", user.RefreshToken, jwtOptions.RefreshTokenValidityInMinutes);
                            Utilities.InsertStringToCookie(HttpContext, "REF-TOKEN", user.RefKey, jwtOptions.RefreshTokenValidityInMinutes);
                            HttpContext.Response.Headers.Append("X-Token", user.SessionKey);

                            loginModel.SessionKey = user.SessionKey;

                            authenticateResponseModel.RefreshInterval = TimeSpan.FromMinutes(jwtOptions.TokenValidityInMinutes).TotalSeconds;
                            authenticateResponseModel.Roles = user.Roles;
                            authenticateResponseModel.Status = user.Status;
                        }
          
                    }
                    else
                    {           
                        logger.LogDebug("Login failed for {Loginname}: User not found.", loginname);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Login failed for Loginname: {loginModel?.Loginname}");
                }
                finally
                {
                    #region Insert Audit Trail
                    loginModel.loginnstatus = authenticateResponseModel != null ? authenticateResponseModel.Status : string.Empty;
                    loginModel.LoginCount = (byte)(authenticateResponseModel?.LoginCount == null ? 0 : authenticateResponseModel.LoginCount);
                    loginModel.NoOfAttempts = authenticateResponseModel?.NoOfAttempts ?? 0;
                    UpdateLoginAudit(loginModel, authenticateResponseModel);

                    #endregion


                }

                await Task.CompletedTask;

                return Ok(authenticateResponseModel);
            }
        }

        private void UpdateLoginAudit(AuthenticateRequestModel loginModel, AuthenticateResponseModel authenticateResponseModel)
        {
            var auditTrailData = new AuditTrailData
            {
                Event = AuditTrailEvent.LOGIN,
                Entity = AuditTrailEntity.USER,
                Module = AuditTrailModule.USERLOGIN,
                UserId = authenticateResponseModel?.UserId ?? 0,
                SessionId = loginModel.SessionKey,
                Remarks = loginModel,
                Response = JsonSerializer.Serialize(authenticateResponseModel)
            };

            if (loginModel.loginnstatus == "E002")
            {
                auditTrailData.ResponseStatus = AuditTrailResponseStatus.LoginFailed;
            }
            else if (loginModel.loginnstatus == "E005")
            {
                auditTrailData.ResponseStatus = AuditTrailResponseStatus.InvalidUser;
            }
            else if (loginModel.loginnstatus == "E003")
            {
                auditTrailData.ResponseStatus = AuditTrailResponseStatus.Error;
            }
            else if(loginModel.loginnstatus=="E006")
            {
                auditTrailData.ResponseStatus = AuditTrailResponseStatus.Suspended;
            }
          
            else
            {
                auditTrailData.ResponseStatus = AuditTrailResponseStatus.Success;
            }

            _ = InsertAuditLogs(auditTrailData);

        }


        /// <summary>
        /// Change password api
        /// </summary>
        /// <param name="changePassword"></param>
        /// <param name="UserLoginId"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/authenticate/change-password")]
        [Route("/api/public/v{version:apiVersion}/authenticate/change-password/{UserLoginId}")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestModel changePassword, string UserLoginId)
        {
            string changePasswordstatus = "";
            logger.LogDebug($"AuthenticateController > Method Name: ChangePassword() started. Change password details = {changePassword} and User loginId = {UserLoginId}");

            try
            {
                changePassword.SessionKey = Guid.NewGuid().ToString();

                if (CurrentUserContext != null || UserLoginId != null)
                {
                    DecryptPasswordIfNeeded(changePassword);

                    string loginId = CurrentUserContext?.LoginId ?? UserLoginId;

                    changePasswordstatus = await AccountService.ChangePassword(changePassword, loginId);

                    if (changePasswordstatus == "U001")
                    {
                        ClearCookiesAndRevokeToken();

                        return Ok(changePasswordstatus);
                    }

                    logger.LogDebug($"AuthenticateController > Method Name: ChangePassword() completed. Change password details = {changePassword} and User loginId = {UserLoginId}");
                    return Ok(changePasswordstatus);
                }
                else
                {
                    logger.LogDebug($"Getting null AuthenticateController > Method Name: ChangePassword() completed. Change password details = {changePassword} and User loginId = {UserLoginId}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Change Pswd page while updating New Pswd: Method Name: ChangePaswd()");
                throw;
            }
            finally
            {
                InsertAuditTrail(changePassword, changePasswordstatus);
            }
        }

        private void DecryptPasswordIfNeeded(ChangePasswordRequestModel changePassword)
        {
            if (AppOptions.AppSettings.IsPasswordEncryptedInClient)
            {
                EncryptDecryptAes.StrEncryptionKey = AppOptions.AppSettings.EncryptionAlgorithmKey;
                changePassword.Newpassword = EncryptDecryptAes.DecryptStringAES(changePassword.Newpassword);
                changePassword.Oldpassword = EncryptDecryptAes.DecryptStringAES(changePassword.Oldpassword);
                changePassword.Cnfnewpassword = EncryptDecryptAes.DecryptStringAES(changePassword.Cnfnewpassword);
            }
        }

        private void ClearCookiesAndRevokeToken()
        {
            string token = HttpContext.Request.Cookies["ACCESS-TOKEN"]?.Trim();
            string refreshToken = HttpContext.Request.Cookies["REFRESH-TOKEN"]?.Trim();

            if (!string.IsNullOrEmpty(refreshToken))
            {
                AccountService.RevokeToken(refreshToken, token, IpAddress(), true);
            }

            foreach (var cookie in HttpContext.Request.Cookies.Keys)
            {
                HttpContext.Response.Cookies.Delete(cookie);
            }
        }

        private void InsertAuditTrail(ChangePasswordRequestModel changePassword, string changePasswordstatus)
        {
            _ = InsertAuditLogs(new AuditTrailData
            {
                Event = AuditTrailEvent.CHANGEPASSWORD,
                Entity = AuditTrailEntity.USER,
                Module = AuditTrailModule.USERLOGIN,
                Remarks = changePassword,
                SessionId = changePassword.SessionKey,
                ResponseStatus = changePasswordstatus == "U001" ? AuditTrailResponseStatus.PasswordChanged : AuditTrailResponseStatus.Error,
                Response = Convert.ToString(changePasswordstatus)
            });
        }


        /// <summary>
        /// Generates a new JWT token using the provided refresh token.
        /// </summary>
        /// <param name="projectid"></param>
        /// <returns></returns>
        [HttpPost, MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [Route("/api/public/v{version:apiVersion}/authenticate/refresh-token")]
        [Route("/api/public/v{version:apiVersion}/authenticate/refresh-token/{projectid}")]
        public async Task<ActionResult<AuthenticateResponseModel>> RefreshToken(long projectid = 0)
        {
            logger.LogDebug($"AuthenticateController > Method Name: RefreshToken() started. ProjectId = {projectid}");

            string Token = string.Empty;
            string RefreshToken = string.Empty;
            if (HttpContext.Request.Cookies.ContainsKey("REFRESH-TOKEN"))
            {
                RefreshToken = HttpContext.Request.Cookies["REFRESH-TOKEN"]?.Trim();
            }


            if (string.IsNullOrEmpty(RefreshToken))
                return Unauthorized(new { message = "Invalid token" });


            string csrftoken = null;
            if (AppOptions.AppSettings.IsCsrfValidationEnabled && HttpContext.Request.Cookies.ContainsKey("X-XSRF-TOKEN"))
            {
                csrftoken = HttpContext.Request.Cookies["X-XSRF-TOKEN"].Trim();
            }

            // Validates the provided JWT token, generates a new JWT token, and refreshes the refresh token.

            AuthenticateResponseModel user = AccountService.RefreshToken(RefreshToken, Token, IpAddress(), projectid, csrftoken);

            if (user == null)
                return Unauthorized(new { message = "Invalid token" });

            AuthenticateResponseModel tokenResponse = new()
            {
                RefreshInterval = TimeSpan.FromMinutes(jwtOptions.TokenValidityInMinutes).TotalSeconds,
                Roles = user.Roles
            };

            //Update jwt and refresh token in the response header.
            Utilities.InsertStringToCookie(HttpContext, "ACCESS-TOKEN", user.Token, jwtOptions.TokenValidityInMinutes);
            Utilities.InsertStringToCookie(HttpContext, "REF-TOKEN", user.RefKey, jwtOptions.RefreshTokenValidityInMinutes);
            Utilities.InsertStringToCookie(HttpContext, "REFRESH-TOKEN", user.RefreshToken, jwtOptions.RefreshTokenValidityInMinutes);

            await Task.Yield();

            logger.LogDebug($"AuthenticateController > Method Name: RefreshToken() completed. ProjectId = {projectid}");
            return Ok(tokenResponse);
        }


        /// <summary>
        /// Performs user logout and invalidates the associated access token.
        /// </summary>        
        /// <returns></returns>  
        [HttpPost, MapToApiVersion("1.0")]
        [Route("/api/public/v{version:apiVersion}/authenticate/LogoutAsync")]
        public async Task<ActionResult> LogoutAsync()
        {
            long? userId = 0;
            bool RemoveStatus = false;
            logger.LogDebug($"AuthenticateController > Method Name: LogoutAsync() started");

            try
            {
                string Token = string.Empty;
                string RefreshToken = string.Empty;
                string SessionId = null;
                if (HttpContext.Request.Cookies.ContainsKey("ACCESS-TOKEN"))
                {
                    userId = GetCurrentUserId();
                    SessionId = GetCurrentSessionKey();
                    Token = HttpContext.Request.Cookies["ACCESS-TOKEN"].Trim();
                    RefreshToken = HttpContext.Request.Cookies["REFRESH-TOKEN"]?.Trim();
                }

                if (!string.IsNullOrEmpty(RefreshToken))
                {
                    // Revokes the specified access token in the database.

                    RemoveStatus = AccountService.RevokeToken(RefreshToken, Token, IpAddress(), true);
                }

                #region Audit Tril
                if (SessionId != null)
                {
                    AuditTrailData auditTrailData = new()
                    {
                        Event = AuditTrailEvent.LOGOUT,
                        Module = AuditTrailModule.USERLOGIN,
                        Entity = AuditTrailEntity.USER,
                        UserId = userId,
                        SessionId = SessionId,
                        ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                        Remarks = "Logged out Successfully",
                        // ResponseStatus = AuditTrailResponseStatus.Success
                        ResponseStatus = (RemoveStatus == true) ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                        Response = JsonSerializer.Serialize(RemoveStatus)

                    };
                    LoggoutAction la = new()
                    {
                        Logout = AuditTrailResponseStatus.Success.ToString()
                    };
                    auditTrailData.SetRemarks(la);
                    AuditService.InsertAuditLogs(auditTrailData);
                }
                #endregion

                foreach (var cookie in HttpContext.Request.Cookies.Keys)
                {
                    HttpContext.Response.Cookies.Delete(cookie);
                }

                logger.LogDebug($"AuthenticateController > Method Name: LogoutAsync() completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Exception in Log out Async");
            }

            await Task.Yield();
            return Ok(RemoveStatus);
        }

        /// <summary>
        /// Get IP address of user
        /// </summary>
        /// <returns></returns>
        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }


        /// <summary>
        /// Checks if the user session is active or expired.
        /// </summary> 
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/authenticate/session-expire")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> SessionExpire()
        {
            bool SessionActive = false;
            logger.LogDebug($"AuthenticateController > Method Name: SessionExpire() started");

            try
            {
                await Task.CompletedTask;

                if (CurrentUserContext != null)
                {
                    SessionActive = true;
                    return Ok(SessionActive);
                }
                else
                {
                    return Ok(SessionActive);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in SessionExpire ");
                throw;
            }
        }


        /// <summary>
        /// Forgot Password : Updating password based in nric and loginid
        /// </summary>
        /// <param name="objForgotpassword"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/authenticate/forgot-password")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel objForgotpassword)
        {
            ForgotPasswordModel result = null;
            logger.LogDebug($"AuthenticateController > Method Name: ForgotPassword() started. ProjectId = {GetCurrentProjectId()} and Fogot password details = {objForgotpassword}");

            try
            {
                objForgotpassword.SessionKey = Guid.NewGuid().ToString();
                if (AppOptions.AppSettings.IsPasswordEncryptedInClient)
                {
                    EncryptDecryptAes.StrEncryptionKey = AppOptions.AppSettings.EncryptionAlgorithmKey;
                    objForgotpassword.Newpassword = EncryptDecryptAes.DecryptStringAES(objForgotpassword.Newpassword);
                    objForgotpassword.Cnfnewpassword = EncryptDecryptAes.DecryptStringAES(objForgotpassword.Cnfnewpassword);
                }

                result = await AccountService.ForgotPassword(objForgotpassword, GetCurrentProjectUserRoleID());

                logger.LogDebug($"AuthenticateController > ForgotPassword() completed. ProjectId = {GetCurrentProjectId()} and Fogot password details = {objForgotpassword}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthenticateController page while updating forgot password for specific Users : Method Name : ForgotPassword()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.FORGOTPASSWORD,
                    Module = AuditTrailModule.USERLOGIN,
                    Entity = AuditTrailEntity.USER,
                    UserId = result?.UserId ?? 0,
                    Remarks = objForgotpassword,
                    SessionId = objForgotpassword.SessionKey,
                    ResponseStatus = result?.status == "U001" ? AuditTrailResponseStatus.PasswordChanged : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }


        /// <summary>
        /// Activate or deactive users
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="activetype"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/{userid}/{activatetype}")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<IActionResult> ActivateorDeactivateUser(long userid, long activetype)
        {
            logger.LogDebug($"AuthenticateController > Method Name: ActivateorDeactivateUser() started. ProjectId = {GetCurrentProjectId()} and UserId = {userid} and ActiveType = {activetype}");
            try
            {
                long projectId = GetCurrentProjectId();
                string result = await AccountService.ActivateorDeactivateUser(userid, activetype, projectId);
                logger.LogDebug($"AuthenticateController > ActivateorDeactivateUser() started. ProjectId = {GetCurrentProjectId()} and UserId = {userid} and ActiveType = {activetype}");
                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthenticateController page while Activating/Deactivating User for specific Project. : Method Name : ActivateorDeactivateUser()");
                throw;
            }
        }

        /// <summary>
        /// Get Captcha image
        /// </summary> 
        /// <returns></returns> 
        [Route("/api/public/v{version:apiVersion}/authenticate/get-captcha")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetCaptcha()
        {
            logger.LogDebug($"AuthenticateController > Method Name: GetCaptcha() started. ProjectId = {GetCurrentProjectId()}");
            try
            {
                logger.LogDebug($"AuthenticateController > Method Name: GetCaptcha() completed. ProjectId = {GetCurrentProjectId()}");
                return Ok(await AccountService.CreateCaptcha());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in GetCaptcha");
                throw;
            }
        }


        /// <summary>
        /// To Generate Tokens For SSOEmarking Archive Login
        /// </summary>
        /// <returns></returns>

        [HttpGet, MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [Route("/api/public/v{version:apiVersion}/authenticate/SSOArchiveLogin")]
        [Authorize(Roles = "AO,SUPERADMIN,SERVICEADMIN")]
        public async Task<ActionResult<string>> SSOArchiveLoginAsync()
        {
            string Url = "";
            EmarkingSsoRequest emarkingSsoRequest = new EmarkingSsoRequest();
            using (logger.BeginScope("Authentication: {header}", Request.Headers))
            {

                try
                {
                    emarkingSsoRequest.UserName = CurrentUserContext.LoginId;
                    //For Token Generation For SSO Archive Login
                    Url = AccountService.EmarkingSSOArchive(emarkingSsoRequest);

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Login failed for Loginname: {loginname}", emarkingSsoRequest.UserName);
                }
                await Task.CompletedTask;
                return Ok(Url);
            }
        }




        /// <summary>
        /// To Generate Token For SSOEmarking Live
        /// </summary>
        /// <returns></returns>
        [HttpGet, MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [Route("/api/public/v{version:apiVersion}/authenticate/SSOEmarkingLogin/{IsArchive?}")]
        [Authorize(Roles = "AO,SUPERADMIN,SERVICEADMIN")]
        public async Task<ActionResult<string>> SSOEmarkingLoginAsync(bool IsArchive = false)
        {
            string Url = "";
            EmarkingSsoRequest emarkingSsoRequest = new EmarkingSsoRequest();
            using (logger.BeginScope("Authentication: {header}", Request.Headers))
            {

                try
                {
                    if (IsArchive)
                    {
                        Url= AppOptions.AppSettings.SSOemarkings.Emarking;
                    }
                    else
                    {
                        emarkingSsoRequest.UserName = CurrentUserContext.LoginId;
                        //For Token Generation For SSO Emarking Live Login
                        Url = AccountService.EmarkingSSOLive(emarkingSsoRequest);

                    }
                   
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Login failed for Loginname: {loginname}", emarkingSsoRequest.UserName);
                }
                await Task.CompletedTask;
                return Ok(Url);
            }
        }




        /// <summary>
        /// To Validate Jwt Token For SSOArchiveIntegration
        /// </summary>
        /// <param name="enc"></param>
        /// <returns></returns>

        [HttpPost, MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [Route("/api/public/v{version:apiVersion}/authenticate/jwt/launch/{enc}")]
        public async Task<ActionResult<AuthenticateResponseModel>> JwtSsoLaunch(string enc)
        {
            if (string.IsNullOrEmpty(enc) || AppOptions.SsoIntegrationOptions.SsoProviderType != Domain.Configuration.SsoProviderType.Jwt)
            {
                logger.LogDebug("Jwt Sso Login failed: Invalid SSO details enc.");
                return BadRequest("Invalid Sso token.");
            }
            AuthenticateRequestModel loginModel = new();

            AuthenticateResponseModel authenticateResponseModel = null;

            try
            {
                //Decrypt the SSO Jwt token and get the User payload to do Authentication
                string LoginName = AccountService.ValidateSsoArchiveToken(enc, AppOptions.SsoIntegrationOptions);

                if (string.IsNullOrEmpty(LoginName))
                {
                    logger.LogDebug("Jwt Sso Login failed: Login Name is required. {enc}", enc);

                    return BadRequest("Login Name is required.");
                   

                }
                loginModel = new()
                {
                    Loginname = LoginName,
                    SessionKey = Guid.NewGuid().ToString(),
                    //Password=CurrentUserContext.

                };


                //Authenticates the login details with DB and genrate jwt token with user details
                authenticateResponseModel = AuthenticateUser(loginModel, true);


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Jwt Sso Login failed for Loginname : {loginname}, enc : {enc}", authenticateResponseModel?.LoginId, enc);
            }
            finally
            {
                #region Insert Audit Trail

                loginModel.loginnstatus = authenticateResponseModel != null ? authenticateResponseModel.Status : string.Empty;

                UpdateLoginAudit(loginModel, authenticateResponseModel);

                #endregion Insert Audit Trail
            }

            await Task.CompletedTask;

            return Ok(authenticateResponseModel);

        }


        /// <summary>
        ///  Authenticates the login details with DB and genrate jwt token with user details.
        /// </summary>
        /// <param name="loginModel"></param>
        /// <param name="IsSso"></param>
        /// <returns></returns>
        private AuthenticateResponseModel AuthenticateUser(AuthenticateRequestModel loginModel, bool IsSso = false)
        {
            AuthenticateResponseModel authenticateResponseModel = null;

            //Authenticates the login details with DB and genrate jwt token with user details
            AuthenticateResponseModel user = AccountService.Authenticate(loginModel, IpAddress(), IsSso);

            if (user != null)
            {
                if (user.Status == "E003" || user.Status == "E002")
                {
                    authenticateResponseModel = new AuthenticateResponseModel
                    {
                        Status = user.Status,
                        UserId = user.UserId,
                        LoginId = user.LoginId
                    };
                }
                else
                {
                    authenticateResponseModel = new AuthenticateResponseModel
                    {
                        IsFirstTimeLoggedIn = user.IsFirstTimeLoggedIn,
                        LoginId = user.LoginId,
                        UserId = user.UserId
                    };

                    //Set jwt token to response header
                    Utilities.InsertStringToCookie(HttpContext, "ACCESS-TOKEN", user.Token, jwtOptions.TokenValidityInMinutes);
                    Utilities.InsertStringToCookie(HttpContext, "REFRESH-TOKEN", user.RefreshToken, jwtOptions.RefreshTokenValidityInMinutes);
                    Utilities.InsertStringToCookie(HttpContext, "REF-TOKEN", user.RefKey, jwtOptions.RefreshTokenValidityInMinutes);
                    HttpContext.Response.Headers.Append("X-Token", user.SessionKey);

                    loginModel.SessionKey = user.SessionKey;

                    authenticateResponseModel.RefreshInterval = TimeSpan.FromMinutes(jwtOptions.TokenValidityInMinutes).TotalSeconds;
                    authenticateResponseModel.Roles = user.Roles;
                    authenticateResponseModel.Status = user.Status;
                }
            }
            else
            {
                logger.LogDebug("Login failed for {Loginname}: User not found.", loginModel?.Loginname);
            }
            return authenticateResponseModel;
        }


    }
}
