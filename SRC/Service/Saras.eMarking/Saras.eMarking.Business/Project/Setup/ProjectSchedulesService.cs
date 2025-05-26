using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.ProjectSchedule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Setup
{
    public class ProjectSchedulesService : BaseService<ProjectSchedulesService>, IProjectScheduleService
    {
        readonly IProjectScheduleRepository _projectScheduleRepository;
        public ProjectSchedulesService(IProjectScheduleRepository projectScheduleRepository, ILogger<ProjectSchedulesService> _logger) : base(_logger)
        {
            _projectScheduleRepository = projectScheduleRepository;
        }

        public async Task<ProjectScheduleModel> GetProjectSchedule(long ProjectID, UserTimeZone userTimeZone)
        {
            logger.LogInformation("ProjectSchedule Service >> GetProjectSchedule() started");
            try
            {
                return await _projectScheduleRepository.GetProjectSchedule(ProjectID, userTimeZone);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in project schedule page while getting Project Schedule details for specific project:Method Name:GetProjectSchedule() and ProjectID: ProjectID=" + ProjectID.ToString());
                throw;
            }
        }
        public async Task<string> CreateUpdateProjectSchedule(ProjectScheduleModel objProjectScheduleModel, long ProjectUserRoleID, UserTimeZone userTimeZone, long UserId, long projectID)
        {
            logger.LogInformation("ProjectSchedule Service >> UpdateProjectSchedule() started");
            try
            {
                DateTime counter;

                var jsondata = objProjectScheduleModel.WorkingDaysConfig;
                List<ProjectScheduleCalendarModel> objProjectScheduleCalendarModel = new();

                ////forloop and add all the date between above range
                for (counter = objProjectScheduleModel.StartDate; counter <= objProjectScheduleModel.ExpectedEndDate; counter = counter.AddDays(1))
                {
                    if (Convert.ToString(jsondata["Saturday"]) == "false" && counter.UtcToProfileDateTime(userTimeZone).DayOfWeek == DayOfWeek.Saturday)
                    {
                        objProjectScheduleModel.DayType = 2;
                    }
                    else if (Convert.ToString(jsondata["Sunday"]) == "false" && counter.UtcToProfileDateTime(userTimeZone).DayOfWeek == DayOfWeek.Sunday)
                    {
                        objProjectScheduleModel.DayType = 2;
                    }
                    else
                    {
                        objProjectScheduleModel.DayType = 1;
                    }

                    DateTime EndDate = counter.UtcToProfileDateTime(userTimeZone);
                    EndDate = EndDate.Date.AddHours(objProjectScheduleModel.EndTime.Hours).AddMinutes(objProjectScheduleModel.EndTime.Minutes);
                    objProjectScheduleCalendarModel.Add(new ProjectScheduleCalendarModel
                    {
                        DayType = objProjectScheduleModel.DayType,
                        CalendarDate = counter,
                        EndDateTime = EndDate.ProfileToUtcDateTime(userTimeZone),
                        StartDateTime = counter,
                    });
                }

                string status = await ValidateProjectscheduleAsync(objProjectScheduleModel, userTimeZone, projectID);
                if (string.IsNullOrEmpty(status))
                {
                    return await _projectScheduleRepository.CreateUpdateProjectSchedule(objProjectScheduleModel, ProjectUserRoleID, objProjectScheduleCalendarModel, userTimeZone, UserId, projectID);
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in  project schedule page while updating Project Schedule Details:Method Name: UpdateProjectSchedule()");
                throw;
            }
        }
        public async Task<DayWiseScheduleModel> GetDayWiseConfig(DayWiseScheduleModel objDayWiseScheduleModel, long ProjectId, UserTimeZone userTimeZone)
        {
            logger.LogInformation("ProjectSchedule Service >> GetDayWiseConfig() started");
            try
            {
                return await _projectScheduleRepository.GetDayWiseConfig(objDayWiseScheduleModel, ProjectId, userTimeZone);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in project schedule page while getting Day Wise Reporting time for specific project:Method Name:GetDayWiseConfig()");
                throw;
            }
        }
        public async Task<string> UpdateDayWiseConfig(long ProjectID, long ProjectUserRoleID, DayWiseScheduleModel objDayWiseScheduleModel, UserTimeZone userTimeZone)
        {
            logger.LogInformation("ProjectSchedule Service >> UpdateDayWiseConfig() started");
            try
            {
                string status = await ValidateDaywiseAsync(objDayWiseScheduleModel, ProjectID, userTimeZone);
                if (string.IsNullOrEmpty(status))
                {
                    status = await _projectScheduleRepository.UpdateDayWiseConfig(ProjectID, ProjectUserRoleID, objDayWiseScheduleModel);
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in project schedule page while updating Day wise Configuration: Method Name: UpdateDayWiseConfig()");
                throw;
            }
        }
        private async Task<string> ValidateProjectscheduleAsync(ProjectScheduleModel objProjectScheduleModel, UserTimeZone userTimeZone, long projectID)
        {
            var status = string.Empty;

            logger.LogInformation("ProjectSchedule Service >> ValidateProjectschedule() started");
            try
            {
                if (objProjectScheduleModel != null)
                {
                    ProjectScheduleModel project = await _projectScheduleRepository.GetProjectSchedule(projectID, userTimeZone);

                    if (objProjectScheduleModel.IsUpdate)
                    {
                        status = ValidateUpdateProjectSchedule(project, objProjectScheduleModel, userTimeZone);
                    }
                    else
                    {
                        status = CompareProjectScheduleDate(objProjectScheduleModel);
                    }
                }
                //if (objProjectScheduleModel != null)
                //{
                //    ProjectScheduleModel project = await _projectScheduleRepository.GetProjectSchedule(projectID, userTimeZone);

                //    if (objProjectScheduleModel.IsUpdate)
                //    {
                //        if (project.StartDate <= DateTime.MinValue)
                //        {
                //            return "Fromdatewarning";
                //        }
                //        project.StartDate = project.StartDate.ProfileToUtcDateTime(userTimeZone);
                //        project.ExpectedEndDate = project.ExpectedEndDate.ProfileToUtcDateTime(userTimeZone);
                //        if (project.StartDate.Date > DateTime.UtcNow.Date)
                //        {
                //            status = CompareProjectScheduleDate(objProjectScheduleModel);
                //        }
                //        else
                //        {
                //            if (objProjectScheduleModel.ExpectedEndDate <= DateTime.UtcNow)
                //            {
                //                status = "Todatewarning";
                //            }
                //        }
                //    }
                //    else
                //    {
                //        status = CompareProjectScheduleDate(objProjectScheduleModel);
                //    }
                //}
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in project schedule page while validating Projectschedule page data: Method Name: ValidateProjectschedule()");
                throw;
            }
        }
        private string ValidateUpdateProjectSchedule(ProjectScheduleModel project, ProjectScheduleModel objProjectScheduleModel, UserTimeZone userTimeZone)
        {
            if (project.StartDate <= DateTime.MinValue)
            {
                return "Fromdatewarning";
            }

            project.StartDate = project.StartDate.ProfileToUtcDateTime(userTimeZone);
            project.ExpectedEndDate = project.ExpectedEndDate.ProfileToUtcDateTime(userTimeZone);

            if (project.StartDate.Date > DateTime.UtcNow.Date)
            {
                return CompareProjectScheduleDate(objProjectScheduleModel);
            }
            else if (objProjectScheduleModel.ExpectedEndDate <= DateTime.UtcNow)
            {
                return "Todatewarning";
            }

            return string.Empty;
        }
        private static string CompareProjectScheduleDate(ProjectScheduleModel objProjectScheduleModel)
        {
            string status = string.Empty;
            if (objProjectScheduleModel.StartDate <= DateTime.MinValue)
            {
                status = "Fromdatewarning";
            }
            else
             if (objProjectScheduleModel.ExpectedEndDate <= DateTime.MinValue)
            {
                status = "Todatewarning";
            }
            else if (objProjectScheduleModel.StartDate.Date < DateTime.UtcNow.Date)
            {
                status = "Futuredatewarning";
            }
            else if (objProjectScheduleModel.ExpectedEndDate <= objProjectScheduleModel.StartDate)
            {
                status = "Fromdatelesswarning";
            }
            else if (objProjectScheduleModel.StartTime >= objProjectScheduleModel.EndTime)
            {
                status = "Fromtimelesswarning";
            }
            return status;
        }

        private async Task<string> ValidateDaywiseAsync(DayWiseScheduleModel objDayWiseScheduleModel, long projectID, UserTimeZone userTimeZone)
        {
            var status = string.Empty;

            logger.LogInformation("ProjectSchedule Service >> ValidateDaywise() started");
            try
            {
                if (objDayWiseScheduleModel != null)
                {
                    ProjectScheduleModel project = await _projectScheduleRepository.GetProjectSchedule(projectID, userTimeZone);
                    if (project == null)
                    {
                        return "Invalid";
                    }

                    if (objDayWiseScheduleModel.EndDateTime <= objDayWiseScheduleModel.StartDateTime || objDayWiseScheduleModel.StartDateTime.UtcToProfileDateTime(userTimeZone).Date < DateTime.UtcNow.UtcToProfileDateTime(userTimeZone).Date || objDayWiseScheduleModel.EndDateTime.UtcToProfileDateTime(userTimeZone).Date < DateTime.UtcNow.UtcToProfileDateTime(userTimeZone).Date)
                    {
                        status = "Validtimealert";
                    }
                    else if (objDayWiseScheduleModel.Remarks != null && objDayWiseScheduleModel.Remarks.Trim().Length > 250)
                    {
                        status = "REMARKLEN";
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in project schedule page while validating ValidateDaywise page data: Method Name: ValidateDaywise()");
                throw;
            }

        }

    }
}
