using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saras.eMarking.Domain.Configuration;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Entities.Security;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Audit;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Security;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Auth;
using SkiaSharp;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TokenLibrary;
using TokenLibrary.EncryptDecrypt.AES;

using System.Data;
using System.Collections.Generic;
using System.Security.Cryptography;
using Exceptionless.DateTimeExtensions;


namespace Saras.eMarking.Business.Account
{
    /// <summary>
    /// AuthService class
    /// </summary>
    public class AuthService : BaseService<AuthService>, IAuthService
    {

        private readonly IAuthRepository userRepository;
        private readonly JwtOptions jwtOptions;

        //new
        private static readonly IDictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>> HashAlgorithms;
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        static AuthService()
        {
            HashAlgorithms = new Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>>
            {
                { JwtHashAlgorithm.HS256, (key, value) => { using (var sha = new HMACSHA256(key)) { return sha.ComputeHash(value); } } },
                { JwtHashAlgorithm.HS384, (key, value) => { using (var sha = new HMACSHA384(key)) { return sha.ComputeHash(value); } } },
                { JwtHashAlgorithm.HS512, (key, value) => { using (var sha = new HMACSHA512(key)) { return sha.ComputeHash(value); } } }
            };
        }


        public AuthService(IAuthRepository _userRepository, ILogger<AuthService> _logger, AppOptions _appOptions, IAuditRepository _auditRepository) : base(_logger, _appOptions)
        {
            userRepository = _userRepository;
            jwtOptions = _appOptions.JwtOptions;
        }

        /// <summary>
        /// Authenticate users and generate Jwt and refresh token
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public AuthenticateResponseModel Authenticate(AuthenticateRequestModel model, string ipAddress, bool isSso=false)
        {
            AuthenticateResponseModel authenticateResponseModel=null;
            List<ForgotPasswordModel> forgotPasswordRequests = new List<ForgotPasswordModel>();
            try
            {
                logger.LogDebug($"AuthService > Authenticate() method started {model}");

                //Hash the user details to compare with DB
                model.Password = TokenLibrary.EncryptDecrypt.Hmac.Hashing.GetHash(AppOptions.AppSettings.ChangePasswords.EncryptionKey, model.Password);

                //Get user details for given details
                // if (usercontext.LoginCount >= 6)
                // {
                if (model.IscaptchaRequired)
                {
                    var ObjForgotPasswordRequestModel = new ForgotPasswordModel
                    {
                        CaptchaText = model.CaptchaText,
                        GUID = model.GUID
                    };
                    ForgotPasswordModel status = ValidateCaptcha(ObjForgotPasswordRequestModel).Result;
                    // Add the status to the list
                    forgotPasswordRequests.Add(status);                 
                    UserInfo userInfo = new UserInfo();
                    authenticateResponseModel = GetdetailsUser(model);
                    authenticateResponseModel = new AuthenticateResponseModel
                    {
                        Status = forgotPasswordRequests[0].status,
                        LoginCount= authenticateResponseModel.LoginCount,
                        LoginId = authenticateResponseModel.LoginId,
                        UserId = authenticateResponseModel.UserId,     
                    };
                }
                if (model.IscaptchaRequired == false|| forgotPasswordRequests[0].status=="S001")
                {
                    UserContext usercontext = userRepository.Authenticate(model, isSso);

                    //return null if user not found
                    if (usercontext == null) return null;

                    //Generate unique session id for every logins
                    string sessionId = Guid.NewGuid().ToString();


                    if (usercontext.Status == "E003" || usercontext.Status == "E002" || usercontext.Status == "E005" )
                    {
                        authenticateResponseModel = new AuthenticateResponseModel
                        {
                            Status = usercontext.Status,
                            LoginId = usercontext.LoginId,
                            UserId = usercontext.UserId,
                            LoginCount = usercontext.LoginCount,
                            LastloginDateTime = usercontext.LastLoginDate
                        };

                    }
                    else if(usercontext.Status == "E006")
                    {
                        DateTime lastLoginDate = (DateTime)usercontext.LastFailedAttempt;
                        DateTime suspendenttime = lastLoginDate.AddMinutes(AppOptions.AppSettings.ChangePasswords.UserSuspendedTime);                 
                        TimeSpan  timeLeft = suspendenttime-DateTime.UtcNow;
                        authenticateResponseModel = new AuthenticateResponseModel
                        {
                            Status = usercontext.Status,
                            LoginId = usercontext.LoginId,
                            UserId = usercontext.UserId,
                            LoginCount = usercontext.LoginCount,
                         
                            Timeleft = (double)timeLeft.TotalMinutes,
                        };
                    }
                    else
                    {
                        //Generate Jwt and refresh token.
                        authenticateResponseModel = GenerareAuthToken(usercontext, sessionId, ipAddress);
                        authenticateResponseModel.SessionKey = sessionId;
                        authenticateResponseModel.IsFirstTimeLoggedIn = usercontext.IsFirstTimeLoggedIn;
                        authenticateResponseModel.LoginId = usercontext.LoginId;
                        authenticateResponseModel.Status = usercontext.Status;
                        authenticateResponseModel.UserId = usercontext.UserId;
                    }

                    logger.LogDebug($"AuthService > Authenticate() method completed {model}");

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $" Error in AuthService > Authenticate() method {model}");
                throw;
            }
            return authenticateResponseModel;
        }
      
