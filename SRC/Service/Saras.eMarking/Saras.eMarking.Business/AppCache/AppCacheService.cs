using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using System.Collections.Generic;

namespace Saras.eMarking.Business.AppCache
{
    public class AppCacheService : IAppCacheService
    {

        readonly IAppCacheRepository _cacheRepository;
        public AppCacheService(IAppCacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public List<AppsettingGroupModel> GetAppsettingGroups()
        {
            return _cacheRepository.GetAppsettingGroups();
        }

        public List<AppSettingKeyModel> GetAppSettingKeys()
        {
            return _cacheRepository.GetAppSettingKeys();
        }

        public EventAuditMasterModel GetAuditMasters()
        {
            return _cacheRepository.GetAuditMasters();
        }

        public List<WorkflowStatusModel> GetWorkflowstatus()
        {
            return _cacheRepository.GetWorkflowstatus();
        }
    }
}
