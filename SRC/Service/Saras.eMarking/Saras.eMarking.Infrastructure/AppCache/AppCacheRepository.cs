using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using System.Collections.Generic;
using System.Linq;

namespace Saras.eMarking.Infrastructure.AppCache
{
    public class AppCacheRepository : IAppCacheRepository
    {
        private readonly ApplicationDbContext context;
        public AppCacheRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public List<WorkflowStatusModel> GetWorkflowstatus()
        {
            List<WorkflowStatusModel> workflowStatusModels = context.WorkflowStatuses.Where(a => !a.IsDeleted).Select(a => new WorkflowStatusModel
            {
                WorkflowCode = a.WorkflowCode,
                WorkflowID = a.WorkflowId,
                WorkflowName = a.WorkflowName,
                WorkflowType = (EnumWorkflowType)a.WorkflowType

            }).ToList();
            return workflowStatusModels;
        }

        public List<AppsettingGroupModel> GetAppsettingGroups()
        {
            List<AppsettingGroupModel> AppsettingGroupModels = context.AppsettingGroups.Where(a => !a.IsDeleted).Select(a => new AppsettingGroupModel
            {
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                OrganizationID = a.OrganizationId,
                SettingGroupCode = a.SettingGroupCode,
                SettingGroupID = a.SettingGroupId,
                SettingGroupName = a.SettingGroupName
            }).ToList();
            return AppsettingGroupModels;
        }

        public List<AppSettingKeyModel> GetAppSettingKeys()
        {
            List<AppSettingKeyModel> appsettingGroupEntitys = context.AppSettingKeys.Where(a => !a.IsDeleted).Select(a => new AppSettingKeyModel
            {
                SettingGroupID = a.SettingGroupId,
                OrganizationID = a.OrganizationId,
                IsDeleted = a.IsDeleted,
                Description = a.Description,
                AppsettingKey = a.AppsettingKey1,
                AppsettingKeyID = a.AppsettingKeyId,
                AppsettingKeyName = a.AppsettingKeyName,
                ParentAppsettingKeyID = a.ParentAppsettingKeyId
            }).ToList();
            return appsettingGroupEntitys;
        }

        public EventAuditMasterModel GetAuditMasters()
        {
            EventAuditMasterModel eventAuditMasterModel = new();
            List<EventMasterModel> eventMasters = context.EventMasters.Select(a => new EventMasterModel
            {
                ID = a.Id,
                EventCode = a.EventCode,
                EventType = a.EventType,
                Description = a.Description,
                IsManualDriven = a.IsManualDriven
            }).ToList();
            eventAuditMasterModel.EventMasters = eventMasters;

            List<ModuleMasterModel> masterModels = context.ModuleMasters.Select(a => new ModuleMasterModel
            {
                ModuleId = a.ModuleId,
                ModuleCode = a.ModuleCode,
                ModuleName = a.ModuleName,
                ParentId = a.ParentId
            }).ToList();
            eventAuditMasterModel.ModuleMasters = masterModels;

            List<EntityMasterModel> entityMasters = context.EntityMasters.Select(a => new EntityMasterModel
            {
                EntityId = a.EntityId,
                EntityCode = a.EntityCode,
                EntityName = a.EntityName,
                EntityDescription = a.EntityDescription
            }).ToList();
            eventAuditMasterModel.EntityMasters = entityMasters;

            return eventAuditMasterModel;
        }

    }
}