        public AuthenticateResponseModel GetdetailsUser(AuthenticateRequestModel model)

        {
            AuthenticateResponseModel authenticateResponseModel = null;
            UserContext usercontext = userRepository.GetuserDetails(model);
            authenticateResponseModel = new AuthenticateResponseModel
            {
                Status = usercontext.Status,
                LoginId = usercontext.LoginId,
                UserId = usercontext.UserId,
                LoginCount = usercontext.LoginCount,
                LastloginDateTime = usercontext.LastLoginDate
            };
            return authenticateResponseModel;

        }
            /// <summary>
            /// Validates the new password against specified criteria and updates it if valid.
            /// </summary>
            /// <param name="ObjCandidatesAnswerModel"></param>
            /// <param name="LoginId"></param>
            /// <returns></returns>
            public async Task<string> ChangePassword(ChangePasswordRequestModel ObjCandidatesAnswerModel, string LoginId)
        {
            logger.LogInformation("AuthService Service >> ChangePassword() started");
            try
            {
                string status = ValidateChangePassword(ObjCandidatesAnswerModel);
                string sessionId = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(status))
                {
                    ObjCandidatesAnswerModel.Oldpassword = TokenLibrary.EncryptDecrypt.Hmac.Hashing.GetHash(AppOptions.AppSettings.ChangePasswords.EncryptionKey, ObjCandidatesAnswerModel.Oldpassword);
                    ObjCandidatesAnswerModel.Newpassword = TokenLibrary.EncryptDecrypt.Hmac.Hashing.GetHash(AppOptions.AppSettings.ChangePasswords.EncryptionKey, ObjCandidatesAnswerModel.Newpassword);
                    ObjCandidatesAnswerModel.Cnfnewpassword = TokenLibrary.EncryptDecrypt.Hmac.Hashing.GetHash(AppOptions.AppSettings.ChangePasswords.EncryptionKey, ObjCandidatesAnswerModel.Cnfnewpassword);
                    ObjCandidatesAnswerModel.SessionKey = sessionId;

                    return await userRepository.ChangePassword(ObjCandidatesAnswerModel, LoginId);
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Change Password page while updating New password marks for specific User: Method Name: ChangePassword()");
                throw;
            }
        }

        private static string ValidateChangePassword(ChangePasswordRequestModel ObjChangePasswordRequestModel)
        {

            Regex regAlphabets = new Regex("(?=.*[a-z])(?=.*[A-Z])");
            Regex regAlphNum = new Regex(@"(?=.*\d)(?=.*[a-zA-Z])");
            Regex regAlphaSpecial = new Regex(@"(?=.*\W)(?=.*[a-zA-Z])");
            Regex specialcharNumber = new(@"^(?=.*\d)(?=.*\W)");

            var matchregAlphabets = regAlphabets.IsMatch(ObjChangePasswordRequestModel.Newpassword);
            var matchregAlphNum = regAlphNum.IsMatch(ObjChangePasswordRequestModel.Newpassword);
            var matchregAlphaSpecial = regAlphaSpecial.IsMatch(ObjChangePasswordRequestModel.Newpassword);
            var matchSpecialcharNumber = specialcharNumber.IsMatch(ObjChangePasswordRequestModel.Newpassword);


            string status = string.Empty;
            if (string.IsNullOrEmpty(ObjChangePasswordRequestModel.Oldpassword.Trim()) ||
                string.IsNullOrEmpty(ObjChangePasswordRequestModel.Newpassword.Trim()) ||
                string.IsNullOrEmpty(ObjChangePasswordRequestModel.Cnfnewpassword.Trim()))
            {
                status = "SERROR";

            }

            if (ObjChangePasswordRequestModel.Newpassword.Trim() != ObjChangePasswordRequestModel.Cnfnewpassword.Trim())
            {
                status = "SERROR";
            }
            if ((ObjChangePasswordRequestModel.Newpassword.Length < 12 || ObjChangePasswordRequestModel.Newpassword.Length > 50) || (ObjChangePasswordRequestModel.Cnfnewpassword.Length < 12 || ObjChangePasswordRequestModel.Cnfnewpassword.Length > 50))
            {
                status = "SERROR";
            }
            if (ObjChangePasswordRequestModel.Newpassword != ObjChangePasswordRequestModel.Cnfnewpassword)
            {
                status = "SERROR";
            }

            if (!matchregAlphabets && !matchregAlphNum && !matchregAlphaSpecial && !matchSpecialcharNumber)
            {
                status = "SERROR";
            }

            return status;
        }

