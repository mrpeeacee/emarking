using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.ProjectSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Setup
{
    public class ProjectSchedulesRepository : BaseRepository<ProjectSchedulesRepository>, IProjectScheduleRepository
    {
        private readonly ApplicationDbContext context;
        public ProjectSchedulesRepository(ApplicationDbContext context, ILogger<ProjectSchedulesRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        /// <summary>
        /// GetProjectSchedule:this api is to Get ProjectSchedule details for specfic project
        /// </summary>
        /// <returns></returns>
        /// </summary>
        public async Task<ProjectScheduleModel> GetProjectSchedule(long ProjectID, UserTimeZone userTimeZone)
        {
            ProjectScheduleModel objGetTime = new();
            try
            {
                var projects = await (from pst in context.ProjectScheduleCalendars
                                      join psd in context.ProjectSchedules on pst.ProjectScheduleId equals psd.ProjectScheduleId
                                      where psd.ProjectId == ProjectID && pst.ProjectScheduleId == psd.ProjectScheduleId && psd.IsActive == true && !psd.Isdeleted && !pst.Isdeleted
                                      select new
                                      {
                                          pst.DayType,
                                          psd.ProjectId,
                                          psd.ScheduleName,
                                          psd.StartDate,
                                          psd.ActualEndDate,
                                          psd.ExpectedEndDate,
                                          psd.WorkingDaysConfig,
                                          psd.ProjectScheduleCalendars
                                      }).FirstOrDefaultAsync();

                if (projects != null)
                {
                    objGetTime.ExpectedEndDate = (DateTime)(projects.ExpectedEndDate?.UtcToProfileDateTime(userTimeZone));
                    objGetTime.StartDate = projects.StartDate.UtcToProfileDateTime(userTimeZone);

                    objGetTime.StartTime = TimeSpan.Parse(objGetTime.StartDate.ToString("HH:mm:ss"));
                    objGetTime.EndTime = TimeSpan.Parse(objGetTime.ExpectedEndDate.ToString("HH:mm:ss"));

                    objGetTime.DayType = projects.DayType;
                    objGetTime.ScheduleName = projects.ScheduleName;
                    objGetTime.WorkingDaysConfigJson = JObject.Parse(projects.WorkingDaysConfig);
                    objGetTime.ScheduleTimeModels = projects?.ProjectScheduleCalendars?.Where(a => a.DayType == 1)
                        .Select(a => new ProjectScheduleCalendarModel { CalendarDate = a.StartTime.UtcToProfileDateTime(userTimeZone) })
                        .ToList();
                    objGetTime.IsUpdate = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in project schedule page while getting Project Schedule details for specific project:Method Name:GetProjectSchedule() and ProjectID: ProjectID=" + ProjectID.ToString());
                throw;
            }
            return objGetTime;
        }

        /// <summary>
        /// CreateProjectSchedule:this api is to create ProjectSchedule for specfic project
        /// </summary>
        /// <param name="objProjectScheduleModel"></param>
        /// <returns></returns>
        private async Task<string> CreateProjectSchedule(ProjectScheduleModel objProjectScheduleModel, long ProjectUserRoleID, List<ProjectScheduleCalendarModel> objProjectScheduleCalendarModel, long projectID, long userId)
        {
            ProjectSchedule scheduleDate;
            ProjectScheduleCalendar scheduleTime;
            ProjectInfo projectinfo;
            string status;
            try
            {
                scheduleDate = new ProjectSchedule()
                {
                    ProjectId = projectID,
                    StartDate = objProjectScheduleModel.StartDate,
                    ExpectedEndDate = objProjectScheduleModel.ExpectedEndDate,
                    Isdeleted = false,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = ProjectUserRoleID,
                    WorkingDaysConfig = JsonSerializer.Serialize(objProjectScheduleModel.WorkingDaysConfig),
                    IsActive = true
                };

                context.ProjectSchedules.Add(scheduleDate);

                _ = context.SaveChanges();

                if (scheduleDate.ProjectScheduleId > 0)
                {
                    projectinfo = await context.ProjectInfos.Where(item => item.ProjectId == projectID).FirstOrDefaultAsync();
                    if (projectinfo != null)
                    {
                        projectinfo.ProjectStartDate = objProjectScheduleModel.StartDate;
                        projectinfo.ProjectEndDate = objProjectScheduleModel.ExpectedEndDate;
                        projectinfo.ModifiedBy = userId;
                        projectinfo.ModifiedDate = DateTime.UtcNow;
                        context.ProjectInfos.Update(projectinfo);
                    }
                }

                if (scheduleDate.ProjectScheduleId > 0)
                {
                    objProjectScheduleCalendarModel.ForEach(data =>
                    {
                        scheduleTime = new ProjectScheduleCalendar()
                        {
                            ProjectScheduleId = scheduleDate.ProjectScheduleId,
                            DayType = data.DayType,
                            StartTime = data.StartDateTime,
                            EndTime = data.EndDateTime,
                            Isdeleted = false,
                            CreatedBy = ProjectUserRoleID,
                            CreatedDate = DateTime.UtcNow,
                        };
                        context.ProjectScheduleCalendars.Add(scheduleTime);
                    });
                }
                _ = context.SaveChanges();
                status = "SU001";
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in  project schedule page while updating Project Schedule Details:Method Name: CreateProjectSchedule() and ProjectID=" + projectID.ToString());
                throw;
            }
            return status;
        }

        /// <summary>
        /// UpdateProjectSchedule:this api is to Update ProjectSchedule for specfic project
        /// </summary>
        /// <param name="objProjectScheduleModel"></param>
        /// <returns></returns>
        public async Task<string> CreateUpdateProjectSchedule(ProjectScheduleModel objProjectScheduleModel, long ProjectUserRoleID, List<ProjectScheduleCalendarModel> objProjectScheduleCalendarModel, UserTimeZone userTimeZone, long userId, long projectID)
        {
            string status = "";

            ProjectInfo projectinfo;
            ProjectScheduleCalendar projectScheduletimecreate;
            using (var DbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    ProjectSchedule projectSchedule = await context.ProjectSchedules.Where(item => item.ProjectId == projectID && item.IsActive == true && !item.Isdeleted).FirstOrDefaultAsync();
                    if (projectSchedule != null)
                    {
                        projectSchedule.ModifiedDate = DateTime.UtcNow;
                        projectSchedule.ModifiedBy = ProjectUserRoleID;
                        projectSchedule.IsActive = false;
                        context.ProjectSchedules.Update(projectSchedule);
                        _ = await context.SaveChangesAsync();

                        List<ProjectScheduleCalendar> OldProjectScheduleCalendars = context.ProjectScheduleCalendars.Where(sid => sid.ProjectScheduleId == projectSchedule.ProjectScheduleId && !sid.Isdeleted).ToList();

                        if (OldProjectScheduleCalendars.Count > 0)
                        {
                            OldProjectScheduleCalendars.ForEach(data =>
                            {
                                data.ModifiedBy = ProjectUserRoleID;
                                data.Modifieddate = DateTime.UtcNow;
                                context.ProjectScheduleCalendars.Update(data);
                            });
                            _ = context.SaveChanges();
                        }

                        ProjectSchedule newProjectSchedule = new ProjectSchedule()
                        {
                            ProjectId = projectID,
                            StartDate = objProjectScheduleModel.StartDate,
                            ExpectedEndDate = objProjectScheduleModel.ExpectedEndDate,
                            Isdeleted = false,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = ProjectUserRoleID,
                            WorkingDaysConfig = JsonSerializer.Serialize(objProjectScheduleModel.WorkingDaysConfig),
                            RefProjectScheduleId = projectSchedule.ProjectScheduleId,
                            IsActive = true
                        };
                        context.ProjectSchedules.Add(newProjectSchedule);
                        _ = await context.SaveChangesAsync();

                        if (newProjectSchedule.ProjectScheduleId > 0)
                        {
                            objProjectScheduleCalendarModel.ForEach(data =>
                            {
                                projectScheduletimecreate = new ProjectScheduleCalendar()
                                {
                                    DayType = data.DayType,
                                    StartTime = data.StartDateTime,
                                    EndTime = data.EndDateTime,
                                    Isdeleted = false,
                                    CreatedBy = ProjectUserRoleID,
                                    CreatedDate = DateTime.UtcNow,
                                    ProjectScheduleId = newProjectSchedule.ProjectScheduleId
                                };
                                context.ProjectScheduleCalendars.Add(projectScheduletimecreate);
                            });
                            _ = context.SaveChanges();
                        }

                        projectinfo = await context.ProjectInfos.Where(item => item.ProjectId == projectID).FirstOrDefaultAsync();
                        if (projectinfo != null)
                        {
                            projectinfo.ProjectStartDate = objProjectScheduleModel.StartDate;
                            projectinfo.ProjectEndDate = objProjectScheduleModel.ExpectedEndDate.Date;
                            projectinfo.ModifiedBy = userId;
                            projectinfo.ModifiedDate = DateTime.UtcNow;
                            context.ProjectInfos.Update(projectinfo);
                            _ = await context.SaveChangesAsync();
                        }

                        List<ProjectScheduleCalendar> NewProjectScheduleCalendars = context.ProjectScheduleCalendars.Where(sid => sid.ProjectScheduleId == newProjectSchedule.ProjectScheduleId && !sid.Isdeleted).ToList();

                        if (objProjectScheduleModel.Confirmdialogeventvalue == 2)
                        {
                            NewProjectScheduleCalendars.ForEach(newdata =>
                            {
                                var newe = OldProjectScheduleCalendars.FirstOrDefault(a => ((DateTime)a.StartTime).Date == ((DateTime)newdata.StartTime).Date);

                                if (newe != null)
                                {
                                    newdata.DayType = newe.DayType;
                                    newdata.StartTime = newe.StartTime;
                                    newdata.EndTime = newe.EndTime;
                                    newdata.Remarks = newe.Remarks;
                                    newdata.ModifiedBy = ProjectUserRoleID;
                                    newdata.Modifieddate = DateTime.UtcNow;
                                    context.ProjectScheduleCalendars.Update(newdata);
                                }
                            });
                            _ = context.SaveChanges();
                        }
                        else
                        {
                            NewProjectScheduleCalendars.ForEach(newdata =>
                            {
                                if (((DateTime)newdata.StartTime).Date < DateTime.UtcNow.Date)
                                {
                                    var newe = OldProjectScheduleCalendars.FirstOrDefault(a => ((DateTime)a.StartTime).Date == ((DateTime)newdata.StartTime).Date);

                                    if (newe != null)
                                    {
                                        newdata.DayType = newe.DayType;
                                        newdata.StartTime = newe.StartTime;
                                        newdata.EndTime = newe.EndTime;
                                        newdata.Remarks = newe.Remarks;
                                        newdata.ModifiedBy = ProjectUserRoleID;
                                        newdata.Modifieddate = DateTime.UtcNow;
                                        context.ProjectScheduleCalendars.Update(newdata);
                                    }
                                }

                            });
                            _ = context.SaveChanges();
                        }
                        status = "U001";
                    }
                    else
                    {
                        status = await CreateProjectSchedule(objProjectScheduleModel, ProjectUserRoleID, objProjectScheduleCalendarModel, projectID, userId);
                    }
                    DbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    DbContextTransaction.Rollback();
                    logger.LogError(ex,"Error in  project schedule page while updating Project Schedule Details:Method Name: UpdateProjectSchedule() and ProjectID=" + projectID.ToString());
                    throw;
                }
            }
            return status;
        }

        /// <summary>
        /// GetDayWiseConfig:this api is to Get DayWise Configurations for specfic project
        /// </summary>
        /// <param name="objDayWiseScheduleModel"></param>
        /// <returns></returns>
        public async Task<DayWiseScheduleModel> GetDayWiseConfig(DayWiseScheduleModel objDayWiseScheduleModel, long ProjectId, UserTimeZone userTimeZone)
        {
            DayWiseScheduleModel objGetTime = new();
            try
            {

                var projects = await (from pst in context.ProjectScheduleCalendars
                                      join psd in context.ProjectSchedules on pst.ProjectScheduleId equals psd.ProjectScheduleId
                                      where psd.ProjectId == ProjectId
                                      && !pst.Isdeleted && psd.IsActive == true && !psd.Isdeleted
                                      select new
                                      {
                                          pst.StartTime,
                                          pst.EndTime,
                                          pst.DayType,
                                          pst.Remarks,
                                          pst.ProjectCalendarId,
                                          pst.ProjectScheduleId,
                                      }).ToListAsync();

                if (projects != null && projects.Any(a => ((DateTime)a.StartTime).UtcToProfileDateTime(userTimeZone).Date == objDayWiseScheduleModel.SelectedDate.UtcToProfileDateTime(userTimeZone).Date))
                {
                    var project = projects.FirstOrDefault(a => ((DateTime)a.StartTime).UtcToProfileDateTime(userTimeZone).Date == objDayWiseScheduleModel.SelectedDate.UtcToProfileDateTime(userTimeZone).Date);
                    if (project != null)
                    {
                        objGetTime.StartTime = TimeSpan.Parse(((DateTime)project.StartTime.UtcToProfileDateTime(userTimeZone)).ToString("HH:mm:ss"));
                        objGetTime.EndTime = TimeSpan.Parse(((DateTime)project.EndTime.UtcToProfileDateTime(userTimeZone)).ToString("HH:mm:ss"));
                        objGetTime.DayType = project.DayType;
                        objGetTime.Remarks = project.Remarks;
                        objGetTime.ProjectCalendarID = project.ProjectCalendarId;
                        objGetTime.ChoosenDate = (DateTime)project.StartTime.UtcToProfileDateTime(userTimeZone);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in project schedule page while getting Day Wise Reporting time for specific project:Method Name:GetDayWiseConfig()");
                throw;
            }
            return objGetTime;
        }

        /// <summary>
        /// UpdateDayWiseConfig:this api is to Update DayWise Configuration for specfic project
        /// </summary>
        /// <param name="objDayWiseScheduleModel"></param>
        /// <returns></returns>
        /// </summary>
        public async Task<string> UpdateDayWiseConfig(long ProjectID, long ProjectUserRoleID, DayWiseScheduleModel objDayWiseScheduleModel)
        {
            string status = "";
            ProjectScheduleCalendar projectScheduleTime;

            try
            {
                var projectSchedule = (from ps in context.ProjectSchedules
                                       select ps).FirstOrDefault(x => x.ProjectId == ProjectID && !x.Isdeleted && x.IsActive == true);
                if (projectSchedule != null)
                {
                    projectScheduleTime = context.ProjectScheduleCalendars.FirstOrDefault(item => item.ProjectScheduleId == projectSchedule.ProjectScheduleId && item.ProjectCalendarId == objDayWiseScheduleModel.ProjectCalendarID && !item.Isdeleted);

                    if (projectScheduleTime != null)
                    {
                        projectScheduleTime.DayType = objDayWiseScheduleModel.DayType;
                        if (projectScheduleTime.DayType == 1)
                        {
                            projectScheduleTime.StartTime = objDayWiseScheduleModel.StartDateTime;
                            projectScheduleTime.EndTime = objDayWiseScheduleModel.EndDateTime;
                        }
                        projectScheduleTime.Remarks = objDayWiseScheduleModel.Remarks;
                        projectScheduleTime.Modifieddate = DateTime.UtcNow;
                        projectScheduleTime.ModifiedBy = ProjectUserRoleID;
                        context.ProjectScheduleCalendars.Update(projectScheduleTime);
                        _ = await context.SaveChangesAsync();
                        status = "U001";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in project schedule page while updating Day wise Configuration: Method Name: UpdateDayWiseConfig()");
                throw;
            }
            return status;
        }

    }
}
