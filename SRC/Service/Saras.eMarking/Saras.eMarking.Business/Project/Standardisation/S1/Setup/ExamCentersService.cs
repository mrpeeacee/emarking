using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Standardisation.S1.Setup
{
    public class ExamCentersService : BaseService<ExamCentersService>, IExamCenterService
    {
        readonly IExamCenterRepository _examCenterRepository;
        public ExamCentersService(IExamCenterRepository examCenterRepository, ILogger<ExamCentersService> _logger) : base(_logger)
        {
            _examCenterRepository = examCenterRepository;
        }
        public async Task<IList<ExamCenterModel>> ProjectCenters(long ProjectId, long QigId)
        {
            logger.LogInformation("ExamCenter Service >> ProjectCenters() started");
            try
            {
                return await _examCenterRepository.ProjectCenters(ProjectId, QigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Standardisation setup page while getting centers for specific Project: Method Name: ProjectCenters() and project: ProjectID=" + ProjectId.ToString());
                throw;
            }
        }
        public async Task<string> UpdateProjectCenters(List<ExamCenterModel> objExamCenterModel, long ProjectUserRoleID, long ProjectId, long QigId)
        {
            logger.LogInformation("ExamCenter Service >> UpdateProjectCenters() started");
            try
            {
                return await _examCenterRepository.UpdateProjectCenters(objExamCenterModel, ProjectUserRoleID, ProjectId, QigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Standardisation setup page while updating ProjectCenters: Method Name: UpdateProjectCenters()");
                throw;
            }
        }
    }
}