        /// <summary>
        /// Generate refresh token
        /// </summary>
        /// <param name="refToken"></param>
        /// <param name="jwttoken"></param>
        /// <param name="ipAddress"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public AuthenticateResponseModel RefreshToken(string refToken, string jwttoken, string ipAddress, long projectid, string csrftoken)
        {
            AuthenticateResponseModel authenticateResponseModel;
            try
            {
                logger.LogDebug($"AuthService > RefreshToken() method started {refToken}");
                UserLoginTokenModel refreshToken = userRepository.GetRefreshToken(refToken, jwttoken);

                // return null if no user found with token
                if (refreshToken == null) return null;

                if (projectid == 0)
                {
                    projectid = refreshToken.ProjectId == null ? 0 : (long)refreshToken.ProjectId;
                }
                else if (projectid == -1)
                {
                    projectid = 0;
                }
                //Get user context from the User details from the refresh token
                UserContext usercontext = userRepository.GetUserContext(refreshToken.LoginID, projectid);

                //Generate new jwt and refresh token
                authenticateResponseModel = GenerareAuthToken(usercontext, refreshToken.SessionId, ipAddress, refreshToken, projectid);

                logger.LogDebug($"AuthService > RefreshToken() method completed {refToken}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $" Error in AuthService > RefreshToken() method {refToken}");
                throw;
            }
            return authenticateResponseModel;
        }

        /// <summary>
        /// Function to genrate jwt and refresh token for given user details
        /// </summary>
        /// <param name="usercontext"></param>
        /// <param name="sessionId"></param>
        /// <param name="IpAddress"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private AuthenticateResponseModel GenerareAuthToken(UserContext usercontext, string sessionId, string IpAddress, UserLoginTokenModel refreshToken = null, long projectId = 0)
        {
            AuthenticateResponseModel authenticateResponseModel = null;

            try
            {

                #region Generate Jwt Token
                if (usercontext == null)
                {
                    return authenticateResponseModel;
                }
                TokenUserContext tokenUserContext = new()
                {
                    EmailId = usercontext.EmailId,
                    LoginId = usercontext.LoginId,
                    SessionId = sessionId,
                    UserId = usercontext.UserId
                };
                if (usercontext.TimeZone != null)
                {
                    tokenUserContext.TimeZone = new TokenLibrary.UserTimeZone
                    {
                        TimeZoneName = usercontext.TimeZone.TimeZoneName,
                        TimeZoneCode = usercontext.TimeZone.TimeZoneCode,
                        BaseUTCOffsetInMin = usercontext.TimeZone.BaseUTCOffsetInMin
                    };
                }
                if (usercontext.CurrentRole != null)
                {
                    tokenUserContext.CurrentRole = new TokenLibrary.UserRole
                    {
                        IsKp = usercontext.CurrentRole.IsKp,
                        ProjectId = usercontext.CurrentRole.ProjectId,
                        ProjectUserRoleID = usercontext.CurrentRole.ProjectUserRoleID,
                        RoleCode = usercontext.CurrentRole.RoleCode,
                        RoleId = usercontext.CurrentRole.RoleId,
                        RoleName = usercontext.CurrentRole.RoleName,
                        RoleType = (TokenLibrary.UserRoleType)usercontext.CurrentRole.RoleType
                    };
                }
                TokenJwtOptions tokenJwtOptions = new()
                {
                    EncryptionAlgorithmKey = AppOptions.AppSettings.EncryptionAlgorithmKey,
                    RefreshTokenValidityInMinutes = jwtOptions.RefreshTokenValidityInMinutes,
                    Secret = jwtOptions.Secret,
                    TokenValidityInMinutes = jwtOptions.TokenValidityInMinutes,
                    ValidAudience = jwtOptions.ValidAudience,
                    ValidIssuer = jwtOptions.ValidIssuer
                };

                //Generate jwt token
                var jwtTokenres = TokenGenerator.GetToken(tokenUserContext, TokenCallerType.JWT, tokenJwtOptions);

                // Save refresh token to database
                if (jwtTokenres != null)
                {
                    UserLoginTokenModel updateToken = new()
                    {
                        SessionId = sessionId,
                        CreatedBy = usercontext.UserId,
                        CreatedDate = DateTime.UtcNow,
                        CsrfToken = null,
                        Expires = DateTime.UtcNow.AddMinutes(jwtOptions.RefreshTokenValidityInMinutes),
                        IpAddress = IpAddress,
                        IsActive = true,
                        IsDeleted = false,
                        IsExpired = false,
                        JwtToken = jwtTokenres.Token,
                        LoginID = tokenUserContext.LoginId,
                        RefreshToken = jwtTokenres.RefreshToken,
                        UserID = tokenUserContext.UserId
                    };

                    if (refreshToken != null)
                    {
                        updateToken.ReplacedByToken = refreshToken.JwtToken;
                        updateToken.Revoked = DateTime.UtcNow;
                        updateToken.RevokedBy = refreshToken.UserID;
                    }
                    if (usercontext != null && usercontext.CurrentRole != null && usercontext.CurrentRole.ProjectId > 0)
                    {
                        updateToken.ProjectId = usercontext.CurrentRole.ProjectId;

                    }
                    userRepository.UpdateRefreshToken(updateToken, IpAddress, false, projectId);

                    var role = usercontext.CurrentRole?.RoleCode;

                    authenticateResponseModel = new()
                    {
                        Token = jwtTokenres.Token,
                        RefreshToken = jwtTokenres.RefreshToken,
                        RefreshInterval = jwtTokenres.RefreshInterval,
                        Roles = usercontext.CurrentRole != null && usercontext.CurrentRole.IsKp ? role + ",KP" : role,
                        SessionKey = usercontext.SessionId,
                        RefKey = jwtTokenres.RefKey
                    };

                }

                #endregion
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $" Error in AuthService > GenerareAuthToken()");
                throw;
            }



            return authenticateResponseModel;
        }


