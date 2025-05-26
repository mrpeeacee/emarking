using Saras.eMarking.Domain.ViewModels.Project.Standardisation;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdTwo.Practice;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation
{
    public interface IPracticeAssessmentRepository
    {
        Task<S2S3AssessmentModel> GetStandardisationScript(long ProjectID, long ProjectUserRoleID, long QigID, int WorkflowStatusID);
        Task<List<PracticeQuestionDetailsModel>> GetPracticeQuestionDetails(long ProjectID, long ProjectUserRoleID, long QigID, long ScriptID, bool iscompleted);     
    }
}
