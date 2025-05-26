using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Setup.QigConfiguration
{
    public class QigSummeryService : BaseService<QigSummeryService>, IQigSummeryService
    {
        readonly IQigSummeryRepository _qigSummeryRepository;

        public QigSummeryService(IQigSummeryRepository qigSummeryRepository, ILogger<QigSummeryService> _logger) : base(_logger)
        {
            _qigSummeryRepository = qigSummeryRepository;
        }

        public async Task<QigSummaryModel> GetQigSummary(long qigId, long ProjectUserRoleID, long ProjectID)
        { 
            try
            {
                QigSummaryModel qigSummaryModel = await _qigSummeryRepository.GetQigSummary(qigId, ProjectUserRoleID, ProjectID);

                return qigSummaryModel;

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSummery Page while save qig summery : Method Name: SaveQigSummery() and ProjectID = " + ProjectID + "");
                throw;
            }
        }

        public async Task<string> SaveQigSummery(long qigId, bool isProjectSetupTrue, bool isLiveMarkingTrue, long ProjectUserRoleID, long ProjectID)
        {
            string status = "";
            try
            {
                status = await _qigSummeryRepository.SaveQigSummery(qigId, isProjectSetupTrue, isLiveMarkingTrue, ProjectUserRoleID, ProjectID);
              
                return status;

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSummery Page while save qig summery : Method Name: SaveQigSummery() and ProjectID = " + ProjectID + "");
                throw;
            }
        }
    }
}
