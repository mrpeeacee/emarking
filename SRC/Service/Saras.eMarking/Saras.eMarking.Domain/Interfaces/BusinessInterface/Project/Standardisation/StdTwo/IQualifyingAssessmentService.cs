using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation
{
    public interface IQualifyingAssessmentService
    {
        Task<S2S3AssessmentModel> GetStandardisationScript(long ProjectID, long ProjectUserRoleID, long QigID, EnumWorkflowStatus WorkflowStatus);
        Task<bool> QualifyingAssessmentUpdateSummary(long ProjectID, long ProjectUserRoleID, long QigID);
        Task<List<AdditionalStdScriptsModel>> GetAssignAdditionalStdScripts(long ProjectID, long QigID, long ProjectUserRoleID);
        Task<string> AssignAdditionalStdScripts(long ProjectID, long AssignedBy, AssignAdditionalStdScriptsModel assignAdditionalStdScriptsModel);
    }
}
