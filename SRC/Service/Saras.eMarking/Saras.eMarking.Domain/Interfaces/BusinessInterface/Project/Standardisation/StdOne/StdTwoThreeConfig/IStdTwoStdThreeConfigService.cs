

using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.S2S3Configuraion;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.StdTwoThreeConfig
{
    public interface IStdTwoStdThreeConfigService
    {
        Task<QualifyingAssessmentCreationModel> GetQualifyScriptdetails(long ProjectId, long QIGId);
        Task<string> CreateQualifyingAssessment(QualifyingAssessmentCreationModel objQualifyingAssessmentModel, long ProjectId, long QIGId, long ProjectUserRoleID);
        Task<string> UpdateQualifyingAssessment(QualifyingAssessmentCreationModel objQualifyingAssessmentModel, long ProjectId,  long ProjectUserRoleID);
        Task<string> CreateWorkflowStatusTracking(S1Complted objS1CompltedModel, long WorkflowID, long ProjectUserRoleID, long ProjectID);
        Task<List<S1Complted>> GetS1CompletedRemarks(long EntityID, byte EntityType, int WorkflowStatusID);
    }
}
