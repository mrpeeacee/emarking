using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Configuration;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using TokenLibrary.EncryptDecrypt.AES;

namespace Saras.eMarking.Api.Controllers
{
    /// <summary>
    /// Base Api Controller
    /// </summary>
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseApiController<T> : Controller where T : class
    {
        /// <summary>
        /// API PREFIX
        /// </summary>
        public const string API_PREFIX = "api/v2";

        /// <summary>
        /// Appsettings key value
        /// </summary>
        public AppOptions AppOptions { get; set; }

        /// <summary>
        /// Logger instance
        /// </summary>
        public readonly ILogger logger;

        /// <summary>
        /// Audit log service
        /// </summary>
        readonly IAuditService AuditService;

        /// <summary>
        /// BaseApi Controller Constructor
        /// </summary>
        protected BaseApiController(AppOptions _AppOptions, ILogger<T> _logger, IAuditService _auditService = null)
        {
            AppOptions = _AppOptions;
            logger = _logger;
            AuditService = _auditService;
        }

        UserContext context = null;
        protected UserContext CurrentUserContext
        {
            get
            {
                if (context != null)
                    return context;

                try
                {
                    context = GetContextData(context);
                }
                catch
                {
                    // Error Ignored
                }

                return context;
            }
        }

        private UserContext GetContextData(UserContext context)
        {
            var tokenContext = HttpContext.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;
            if (string.IsNullOrEmpty(tokenContext))
                return context;

            EncryptDecryptAes.StrEncryptionKey = AppOptions.AppSettings.EncryptionAlgorithmKey;
            context = JsonSerializer.Deserialize<UserContext>(EncryptDecryptAes.DecryptStringAES(tokenContext));

            if (context != null && context.TimeZone == null)
            {
                if (AppOptions.AppSettings.Calendar.TimeZoneFrom == EnumTimeZoneFrom.UserProfile)
                {
                    context.TimeZone = AppOptions.AppSettings.Calendar.DefaultTimeZone;
                }
                else if (AppOptions.AppSettings.Calendar.TimeZoneFrom == EnumTimeZoneFrom.UserBrowser)
                {
                    string zone = Convert.ToString(HttpContext.Request.Headers["Local-Time-Zone"]);
                    string offst = Convert.ToString(HttpContext.Request.Headers["Local-Time-Offset"]);

                    if (!string.IsNullOrEmpty(zone) && !string.IsNullOrEmpty(offst))
                    {
                        string zoneName = Convert.ToString(DateTimeUtils.OlsonTimeZoneToTimeZoneInfo(zone));
                        context.TimeZone = new UserTimeZone
                        {
                            TimeZoneCode = zoneName,
                            TimeZoneName = zoneName,
                            BaseUTCOffsetInMin = Convert.ToInt32(offst)
                        };
                    }
                }
            }

            return context;
        }


        protected static string SafeText(string textdata)
        {
            return System.Net.WebUtility.HtmlEncode(textdata);
        }

        protected long GetCurrentUserId()
        {
            if (CurrentUserContext != null)
            {
                return CurrentUserContext.UserId;
            }
            else
            {
                return 0;
            }
        }
        protected long GetCurrentProjectId()
        {
            if (CurrentUserContext != null && CurrentUserContext.CurrentRole != null)
            {
                return CurrentUserContext.CurrentRole.ProjectId;
            }
            else
            {
                return 0;
            }
        }
        protected string GetCurrentSessionKey()
        {
            if (CurrentUserContext != null && CurrentUserContext.SessionId != null)
            {
                return CurrentUserContext.SessionId;
            }
            else
            {
                return null;
            }
        }

        protected long GetCurrentProjectUserRoleID()
        {
            if (CurrentUserContext != null && CurrentUserContext.CurrentRole != null)
            {
                return CurrentUserContext.CurrentRole.ProjectUserRoleID;
            }
            else
            {
                return 0;
            }
        }

        protected string GetCurrentProjectUserRoleCode()
        {
            string CurRole = null;
            if (CurrentUserContext != null && CurrentUserContext.CurrentRole != null)
            {
                CurRole = CurrentUserContext.CurrentRole.RoleCode;
            }
            return CurRole;
        }

        protected bool InsertAuditLogs(AuditTrailData auditTrailData)
        {
            if (auditTrailData == null || AuditService == null || !AppOptions.AppSettings.IsAuditLogEnabled)
                return false;

            try
            {
                long userId = GetCurrentUserId();
                long userRoleId = GetCurrentProjectUserRoleID();
                if (userId > 0)
                {
                    auditTrailData.UserId = userId;
                    auditTrailData.SessionId = GetCurrentSessionKey();   
                }

                
                if (userRoleId > 0)
                {
                    auditTrailData.ProjectUserRoleID = userRoleId;
                    auditTrailData.SessionId = GetCurrentSessionKey();
                }

                if (string.IsNullOrEmpty(auditTrailData.SessionId))
                {
                    auditTrailData.SessionId = GetCurrentSessionKey();
                    auditTrailData.ResponseStatus = AuditTrailResponseStatus.Error;
                }

                AuditService.InsertAuditLogs(auditTrailData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in InsertAuditLogs");
            }

            return true;
        }


        protected UserTimeZone GetCurrentContextTimeZone()
        {
            UserTimeZone userTimeZone = null;

            if (CurrentUserContext != null && CurrentUserContext.TimeZone != null && CurrentUserContext.TimeZone.TimeZoneCode != null)
            {
                userTimeZone = CurrentUserContext.TimeZone;
            }
            if (userTimeZone == null && AppOptions.AppSettings.Calendar.TimeZoneFrom == EnumTimeZoneFrom.UserBrowser)
            {
                string zone = Convert.ToString(HttpContext.Request.Headers["Local-Time-Zone"]);
                string offst = Convert.ToString(HttpContext.Request.Headers["Local-Time-Offset"]);
                if (!string.IsNullOrEmpty(zone) && !string.IsNullOrEmpty(offst))
                {
                    string zonename = Convert.ToString(DateTimeUtils.OlsonTimeZoneToTimeZoneInfo(zone));
                    userTimeZone = new UserTimeZone
                    {
                        TimeZoneCode = zonename,
                        TimeZoneName = zonename,
                        BaseUTCOffsetInMin = Convert.ToInt32(offst)
                    };
                }
            }
            return userTimeZone;
        }
    }
}
