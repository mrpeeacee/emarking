using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdTwo.Practice;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation
{
    public interface IStdAssessmentService
    {
        Task<PracticeEnableModel> IsPracticeQualifyingEnable(long ProjectID, long ProjectUserRoleID, long QigID);
        Task<int> AssessmentStatus(long ProjectID, long ProjectUserRoleID, long QigID);
    }
}
