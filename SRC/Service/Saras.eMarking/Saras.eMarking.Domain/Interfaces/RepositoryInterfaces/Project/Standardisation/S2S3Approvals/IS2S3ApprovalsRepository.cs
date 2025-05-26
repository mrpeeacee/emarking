using Saras.eMarking.Domain.ViewModels.Project.Standardisation.S2S3Approvals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.S2S3Approvals
{
    public interface IS2S3ApprovalsRepository
    {//GetMarkingPersonal
        Task<S2S3ApprovalsModel> GetApprovalStatus(long qigId, long projectId, long projectUserRoleID);
        Task<List<MarkingPersonal>> GetMarkingPersonal(long qigId, long projectId, long projectUserRoleID, long loginId);
        Task<string> scriptApproval(long qigId, long projectId, long projectUserRoleID, long ActionBy, ApprovalDetails approvalDetails);
        bool IsEligibleForS2S3LiveMarking(long qigId, long ProjectId, long ProjectUserRoleID);
    }
}
