using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Dashboard;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Dashboard;
using Saras.eMarking.Domain.ViewModels.Project.Privilege;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Saras.eMarking.Infrastructure.Dashboard
{
    public class DashboardRepository : BaseRepository<DashboardRepository>, IDashboardRepository
    {
        private readonly ApplicationDbContext context;
        public DashboardRepository(ApplicationDbContext context, ILogger<DashboardRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        /// <summary>
        /// Method to Get project stastics for specific marker
        /// </summary> 
        /// <returns></returns>
        public async Task<DashboardStasticsModel> GetProjectStatistics(long UserId)
        {
            logger.LogDebug("DashboardRepository >> GetProjectStatistics Method started.");
            DashboardStasticsModel statisticsModel = new();
            try
            {
                //----> Get all Project statistics.
                List<ProjectInfo> projects = await (from pth in context.ProjectQigteamHierarchies
                                                    join
                                                      uri in context.ProjectUserRoleinfos on pth.ProjectUserRoleId equals uri.ProjectUserRoleId
                                                    join u in context.UserInfos on uri.UserId equals u.UserId
                                                    join p in context.ProjectInfos on uri.ProjectId equals p.ProjectId
                                                    where uri.UserId == UserId && !uri.Isdeleted && uri.IsActive == true && !p.IsDeleted && uri.IsActive == true && !u.IsDeleted && !pth.Isdeleted && pth.IsActive == true
                                                    select p).Distinct().ToListAsync();

                if (projects != null && projects.Count > 0)
                {
                    //----> Get all total count.
                    statisticsModel.TotalProjects = projects.Count;
                    statisticsModel.ProjectsInProgress = projects.Count(x => x.ProjectStatus == 1);
                    //Project complete = 2 is not in use. Project closer = 3
                    statisticsModel.ProjectsCompleted = projects.Count(x => x.ProjectStatus == 3);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Getting error while Get project statistics for specific marker in DashboardRepository page. Method : GetProjectStatistics()");
                throw;
            }
            return statisticsModel;
        }

        /// <summary>
        /// List the exam year
        /// </summary>
        /// <returns></returns>

        public async Task<List<AllExamYear>> ListAllExamYear()
        {
            logger.LogDebug("DashboardRepository >> ListAllExamYear Method started.");
            List<AllExamYear> result = new List<AllExamYear>();
            try
            {
                //----> Get list of exam year.
                result = await (from ey in context.ExamYears
                                where !ey.IsDeleted
                                select new AllExamYear()
                                {
                                    YearId = ey.YearId,
                                    Year = ey.Year
                                }).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Getting error while get all Exam years. Method : ListAllExamYear()");
                throw;
            }
        }

        /// <summary>
        /// Method to get all activities created
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="TimeZone"></param>
        /// <returns></returns>
        public async Task<IList<DashboardProjectModel>> GetAll(long UserId, UserTimeZone TimeZone, int Year = 0, string RoleCode="")
        {
            logger.LogDebug("DashboardRepository >> GetAll Method started.");
            try
            {
                IList<DashboardProjectModel> result = await GetProjects(UserId, TimeZone, null, Year, RoleCode);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while get all activities created in DashboardRepository page. Method : GetAll() and TokenID : UserId {UserId} and {TimeZone} and  {Year}");
                throw;
            }
        }

        /// <summary>
        /// Method to the project by given LoginName
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="TimeZone"></param>
        /// <param name="name"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        private async Task<IList<DashboardProjectModel>> GetProjects(long UserId, UserTimeZone TimeZone, string name = null, int Year = 0, string RoleCode = "")
        {
            logger.LogDebug("DashboardRepository >> GetProjects Method started.");
            List<DashboardProjectModel> result = null;
            try
            {


                //----> Get project list.

                List<ProjectDetails> allProjects =GetActiveProjects(UserId, RoleCode);

                if (allProjects != null)
                {
                    List<long> projids = allProjects.Select(x => x.ProjectId).ToList();

                    List<long> projroleids = allProjects.Select(x => x.ProjectUserRoleId).ToList();

                    //----> Get QIG's under the project.
                    var projQigs = (await (from pq in context.ProjectQigs
                                    join pqth in context.ProjectQigteamHierarchies on pq.ProjectQigid equals pqth.Qigid
                                    where projids.Contains(pq.ProjectId == null ? 0 : (long)pq.ProjectId) && pq.IsManualMarkingRequired
                                    && !pq.IsDeleted && !pqth.Isdeleted && pqth.IsActive == true && projroleids.Contains(pqth.ProjectUserRoleId)
                                    select pq).ToListAsync()).ToList();

                    result = new List<DashboardProjectModel>();

                    var role = (from u in context.UserInfos
                                join um in context.UserToRoleMappings on u.UserId equals um.UserId
                                join r in context.Roleinfos on um.RoleId equals r.RoleId
                                where u.UserId == UserId && !u.IsDeleted && !um.IsDeleted && !r.Isdeleted
                                select new { r.RoleCode }).FirstOrDefault();

                    if (Year > 0)
                    {
                        allProjects = allProjects.Where(x => x.ExamYear == Year).ToList();
                    }
                    foreach (var grpProject in allProjects.GroupBy(f => f.ProjectUserRoleId))
                    {
                        List<DashboardProjectQigModel> qigs = projQigs.Where(qg => qg.IsManualMarkingRequired).Select(qig => new DashboardProjectQigModel
                        {
                            QigCreatedDate = qig.CreatedDate,
                            ProjectQigid = qig.ProjectQigid,
                            QigName = qig.Qigname,
                            ProjectId = qig.ProjectId,
                            QigCode = qig.Qigcode
                        }).OrderBy(q => q.QigCode).ToList();

                        DashboardProjectModel project = grpProject.Select(p => new DashboardProjectModel
                        {
                            ProjectID = p.ProjectId,
                            ProjectName = p.ProjectName,
                            ProjectCode = p.ProjectCode,
                            ProjectStartDate = p.ProjectStartDate == null ? null : ((DateTime)p.ProjectStartDate).UtcToProfileDateTime(TimeZone),
                            ProjectEndDate = p.ProjectEndDate == null ? null : ((DateTime)p.ProjectEndDate).UtcToProfileDateTime(TimeZone),
                            CreationType = p.CreationType,
                            ProjectStatus = p.ProjectStatus,
                            Year = p.ExamYear,
                            CreatedDate = p.ProjectCreatedDate,
                            UserIsActive = p.UserIsActive,
                            ProjectUserRole = new ProjectUserroleModel
                            {
                                UserID = p.UserId,
                                RoleCode = p.RoleCode,
                                RoleName = p.RoleName,
                                RoleID = p.RoleId,
                                ProjectUserRoleID = p.ProjectUserRoleId,
                                ReportingTo = p.ReportingTo

                            },
                            ProjectQigs = qigs.Where(prq => prq.ProjectId == p.ProjectId).OrderBy(z => z.QigCode).ToList(),
                            RoleCode = role.RoleCode
                        }).FirstOrDefault();

                        if (project != null)
                        {
                            if (project.ProjectUserRole.RoleCode is "AO"
                                or "EO"
                                or "SUPERADMIN"
                                or "SERVICEADMIN"
                                or "Admin"
                                or "EM")
                            {
                                result.Add(project);
                            }
                            else
                            {
                                if (project?.ProjectQigs?.Count > 0)
                                {
                                    result.Add(project);
                                }
                            }

                            

                        }
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        return result.Where(a => a.ProjectName == name).ToList();
                    }
                    result = result.OrderBy(q => q.ProjectID).ToList();

                   
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while project by given LoginName in DashboardRepository page. Method : GetProjects() and TokenID : {UserId} and {TimeZone} and {name} and {Year}");
                throw;
            }
            return result;
        }
        private List<ProjectDetails> GetArchiveProjects(long userId, bool archive)
        {
            List<ProjectDetails> result = new List<ProjectDetails>();

            result = (from uri in context.ProjectUserRoleinfoArchives
                      join u in context.UserInfos on uri.UserId equals u.UserId
                      join p in context.ProjectInfos on uri.ProjectId equals p.ProjectId
                      join r in context.Roleinfos on uri.RoleId equals r.RoleId
                      where !r.Isdeleted && !u.IsDeleted && !p.IsDeleted &&
                      uri.UserId == userId && !uri.Isdeleted && !p.IsDeleted && uri.IsActive == true && p.IsArchive == archive

                      select new ProjectDetails
                      {
                          ProjectId = p.ProjectId,
                          ProjectName = p.ProjectName,
                          ProjectCode = p.ProjectCode,
                          ProjectStartDate = p.ProjectStartDate,
                          ProjectEndDate = p.ProjectEndDate,
                          CreationType = p.CreationType,
                          ProjectStatus = p.ProjectStatus,
                          IsArchive = p.IsArchive,
                          UserIsActive = u.IsActive,
                          UserId = uri.UserId,
                          RoleCode = r.RoleCode,
                          RoleName = r.RoleName,
                          RoleId = uri.RoleId,
                          ProjectUserRoleId = uri.ProjectUserRoleId,
                          ReportingTo = uri.ReportingTo,
                          ExamYear = p.ExamYear,
                          ProjectCreatedDate = p.CreateDate,
                      }).ToList();

            return result;
        }
        private List<ProjectDetails> GetActiveProjects(long UserId, string RoleCode)
        {
           

            List<ProjectDetails> result = new List<ProjectDetails>();

            if (RoleCode == "MARKINGPERSONNEL" || RoleCode== "CM" ||RoleCode== "ACM" || RoleCode== "TL" || RoleCode== "ATL")
            {
                DateTime CurrentDate = DateTime.UtcNow.Date;
                DateTime StartDate = DateTime.UtcNow;


                result = (from uri in context.ProjectUserRoleinfos
                          join u in context.UserInfos on uri.UserId equals u.UserId
                          join p in context.ProjectInfos on uri.ProjectId equals p.ProjectId
                          join r in context.Roleinfos on uri.RoleId equals r.RoleId
                          where !r.Isdeleted && !u.IsDeleted && !p.IsDeleted &&
                          uri.UserId == UserId && !uri.Isdeleted && !p.IsDeleted && uri.IsActive == true && p.ProjectStartDate <= StartDate &&
                          p.ProjectEndDate >= CurrentDate && p.ProjectStatus != 3

                          select new ProjectDetails
                          {
                              ProjectId = p.ProjectId,
                              ProjectName = p.ProjectName,
                              ProjectCode = p.ProjectCode,
                              ProjectStartDate = p.ProjectStartDate,
                              ProjectEndDate = p.ProjectEndDate,
                              CreationType = p.CreationType,
                              ProjectStatus = p.ProjectStatus,
                              IsArchive = p.IsArchive,
                              UserIsActive = u.IsActive,
                              UserId = uri.UserId,
                              RoleCode = r.RoleCode,
                              RoleName = r.RoleName,
                              RoleId = uri.RoleId,
                              ProjectUserRoleId = uri.ProjectUserRoleId,
                              ReportingTo = uri.ReportingTo,
                              ExamYear = p.ExamYear,
                              ProjectCreatedDate = p.CreateDate,
                          }).ToList();

            }
            else
            {
                result = (from uri in context.ProjectUserRoleinfos
                          join u in context.UserInfos on uri.UserId equals u.UserId
                          join p in context.ProjectInfos on uri.ProjectId equals p.ProjectId
                          join r in context.Roleinfos on uri.RoleId equals r.RoleId
                          where !r.Isdeleted && !u.IsDeleted && !p.IsDeleted &&
                          uri.UserId == UserId && !uri.Isdeleted && !p.IsDeleted && uri.IsActive == true

                          select new ProjectDetails
                          {
                              ProjectId = p.ProjectId,
                              ProjectName = p.ProjectName,
                              ProjectCode = p.ProjectCode,
                              ProjectStartDate = p.ProjectStartDate,
                              ProjectEndDate = p.ProjectEndDate,
                              CreationType = p.CreationType,
                              ProjectStatus = p.ProjectStatus,
                              IsArchive = p.IsArchive,
                              UserIsActive = u.IsActive,
                              UserId = uri.UserId,
                              RoleCode = r.RoleCode,
                              RoleName = r.RoleName,
                              RoleId = uri.RoleId,
                              ProjectUserRoleId = uri.ProjectUserRoleId,
                              ReportingTo = uri.ReportingTo,
                              ExamYear = p.ExamYear,
                              ProjectCreatedDate = p.CreateDate,
                          }).ToList();

            }
        
            return result;
        }

        /// <summary>
        /// To Get Project Status For MARKINGPERSONNE
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<ProjectStatusDetails> GetProjectStatus(int projectId)
        {
            string Status = "S001";
            List<ProjectStatusDetails> result = null;
            try
            {
                 using (SqlConnection connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand("[Marking].[USPGetProjectScheduleStatus]", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@ProjectId", SqlDbType.Int).Value = projectId;
                    SqlParameter statusParam = new SqlParameter("@Status", SqlDbType.VarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    sqlCommand.Parameters.Add(statusParam);
                    connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    Status = sqlCommand.Parameters["@Status"].Value.ToString(); 
                    
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    if (reader.HasRows)
                    {
                        result = new List<ProjectStatusDetails>();
                        while (reader.Read())
                        {
                            if (Status != "E001" && Status != "E002" && Status != "E003")
                            {
                                ProjectStatusDetails projectStatus = new()
                                {
                                    StartTime = (DateTime)reader["StartTime"],
                                    EndTime = (DateTime)reader["EndTime"],
                                    DayType = Convert.ToByte(reader["DayType"])
                                };
                                result.Add(projectStatus);
                            }
                            else
                            {
                                ProjectStatusDetails projectStatus = new()
                                {
                                    status = Status
                                };
                                result.Add(projectStatus);

                            }
                        }
                    }
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while getting GetProjectStatus in DashboardRepository page. Method :  GetProjectStatus and projectId : {projectId}");
                throw;
            }
            return result;
        }
    }
}