        public bool RevokeToken(string token, string jwttoken, string ipAddress, bool IsLogout)
        {
            try
            {
                logger.LogDebug($"AuthService > RevokeToken() method started token = {token}");

                UserLoginTokenModel refreshToken = userRepository.GetRefreshToken(token, jwttoken);
                if (refreshToken != null)
                {
                    refreshToken.IsExpired = true;
                    refreshToken.Revoked = DateTime.UtcNow;
                    refreshToken.IsExpired = true;
                    refreshToken.IsActive = false;
                    refreshToken.IpAddress = ipAddress;
                    refreshToken.JwtToken = jwttoken;
                    refreshToken.IsDeleted = true;
                    userRepository.UpdateRefreshToken(refreshToken, ipAddress, IsLogout);
                }
                logger.LogDebug($"AuthService > RevokeToken() method completed token = {token}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $" Error in AuthService > RevokeToken() method token= {token}");
            }
            return true;
        }

        public UserLoginToken IsTokenValid(string jwttoken, string refToken, string refAccessToken)
        {
            UserLoginToken userToken = null;
            try
            {
                logger.LogDebug($"AuthService > IsTokenValid() method started token = {jwttoken}");

                EncryptDecryptAes.StrEncryptionKey = AppOptions.AppSettings.EncryptionAlgorithmKey;
                var userid = EncryptDecryptAes.DecryptStringAES(refAccessToken);

                userToken = userRepository.IsTokenValid(jwttoken, refToken, Convert.ToInt64(userid));

                logger.LogDebug($"AuthService > IsTokenValid() method completed token = {jwttoken}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $" Error in AuthService > IsTokenValid() method token= {jwttoken}");

            }
            return userToken;
        }

        public bool IsValidProject(long ProjectId, long ProjectUserRoleId)
        {
            bool status;
            logger.LogDebug($"AuthService > IsValidProject() method started ProjectId = {ProjectId}, ProjectUserRoleId = {ProjectUserRoleId}");

            status = userRepository.IsValidProject(ProjectId, ProjectUserRoleId);

            logger.LogDebug($"AuthService > IsValidProject() method completed ProjectId = {ProjectId}, ProjectUserRoleId = {ProjectUserRoleId}");
            return status;
        }

        public bool IsValidProjectQig(long ProjectId, long ProjectUserRoleId, long QigId, bool? IsKp = null)
        {
            bool status;
            logger.LogDebug($"AuthService > IsValidProjectQig() method started ProjectId = {ProjectId}, ProjectUserRoleId = {ProjectUserRoleId}");

            status = userRepository.IsValidProjectQig(ProjectId, ProjectUserRoleId, QigId, IsKp);

            logger.LogDebug($"AuthService > IsValidProjectQig() method completed ProjectId = {ProjectId}, ProjectUserRoleId = {ProjectUserRoleId}");
            return status;
        }

        public bool IsValidProjectQigScript(long ProjectId, long QigId, long ScriptId)
        {
            bool status;
            logger.LogDebug($"AuthService > IsValidProjectQigScript() method started ProjectId = {ProjectId}, QigId = {QigId} , ScriptId = {ScriptId}");

            status = userRepository.IsValidProjectQigScript(ProjectId, QigId, ScriptId);

            logger.LogDebug($"AuthService > IsValidProjectQigScript() method completed  ProjectId = {ProjectId}, QigId = {QigId} , ScriptId = {ScriptId}");
            return status;
        }

        /// <summary>
        /// Validates the given user details, generates a new password.
        /// </summary>
        /// <param name="objForgotpassword"></param>
        /// <param name="CurrentProjUserRoleId"></param>
        /// <returns></returns>
        public async Task<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel objForgotpassword, long CurrentProjUserRoleId)
        {
            logger.LogInformation("AutomaticQuestions Service >> UpdateModerateScore() started");
            try
            {
                objForgotpassword = await ValidateForgotPasswordAsync(objForgotpassword);
                string sessionId = Guid.NewGuid().ToString();
                if (objForgotpassword != null && objForgotpassword.status == "S001")
                {
                    objForgotpassword.Newpassword = TokenLibrary.EncryptDecrypt.Hmac.Hashing.GetHash(AppOptions.AppSettings.ChangePasswords.EncryptionKey, objForgotpassword.Newpassword);
                    objForgotpassword.Cnfnewpassword = TokenLibrary.EncryptDecrypt.Hmac.Hashing.GetHash(AppOptions.AppSettings.ChangePasswords.EncryptionKey, objForgotpassword.Cnfnewpassword);
                    objForgotpassword.SessionKey = sessionId;

                    return await userRepository.ForgotPassword(objForgotpassword, CurrentProjUserRoleId);
                }
                return objForgotpassword;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthenticateController page while updating forgot password for specific Users : Method Name : ForgotPassword()");
                throw;
            }
        }

        private async Task<ForgotPasswordModel> ValidateForgotPasswordAsync(ForgotPasswordModel ObjForgotPasswordRequestModel)
        {
            ObjForgotPasswordRequestModel = await userRepository.GetUserId(ObjForgotPasswordRequestModel);
            Regex regAlphabets = new("(?=.*[a-z])(?=.*[A-Z])");
            Regex regAlphNum = new(@"(?=.*\d)(?=.*[a-zA-Z])");
            Regex regAlphaSpecial = new(@"(?=.*\W)(?=.*[a-zA-Z])");
            Regex specialcharNumber = new(@"^(?=.*\d)(?=.*\W)");

            var matchregAlphabets = regAlphabets.IsMatch(ObjForgotPasswordRequestModel.Newpassword);
            var matchregAlphNum = regAlphNum.IsMatch(ObjForgotPasswordRequestModel.Newpassword);
            var matchregAlphaSpecial = regAlphaSpecial.IsMatch(ObjForgotPasswordRequestModel.Newpassword);
            var matchSpecialcharNumber = specialcharNumber.IsMatch(ObjForgotPasswordRequestModel.Newpassword);


            if (string.IsNullOrEmpty(ObjForgotPasswordRequestModel.Newpassword.Trim()) ||
                string.IsNullOrEmpty(ObjForgotPasswordRequestModel.Cnfnewpassword.Trim()) ||
                string.IsNullOrEmpty(ObjForgotPasswordRequestModel.NRIC.Trim()) ||
                string.IsNullOrEmpty(ObjForgotPasswordRequestModel.LoginID.Trim()))
            {
                ObjForgotPasswordRequestModel.status = "SERROR";

            }

            else if (ObjForgotPasswordRequestModel.Newpassword.Trim() != ObjForgotPasswordRequestModel.Cnfnewpassword.Trim())
            {
                ObjForgotPasswordRequestModel.status = "U004";
            }

            else if ((ObjForgotPasswordRequestModel.Newpassword.Length < 12 || ObjForgotPasswordRequestModel.Newpassword.Length > 50) || (ObjForgotPasswordRequestModel.Cnfnewpassword.Length < 12 || ObjForgotPasswordRequestModel.Cnfnewpassword.Length > 50))
            {
                ObjForgotPasswordRequestModel.status = "SERROR";
            }
            else if (ObjForgotPasswordRequestModel.Newpassword != ObjForgotPasswordRequestModel.Cnfnewpassword)
            {
                ObjForgotPasswordRequestModel.status = "SERROR";
            }

            else if (!matchregAlphabets && !matchregAlphNum && !matchregAlphaSpecial && !matchSpecialcharNumber)
            {
                ObjForgotPasswordRequestModel.status = "SERROR";
            }
            #region Call the captcha valid sp and validate. 
            if (!AppOptions.AppSettings.IsCaptchaEnabled)
            {
                return ObjForgotPasswordRequestModel;
            }

            else if (string.IsNullOrEmpty(ObjForgotPasswordRequestModel.GUID) || string.IsNullOrEmpty(ObjForgotPasswordRequestModel.CaptchaText))
            {
                ObjForgotPasswordRequestModel.status = "INVALCAP";
            }
            else if (ObjForgotPasswordRequestModel.CaptchaText.Length != 4)
            {
                ObjForgotPasswordRequestModel.status = "INVALCAP";
            }
            else
            {
                ForgotPasswordModel IsValidateCaptcha = await userRepository.IsValidateCaptcha(ObjForgotPasswordRequestModel);
                if (IsValidateCaptcha.status != "S001")
                {
                    ObjForgotPasswordRequestModel.status = "INVALCAP";
                }
            }
            #endregion
            return ObjForgotPasswordRequestModel;
        }
        private  async Task<ForgotPasswordModel> ValidateCaptcha(ForgotPasswordModel ObjForgotPasswordRequestModel)
        {
            
            ForgotPasswordModel IsValidateCaptcha = await  userRepository.IsValidateCaptcha(ObjForgotPasswordRequestModel);
            if (IsValidateCaptcha.status != "S001")
            {
                ObjForgotPasswordRequestModel.status = "INVALCAP";
            }

            return ObjForgotPasswordRequestModel;
        }
            public async Task<string> ActivateorDeactivateUser(long userid, long activetype, long projectId)
        {
            logger.LogDebug($"AuthService ActivateorDeactivateUser() method started.");

            try
            {
                return await userRepository.ActivateorDeactivateUser(userid, activetype, projectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error  while Activating/Deactivating the suer for specific user : method name: ActivateorDeactivateUser()");
                throw;
            }
        }

        public async Task<CaptchaModel> CreateCaptcha()
        {
            logger.LogInformation("AutomaticQuestions Service >> UpdateModerateScore() started");
            try
            {
                CaptchaModel captchaModel = null;
                if (AppOptions.AppSettings.IsCaptchaEnabled)
                {
                    string CaptchText = CreateCaptchaText();
                    var captchaGuid = await userRepository.CreateCaptcha(CaptchText);

                    string Imagestting = GenerateBase64CaptchaImage(CaptchText, 150, 40, 20);

                    captchaModel = new()
                    {
                        CaptchaImage = Imagestting,
                        GUID = captchaGuid.GUID,
                    };
                }
                return captchaModel;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthenticateController page while updating forgot password for specific Users : Method Name : ForgotPassword()");
                throw;
            }
        }

        private static string GenerateBase64CaptchaImage(string text, int width, int height, int fontSize)
        {
            using (var bitmap = new SKBitmap(width, height))
            {
                using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
                {
                    var canvas = surface.Canvas;
                    canvas.Clear(SKColors.White);

                    using (var paint = new SKPaint())
                    {
                        paint.TextSize = fontSize;
                        paint.IsAntialias = true;
                        paint.Color = SKColors.Black;

                        using (var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.BoldItalic))
                        {
                            paint.Typeface = typeface;
                            paint.TextAlign = SKTextAlign.Center;

                            var textBounds = new SKRect();
                            paint.MeasureText(text, ref textBounds);

                            var x = (width - textBounds.Width) / 2;
                            var y = (height - textBounds.Height) / 2 + Math.Abs(textBounds.Top);

                            canvas.DrawText(text, x, y, paint);
                        }
                    }

                    using (var borderPaint = new SKPaint())
                    {
                        borderPaint.Style = SKPaintStyle.Stroke;
                        borderPaint.Color = SKColors.DarkRed;
                        borderPaint.StrokeWidth = 2;

                        var borderRect = SKRect.Create(0, 0, width, height);
                        canvas.DrawRect(borderRect, borderPaint);
                    }

                    using (var image = surface.Snapshot())
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var stream = new MemoryStream())
                    {
                        data.SaveTo(target: stream);
                        var imageBytes = stream.ToArray();
                        var base64String = Convert.ToBase64String(imageBytes);
                        return base64String;
                    }
                }
            }
        }

