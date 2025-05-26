using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Dashboards;
using Saras.eMarking.Domain.ViewModels.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels.Project.QualityChecks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.QualityChecks
{
    public interface IQualityChecksRepository
    {
        Task<List<QualityChecksModel>> GetQIGProjectUserReportees(long QigId, long ProjectId, long ProjectUserRoleID);
        Task<MarkingOverviewsModel> GetQIGHierarchyLiveMarkingScriptCountDetails(long QigId, long ProjectId, long ProjectUserRoleID);
        Task<List<QualityCheckScriptDetailsModel>> GetLiveMarkingScriptCountDetails(long QigId, long ProjectId, long ProjectUserRoleID, int scriptPool, long filterProjectUserRoleID);
        Task<QualityCheckInitialScriptModel> GetScriptInDetails(long QigId, long ScriptId, long ProjectId, long ProjectUserRoleID, ViewModels.UserTimeZone userTimeZone);
        Task<string> LiveMarkingScriptApprovalStatus(LivemarkingApproveModel livemarkingApproveModel, long projectId, string roleCode);
        bool IsEligibleForLiveMarking(long qigId, long projectId, long projectUserRoleID);
        Task<QualityCheckCountSummary> GetQualityCheckSummary(long qigId, long projectId, long projectUserRoleID, long filterProjectUserRoleID);
        QualityCheckScript GetTeamStatistics(TeamStatistics teamStatistics, UserTimeZone userTimeZone, UserRole userRole);

        Task<string> CheckedOutScript(LivemarkingApproveModel livemarkingApproveModel, long projectId);
        Task<string> AddMarkingRecord(TrialmarkingScriptDetails trialmarkingScriptDetails);
        Task<List<Qualitycheckedbyusers>> Getcheckedbyuserslist(long ProjectId, long QigId, long ProjectUserRoleID);
        Task<string> GetUserStatus(long projectId, long qigId, long projectUserRoleId);
        Task<string> ScriptToBeSubmit(Livepoolscript livepoolscript);
        Task<string> RevertCheckout(QualityCheckScriptDetailsModel[] scriptsCheckedout, long projectId);
    }
}
