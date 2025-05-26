using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Notification
{
    /// <summary>
    /// Nofification Api
    /// </summary>
    [Route("/api/public/v{version:apiVersion}/notifications")]
    [ApiController]
    public class NotificationsController : BaseApiController<NotificationsController>
    {
        /// <summary>
        /// Notifications constructor
        /// </summary>
        /// <param name="_logger"></param>
        /// <param name="appOptions"></param>
        public NotificationsController(ILogger<NotificationsController> _logger, AppOptions appOptions) : base(appOptions, _logger)
        {

        }

        /// <summary>
        /// To get server date and time
        /// </summary> 
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/notifications/serverdatetime")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetServerDateTime()
        {
            try
            {
                logger.LogDebug($"NotificationsController > Method Name: GetServerDateTime() started. ProjectId={GetCurrentProjectId()}");

                DateTime? dateTime = DateTime.UtcNow.UtcToProfileDateTime(GetCurrentContextTimeZone());

                logger.LogDebug($"NotificationsController > Method Name: GetServerDateTime() completed. ProjectId={GetCurrentProjectId()}");

                await Task.CompletedTask;

                return Ok(dateTime);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in NotificationsController > GetServerDateTime().");
                throw;
            }
        }
        /// <summary>
        /// To get server date and time
        /// </summary> 
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/notifications/buildnumber")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBuildNumberAsync()
        {
            try
            {
                logger.LogDebug($"NotificationsController > Method Name: GetBuildNumber() started. ProjectId={GetCurrentProjectId()}");

                BrandingModel buildVersionModel = new()
                {
                    DisplayBuildNumber = AppOptions.AppSettings.IsBuildNumberDisplayEnabled,
                    Branding = AppOptions.AppSettings.Branding
                };
                if (buildVersionModel.DisplayBuildNumber)
                {
                    buildVersionModel.BuildNumber = AppOptions.AppSettings.BuildNumber;
                }

                logger.LogDebug($"NotificationsController > Method Name: GetBuildNumber() completed. ProjectId={GetCurrentProjectId()}");

                await Task.CompletedTask;

                return Ok(buildVersionModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in NotificationsController > GetBuildNumber().");
                throw;
            }
        }
    }
}
