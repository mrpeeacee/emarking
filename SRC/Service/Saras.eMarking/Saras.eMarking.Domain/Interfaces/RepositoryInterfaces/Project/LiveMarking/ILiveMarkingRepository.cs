using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels.Project.QualityChecks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.LiveMarking
{
    public interface ILiveMarkingRepository
    {
        Task<string> DownloadScripts(long QigId, long projectId, long ProjectUserRoleID);
        Task<LiveMarkingModel> GetLiveScripts(ClsLiveScript clsLiveScript, long projectId, long projectUserRoleID, UserTimeZone userTimeZone);
        Task<string> MoveScriptToGracePeriod(QigScriptModel qigScriptModel, long projectId, long projectUserRoleID, string roleCode);
        Task<string> RevokeScriptFromGracePeriod(long qigId, long scriptId, long projectId, long projectUserRoleID);
        Task<string> UpdateScriptStatus(QigScriptModel qigScriptModel, long projectId, long projectUserRoleID, bool scriptStatus);
        Task<List<Qualitycheckedbyusers>> GetDownloadedScriptUserList(long projectId, long qigId, long projectUserRoleID);
        Task<string> MoveScriptsToLivePool(Livepoolscript livepoolscript);
        Task<string> CheckScriptIsLivePool(long scriptId, long projectId, long projectUserRoleID);
    }
}
