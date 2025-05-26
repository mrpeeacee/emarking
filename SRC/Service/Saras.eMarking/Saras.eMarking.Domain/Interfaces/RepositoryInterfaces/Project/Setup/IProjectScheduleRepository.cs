using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.ProjectSchedule;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup
{
    public interface IProjectScheduleRepository
    {
        Task<ProjectScheduleModel> GetProjectSchedule(long ProjectID, UserTimeZone userTimeZone);
        Task<string> CreateUpdateProjectSchedule(ProjectScheduleModel objProjectScheduleModel, long ProjectUserRoleID, List<ProjectScheduleCalendarModel> objProjectScheduleCalendarModel, UserTimeZone userTimeZone, long userId, long projectID);
        Task<DayWiseScheduleModel> GetDayWiseConfig(DayWiseScheduleModel objDayWiseScheduleModel, long ProjectId, UserTimeZone userTimeZone);
        Task<string> UpdateDayWiseConfig(long ProjectID, long ProjectUserRoleID, DayWiseScheduleModel objDayWiseScheduleModel);
    }
}