        private static string CreateCaptchaText()
        {
            const string capitalLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string smallLetters = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "1234567890";

            Random random = new();
            StringBuilder captchaTextBuilder = new();

            captchaTextBuilder.Append(capitalLetters[random.Next(0, capitalLetters.Length)]);
            captchaTextBuilder.Append(numbers[random.Next(0, numbers.Length)]);
            captchaTextBuilder.Append(smallLetters[random.Next(0, smallLetters.Length)]);
            captchaTextBuilder.Append(capitalLetters[random.Next(0, capitalLetters.Length)]);

            string captchaText = captchaTextBuilder.ToString();
            char[] captchaChars = captchaText.ToCharArray();

            for (int i = captchaChars.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                char temp = captchaChars[i];
                captchaChars[i] = captchaChars[j];
                captchaChars[j] = temp;
            }

            return new string(captchaChars);
        }

        public bool IsValidProjectQigUser(long ProjectId, long ProjectUserRoleId, long QigId)
        {
            bool status;
            logger.LogDebug($"AuthService > IsValidProjectQigUser() method started ProjectId = {ProjectId}, ProjectUserRoleId = {ProjectUserRoleId}");

            status = userRepository.IsValidProjectQigUser(ProjectId, ProjectUserRoleId, QigId);

            logger.LogDebug($"AuthService > IsValidProjectQigUser() method completed ProjectId = {ProjectId}, ProjectUserRoleId = {ProjectUserRoleId}");
            return status;
        }



