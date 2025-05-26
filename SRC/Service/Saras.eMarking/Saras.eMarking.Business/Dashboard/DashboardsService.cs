using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Dashboard;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Dashboard;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Dashboard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using Saras.eMarking.Domain;

namespace Saras.eMarking.Business.Dashboard
{
    public class DashboardsService : BaseService<DashboardsService>, IDashboardService
    {
        readonly IDashboardRepository _DashboardRepository;
        public DashboardsService(IDashboardRepository DashboardRepository, ILogger<DashboardsService> _logger) : base(_logger)
        {
            _DashboardRepository = DashboardRepository;
        }

        /// <summary>
        /// Method to Get project stastics for specific marker
        /// </summary> 
        /// <returns></returns>
        public async Task<DashboardStasticsModel> GetProjectStatistics(long UserId)
        {
            try
            {
                return await _DashboardRepository.GetProjectStatistics(UserId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while Get project stastics for specific marker in DashboardService page. " +
                    "Method : GetProjectStatistics()");
                throw;
            }
        }

        /// <summary>
        ///  List the exam year
        /// </summary>
        /// <returns></returns>
        public async Task<List<AllExamYear>> ListAllExamYear()
        {
            try
            {

                return await _DashboardRepository.ListAllExamYear();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while get all Exam years. Method : ListAllExamYear()");
                throw;
            }

        }

        /// <summary>
        ///  Method to get all activities created
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="TimeZone"></param>
        /// <returns></returns>
        public async Task<IList<DashboardProjectModel>> GetAll(long UserId, UserTimeZone TimeZone, int Year = 0, long projectId = 0,string RoleCode="")
        {
            try
            {
                IList<DashboardProjectModel> projects = await _DashboardRepository.GetAll(UserId, TimeZone, Year, RoleCode);
                if (projectId > 0)
                {
                    projects = projects.Where(a => a.ProjectID == projectId).ToList();
                }
                return projects;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while get all activities created in DashboardService page. Method : GetAll() and TokenID : {UserId} and {TimeZone} and {Year}");
                throw;
            }

        }


        /// <summary>
        /// To Get Project Status For MARKINGPERSONNE
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="userTimeZone"></param>
        /// <returns></returns>
        public ProjectStatusDetails GetProjectStatus(int ProjectId, UserTimeZone userTimeZone)
        {
            ProjectStatusDetails ProjectStatus = new ();
            List<ProjectStatusDetails> ProjectList = new();
            try
            {

                ProjectList = _DashboardRepository.GetProjectStatus(ProjectId);
                if (ProjectList.Count >0)
                {
                    int count = 0;
                    foreach (var item in ProjectList)
                    {
                        if (item.DayType == 1)
                        {
                            item.StartTime = DateTimeUtils.UtcToProfileDateTime(item.StartTime, userTimeZone);
                            item.EndTime = DateTimeUtils.UtcToProfileDateTime(item.EndTime, userTimeZone);
                            item.curDateTime = DateTimeUtils.UtcToProfileDateTime(DateTime.UtcNow, userTimeZone);
                            item.DayType = item.DayType;

                            if (item.StartTime <= item.curDateTime && item.EndTime >= item.curDateTime && item.DayType == 1)
                            {
                                count++;
                            }

                        }
                        else
                        {
                            if(!string.IsNullOrEmpty(item.status))
                            {
                                ProjectStatus.status = item.status;
                                return ProjectStatus;
                            }
                            else
                            {
                                ProjectStatus.status = "E002";
                                return ProjectStatus;
                            }
                           
                        }
                       
                    }
                    if (count > 0)
                    {
                        ProjectStatus.status = "S001";

                        return ProjectStatus;
                    }
                    else
                    {
                        ProjectStatus.status = "E004";
                        return ProjectStatus;
                    }
                }
                return ProjectStatus;


            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while getting Project Status.Method:GetProjectStatus() and ProjectId:{ProjectId}");
                throw;
            }
        }
    }
}
