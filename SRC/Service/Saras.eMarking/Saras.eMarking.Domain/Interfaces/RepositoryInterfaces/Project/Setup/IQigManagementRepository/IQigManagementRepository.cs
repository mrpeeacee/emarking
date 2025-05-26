using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Auth;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.IQigManagementRepository
{
    public interface IQigManagementRepository
    {
        Task<List<QigManagementModel>> GetQigQuestions(long projectId, int questionType);
        Task<QigDetails> GetQigDetails(long projectId, long qigId);
        Task<GetManagedQigListDetails> GetManagedQigDetails(long projectId);
        Task<string> CreateQigs(CreateQigsModel createqigsModel,long projectId, long ProjectUserRoleID);
        Task<string> UpdateMandatoryQuestion(QigDetails qigDetails);
        Task<QigQuestionModel> GetQuestionxml(long ProjectId, long QigId, long QuestionId);
        Task<QigQuestionsDetails> GetQuestionDetails(long QigType, long ProjectQigId,long ProjectId,long QnsType);
        Task<string> MoveorTagQIG(Tagqigdetails tagqigdetails, long ProjectUserRoleID,long projectid);
        Task<List<BlankQuestionDetails>> GetBlankQuestions(long ProjectId, long parentQuestionId);
        Task<string> SaveQigQuestions(long projectId, long ProjectUserRoleID, FinalRemarks remarks);
        Task<string> SaveQigQuestionsDetails(SaveQigQuestions saveQigQuestions, long projectId, long ProjectUserRoleID);
        Task<List<QigQuestionsDetails>> GetUntaggedQuestions(long ProjectId);
        Task<string> DeleteQig(long ProjectId, long QigId);
        Task<string> QigReset(long ProjectId,long currentprojectuserroleId);
        Task<string> SaveUserDetails(AuthenticateRequestModel loginCredential, long projectId, long projectUserRoleID);
    } 
}
