using Saras.eMarking.Domain.ViewModels.Project.Setup;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup
{
    public interface IBasicDetailsService
    {
        Task<BasicDetailsModel> GetBasicDetails(long ProjectID);
        Task<string> UpdateBasicDetails(BasicDetailsModel basicdeatilmodel, long UserId, long ProjectID);
        Task<GetModeOfAssessmentModel> GetModeOfAssessment(long projectId);
    }
}
