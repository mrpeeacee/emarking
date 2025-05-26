using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels.Project.QualityChecks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.LiveMarking
{
    public interface ILiveMarkingService
    {
        Task<string> DownloadScripts(long QigId, long projectId, long ProjectUserRoleID);
        Task<LiveMarkingModel> GetLiveScripts(ClsLiveScript clsLiveScript, long projectId, long ProjectUserRoleID, UserTimeZone userTimeZone);
        Task<string> MoveScriptToGracePeriod(QigScriptModel qigScriptModel, long projectId, long ProjectUserRoleID, string RoleCode);
        Task<string> RevokeScriptFromGracePeriod(long qigId, long scriptId, long projectId, long ProjectUserRoleID);
        Task<string> UpdateScriptStatus(QigScriptModel qigScriptModel, long projectId, long ProjectUserRoleID, bool scriptStatus);
        Task<List<Qualitycheckedbyusers>> GetDownloadedScriptUserList(long projectId, long qigId, long ProjectUserRoleID);
        Task<string> MoveScriptsToLivePool(Livepoolscript livepoolscript);
        Task<string> CheckScriptIsLivePool(long scriptId, long projectId, long ProjectUserRoleID);
    }
}
