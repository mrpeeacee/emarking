using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Standardisation.S1.Setup
{
    public class KeyPersonnelsService : BaseService<KeyPersonnelsService>, IKeyPersonnelService
    {
        readonly IKeyPersonnelRepository _keyPersonnelRepository;
        public KeyPersonnelsService(IKeyPersonnelRepository keyPersonnelRepository, ILogger<KeyPersonnelsService> _logger) : base(_logger)
        {
            _keyPersonnelRepository = keyPersonnelRepository;
        }
        public async Task<IList<KeyPersonnelModel>> ProjectKps(long ProjectId, long QigId)
        {
            logger.LogInformation("KeyPersonnel Service >> ProjectKps() started");
            try
            {
                return await _keyPersonnelRepository.ProjectKps(ProjectId, QigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Standardisation setup page while getting Project Kp's for specific Project: Method Name: ProjectKps() and project: ProjectID=" + ProjectId.ToString());
                throw;
            }
        }
        public async Task<string> UpdateKeyPersonnels(List<KeyPersonnelModel> objKeyPersonnelModel, long ProjectUserRoleID, long ProjectId, long QigId)
        {
            logger.LogInformation("KeyPersonnel Service >> UpdateKeyPersonnels() started");
            try
            {
                return await _keyPersonnelRepository.UpdateKeyPersonnels(objKeyPersonnelModel, ProjectUserRoleID, ProjectId, QigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Standardisation setup page while updating Key Personnels: Method Name: UpdateKeyPersonnels()");
                throw;
            }
        }

    }
}
