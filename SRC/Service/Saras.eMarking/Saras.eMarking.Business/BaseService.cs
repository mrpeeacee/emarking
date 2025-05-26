using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using System.Net;

namespace Saras.eMarking.Business
{
    public abstract class BaseService<T> where T : class
    {
        public readonly ILogger logger;
        /// <summary>
        /// Appsettings key value
        /// </summary>
        public AppOptions AppOptions { get; set; }
        protected BaseService(ILogger<T> _logger, AppOptions _appOptions = null)
        {
            logger = _logger;
            AppOptions = _appOptions;
        }

        protected string HtmlDecode(string value)
        {
            return WebUtility.HtmlDecode(value);
        } 

        protected string HtmlEncode(string value)
        {
            return WebUtility.HtmlEncode(value);
        }
    }
}
