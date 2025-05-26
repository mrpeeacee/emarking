using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.ProjectSchedule;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup
{
    public interface IProjectScheduleService
    {
        Task<ProjectScheduleModel> GetProjectSchedule(long ProjectID, UserTimeZone userTimeZone);
        Task<string> CreateUpdateProjectSchedule(ProjectScheduleModel objProjectScheduleModel, long ProjectUserRoleID, UserTimeZone userTimeZone, long UserId, long projectID);
        Task<DayWiseScheduleModel> GetDayWiseConfig(DayWiseScheduleModel objDayWiseScheduleModel, long ProjectId, UserTimeZone userTimeZone);
        Task<string> UpdateDayWiseConfig(long ProjectID, long ProjectUserRoleID, DayWiseScheduleModel objDayWiseScheduleModel, UserTimeZone userTimeZone);
    }
}