        /// <summary>
        /// To call Token Generation For Emarking SSOArchive 
        /// </summary>
        /// <param name="emarkingSsoRequest"></param>
        /// <returns></returns>
        public string EmarkingSSOArchive(EmarkingSsoRequest emarkingSsoRequest)
        {

            string Status = "";
            string JwtToken = "";
            Status = userRepository.CheckEmarkingUser(emarkingSsoRequest);
            if (Status != "E002")
            {
                JwtToken = AppOptions.AppSettings.SSOemarkings.SSOForEmarkingArchive;
                JwtToken += GenerateToken(emarkingSsoRequest);
            }
            return JwtToken;

        }


        /// <summary>
        /// To Call Token Generation For Emarking Live
        /// </summary>
        /// <param name="emarkingSsoRequest"></param>
        /// <returns></returns>
        public string EmarkingSSOLive(EmarkingSsoRequest emarkingSsoRequest)
        {

            string Status = "";
            string JwtToken = "";
            Status = userRepository.CheckEmarkingUser(emarkingSsoRequest);
            if (Status != "E002")
            {
                JwtToken = AppOptions.AppSettings.SSOemarkings.SSOForEmarking;
                JwtToken += GenerateToken(emarkingSsoRequest);
            }
            return JwtToken;

        }





        /// <summary>
        /// To Validate Token For SSOArchive Login.
        /// </summary>
        /// <param name="SsoJwtToken"></param>
        /// <param name="ssoIntegrationOptions"></param>
        /// <returns></returns>
        public string ValidateSsoArchiveToken(string SsoJwtToken, SsoIntegrationOptions ssoIntegrationOptions)
        {
            string loginId = string.Empty;
            logger.LogDebug("AuthService > ValidateSsoToken() method started SsoJwtToken = {SsoJwtToken}", SsoJwtToken);

            try
            {
                TokenResponse tokenresp = JsonConvert.DeserializeObject<TokenResponse>(ValidateToken(SsoJwtToken, ssoIntegrationOptions.SsoJwtOptions.Secret));
                if (tokenresp != null)
                {
                    loginId = tokenresp.Token;
                }
                logger.LogDebug("AuthService > ValidateSsoToken() method started SsoJwtToken = {SsoJwtToken}, loginId = {loginId}", SsoJwtToken, loginId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "JwtSso AuthService > ValidateSsoToken() method. Login failed for enc : {SsoJwtToken}", SsoJwtToken);
                return null;
            }

            logger.LogDebug("JwtSso AuthService > ValidateSsoToken()  method completed SsoJwtToken = {SsoJwtToken}", SsoJwtToken);
            return loginId;
        }






      
        /// <summary>
        /// To Generate Token in ServiceLayer.
        /// </summary>
        /// <param name="emarkingSsoRequest"></param>
        /// <returns></returns>

