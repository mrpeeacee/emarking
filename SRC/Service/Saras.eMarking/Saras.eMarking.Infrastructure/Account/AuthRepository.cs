using Exceptionless.DateTimeExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Security;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Auth;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Saras.eMarking.Infrastructure
{
    /// <summary>
    /// AuthRepository class
    /// </summary>
    public class AuthRepository : BaseRepository<AuthRepository>, IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        readonly IAuthService AccountService;
        public AppOptions AppOptions { get; set; }

        public AuthRepository(ApplicationDbContext context, ILogger<AuthRepository> _logger, AppOptions appOptions) : base(_logger)
        {
            _context = context;
            AppOptions = appOptions;
        }

        /// <summary>
        /// Authenticate user and return user details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserContext Authenticate(AuthenticateRequestModel model, bool isSso = false)
        {
            UserContext userContext = null;
            string status = string.Empty;

            try
            {
                logger.LogDebug($"AuthRepository > Authenticate() method started {model}");

                using (SqlConnection sqlCon = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Marking.USPValidateUserPassword", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@LoginID", SqlDbType.NVarChar).Value = model.Loginname;
                        sqlCmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = model.Password;
                        sqlCmd.Parameters.Add("@NoOfAttempts", SqlDbType.SmallInt).Value = AppOptions.AppSettings.ChangePasswords.NoOfAttemps;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                        sqlCmd.Parameters.Add("@IsSSO", SqlDbType.Bit).Value = isSso;
                        sqlCmd.Parameters.Add("@NoOfMinutes", SqlDbType.SmallInt).Value = AppOptions.AppSettings.ChangePasswords.UserSuspendedTime;
                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                        UserInfo userlogin = _context.UserInfos.SingleOrDefault(x => x.LoginId == model.Loginname && !x.IsDeleted);
                                         
                        if (status == "S001" || status == "E001")
                        {



                            UserInfo user = new();
                            if (isSso != true)
                            {
                                user = _context.UserInfos.SingleOrDefault(x => x.LoginId == model.Loginname && x.Password == model.Password && !x.IsDeleted);
                            }
                            else
                            {
                                user = _context.UserInfos.SingleOrDefault(x => x.LoginId == model.Loginname && !x.IsDeleted);

                            }


                            if (user == null) return null;
                            _context.Entry(user).Reload();
                            userContext = new UserContext
                            {
                                EmailId = user.EmailId,
                                LoginId = user.LoginId,
                                UserId = user.UserId,
                                IsFirstTimeLoggedIn = user.IsFirstTimeLoggedIn,
                                Status = status,
                                LastLoginDate = DateTime.UtcNow.ToString(),
                                LoginCount = (byte)(status=="S001"?0:user.LoginCount),
                            };

                            userContext.TimeZone = GetUserTimeZone(userContext.UserId);

                            user.LastLoginDate = DateTime.UtcNow;
                            _context.UserInfos.Update(user);
                            _context.SaveChanges();
                        }
                        else if (status == "E005")
                        {
                            if (userlogin == null)
                            {
                                userContext = new UserContext
                                {
                                    Status = status,
                                    LoginId = model.Loginname
                                };
                            }
                            else
                            {
                                userContext = new UserContext
                                {
                                    Status = status,
                                    LoginId = userlogin.LoginId,
                                    UserId = userlogin.UserId
                                };
                            }
                        }
                        else if(status=="E006")
                        {
                            if (userlogin == null) return null;
                            _context.Entry(userlogin).Reload();
                            userContext = new UserContext
                            {
                                EmailId = userlogin.EmailId,
                                LoginId = userlogin.LoginId,
                                UserId = userlogin.UserId,
                                IsFirstTimeLoggedIn = userlogin.IsFirstTimeLoggedIn,
                                Status = status,
                               
                                LoginCount = (byte)userlogin.LoginCount,
                                LastFailedAttempt=(DateTime)userlogin.LastFailedAttempt,

                            };
                        }
                        else if (status == "E002")
                        {

                            UserInfo user = _context.UserInfos.SingleOrDefault(x => x.LoginId == model.Loginname && !x.IsDeleted);
                            userContext = new UserContext
                            {

                                EmailId = user.EmailId,
                                LoginId = user.LoginId,
                                UserId = user.UserId,
                                IsFirstTimeLoggedIn = user.IsFirstTimeLoggedIn,
                                Status = status,
                                LastLoginDate = DateTime.UtcNow.ToString(),
                                LoginCount = (byte)user.LoginCount,
                            };

                        }

                        else
                        {
                            if (userlogin == null) { return null; }
                            else
                            {
                                userContext = new UserContext
                                {
                                    Status = status,
                                    LoginId = userlogin.LoginId,
                                    UserId = userlogin.UserId
                                };
                            }
                        }
                    }
                }

                logger.LogDebug($"AuthRepository > Authenticate() method completed {model}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, " Error in AuthRepository > Authenticate() method {model}");
                throw;
            }

            return userContext;
        }

        public UserContext GetuserDetails(AuthenticateRequestModel model)
        {
            UserInfo userlogin = _context.UserInfos.SingleOrDefault(x => x.LoginId == model.Loginname && !x.IsDeleted);
            return userlogin == null ? null : new UserContext
            {
                UserId=userlogin.UserId,
                LoginCount = (byte)userlogin.LoginCount,    
                LoginId=userlogin.LoginId,

            };
        }
        /// <summary>
        /// Change Password : this method used to change the password to a particular user
        /// </summary>
        /// <param name="ObjCandidatesAnswerModel"></param>
        /// <param name="LoginId"></param>
        /// <returns></returns>
        public Task<string> ChangePassword(ChangePasswordRequestModel ObjCandidatesAnswerModel, string LoginId)
        {
            string status = "";
            try
            {
                logger.LogDebug($"AuthRepository > ChangePassword() method started {ObjCandidatesAnswerModel}");

                using (SqlConnection sqlCon = new(_context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new("Marking.ChangePassword", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@LoginID", SqlDbType.NVarChar).Value = LoginId;
                        sqlCmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = ObjCandidatesAnswerModel.Newpassword;
                        sqlCmd.Parameters.Add("@OldPassword", SqlDbType.NVarChar).Value = ObjCandidatesAnswerModel.Oldpassword;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                        if (status == "U001")
                        {
                            UserInfo user = _context.UserInfos.SingleOrDefault(x => x.LoginId == LoginId && x.Password == ObjCandidatesAnswerModel.Newpassword && !x.IsDeleted);

                            if (user != null)
                            {
                                user.IsFirstTimeLoggedIn = false;
                                user.PasswordLastModifiedDate = DateTime.UtcNow;
                                _context.UserInfos.Update(user);
                                _context.SaveChanges();
                                status = "U001";
                            }
                        }
                        else
                        {
                            return Task.FromResult(status);
                        }
                    }
                }

                logger.LogDebug($"AuthRepository > ChangePassword() method completed {ObjCandidatesAnswerModel}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AuthRepository->ChangePassword() for specific User and parameters are project: projectId = ProjectID,UserId=CurrentProjUserRoleId");
                throw;
            }
            return Task.FromResult(status);
        }

        /// <summary>
        /// Get User Time Zone
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        private UserTimeZone GetUserTimeZone(long UserId)
        {
            UserTimeZone timeZone = null;
            if (AppOptions.AppSettings.Calendar.TimeZoneFrom == Domain.Configuration.EnumTimeZoneFrom.UserProfile)
            {
                timeZone = (from ustz in _context.UserToTimeZoneMappings
                            join tz in _context.TimeZones on ustz.TimeZoneId equals tz.TimeZoneId
                            where !ustz.IsDeleted && ustz.IsActive == true && !tz.IsDeleted && tz.IsActive == true && ustz.UserId == UserId
                            select new UserTimeZone(null)
                            {
                                BaseUTCOffsetInMin = tz.BaseUtcoffsetInMin,
                                TimeZoneCode = tz.TimeZoneCode,
                                TimeZoneName = tz.TimeZoneName
                            }).FirstOrDefault();
            }

            return timeZone;
        }

        /// <summary>
        /// Update refresh token to database
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public bool UpdateRefreshToken(UserLoginTokenModel token, string ipAddress, bool IsLogout, long projectId = 0)
        {
            try
            {
                logger.LogDebug($"AuthRepository > UpdateRefreshToken() method started {token}");

                if (!IsLogout)
                {
                    //Delete Existing User login token only if any project is selected. This is to fix session expire issue in dashboard
                    if (projectId > 0)
                    {
                        DeleteExistingUserToken(token);
                    }

                    UserLoginToken refreshToken = new()
                    {
                        RefreshToken = token.RefreshToken,
                        UserId = token.UserID,
                        LoginId = token.LoginID,
                        CreatedDate = DateTime.UtcNow,
                        Expires = token.Expires,
                        IsActive = true,
                        IsExpired = false,
                        CreatedBy = token.CreatedBy,
                        IsDeleted = false,
                        JwtToken = token.JwtToken,
                        Ipaddress = ipAddress,
                        SessionId = token.SessionId,
                        CsrfToken = token.CsrfToken,
                        ReplacedByToken = token.ReplacedByToken,
                        Revoked = token.Revoked,
                        RevokedBy = token.RevokedBy,
                        ProjectId = token.ProjectId
                    };
                    _context.UserLoginTokens.Add(refreshToken);
                }
                else
                {
                    var reftokens = _context.UserLoginTokens.Where(x => x.UserId == token.UserID && x.LoginId == token.LoginID).ToList();

                    UserInfo user = _context.UserInfos.SingleOrDefault(x => x.LoginId == token.LoginID && !x.IsDeleted);

                    if (reftokens != null)
                    {
                        user.LastLogoutDate = DateTime.UtcNow;
                        _context.UserInfos.Update(user);
                        _context.SaveChanges();

                        reftokens.ForEach(elem =>
                        {
                            AddUserTokentoArchive(elem);
                        });
                    }
                }
                _context.SaveChanges();
                logger.LogDebug($"AuthRepository > UpdateRefreshToken() method completed token = {token}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthRepository > UpdateRefreshToken() method token = {token}");
                throw;
            }
            return true;
        }

        private void DeleteExistingUserToken(UserLoginTokenModel token)
        {
            var reftokens = _context.UserLoginTokens.Where(x => x.UserId == token.UserID && x.LoginId == token.LoginID && !x.IsDeleted).ToList();
            if (reftokens != null)
            {
                reftokens.ForEach(elem =>
                {
                    elem.IsDeleted = true;
                    elem.IsActive = false;
                    elem.IsExpired = true;
                });
                _context.SaveChanges();
            }
        }

        private void AddUserTokentoArchive(UserLoginToken elem)
        {
            if (!elem.IsDeleted)
            {
                elem.IsDeleted = true;
                elem.IsActive = false;
                elem.IsExpired = true;
                elem.Revoked = DateTime.UtcNow;
            }

            _context.UserLoginTokenArchives.Add(new UserLoginTokenArchive
            {
                CreatedBy = elem.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                CsrfToken = elem.CsrfToken,
                Expires = elem.Expires,
                Ipaddress = elem.Ipaddress,
                SessionId = elem.SessionId,
                IsActive = elem.IsActive,
                IsDeleted = elem.IsDeleted,
                IsExpired = elem.IsExpired,
                JwtToken = elem.JwtToken,
                LoginId = elem.LoginId,
                RefreshToken = elem.RefreshToken,
                ReplacedByToken = elem.ReplacedByToken,
                Revoked = elem.Revoked,
                RevokedBy = elem.RevokedBy,
                TokenId = elem.TokenId,
                UserId = elem.UserId,
                ProjectId = elem.ProjectId
            });
            _context.UserLoginTokens.Remove(elem);
            _context.SaveChanges();
        }

        /// <summary>
        /// Get Refresh token
        /// </summary>
        /// <param name="refreshtoken"></param>
        /// <param name="jwttoken"></param>
        /// <returns></returns>
        public UserLoginTokenModel GetRefreshToken(string refreshtoken, string jwttoken)
        {
            try
            {
                logger.LogDebug($"AuthRepository > GetRefreshToken() method started {refreshtoken}");

                UserLoginToken refreshToken = _context.UserLoginTokens.FirstOrDefault(x => x.RefreshToken == refreshtoken && !x.IsDeleted);
                if (refreshToken == null) return null;

                logger.LogDebug($"AuthRepository > GetRefreshToken() method completed {refreshtoken}");

                // return null if token is no longer active
                return refreshToken.IsActive == false ? null : new UserLoginTokenModel
                {
                    UserID = refreshToken.UserId,
                    LoginID = refreshToken.LoginId,
                    CreatedBy = refreshToken.UserId,
                    CreatedDate = refreshToken.CreatedDate,
                    Expires = refreshToken.Expires,
                    IsDeleted = refreshToken.IsDeleted,
                    ReplacedByToken = refreshToken.ReplacedByToken,
                    Revoked = refreshToken.Revoked,
                    RevokedBy = refreshToken.RevokedBy,
                    RefreshToken = refreshToken.RefreshToken,
                    TokenID = refreshToken.TokenId,
                    IsActive = refreshToken.IsActive,
                    IsExpired = refreshToken.IsExpired,
                    SessionId = refreshToken.SessionId,
                    ProjectId = refreshToken.ProjectId,
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $" Error in AuthRepository > GetRefreshToken() method token = {refreshtoken}");
                return null;
            }
        }

        /// <summary>
        /// Revoke Refresh token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool RevokeToken(string token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get user context for token
        /// </summary>
        /// <param name="LoginID"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public UserContext GetUserContext(string LoginID, long projectid = 0, bool IsArchive = false)
        {
            UserContext userContext = null;
            try
            {
                logger.LogDebug($"AuthRepository > GetUserContext() method started LoginID = {LoginID}, ProjectId = {projectid}");
                if (projectid > 0)
                {
                    userContext = !IsArchive ? GetActUserDtls(LoginID, projectid) : GetArhUserDtls(LoginID, projectid);

                    if (userContext != null && userContext.CurrentRole != null)
                    {
                        userContext.CurrentRole.IsKp = _context.ProjectQigteamHierarchies.Any(a => a.IsActive == true && !a.Isdeleted && a.ProjectId == projectid && a.ProjectUserRoleId == userContext.CurrentRole.ProjectUserRoleID && a.IsKp);
                    }
                }
                else
                {
                    userContext = (from uri in _context.UserToRoleMappings
                                   join u in _context.UserInfos on uri.UserId equals u.UserId
                                   join r in _context.Roleinfos on uri.RoleId equals r.RoleId
                                   where u.LoginId == LoginID && !uri.IsDeleted && !r.Isdeleted && !u.IsDeleted
                                   select new UserContext
                                   {
                                       EmailId = u.EmailId,
                                       LoginId = u.LoginId,
                                       UserId = u.UserId,
                                       CurrentRole = new UserRole
                                       {
                                           RoleId = r.RoleId,
                                           RoleName = r.RoleName,
                                           RoleCode = r.RoleCode,
                                           //ProjectUserRoleID = uri.MappingId,
                                           RoleType = (UserRoleType)r.RoleId,
                                           ProjectId = projectid
                                       }
                                   }).FirstOrDefault();
                }
                if (userContext != null)
                {
                    userContext.TimeZone = userContext.TimeZone = GetUserTimeZone(userContext.UserId);
                }

                logger.LogDebug($"AuthRepository > GetUserContext() method completed LoginID = {LoginID}, ProjectId = {projectid}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthRepository > GetUserContext() method LoginID = {LoginID}, ProjectId = {projectid}");
                return null;
            }
            return userContext;
        }
        private UserContext GetArhUserDtls(string loginID, long projectid)
        {
            UserContext userContext = new UserContext();

            userContext = (from uri in _context.ProjectUserRoleinfoArchives
                           join u in _context.UserInfos on uri.UserId equals u.UserId
                           where !u.IsDeleted
                           join p in _context.ProjectInfos on uri.ProjectId equals p.ProjectId
                           where !p.IsDeleted
                           join r in _context.Roleinfos on uri.RoleId equals r.RoleId
                           where !r.Isdeleted
                           where u.LoginId == loginID && !uri.Isdeleted && uri.ProjectId == projectid && uri.IsActive == true
                           select new UserContext
                           {
                               EmailId = u.EmailId,
                               LoginId = u.LoginId,
                               UserId = u.UserId,
                               CurrentRole = new UserRole
                               {
                                   RoleId = r.RoleId,
                                   RoleName = r.RoleName,
                                   RoleCode = r.RoleCode,
                                   ProjectUserRoleID = uri.ProjectUserRoleId,
                                   RoleType = (UserRoleType)r.RoleId,
                                   IsKp = uri.IsKp == false,
                                   ProjectId = projectid
                               }
                           }).FirstOrDefault();

            return userContext;
        }
        private UserContext GetActUserDtls(string LoginID, long projectid = 0)
        {
            UserContext userContext = new UserContext();

            userContext = (from uri in _context.ProjectUserRoleinfos
                           join u in _context.UserInfos on uri.UserId equals u.UserId
                           where !u.IsDeleted
                           join p in _context.ProjectInfos on uri.ProjectId equals p.ProjectId
                           where !p.IsDeleted
                           join r in _context.Roleinfos on uri.RoleId equals r.RoleId
                           where !r.Isdeleted
                           where u.LoginId == LoginID && !uri.Isdeleted && uri.ProjectId == projectid && uri.IsActive == true
                           select new UserContext
                           {
                               EmailId = u.EmailId,
                               LoginId = u.LoginId,
                               UserId = u.UserId,
                               CurrentRole = new UserRole
                               {
                                   RoleId = r.RoleId,
                                   RoleName = r.RoleName,
                                   RoleCode = r.RoleCode,
                                   ProjectUserRoleID = uri.ProjectUserRoleId,
                                   RoleType = (UserRoleType)r.RoleId,
                                   IsKp = uri.IsKp == false,
                                   ProjectId = projectid
                               }
                           }).FirstOrDefault();

            return userContext;
        }

        /// <summary>
        /// IsToken Valid
        /// </summary>
        /// <param name="jwttoken"></param>
        /// <param name="refToken"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public UserLoginToken IsTokenValid(string jwttoken, string refToken, long userid)
        {
            UserLoginToken userLoginToken = null;
            try
            {
                logger.LogDebug($"AuthRepository > IsTokenValid() method started {jwttoken}");

                var userLoginTokens = _context.UserLoginTokens.Where(x => x.UserId == userid && x.IsExpired == false && DateTime.UtcNow <= x.Expires).ToList();

                if (userLoginTokens != null && userLoginTokens.Count > 0 && userLoginTokens.Exists(a => a.RefreshToken == refToken && a.JwtToken == jwttoken))
                {
                    userLoginToken = userLoginTokens.Find(a => a.RefreshToken == refToken && a.JwtToken == jwttoken);
                }

                logger.LogDebug($"AuthRepository > IsTokenValid() method completed {jwttoken}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthRepository > IsTokenValid() method token = {jwttoken}");
            }
            return userLoginToken;
        }

        /// <summary>
        /// IsValid Project
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <returns></returns>
        public bool IsValidProject(long ProjectId, long ProjectUserRoleId)
        {
            return _context.ProjectUserRoleinfos.Any(a => a.ProjectId == ProjectId && a.ProjectUserRoleId == ProjectUserRoleId && !a.Isdeleted && a.IsActive == true);
        }

        /// <summary>
        /// IsValid Project Qig
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <param name="QigId"></param>
        /// <param name="IsKp"></param>
        /// <returns></returns>
        public bool IsValidProjectQig(long ProjectId, long ProjectUserRoleId, long QigId, bool? IsKp = null)
        {
            bool IsRoleExist;
            if (IsKp != null)
            {
                IsRoleExist = _context.ProjectQigteamHierarchies.Any(a => a.ProjectId == ProjectId && a.ProjectUserRoleId == ProjectUserRoleId && a.Qigid == QigId && !a.Isdeleted && a.IsActive == true && a.IsKp == IsKp);
            }
            else
            {
                IsRoleExist = _context.ProjectQigteamHierarchies.Any(a => a.ProjectId == ProjectId && a.ProjectUserRoleId == ProjectUserRoleId && a.Qigid == QigId && !a.Isdeleted && a.IsActive == true);
            }
            return IsRoleExist;
        }

        /// <summary>
        /// IsValid Project Qig User
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <param name="QigId"></param>
        /// <returns></returns>
        public bool IsValidProjectQigUser(long ProjectId, long ProjectUserRoleId, long QigId)
        {
            bool IsRoleExist;

            IsRoleExist = _context.ProjectQigteamHierarchies.Any(a => a.ProjectId == ProjectId && a.ProjectUserRoleId == ProjectUserRoleId && a.Qigid == QigId && !a.Isdeleted);

            return IsRoleExist;
        }

        /// <summary>
        /// IsValid Project Qig Script
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="QigId"></param>
        /// <param name="ScriptId"></param>
        /// <returns></returns>
        public bool IsValidProjectQigScript(long ProjectId, long QigId, long ScriptId)
        {
            bool IsScriptExist;

            IsScriptExist = _context.ProjectUserScripts.Any(a => a.ProjectId == ProjectId && a.Qigid == QigId && a.ScriptId == ScriptId && !a.Isdeleted);

            return IsScriptExist;
        }

        /// <summary>
        /// Forgot Password : Add last four digits of NRIC number to change the password
        /// </summary>
        /// <param name="objForgotpassword"></param>
        /// <param name="CurrentProjUserRoleId"></param>
        /// <returns></returns>
        public async Task<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel objForgotpassword, long CurrentProjUserRoleId)
        {
            UserInfo objUserInfo;
            UserPwdDetail objUserPwdDetail;
            var forgotPasswordNoofAttempts = AppOptions.AppSettings.ForgotPasswords.ForgotPasswordNoofAttemps;
            try
            {
                var Isuserdisabled = _context.UserInfos
                    .Where(item => item.LoginId == objForgotpassword.LoginID && item.IsDisable && !item.IsDeleted)
                    .FirstOrDefault();
                var suspendeduser = _context.UserInfos
                  .Where(item => item.LoginId == objForgotpassword.LoginID && item.LoginCount==5).FirstOrDefault();
                var utctime = DateTime.UtcNow;
                var lastlogindatetime = (DateTime)suspendeduser.LastFailedAttempt;
                var suspendedtime = lastlogindatetime.AddMinutes(AppOptions.AppSettings.ChangePasswords.UserSuspendedTime);
                TimeSpan timeLeft = suspendedtime - DateTime.UtcNow;
                if (Isuserdisabled != null)
                {
                    objForgotpassword.status = "Disabled";
                }

                else if (suspendeduser != null && suspendeduser.LoginCount==5 && utctime < suspendedtime)
                {
                   
                    
                   
                    if (utctime < suspendedtime)
                    {
                        
                        objForgotpassword.status = "Suspended";
                        objForgotpassword.Timeleft = (double)timeLeft.TotalMinutes;
                    }

                }

                else
                {
                    objUserInfo = _context.UserInfos
                        .Where(item => item.LoginId == objForgotpassword.LoginID && item.Nric.Substring(item.Nric.Length - 4) == objForgotpassword.NRIC && !item.IsDeleted)
                        .FirstOrDefault();

                    if (objUserInfo != null && objUserInfo.IsBlock)
                    {
                        objForgotpassword.status = "BLOCKED";
                    }
                    else if (objUserInfo == null)
                    {
                        objUserInfo = _context.UserInfos
                            .Where(item => item.LoginId == objForgotpassword.LoginID && item.Nric.Substring(item.Nric.Length - 4) != objForgotpassword.NRIC && !item.IsDeleted)
                            .FirstOrDefault();

                        if (objUserInfo == null)
                        {
                            objForgotpassword.status = "InvalidNRICorLoginId";
                            return objForgotpassword;
                        }
                        else
                        {
                            if (objUserInfo.IsBlock || (objUserInfo.ForgotPasswordCount > 0 && objUserInfo.ForgotPasswordCount >= forgotPasswordNoofAttempts))
                            {
                                objUserInfo.IsBlock = true;
                                _context.UserInfos.Update(objUserInfo);
                                _context.SaveChanges();
                                objForgotpassword.status = "BLOCKED";
                            }
                            else
                            {
                                objUserInfo.ForgotPasswordCount = (short)(objUserInfo.ForgotPasswordCount <= 0 ? 1 : objUserInfo.ForgotPasswordCount + 1);
                                objUserInfo.ModifiedDate = DateTime.UtcNow;
                                objUserInfo.ModifiedBy = objUserInfo.UserId;
                                _context.UserInfos.Update(objUserInfo);
                                _context.SaveChanges();
                                objForgotpassword.status = "InvalidNRICorLoginId";
                            }
                        }
                    }
                    else if (objUserInfo != null)
                    {
                        objUserInfo.Password = objForgotpassword.Newpassword;
                        objUserInfo.ModifiedDate = DateTime.UtcNow;
                        objUserInfo.ModifiedBy = objUserInfo.UserId;
                        objUserInfo.ForgotPasswordCount = 0;
                        _context.UserInfos.Update(objUserInfo);
                        _context.SaveChanges();
                        objUserPwdDetail = _context.UserPwdDetails
                            .Where(item => item.UserId == objUserInfo.UserId && item.IsActive == true)
                            .FirstOrDefault();

                        if (objUserPwdDetail != null)
                        {
                            objUserPwdDetail.IsActive = false;
                            objUserPwdDetail.ActivationEnddate = DateTime.UtcNow;
                            _context.UserPwdDetails.Update(objUserPwdDetail);
                            _context.SaveChanges();
                        }

                        UserPwdDetail objUserPwdDetail1 = new UserPwdDetail();
                        objUserPwdDetail1.IsActive = true;
                        objUserPwdDetail1.ActivationStartdate = DateTime.UtcNow;
                        objUserPwdDetail1.UserId = objUserInfo.UserId;
                        objUserPwdDetail1.CreatedDate = DateTime.UtcNow;
                        objUserPwdDetail1.Password = objForgotpassword.Newpassword;

                        _context.UserPwdDetails.Add(objUserPwdDetail1);
                        _context.SaveChanges();
                        objForgotpassword.status = "U001";
                        if (AppOptions.AppSettings.IsCaptchaEnabled)
                        {
                            _ = await UpdateCaptcha(objForgotpassword.GUID);
                        }
                    }
                    else
                    {
                        objForgotpassword.status = "InvalidNRICorLoginId";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthRepository->ForgotPassword() for specific Project and parameters are project: UserId={CurrentProjUserRoleId}");
                throw;
            }
            return objForgotpassword;
        }

        /// <summary>
        /// Activate or Deactivate User
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="activetype"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public async Task<string> ActivateorDeactivateUser(long userid, long activetype, long projectid)
        {
            string status = String.Empty;
            ProjectUserRoleinfo projectuseruoleinfo;
            ProjectQigteamHierarchy projectqigteamHierarchy;

            try
            {
                var IsAo = await (from puri in _context.ProjectUserRoleinfos
                                  join ri in _context.Roleinfos
                                  on puri.RoleId equals ri.RoleId
                                  where ri.RoleCode == "AO" && puri.ProjectId == projectid && puri.IsActive == true
                                  select puri).FirstOrDefaultAsync();

                //var IsAos = _context.ProjectUserRoleinfos.Join(_context.Roleinfos, puri => puri.RoleId, ri=> ri.RoleId,(ri,puri) => new {puri}).Where(k=> k.ri.RoleCode == "AO" && k.puri.proj)

                if (IsAo == null)
                {
                    projectuseruoleinfo = _context.ProjectUserRoleinfos.Where(item => item.ProjectUserRoleId == userid && item.ProjectId == projectid && !item.Isdeleted && item.IsActive == true).FirstOrDefault();
                    projectqigteamHierarchy = _context.ProjectQigteamHierarchies.Where(item => item.ProjectUserRoleId == userid && item.ProjectId == projectid && !item.Isdeleted).FirstOrDefault();

                    if (activetype == 0)
                    {
                        if (projectuseruoleinfo != null)
                        {
                            projectuseruoleinfo.IsActive = false;
                            _context.ProjectUserRoleinfos.Update(projectuseruoleinfo);
                            if (projectqigteamHierarchy != null)
                            {
                                projectqigteamHierarchy.IsActive = false;
                                _context.ProjectQigteamHierarchies.Update(projectqigteamHierarchy);
                            }
                            _context.SaveChanges();
                        }
                    }
                    else if (activetype == 1 && projectuseruoleinfo != null)
                    {
                        projectuseruoleinfo.IsActive = true;
                        _context.ProjectUserRoleinfos.Update(projectuseruoleinfo);
                        if (projectqigteamHierarchy != null)
                        {
                            projectqigteamHierarchy.IsActive = true;
                            _context.ProjectQigteamHierarchies.Update(projectqigteamHierarchy);
                        }
                        _context.SaveChanges();
                    }
                }
                else
                {
                    status = "EOO1";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthRepository > ActivateorDeactivateUser() method token = {projectid}");
            }

            return status;
        }

        /// <summary>
        /// GetUserId : This Method validates the captchtext using captchaGUID
        /// </summary>
        /// <param name="objForgotpassword"></param>
        /// <returns></returns>
        public async Task<ForgotPasswordModel> GetUserId(ForgotPasswordModel ObjForgotPasswordRequestModel)
        {
            try
            {
                var objUserId = _context.UserInfos
                           .Where(item => item.LoginId == ObjForgotPasswordRequestModel.LoginID && !item.IsDeleted)
                           .FirstOrDefault();

                if (objUserId != null)
                {
                    ObjForgotPasswordRequestModel.UserId = objUserId.UserId;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthRepository->GetUserId() for specific Project and parameters are project: ForgotPasswordModel = {ObjForgotPasswordRequestModel}");
                throw;
            }
            await Task.CompletedTask;

            return ObjForgotPasswordRequestModel;
        }

        /// <summary>
        /// IsValidateCaptcha : This Method validates the captchtext using captchaGUID
        /// </summary>
        /// <param name="objForgotpassword"></param>
        /// <returns></returns>
        public async Task<ForgotPasswordModel> IsValidateCaptcha(ForgotPasswordModel ObjForgotPasswordRequestModel)
        {
            ForgotPasswordModel objForgotpassword = ObjForgotPasswordRequestModel;
            try
            {
                logger.LogInformation($"AuthRepository ValidateCaptcha() Method started.  CaptchaguId = {ObjForgotPasswordRequestModel.GUID}");
                using (SqlConnection sqlCon = new(_context.Database.GetDbConnection().ConnectionString))
                {
                    using SqlCommand sqlCmd = new("Marking.USPValidateCaptchaText", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("@CaptchaGUID", SqlDbType.NVarChar).Value = (string)ObjForgotPasswordRequestModel.GUID;
                    sqlCmd.Parameters.Add("@UserCaptcha", SqlDbType.NVarChar).Value = (string)ObjForgotPasswordRequestModel.CaptchaText;
                    sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                    sqlCon.Open();
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                    objForgotpassword.status = Convert.ToString(sqlCmd.Parameters["@Status"].Value);
                }

                logger.LogInformation($"AuthRepository -> ValidateCaptcha() Method ended.  CaptchaguId = {ObjForgotPasswordRequestModel.GUID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AuthRepository->ValidateCaptcha() for specific Project and parameters are project: CaptchaguId = {ObjForgotPasswordRequestModel.GUID}");
                throw;
            }
            await Task.CompletedTask;

            return objForgotpassword;
        }

        /// <summary>
        /// CreateCaptcha : This Method will insert the captch to db and provides captchaGUID
        /// </summary>
        /// <param name="CaptchText"></param>
        /// <returns></returns>
        public Task<CaptchaModel> CreateCaptcha(string CaptchText)
        {
            CaptchaModel CaptchaModels = new();
            try
            {
                logger.LogDebug($"AuthRepository > CreateCaptcha() method started {CaptchText}");

                using (SqlConnection sqlCon = new(_context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new("Marking.USPInsertCaptchaText", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@CaptchaText", SqlDbType.NVarChar).Value = CaptchText;

                        sqlCon.Open();
                        SqlDataReader reader = sqlCmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                CaptchaModels.GUID = reader["Captcha_GUID"] != DBNull.Value ? Convert.ToString(reader["Captcha_GUID"]) : string.Empty;
                            }
                        }
                        if (!reader.IsClosed) { reader.Close(); }

                        sqlCon.Close();
                    }
                }

                logger.LogDebug($"AuthRepository > CreateCaptcha() method completed {CaptchText}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AuthRepository -> CreateCaptcha() for specific User and parameters are project: projectId = ProjectID,UserId=CurrentProjUserRoleId");
                throw;
            }
            return Task.FromResult(CaptchaModels);
        }

        /// <summary>
        /// UpdateCaptcha : This Method will update the captch in db and return the status
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        private async Task<string> UpdateCaptcha(string GUID)
        {
            string status = string.Empty;
            try
            {
                using (SqlConnection sqlCon = new(_context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new("Marking.USPUpdateCaptchaText", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@CaptchaGUID", SqlDbType.NVarChar).Value = GUID;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = Convert.ToString(sqlCmd.Parameters["@Status"].Value);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AuthRepository-> UpdateCaptcha()");
                throw;
            }
            await Task.CompletedTask;

            return status;
        }


        /// <summary>
        /// To Check User Valid or Not
        /// </summary>
        /// <param name="objUser"></param>
        /// <returns></returns>
        public string CheckEmarkingUser(EmarkingSsoRequest objUser)
        {
            string Status = "";
            UserInfo User = new();
            try
            {
                User = _context.UserInfos.Where(t => t.LoginId == objUser.UserName && !t.IsDeleted).FirstOrDefault();
                if (User == null)
                {
                    Status = "E002";
                }

            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Error in AuthRepository-> CheckEmarkingUser()");
                throw;
            }
            return Status;
        }  
        }
}