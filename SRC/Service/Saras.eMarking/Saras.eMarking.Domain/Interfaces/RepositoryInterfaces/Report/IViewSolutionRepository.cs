using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Report;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report
{
    public interface IViewSolutionRepository
    {
        public Task<ScheduleDetailsModel> GetUserScheduleDetails(long ProjectId, long UserId);
        public Task<List<UserQuestionResponse>> GetUserResponse(long ProjectId, long UserId, bool Isfrommarkingplayer, long Testcentreid, bool reportstatus);

        public Task<List<AllUserQuestionResponses>> GetUserResponses(long ProjectId, int MaskingRequired, int PreOrPostMarking, SearchFilterModel searchFilterModel);
        public Task<List<SchoolInfoDetails>> GetSchools(long ProjectId);
    }
}