        private string GenerateToken(EmarkingSsoRequest emarkingSsoRequest)
        {
            string acessToken = "";
            try
            {
                string jwtSecretKey = AppOptions.AppSettings.SSOemarkings.JWT_SecretKey.ToString();
                int jwtTokenTimeout = Convert.ToInt16(AppOptions.AppSettings.SSOemarkings.JWT_TokenTimeOut);
                bool _isStopWatchEnabled = Convert.ToBoolean(AppOptions.AppSettings.SSOemarkings.IsStopWatchEnabled);

                Dictionary<string, object> _dicObject = new Dictionary<string, object>
                {
                    { "Token", emarkingSsoRequest.UserName }
                };
                acessToken = GetToken(_dicObject, jwtSecretKey, jwtTokenTimeout, _isStopWatchEnabled);

            }
            catch (Exception)
            {

                throw;
            }
            return acessToken;

        }


        /// <summary>
        /// To Generate Token For SSOArchive Login
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="secretKey"></param>
        /// <param name="TokenTimeout"></param>
        /// <param name="IsStopWatchEnabled"></param>
        /// <returns></returns>
        private static string GetToken(Dictionary<string, object> payload, string secretKey, double TokenTimeout, bool IsStopWatchEnabled = false)
        {

            string token = string.Empty;
            try
            {
                string payLoadString = JsonConvert.SerializeObject(payload);
               
                if (payload != null && payload.Count > 0)
                {
                    var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var now = Math.Round((DateTime.UtcNow.AddSeconds(TokenTimeout) - unixEpoch).TotalSeconds);
                  
                    payload.Add("exp", now);
                    token =Encode(payload, secretKey, JwtHashAlgorithm.HS256);

                  
                }
            }
            catch (Exception ex)
            {
                throw ex;

                
            }
            
            return token;
        }

        /// <summary>
        /// For Encode Generated Token For SSOArchive
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="key"></param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        private static string Encode(object payload, string key, JwtHashAlgorithm algorithm)
        {
            return Encode(new Dictionary<string, object>(), payload, Encoding.UTF8.GetBytes(key), algorithm);
        }





