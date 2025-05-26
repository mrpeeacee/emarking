using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.ResponseProcessing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.ResponseProcessing.SemiAutomaticQuestions
{
    public interface IFrequencyDistributionsService
    {
        Task<FibDiscrepencyReportModel> GetDiscrepancyReportFIB(long ProjectId, string candidateResponse, long ProjectQuestionId);
        Task<IList<QigQuestionModel>> GetAllViewQuestions(long ProjectId);
        Task<string> UpdateModerateMarks(CandidatesAnswerModel ObjCandidatesAnswerModel, long CurrentProjUserRoleId, long ProjectID);
        Task<ViewFrequencyDistributionModel> GetFrequencyDistribution(long ProjectId, long QuestionId);
        Task<string> UpdateOverallModerateMarks(long ProjectQuestionId, long CurrentProjUserRoleId, long ProjectID);
        Task<IList<ViewAllBlankSummaryModel>> GetAllBlankSummary(long ProjectId, long ParentQuestionId);
        Task<string> UpdateManualMarkig(EnableManualMarkigModel ObjManualMarkigModel, long CurrentProjUserRoleId, long ProjectID);
        Task<string> UpdateNormaliseScore(DiscrepencyNormalizeScoreModel ObjFibReportModel, long CurrentProjUserRoleId, long ProjectID);
        Task<string> UpdateAllResponsestoManualMarkig(long ParentQuestionId, long CurrentProjUserRoleId, long ProjectID);
        Task<string> UpdateAcceptDescrepancy(DiscrepencyNormalizeScoreModel ObjFibReportModel, long CurrentProjUserRoleId, long ProjectID);
    }
}
