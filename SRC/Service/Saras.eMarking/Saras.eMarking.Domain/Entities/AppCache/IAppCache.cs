using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;

namespace Saras.eMarking.Domain.Entities
{
    public interface IAppCache
    {
        void GenerateAppCache(IAppCacheService appCacheService);
        int GetWorkflowStatusId(EnumWorkflowStatus workflowStatus, EnumWorkflowType workflowType);
        string GetWorkflowStatusCode(int? WorkflowID, EnumWorkflowType workflowType);
        long GetAppsettingKeyId(EnumAppSettingKey appSettingKey);
        long? GetAppsettingKeyGroupId(EnumAppSettingKey appSettingKey);
        long GetAppsettingGroupId(EnumAppSettingGroup appSettingGroup);
        long GetAuditEventMasterId(AuditTrailEvent auditTrailEvent);
        long GetAuditModuleMasterId(AuditTrailModule auditTrailModule);
        long GetAuditEntityMasterId(AuditTrailEntity auditTrailEntity);
    }
}