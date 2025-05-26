using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdTwo.Practice;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation
{
    public interface IStdAssessmentRepository
    {
        Task<PracticeEnableModel> IsPracticeQualifyingEnable(long projectID, long ProjectUserRoleID, long qigID);
        Task<int> AssessmentStatus(long ProjectID, long ProjectUserRoleID, long QigID);
    }
}
