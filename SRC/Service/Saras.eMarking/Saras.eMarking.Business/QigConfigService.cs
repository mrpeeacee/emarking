using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Saras.eMarking.Domain.ViewModels.Project.ScoringComponentLibrary.ScoringComponentLibraryModel;

namespace Saras.eMarking.Business
{
    public class QigConfigService : BaseService<QigConfigService>, IQigConfigService
    {
        readonly IQigConfigRepository _qigRepository;
        public QigConfigService(IQigConfigRepository qigRepository, ILogger<QigConfigService> _logger) : base(_logger)
        {
            _qigRepository = qigRepository;
        }
        /// <summary>
        /// Get all Qig Questions
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="QigId"></param>
        /// <returns></returns>
        public async Task<IList<QigQuestionModel>> GetAllQigQuestions(long ProjectId, long QigId)
        {
            try
            {
                return await _qigRepository.GetAllQigQuestions(ProjectId, QigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigConfigService Page: Method Name: GetAllQigQuestions() and ProjectID = " + ProjectId.ToString() + ", QigId = " + QigId.ToString() + "");
                throw;
            }
        }

		
			public async Task<bool> IsCBPproject(long ProjectId)
		    {
			  try
			  {
				return await  _qigRepository.IsCBPproject(ProjectId);
			   }
			catch (Exception ex)
			  {
				logger.LogError(ex, "Error in QigConfigService Page: Method Name: IsCBPproject() and ProjectID = " + ProjectId.ToString());
				throw;
			  }
		     }

		public async Task<ProjectQigModel> GetQigQuestionandMarks(long QigId, long ProjectId)
        {
            logger.LogInformation("QigConfigService >> GetQigQuestionandMarks() started");
            try
            {
                return await _qigRepository.GetQigQuestionandMarks(QigId, ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigConfigService page while getting Qig Questions and Marks : Method Name : GetQigQuestionandMarks() and QidId= " + QigId.ToString());
                throw;
            }
        }
        public async Task<IList<QuestionModel>> Getavailablemarkschemes(decimal Maxmarks, long ProjectId, int? markschemeType = null)
        {
            logger.LogInformation("QigConfigService >> Getavailablemarkschemes() started");
            try
            {
                return await _qigRepository.Getavailablemarkschemes(Maxmarks, ProjectId, markschemeType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigConfigService page while getting Available marks schemes : Method Name : Getavailablemarkschemes()" + ex.Message);
                throw;
            }
        }
		
			  public async Task<List<ScoreComponentLibraryName>> GetavailableScoringLibrary(long ProjectId,decimal Maxmarks)
		{
			logger.LogInformation("QigConfigService >> Getavailablemarkschemes() started");
			try
			{
				return await _qigRepository.GetavailableScoringLibrary(ProjectId, Maxmarks);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in QigConfigService page while getting Available marks schemes : Method Name : Getavailablemarkschemes()" + ex.Message);
				throw;
			}
		}
		public async Task<string> TagAvailableMarkScheme(QigQuestionModel ObjQigQuestionModel, long CurrentProjUserRoleId, long ProjectID)
        {
            logger.LogInformation("QigConfigService >> TagAvailableMarkScheme() started");
            try
            {
                return await _qigRepository.TagAvailableMarkScheme(ObjQigQuestionModel, CurrentProjUserRoleId, ProjectID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigConfigService page while Tagging Available marks scheme for specific Project: Method Name: TagAvailableMarkScheme()" + "");
                throw;
            }
        }

		public async Task<string> SaveScoringComponentLibrary(QigQuestionModel ObjQigQuestionModel, long CurrentProjUserRoleId, long ProjectID)
		{
			logger.LogInformation("QigConfigService >> SaveScoringComponentLibrary() started");
			try
			{
				return await _qigRepository.SaveScoringComponentLibrary(ObjQigQuestionModel, CurrentProjUserRoleId, ProjectID);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in QigConfigService page while Tagging Available marks scheme for specific Project: Method Name: SaveScoringComponentLibrary()" + "");
				throw;
			}
		}

		public async Task<IList<WorkflowStatus>> GetSetupStatus(long ProjectId)
        {
            logger.LogInformation("QigConfigService >> GetSetupStatus() started");
            try
            {
                return await _qigRepository.GetSetupStatus(ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigConfigService page while getting QIG questions and Total Marks Script : Method Name : GetSetupStatus()" + ex.Message);
                throw;
            }
        }

        public async Task<string> UpdateMaxMarks(long projectQuestionId, long questionMaxmarks, long CurrentProjUserRoleId, long ProjectID)
        {
            logger.LogInformation("QigConfigService >> UpdateResolutionCOI() started");
            try
            {
                return await _qigRepository.UpdateMaxMarks(projectQuestionId, questionMaxmarks, CurrentProjUserRoleId, ProjectID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigConfigService page while updating Question marks : Method Name : UpdateMaxMarks()" + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get All Qig Questions
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="QigId"></param>
        /// <returns></returns>
        public async Task<IList<QigConfigDetailsModel>> GetQIGConfigDetails(long ProjectId, long QigId)
        {
            try
            {
                return await _qigRepository.GetQIGConfigDetails(ProjectId, QigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigConfigService Page: Method Name: GetQIGConfigDetails(), Sp Name: [Marking].[USPGetQIGConfigDetails],  and ProjectID = " + ProjectId.ToString() + ", QigId = " + QigId.ToString() + "");
                throw;
            }
        }
    }
}
