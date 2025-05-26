using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Domain.ViewModels.Project.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Setup
{
    public class BasicDetailsRepository : BaseRepository<BasicDetailsRepository>, IBasicDetailsRepository
    {
        private readonly ApplicationDbContext context;
        public BasicDetailsRepository(ApplicationDbContext context, ILogger<BasicDetailsRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        public async Task<BasicDetailsModel> GetBasicDetails(long ProjectID)
        {
            BasicDetailsModel result;
            try
            {

                var Basicdeatils = await (from projectinfo in context.ProjectInfos
                                          join examlevel in context.ExamLevels on projectinfo.ExamLevelId equals examlevel.ExamLevelId
                                          join examseries in context.ExamSeries on projectinfo.ExamseriesId equals examseries.ExamSeriesId
                                          join subinfo in context.SubjectInfos on projectinfo.SubjectId equals subinfo.SubjectId
                                          join paperinfo in context.SubjectPaperInfos on projectinfo.PaperId equals paperinfo.PaperId
                                          join moa in context.ModeOfAssessments on projectinfo.Moa equals moa.Moaid
                                          where projectinfo.ProjectId == ProjectID && !projectinfo.IsDeleted && !examlevel.IsDeleted
                                          && !examseries.IsDeleted && !subinfo.IsDeleted && !paperinfo.IsDeleted
                                          && !moa.IsDeleted
                                          select new
                                          {
                                              projectinfo.ProjectCode,
                                              projectinfo.ProjectName,
                                              projectinfo.ExamYear,
                                              examlevel.ExamLevelName,
                                              examseries.ExamSeriesName,
                                              subinfo.SubjectName,
                                              subinfo.SubjectCode,
                                              paperinfo.PaperName,
                                              moa.Moaname,
                                              projectinfo.ProjectInfo1
                                          }
                                 ).FirstOrDefaultAsync();

                result = new BasicDetailsModel()
                {
                    ProjectCode = Basicdeatils.ProjectCode,
                    ProjectName = Basicdeatils.ProjectName,
                    ExamYear = Basicdeatils.ExamYear,
                    ExamSeriesName = Basicdeatils.ExamSeriesName,
                    ExamLevelName = Basicdeatils.ExamLevelName,
                    SubjectName = Basicdeatils.SubjectName,
                    SubjectCode = Basicdeatils.SubjectCode,
                    PaperName = Basicdeatils.PaperName,
                    MOAName = Basicdeatils.Moaname,
                    ProjectInfo = Basicdeatils.ProjectInfo1
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in basic details page while getting particular project details for specific project: Method Name: GetProjectDetails(): ProjectID=" + ProjectID.ToString());
                throw;
            }
            return result;
        }

        public async Task<string> UpdateBasicDetails(BasicDetailsModel basicdeatilmodel, long UserId, long ProjectID)
        {
            string status = "P000";
            ProjectInfo project;
            try
            {
                project = await context.ProjectInfos.Where(item => item.ProjectId == ProjectID).FirstOrDefaultAsync();
                if (project != null)
                {
                    project.ProjectInfo1 = basicdeatilmodel.ProjectInfo;
                    project.ModifiedDate = DateTime.UtcNow;
                    project.ModifiedBy = UserId;
                    context.ProjectInfos.Update(project);
                    context.SaveChanges();
                    status = "P001";
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in basic details page while updating project details: Method Name: UpdateBasicDetails(): ProjectID=" + basicdeatilmodel.ProjectID.ToString());
                throw;
            }
            return status;
        }
        public async Task<GetModeOfAssessmentModel> GetModeOfAssessment(long ProjectID)
        {
            GetModeOfAssessmentModel result = new GetModeOfAssessmentModel();
            try
            {
                var Moa = await (from M in context.ModeOfAssessments join
                                 P in context.ProjectInfos on M.Moaid equals P.Moa
                                 where P.ProjectId == ProjectID && !P.IsDeleted && !M.IsDeleted
                                 select new GetModeOfAssessmentModel
                                 {
                                     MOACode = M.Moacode
                                 }).FirstOrDefaultAsync();
                if (Moa != null)
                {
                    result.MOACode = Moa.MOACode;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in basic details page while getting particular project details for specific project: Method Name: GetModeOfAssessment(): ProjectID=" + ProjectID.ToString());
                throw;
            }
            return result;
        }
    }
}
