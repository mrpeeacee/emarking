using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Dashboards;
using Saras.eMarking.Domain.ViewModels.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels.Project.QualityChecks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.QualityChecks
{
    public interface IQualityChecksService
    {
        // Task<MarkingOverviewsModel> GetQIGProjectUserReportees(long ProjectID, long QigId,long ProjectUserRoleID)
        Task<List<QualityChecksModel>> GetQIGProjectUserReportees(long QigId, long ProjectId, long ProjectUserRoleID);

        Task<MarkingOverviewsModel> GetQIGHierarchyLiveMarkingScriptCountDetails(long QigId, long ProjectId, long ProjectUserRoleID);

        Task<QualityCheckCountSummary> GetTeamStatistics(TeamStatistics teamStatistics, UserTimeZone userTimeZone, UserRole userRole);
        Task<List<QualityCheckScriptDetailsModel>> GetTeamStatisticsList(TeamStatistics teamStatistics, UserTimeZone userTimeZone, UserRole userRole); 

        Task<List<QualityCheckScriptDetailsModel>> GetLiveMarkingScriptCountDetails(long QigId, long ProjectId, long ProjectUserRoleID,int scriptPool, long FilterProjectUserRoleID);
        Task<QualityCheckInitialScriptModel> GetScriptInDetails(long QigId, long ScriptId, long ProjectId,long ProjectUserRoleID, UserTimeZone userTimeZone);
        Task<string> LiveMarkingScriptApprovalStatus(LivemarkingApproveModel livemarkingApproveModel, long projectId, string roleCode);
        Task<QualityCheckCountSummary> GetQualityCheckSummary(long QigId, long ProjectId, long ProjectUserRoleID, long FilterProjectUserRoleID);
        bool IsEligibleForLiveMarking(long qigId, long ProjectId, long ProjectUserRoleID);

        Task<string> CheckedOutScript(LivemarkingApproveModel livemarkingApproveModel, long projectId);
        Task<string> AddMarkingRecord(TrialmarkingScriptDetails trialmarkingScriptDetails);
        Task<List<Qualitycheckedbyusers>> Getcheckedbyuserslist(long ProjectId, long QigId, long ProjectUserRoleID);
        Task<string> GetUserStatus(long ProjectId, long qigId, long projectUserRoleId);
        Task<string> ScriptToBeSubmit(Livepoolscript livepoolscript);
        Task<string> RevertCheckout(QualityCheckScriptDetailsModel[] scriptsCheckedout, long projectId);
    }
}
