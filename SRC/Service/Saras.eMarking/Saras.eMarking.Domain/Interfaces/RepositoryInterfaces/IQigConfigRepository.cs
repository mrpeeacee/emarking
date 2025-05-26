using System.Collections.Generic;
using System.Threading.Tasks;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using static Saras.eMarking.Domain.ViewModels.Project.ScoringComponentLibrary.ScoringComponentLibraryModel;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces
{
    public interface IQigConfigRepository
    {
        Task<IList<QigQuestionModel>> GetAllQigQuestions(long ProjectId, long QigId);
        Task<ProjectQigModel> GetQigQuestionandMarks(long QigId, long ProjectId);
		Task<List<ScoreComponentLibraryName>> GetavailableScoringLibrary(long ProjectId,decimal Maxmarks);
		Task<IList<QuestionModel>> Getavailablemarkschemes(decimal Maxmarks, long ProjectId, int? markschemeType = null);
        Task<IList<WorkflowStatus>> GetSetupStatus(long ProjectId);     
        Task<string> TagAvailableMarkScheme(QigQuestionModel objQigQuestionModel, long CurrentProjUserRoleId, long ProjectID);
		Task<string> SaveScoringComponentLibrary(QigQuestionModel ObjQigQuestionModel, long CurrentProjUserRoleId, long ProjectID);
		Task<string> UpdateMaxMarks(long projectQuestionId, long questionMaxmarks, long CurrentProjUserRoleId, long ProjectID);
        Task<IList<QigConfigDetailsModel>> GetQIGConfigDetails(long ProjectId, long QigId);

		Task<bool> IsCBPproject(long projectId);
	}
}
