using Microsoft.AspNetCore.Mvc;
using Saras.eMarking.Domain.Configuration;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;

namespace Saras.eMarking.Api.Controllers
{
    /// <summary>
    /// Base Api Controller
    /// </summary> 
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
        private readonly IAuditService? AuditService;

        /// <summary>
        /// BaseApi Controller Constructor
        /// </summary>
        protected BaseApiController(AppOptions _AppOptions, ILogger<T> _logger, IAuditService? _auditService = null)
        {
            AppOptions = _AppOptions;
            logger = _logger;
            AuditService = _auditService;
        } 
         
        protected static string SafeText(string textdata)
        {
            return System.Net.WebUtility.HtmlEncode(textdata);
        }  
    }
}
