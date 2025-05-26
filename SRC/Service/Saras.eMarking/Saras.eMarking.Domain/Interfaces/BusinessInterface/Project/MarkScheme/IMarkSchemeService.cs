using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.MarkScheme
{
    public interface IMarkSchemeService
    {
        Task<IList<MarkSchemeModel>> GetAllMarkScheme(long projectId);
        Task<MarkSchemeModel> MarkSchemeWithId(long projectId, long schemeId);
        Task<string> CreateMarkScheme(MarkSchemeModel markScheme, long projectId, long ProjectUserRoleID);
        Task<string> UpdateMarkScheme(MarkSchemeModel markScheme, long projectId, long schemeId, long ProjectUserRoleID);
        Task<IList<ProjectTaggedQuestionModel>> GetAllQuestions(long projectId, long schmeId, decimal maxMark);
        Task<ProjectQuestionsModel> GetQuestionText(long projectId, long questionId);
        Task<string> MarkSchemeMapping(long projectId, List<ProjectTaggedQuestionModel> questionList, long ProjectUserRoleID);
        Task<string> DeleteMarkScheme(long projectId, List<long> markSchemeids, long ProjectUserRoleID);
        Task<PaginationModel<ProjectTaggedQuestionsModel>> GetQuestionsUnderProject(long projectId, int? pagenumber);
    }
}
