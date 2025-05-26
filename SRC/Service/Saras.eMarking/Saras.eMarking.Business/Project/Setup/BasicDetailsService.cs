using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Domain.ViewModels.Project.Setup;

namespace Saras.eMarking.Business.Project.Setup
{
    public class BasicDetailsService : BaseService<BasicDetailsService>, IBasicDetailsService
    {
        readonly IBasicDetailsRepository _basicDetailRepository;
        public BasicDetailsService(IBasicDetailsRepository basicDetailRepository, ILogger<BasicDetailsService> _logger) : base(_logger)
        {
            _basicDetailRepository = basicDetailRepository;
        }
        public async Task<BasicDetailsModel> GetBasicDetails(long ProjectID)
        {

            logger.LogInformation("Basic details Service >> GetBasicDetails() started");
            try
            {
                return await _basicDetailRepository.GetBasicDetails(ProjectID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Basic details page while getting particular project details for specific project: Method Name: GetBasicDetails(): ProjectID=" + ProjectID.ToString());
                throw;
            }
        }

        public async Task<string> UpdateBasicDetails(BasicDetailsModel basicdeatilmodel, long UserId, long ProjectID)
        {
            logger.LogInformation("Basic details Service >> UpdateBasicDetails() started");
            try
            {
                string status = ValidateBasicDetails(basicdeatilmodel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _basicDetailRepository.UpdateBasicDetails(basicdeatilmodel, UserId, ProjectID);
                }
                return status;
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in basic details page while updating project details: Method Name: UpdateBasicDetails(): ProjectID=" + basicdeatilmodel.ProjectID.ToString());
                throw;
            }
        }
        private static string ValidateBasicDetails(BasicDetailsModel basicdeatilmodel)
        {
            string status = string.Empty;
            if (string.IsNullOrEmpty(basicdeatilmodel.ProjectInfo)|| (basicdeatilmodel.ProjectInfo.Length < 1 || basicdeatilmodel.ProjectInfo.Trim().Length > 250))
            {
                status = "Invalid";

            }
            
            return status;
        }
        public async Task<GetModeOfAssessmentModel> GetModeOfAssessment(long projectId)
        {

            logger.LogInformation("Basic details Service >> getModeOfAssessment() started");
            try
            {
                return await _basicDetailRepository.GetModeOfAssessment(projectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Basic details page while getting particular project details for specific project: Method Name: getModeOfAssessment(): ProjectID=" + projectId);
                throw;
            }
        }
    }
}
