using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Domain.ViewModels.Project.Setup.ResolutionOfCoi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Setup
{
    public class ResolutionOfCoiService : BaseService<ResolutionOfCoiService>, IResolutionOfCoiService
    {
        readonly IResolutionOfCoiRepository _resolutionOfCoiRepository;
        public ResolutionOfCoiService(IResolutionOfCoiRepository resolutionOfCoiRepository, ILogger<ResolutionOfCoiService> _logger) : base(_logger)
        {
            _resolutionOfCoiRepository = resolutionOfCoiRepository;
        }
        public async Task<IList<ResolutionOfCoiModel>> GetResolutionCOI(long ProjectId)
        {
            logger.LogInformation("ResolutionCOI Service >> GetResolutionCOI() started");
            try
            {
                return await _resolutionOfCoiRepository.GetResolutionCOI(ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ResolutionCOI page while getting markers, default school name and Exception schools for specific Project: Method Name: GetResolutionCOI() and QIGId=" + "");
                throw;
            }
        }
        public async Task<IList<CoiSchoolModel>> GetSchoolsCOI(long ProjectId)
        {
            logger.LogInformation("ResolutionCOI Service >> GetSchoolsCOI() started");
            try
            {
                return await _resolutionOfCoiRepository.GetSchoolsCOI(ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ResolutionCOI page while getting schools for specific Project: Method Name: GetSchoolsCOI() and QIGId = " + "");
                throw;
            }
        }
        public async Task<string> UpdateResolutionCOI(List<CoiSchoolModel> ObjCoiSchoolModel, long ProjectUserRoleID, long CurrentProjUserRoleId, long ProjectID)
        {
            logger.LogInformation("ResolutionCOI Service >> UpdateResolutionCOI() started");
            try
            {
                return await _resolutionOfCoiRepository.UpdateResolutionCOI(ObjCoiSchoolModel, ProjectUserRoleID, CurrentProjUserRoleId, ProjectID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ResolutionCOI page while updating markers, default school name and Exception schools for specific Project: Method Name: GetResolutionCOI()" + "");
                throw;
            }
        }
    }
}