        /// <summary>
        /// Method For Encode Generated Token For SSOArchive.
        /// </summary>
        /// <param name="extraHeaders"></param>
        /// <param name="payload"></param>
        /// <param name="key"></param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        private static string Encode(IDictionary<string, object> extraHeaders, object payload, byte[] key, JwtHashAlgorithm algorithm)
        {
            var segments = new List<string>();
            var header = new Dictionary<string, object>(extraHeaders)
            {
                { "typ", "JWT" },
                { "alg", algorithm.ToString() }
            };

            byte[] headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header));
            byte[] payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));

            segments.Add(Base64UrlEncode(headerBytes));
            segments.Add(Base64UrlEncode(payloadBytes));

            var stringToSign = string.Join(".", segments.ToArray());

            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            byte[] signature = HashAlgorithms[algorithm](key, bytesToSign);
            segments.Add(Base64UrlEncode(signature));

            return string.Join(".", segments.ToArray());
        }


       

        /// <summary>
        /// To Convert Base 64  EncodeFormat.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
            private static string Base64UrlEncode(byte[] input)
            {
                var output = Convert.ToBase64String(input);
                output = output.Split('=')[0]; // Remove any trailing '='s
                output = output.Replace('+', '-'); // 62nd char of encoding
                output = output.Replace('/', '_'); // 63rd char of encoding
                return output;
            }

        /// <summary>
        /// To Convert Base 64 Decode Format.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

            // from JWT spec
            private static byte[] Base64UrlDecode(string input)
            {
                var output = input;
                output = output.Replace('-', '+'); // 62nd char of encoding
                output = output.Replace('_', '/'); // 63rd char of encoding
                switch (output.Length % 4) // Pad with trailing '='s
                {
                    case 0: break; // No pad chars in this case
                    case 2: output += "=="; break; // Two pad chars
                    case 3: output += "="; break;  // One pad char
                    default: throw new Exception("Illegal base64url string!");
                }
                var converted = Convert.FromBase64String(output); // Standard base64 decoder
                return converted;
            }




        /// <summary>
        /// To Validate Token for Archival Login For SSO in Service Layer.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secretKey"></param>
        /// <param name="IsStopWatchEnabled"></param>
        /// <returns></returns>
        public static string ValidateToken(string token, string secretKey, bool IsStopWatchEnabled = false)
        {
            string jsonPayload = string.Empty;
            try
            {
                jsonPayload = Decode(token, secretKey);
            }
            catch (Exception ex)
            {
                jsonPayload = "";
                throw ex;
            }
            return jsonPayload;
        }


        /// <summary>
        /// Calling For Decrypting the Token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="key"></param>
        /// <param name="verify"></param>
        /// <returns></returns>
        private static string Decode(string token, string key, bool verify = true)
        {
            return Decode(token, Encoding.UTF8.GetBytes(key), verify);
        }


        /// <summary>
        /// Method For Decrypting Token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="key"></param>
        /// <param name="verify"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static string Decode(string token, byte[] key, bool verify = true)
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Token must consist from 3 delimited by dot parts");
            }
            var header = parts[0];
            var payload = parts[1];
            var crypto = Base64UrlDecode(parts[2]);

            var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));

            var headerData = JsonConvert.DeserializeObject<Dictionary<string, object>>(headerJson);

            if (verify)
            {
                var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
                var algorithm = (string)headerData["alg"];

                var signature = HashAlgorithms[GetHashAlgorithm(algorithm)](key, bytesToSign);
                var decodedCrypto = Convert.ToBase64String(crypto);
                var decodedSignature = Convert.ToBase64String(signature);

                Verify(decodedCrypto, decodedSignature, payloadJson);
            }

            return payloadJson;
        }

        /// <summary>
        /// Method For Verify the Token For SSO Archive Login
        /// </summary>
        /// <param name="decodedCrypto"></param>
        /// <param name="decodedSignature"></param>
        /// <param name="payloadJson"></param>
        /// <exception cref="SignatureVerificationException"></exception>

        private static void Verify(string decodedCrypto, string decodedSignature, string payloadJson)
        {
            if (decodedCrypto != decodedSignature)
            {
                throw new SignatureVerificationException(string.Format("Invalid signature. Expected {0} got {1}", decodedCrypto, decodedSignature));
            }

            var payloadData = JsonConvert.DeserializeObject<Dictionary<string, object>>(payloadJson);
            if (payloadData.ContainsKey("exp") && payloadData["exp"] != null)
            {
                // safely unpack a boxed int 
                int exp;
                try
                {
                    exp = Convert.ToInt32(payloadData["exp"]);
                }
                catch (Exception)
                {
                    throw new SignatureVerificationException("Claim 'exp' must be an integer.");
                }
                var secondsSinceEpoch = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
                if (secondsSinceEpoch >= exp)
                {
                    throw new SignatureVerificationException("Token has expired.");
                }
            }
        }

        /// <summary>
        /// To Get Hash Algorithm.
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        /// <exception cref="SignatureVerificationException"></exception>

        private static JwtHashAlgorithm GetHashAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                case "HS256": return JwtHashAlgorithm.HS256;
                case "HS384": return JwtHashAlgorithm.HS384;
                case "HS512": return JwtHashAlgorithm.HS512;
                default: throw new SignatureVerificationException("Algorithm not supported.");
            }
        }

        

    }
}
