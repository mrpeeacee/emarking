using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Saras.eMarking.Inbound.Services.Model;

namespace Saras.eMarking.Inbound.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseApiController<T> : ControllerBase where T : class
    {
        /// <summary>
        /// Logger instance
        /// </summary>
        public readonly ILogger logger;

        protected BaseApiController(ILogger<T> logger)
        {

            this.logger = logger;
        }
    }
}
