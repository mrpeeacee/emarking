using Microsoft.Extensions.Logging;
using Saras.eMarking.Business.Project.MarkScheme;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.MarkScheme;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.ScoringComponentLibrary;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.MarkScheme;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.ScoringComponentLibrary;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Saras.eMarking.Domain.ViewModels.Project.ScoringComponentLibrary.ScoringComponentLibraryModel;

namespace Saras.eMarking.Business.Project.ScoringComponentLibrary
{
    public class ScoringComponentLibraryService: BaseService<ScoringComponentLibraryService>, IScoringComponentLibraryService
    {
        readonly IScoringComponentLibraryRepository _scoringComponentLibraryRepository;
        public ScoringComponentLibraryService(IScoringComponentLibraryRepository ScoringComponentLibraryRepository, ILogger<ScoringComponentLibraryService> _logger) : base(_logger)
        {
            _scoringComponentLibraryRepository = ScoringComponentLibraryRepository;
        }

        /// <summary>
        /// To get All Scoring Component Libraries 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<IList<ScoreComponentLibraryName>> GetAllScoringComponentLibrary(long projectId)
        {
            logger.LogDebug($"ScoringComponentLibraryService GetAllScoringComponentLibrary() method started. ProjectId = {projectId}");

            IList<ScoreComponentLibraryName> schemeResp = await _scoringComponentLibraryRepository.GetAllScoringComponentLibrary(projectId);

            logger.LogDebug($"ScoringComponentLibraryService GetAllScoringComponentLibrary() method completed. ProjectId = {projectId}");
            return schemeResp;
        }


        /// <summary>
        /// Method to Create New Scoring Component Libraries.
        /// </summary>
        /// <param name="markScheme"></param>
        /// <param name="projectId"></param>
        /// <param name="ProjectUserRoleID"></param>
        /// <returns></returns>
        public async Task<string> CreateScoringComponentLibrary(ScoreComponentLibraryName markScheme, long projectId, long ProjectUserRoleID)
		{
			logger.LogDebug($"MarkSchemeService CreateMarkScheme() method started. List of {markScheme} and projectId {projectId}, userId {ProjectUserRoleID}");
			string status = "";
			if (markScheme != null)
			{
				

					status = await _scoringComponentLibraryRepository.CreateScoringSomponentLibrary(markScheme, projectId, ProjectUserRoleID);
				
			}
			else
			{
				status = "EMPTY";
			}
			logger.LogDebug($"MarkSchemeService CreateMarkScheme() method completed. List of {markScheme} and projectId {projectId},userId {ProjectUserRoleID}");

			return status;
		}

        /// <summary>
        /// To delete untagged Scoring Component Library.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="ScoreComponentID"></param>
        /// <param name="ProjectUserRoleID"></param>
        /// <returns></returns>
        public async Task<string> DeleteScoringComponentLibrary(long projectId, List<long> ScoreComponentID, long ProjectUserRoleID)
        {
            logger.LogDebug($"MarkSchemeService > DeleteScoringComponentLibrary() started. projectId {projectId} and List Of ScoreComponentID {ScoreComponentID} and Userid {ProjectUserRoleID}");
            string status = string.Empty;
            if (ScoreComponentID != null && ScoreComponentID.Count > 0)
            {
                status = await _scoringComponentLibraryRepository.DeleteScoringComponentLibrary(projectId, ScoreComponentID, ProjectUserRoleID);
            }
            else
            {
                status = "MANDFD";
            }
            logger.LogDebug($"MarkSchemeService > DeleteScoringComponentLibrary() started. projectId {projectId} and List Of ScoreComponentID {ScoreComponentID} and Userid {ProjectUserRoleID}");

            return status;
        }

        /// <summary>
        /// To view Detailed Scoring Component Library.
        /// </summary>
        /// <param name="ComponentId"></param>
        /// <returns></returns>
        public async Task<ScoreComponentLibraryName> ViewScoringComponentLibrary(long ComponentId)
        {
            ScoreComponentLibraryName Library = new ScoreComponentLibraryName();
            logger.LogDebug($"MarkSchemeService > ViewScoringComponentLibrary() started. List Of ScoreComponentID {ComponentId}");
            if (ComponentId != 0)
            {
                Library = await _scoringComponentLibraryRepository.ViewScoringComponentLibrary(ComponentId);
                logger.LogDebug($"MarkSchemeService > ViewScoringComponentLibrary() Completed.List Of ScoreComponentID {ComponentId}");

            }
            return Library; ;



        }

    }
}
