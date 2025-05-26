using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface
{
    public interface IAppCacheService
    {
        public List<WorkflowStatusModel> GetWorkflowstatus();
        public List<AppSettingKeyModel> GetAppSettingKeys();
        public List<AppsettingGroupModel> GetAppsettingGroups();
        public EventAuditMasterModel GetAuditMasters();
    }
}
