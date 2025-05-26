using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Domain.ViewModels.Project.Setup.ResolutionOfCoi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Setup
{
    public class ResolutionOfCoiRepository : BaseRepository<ResolutionOfCoiRepository>, IResolutionOfCoiRepository
    {
        private readonly ApplicationDbContext context;
        public ResolutionOfCoiRepository(ApplicationDbContext context, ILogger<ResolutionOfCoiRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        /// <summary>
        /// GetResolutionCOI : This GET Api is used to get all conflict of Interest  schools
        /// </summary> 
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<IList<ResolutionOfCoiModel>> GetResolutionCOI(long ProjectId)
        {
            List<ResolutionOfCoiModel> objresolutionCOI = null;

            try
            {
                logger.LogInformation($"ResolutionOfCoiRepository GetResolutionCOI() Method started.  projectId = {ProjectId}");

                objresolutionCOI = (await (from puri in context.ProjectUserRoleinfos
                                           join ri in context.Roleinfos on puri.RoleId equals ri.RoleId
                                           join ui in context.UserInfos on puri.UserId equals ui.UserId
                                           where puri.ProjectId == ProjectId && ri.RoleName == "MARKER" && puri.IsActive == true
                                           && !ri.Isdeleted && !ui.IsDeleted && !puri.Isdeleted
                                           select new ResolutionOfCoiModel
                                           {
                                               ProjectUserRoleID = puri.ProjectUserRoleId,
                                               USERID = ui.UserId,
                                               UserName = ui.FirstName + " " + ui.LastName,
                                               RoleName = ri.RoleName,
                                               SendingSchoolID = puri.SendingSchoolId
                                           }).ToListAsync()).ToList();


                objresolutionCOI.ForEach(schoollst =>
                {
                    schoollst.SchoolList = (from pusm in context.ProjectUserSchoolMappings
                                            join si in context.SchoolInfos on pusm.ExemptionSchoolId equals si.SchoolId
                                            where pusm.ProjectUserRoleId == schoollst.ProjectUserRoleID && !si.IsDeleted && !pusm.IsDeleted
                                            select new CoiSchoolModel
                                            {
                                                SchoolName = si.SchoolName,
                                                SchoolCode = si.SchoolCode,
                                                ExemptionSchoolID = pusm.ExemptionSchoolId,
                                                IsSendingSchool = pusm.IsSendingSchool
                                            }

                                          ).ToList();

                    schoollst.SchoolList = schoollst?.SchoolList.OrderBy(a => a.SchoolName).ToList();

                    var sendingSchool = context.SchoolInfos.Where(a => a.SchoolId == schoollst.SendingSchoolID && !a.IsDeleted).FirstOrDefault();

                    schoollst.SendingSchoolName = sendingSchool?.SchoolName;
                    schoollst.SendingSchoolCode = sendingSchool?.SchoolCode;

                }
                );

                logger.LogInformation($"ResolutionOfCoiRepository -> GetResolutionCOI() Method ended.  projectId = {ProjectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ResolutionOfCoiRepository->GetResolutionCOI() for getting markers, default school name and Exception schools for specific Project and parameters are project : projectId = {ProjectId}");
                throw;
            }
            return objresolutionCOI;

        }

        /// <summary>
        /// GetSchoolsCOI : This GET Api is used to get all schools preset in emarking
        /// </summary> 
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<IList<CoiSchoolModel>> GetSchoolsCOI(long ProjectId)
        {
            List<CoiSchoolModel> objSchoolsCOI = null;

            try
            {
                logger.LogInformation($"ResolutionOfCoiRepository GetSchoolsCOI() Method started.  projectId = {ProjectId}");

                objSchoolsCOI = (await (from si in context.SchoolInfos
                                        where si.ProjectId == ProjectId && !si.IsDeleted
                                        select new CoiSchoolModel
                                        {
                                            SchoolName = si.SchoolName,
                                            SchoolID = si.SchoolId,
                                            SchoolCode = si.SchoolCode,
                                            ExemptionSchoolID = si.SchoolId
                                        }).OrderBy(a => a.SchoolName).ToListAsync()).ToList();


                logger.LogInformation($"ResolutionOfCoiRepository -> GetSchoolsCOI() Method ended.  projectId = {ProjectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ResolutionOfCoiRepository->GetSchoolsCOI() for specific Project and parameters are project: projectId = {ProjectId}");
                throw;
            }
            return objSchoolsCOI;

        }

        /// <summary>
        /// UpdateResolutionCOI : This POST Api is used to Update Resolution of Conflict of Interest(Coi) schools
        /// </summary>
        /// <param name="ObjCoiSchoolModel"></param>
        /// <param name="ProjectUserRoleID"></param>
        /// <param name="CurrentProjUserRoleId"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<string> UpdateResolutionCOI(List<CoiSchoolModel> ObjCoiSchoolModel, long ProjectUserRoleID, long CurrentProjUserRoleId, long ProjectID)
        {
            string status = "";
            List<ProjectUserSchoolMapping> ProjectUserSchoolMapping;
            ProjectUserSchoolMapping ProjectUserSchoolMappingcreate;
            try
            {


                ProjectUserSchoolMapping = (await (from pusm in context.ProjectUserSchoolMappings
                                                   join si in context.SchoolInfos on pusm.ExemptionSchoolId equals si.SchoolId
                                                   where pusm.ProjectUserRoleId == ProjectUserRoleID && !pusm.IsDeleted && !si.IsDeleted
                                                   select pusm).ToListAsync()).ToList();

                if (ObjCoiSchoolModel.Count == 0 && ProjectUserSchoolMapping.Count == 0)
                {
                    status = "UP001";
                }

                ProjectUserSchoolMapping.ForEach(projectschoolmapping =>
                {
                    if (!ObjCoiSchoolModel.Any(a => a.SchoolID == projectschoolmapping.ExemptionSchoolId))
                    {
                        projectschoolmapping.ModifiedDate = DateTime.UtcNow;
                        projectschoolmapping.ModifiedBy = CurrentProjUserRoleId;
                        projectschoolmapping.IsDeleted = true;
                        context.ProjectUserSchoolMappings.Update(projectschoolmapping);
                    }
                    _ = context.SaveChanges();
                    status = "UP001";
                });

                ProjectUserSchoolMapping = (await (from pusm in context.ProjectUserSchoolMappings
                                                   join si in context.SchoolInfos on pusm.ExemptionSchoolId equals si.SchoolId
                                                   where pusm.ProjectUserRoleId == ProjectUserRoleID && !pusm.IsDeleted && !si.IsDeleted
                                                   select pusm).ToListAsync()).ToList();

                ObjCoiSchoolModel.ForEach(resolutionmodel =>
                {
                    if (!ProjectUserSchoolMapping.Any(a => a.ExemptionSchoolId == resolutionmodel.SchoolID))
                    {
                        ProjectUserSchoolMappingcreate = new ProjectUserSchoolMapping()
                        {
                            ProjectUserRoleId = ProjectUserRoleID,
                            ExemptionSchoolId = resolutionmodel.SchoolID,
                            IsSendingSchool = resolutionmodel.IsSendingSchool,
                            IsDeleted = false,
                            CreatedBy = CurrentProjUserRoleId,
                            CreatedDate = DateTime.UtcNow,
                        };
                        context.ProjectUserSchoolMappings.Add(ProjectUserSchoolMappingcreate);
                        _ = context.SaveChanges();
                    }
                    status = "UP001";
                });

                logger.LogInformation($"ResolutionOfCoiRepository UpdateResolutionCOI() Method ended.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ResolutionOfCoiRepository->UpdateResolutionCOI() for specific Project and parameters are project: projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
                throw;
            }
            return status;
        }
    }
}
