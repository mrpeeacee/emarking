using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Api.Controllers.Project.MarkScheme;
using Saras.eMarking.Business.Project.MarkScheme;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.MarkScheme;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.ScoringComponentLibrary;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using static Saras.eMarking.Domain.ViewModels.Project.ScoringComponentLibrary.ScoringComponentLibraryModel;
using System.Text.Json;
using Nest;
using Microsoft.Extensions.DependencyModel;

namespace Saras.eMarking.Api.Controllers.Projects.ScoringComponentLibrary
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/ScoringComponentLibrary")]
    public class ScoringComponentLibraryController:BaseApiController<ScoringComponentLibraryController>
    {
        private readonly IScoringComponentLibraryService _scoringComponentLibraryService;
        private readonly IAuthService AuthService;

        public ScoringComponentLibraryController(IScoringComponentLibraryService scoringComponentLibraryService, ILogger<ScoringComponentLibraryController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            _scoringComponentLibraryService = scoringComponentLibraryService;
            AuthService = _authService;
        }

        /// <summary>
        /// To Get All Scoring Component Libraries.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetAllScoringComponents()
        
        {
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();
                logger.LogDebug($"ScoringComponentLibraryController > GetAllScoringComponents() started. ProjectId = {ProjectId}");

                if (!AuthService.IsValidProject(ProjectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                IList<ScoreComponentLibraryName> res = await _scoringComponentLibraryService.GetAllScoringComponentLibrary(ProjectId);

                logger.LogDebug($"ScoringComponentLibraryController >GetAllScoringComponents() completed. ProjectId = {ProjectId}");

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ScoringComponentLibraryController > GetAllScoringComponents(). ProjectId = {ProjectId}");
                throw;
            }
        }


        /// <summary>
        /// To Create Scoring Component Libraries.
        /// </summary>
        /// <param name="ScoreComponentLibraryNames"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
		[HttpPost]
		 [Route("markschemecreation")]
		[ApiVersion("1.0")]
		[Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
		public async Task<IActionResult> CreateScoringComponentLibrary(ScoreComponentLibraryName ScoreComponentLibraryNames)
		{
			long projectId = 0;
			string result = string.Empty;
			long userId = 0;
			long currentprojectuserroleId = 0;
			try
			{
				userId = GetCurrentUserId();
				currentprojectuserroleId = GetCurrentProjectUserRoleID();
				projectId = GetCurrentProjectId();
				logger.LogDebug($"MarkSchemeController > CreateMarkScheme() started. ProjectId = {projectId} and list of {CreateScoringComponentLibrary}");

				if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
				{
					return new ForbidResult();
				}
				result = await _scoringComponentLibraryService.CreateScoringComponentLibrary(ScoreComponentLibraryNames, projectId, GetCurrentProjectUserRoleID());

				logger.LogDebug($"MarkSchemeController > CreateMarkScheme() completed. ProjectId = {projectId} and list of {CreateScoringComponentLibrary}");

				return Ok(result);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error in MarkSchemeController > CreateMarkScheme(). ProjectId = {projectId} and list of {CreateScoringComponentLibrary}");
				throw;
			}
			

		}

        /// <summary>
        /// To delete Untagged Scoring Component Library.
        /// </summary>
        /// <param name="ScoreComponentID"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("delete")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> DeleteScoringComponentLibrary(List<long> ScoreComponentID)
        {
            long projectId = 0;
            string result = string.Empty;
            long userId = 0;
            long currentprojectuserroleId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                userId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                logger.LogDebug($"ScoringComponentController > DeleteScoringComponentLibrary() started. projectId {projectId} and List Of ScoreComponentID {ScoreComponentID}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await _scoringComponentLibraryService.DeleteScoringComponentLibrary(projectId, ScoreComponentID, GetCurrentProjectUserRoleID());

                logger.LogDebug($"ScoringComponentController > DeleteScoringComponentLibrary() completed. projectId {projectId} and List Of ScoreComponentID {ScoreComponentID}");



                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ScoringComponentController > DeleteScoringComponentLibrary(). projectId {projectId}, List Of ScoreComponentID {ScoreComponentID}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.DELETE,
                    Entity = AuditTrailEntity.MARKSCHEME,
                    Module = AuditTrailModule.MARKSCHEMELIB,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = ScoreComponentID,
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }


        /// <summary>
        /// To View Detailed Scoring Component Library.
        /// </summary>
        /// <param name="ComponentId"></param>
        /// <returns></returns>
        [Route("{ComponentId}/View")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> ViewScoringComponentLibrary(long ComponentId)
        {
            long projectId = 0;
          
            try
            {
                projectId = GetCurrentProjectId();
                logger.LogDebug($"ScoringComponentController > ViewScoringComponentLibrary() started. List Of ScoreComponentID {ComponentId}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                 ScoreComponentLibraryName  Library = await _scoringComponentLibraryService.ViewScoringComponentLibrary(ComponentId);

                logger.LogDebug($"ScoringComponentController > ViewScoringComponentLibrary() completed.  List Of ScoreComponentID {ComponentId}");
                return Ok(Library);
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
