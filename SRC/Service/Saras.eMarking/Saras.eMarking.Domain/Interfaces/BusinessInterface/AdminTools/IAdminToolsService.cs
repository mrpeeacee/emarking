using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AdminTools;
using Saras.eMarking.Domain.ViewModels.Banding;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.AdminTools
{
    public interface IAdminToolsService
    {
        Task<List<BindAllProjectModel>> BindAllProject(long UserId, long ProjectId);
        Task<List<LiveMarkingProgressModel>> LiveMarkingProgressDetails(long projectid, long UserId);
        Task<AdminToolsModel> QualityCheckDetails(long projectId, short selectedrpt, long UserId);
        Task<List<CandidateScriptModel>> CandidateScriptDetails(long projectId, SearchFilterModel searchFilterModel, long UserId, bool IsDownload = false);
        Task<List<FrequencyDistributionModel>> FrequencyDistributionReport(long projectId, long questionType, SearchFilterModel searchFilterModel, long UserId, bool IsDownload = false);

        /// <summary>
        /// Gets All Answer Keys Report
        /// </summary>
        /// <param name="projectId"></param>   
        /// <returns></returns>
        Task<List<AllAnswerKeysModel>> AllAnswerKeysReport(long projectId, long UserId, SearchFilterModel searchFilterModel, bool IsDownload = false);
        Task<List<Mailsentdetails>> MailSentDetails(ClsMailSent clsMailSent);
        Task<FIDIReportModel> FIDIReportDetails(long projectId, SearchFilterModel searchFilterModel, bool syncMetaData, bool IsDownload = false);
        Task<byte> ProjectStatus(long projectId);
        Task<List<BindAllMarkerPerformanceModel>> GetMarkerPerformanceReport(long projectId, SearchFilterModel searchFilterModel, long UserId, UserTimeZone TimeZone, bool IsDownload = false);

        Task<SyncMetaDataResult> SyncMetaData(List<SyncMetaDataModel> syncMetaData);
    }
}
