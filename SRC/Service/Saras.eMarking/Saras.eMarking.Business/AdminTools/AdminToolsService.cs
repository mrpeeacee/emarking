using Microsoft.Extensions.Logging;
using Nest;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.AdminTools;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.AdminTools;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AdminTools;
using Saras.eMarking.Domain.ViewModels.Banding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.AdminTools
{
	public class AdminToolsService : BaseService<AdminToolsService>, IAdminToolsService
	{
		readonly IAdminToolsRepository admintoolsRepository;

		public AdminToolsService(IAdminToolsRepository _admintoolsRepository, ILogger<AdminToolsService> _logger) : base(_logger)
		{
			admintoolsRepository = _admintoolsRepository;
		}
		public async Task<List<BindAllProjectModel>> BindAllProject(long UserId, long ProjectId)
		{
			try
			{
				List<BindAllProjectModel> res = null;
				logger.LogInformation("AdminToolsController > BindAllProject() started. ProjectId = {ProjectId} and UserId = {UserId}", ProjectId, UserId);

				res = await admintoolsRepository.BindAllProject(UserId, ProjectId);

				logger.LogInformation("AdminToolsController > BindAllProject() completed. ProjectId = {ProjectId} and UserId = {UserId}", ProjectId, UserId);

				return res;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > BindAllProject() method. ProjectId = {ProjectId} and UserId = {UserId}", ProjectId, UserId);
				throw;
			}
		}
		public async Task<List<LiveMarkingProgressModel>> LiveMarkingProgressDetails(long projectid, long UserId)
		{
			try
			{
				List<LiveMarkingProgressModel> res = null;
				logger.LogInformation("AdminToolsController > LiveMarkingProgressDetails() started. ProjectID = {projectid} and UserId = {UserId}", projectid, UserId);

				res = await admintoolsRepository.LiveMarkingProgressDetails(projectid, UserId);

				logger.LogInformation("AdminToolsController > LiveMarkingProgressDetails() completed. ProjectID = {projectid} and UserId = {UserId}", projectid, UserId);

				return res;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > LiveMarkingProgressDetails() method. ProjectID = {projectid} and UserId = {UserId}", projectid, UserId);
				throw;
			}
		}
		public async Task<AdminToolsModel> QualityCheckDetails(long projectId, short selectedrpt, long UserId)
		{
			try
			{
				AdminToolsModel res = null;
				logger.LogInformation("AdminToolsController > QualityCheckDetails() started. ProjectId = {projectId} and Selected Report = {selectedrpt} and UserId = {UserId}", projectId, selectedrpt, UserId);

				res = await admintoolsRepository.QualityCheckDetails(projectId, selectedrpt, UserId);

				logger.LogInformation("AdminToolsController > QualityCheckDetails() completed. ProjectId = {projectId} and Selected Report = {selectedrpt} and UserId = {UserId}", projectId, selectedrpt, UserId);

				return res;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > QualityCheckDetails() method. ProjectId = {projectId} and Selected Report = {selectedrpt} and UserId = {UserId}", projectId, selectedrpt, UserId);
				throw;
			}
		}
		public async Task<List<CandidateScriptModel>> CandidateScriptDetails(long projectId, SearchFilterModel searchFilterModel, long UserId, bool IsDownload = false)
		{
			try
			{
				List<CandidateScriptModel> res = null;
				logger.LogInformation("AdminToolsController > CandidateScriptDetails() started. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}.", projectId, searchFilterModel, UserId, IsDownload);

				res = await admintoolsRepository.CandidateScriptDetails(projectId, searchFilterModel, UserId, IsDownload);

				logger.LogInformation("AdminToolsController > CandidateScriptDetails() completed. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}.", projectId, searchFilterModel, UserId, IsDownload);

				return res;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > CandidateScriptDetails() method. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}.", projectId, searchFilterModel, UserId, IsDownload);
				throw;
			}
		}
		public async Task<List<FrequencyDistributionModel>> FrequencyDistributionReport(long projectId, long questionType, SearchFilterModel searchFilterModel, long UserId, bool IsDownload = false)
		{
			try
			{
				List<FrequencyDistributionModel> res = null;
				logger.LogInformation("AdminToolsController > FrequencyDistributionReport() started. ProjectID = {projectId} and QuestionType = {questionType} and SearchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}", projectId, questionType, searchFilterModel, UserId, IsDownload);

				res = await admintoolsRepository.FrequencyDistributionReport(projectId, questionType, searchFilterModel, UserId, IsDownload);

				logger.LogInformation("AdminToolsController > FrequencyDistributionReport() completed. ProjectID = {projectId} and QuestionType = {questionType} and SearchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}", projectId, questionType, searchFilterModel, UserId, IsDownload);

				return res;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > FrequencyDistributionReport() method. ProjectID = {projectId} and QuestionType = {questionType} and SearchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}", projectId, questionType, searchFilterModel, UserId, IsDownload);
				throw;
			}
		}

		public async Task<List<AllAnswerKeysModel>> AllAnswerKeysReport(long projectId, long UserId, SearchFilterModel searchFilterModel, bool IsDownload = false)
		{
			try
			{
				List<AllAnswerKeysModel> res = null;

				logger.LogInformation("AdminToolsController > AllAnswerKeysReport() started. ProjectID = {projectId}  ", projectId);
				res = await admintoolsRepository.AllAnswerKeysReport(projectId, UserId, searchFilterModel, IsDownload);

				logger.LogInformation("AdminToolsController > AllAnswerKeysReport() completed. ProjectID = {projectId} ", projectId);

				return res;

			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController > AllAnswerKeysReport() completed. ProjectID = {projectId} ", projectId);
				throw;
			}
		}

		public async Task<List<Mailsentdetails>> MailSentDetails(ClsMailSent clsMailSent)
		{

			try
			{
				logger.LogDebug($"AdminToolsController > MailSentDetails() started. MailSentModel = {clsMailSent} ");

				List<Mailsentdetails> result = await admintoolsRepository.MailSentDetails(clsMailSent);

				logger.LogDebug($"AdminToolsController > MailSentDetails() completed. MailSentModel = {clsMailSent}");

				return result;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error in AdminToolsController > MailSentDetails() method.MailSentModel = {clsMailSent}");
				throw;
			}
		}

		public async Task<FIDIReportModel> FIDIReportDetails(long projectId, SearchFilterModel searchFilterModel, bool syncMetaData, bool IsDownload = false)
		{
			try
			{
				logger.LogDebug($"AdminToolsService > FIDIModel() started.");
				FIDIReportModel result = await admintoolsRepository.FIDIReportDetails(projectId, searchFilterModel, IsDownload, syncMetaData);
				logger.LogDebug($"AdminToolsService > FIDIReportDetails() completed.");
				return result;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error in AdminToolsService > FIDIReportDetails() method.");
				throw;
			}
		}
		public async Task<byte> ProjectStatus(long projectId)
		{
			try
			{
				logger.LogDebug($"AdminToolsService > FIDIModel() started.");
				byte result = await admintoolsRepository.ProjectStatus(projectId);
				logger.LogDebug($"AdminToolsService > FIDIReportDetails() completed.");
				return result;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error in AdminToolsService > ProjectStatus() method.");
				throw;
			}
		}


		/// <summary>
		/// Gets the project assessment info.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <returns><![CDATA[Task<List<ProjectAssessmentInfoModel>>]]></returns>
		public async Task<List<BindAllMarkerPerformanceModel>> GetMarkerPerformanceReport(long projectId, SearchFilterModel searchFilterModel, long UserId, UserTimeZone TimeZone, bool IsDownload = false)
		{
			try
			{
				List<BindAllMarkerPerformanceModel> res = null;
				////logger.LogInformation("AdminToolsController > GetMarkerPerformanceReport() started. ProjectId = {projectId}", projectId);

				res = await admintoolsRepository.GetMarkerPerformanceReport(projectId, searchFilterModel, UserId, TimeZone, IsDownload);

				////logger.LogInformation("AdminToolsController > GetMarkerPerformanceReport() completed. ProjectId = {projectId}", projectId);

				return res;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController : Method Name: GetMarkerPerformanceReport() and ProjectID = {projectId}", projectId);
				throw;
			}
		}

		public async Task<SyncMetaDataResult> SyncMetaData(List<SyncMetaDataModel> syncMetaData)
		{
			try
			{
				SyncMetaDataResult res = new SyncMetaDataResult();
				logger.LogDebug("AdminToolsService > SyncMetaData() started");

				res = await admintoolsRepository.SyncMetaData(syncMetaData);

				logger.LogDebug("AdminToolsService > SyncMetaData() completed.");

				return res;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error in AdminToolsController : Method Name: SyncMetaData()");
				throw;
			}
		}



	}

}
