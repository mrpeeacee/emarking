using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using System.Text;
using Saras.eMarking.Api.Controllers;
using System;
using Microsoft.Extensions.Hosting;
using Saras.eMarking.Domain.Entities;
using System.Threading.Tasks;

namespace Saras.eMarking.Api
{
    /// <summary>
    /// Error handling 
    /// </summary>
    public class ErrorController : BaseApiController<ErrorController>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_logger"></param>
        /// <param name="appOptions"></param>
        public ErrorController(ILogger<ErrorController> _logger, AppOptions appOptions) : base(appOptions, _logger)
        {
        }
        /// <summary>
        /// Error
        /// </summary>
        /// <returns></returns>
        [Route("Error")]
        [HttpGet, MapToApiVersion("1.0")]
        [AllowAnonymous]
        public async Task<IActionResult> ErrorAsync([FromServices] IHostEnvironment webHostEnvironment)
        {
            logger.LogDebug($"ErrorController > Method Name: ErrorAsync() started");

            StringBuilder errordetails = new();
            IExceptionHandlerFeature feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Exception ex = feature?.Error;
            bool isDev = webHostEnvironment.IsDevelopment();
            ProblemDetails problemDetails = new()
            {
                Status = (int)HttpStatusCode.InternalServerError,
            };

            if (ex != null)
            {
                if (isDev)
                {
                    problemDetails.Title = $"{ex.GetType().Name}: {ex.Message}";
                    problemDetails.Detail = ex.StackTrace;
                }
                else
                {
                    problemDetails.Title = "An error occurred.";
                    problemDetails.Detail = string.Empty;
                }
            }
            else
            {
                problemDetails.Title = string.Empty;
                problemDetails.Detail = string.Empty;
            }

            _ = errordetails.Append("status : " + problemDetails.Status.Value);
            _ = errordetails.Append("Title : " + problemDetails.Title);
            _ = errordetails.Append("Detail : " + problemDetails.Detail);
            logger.LogError(errordetails.ToString());

            await Task.CompletedTask;

            logger.LogDebug($"ErrorController > Method Name: ErrorAsync() completed");

            return StatusCode(problemDetails.Status.Value, problemDetails);
        }
    }
}