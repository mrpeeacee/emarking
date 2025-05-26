using System.Collections.Generic;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.ViewModels;
using System.Linq;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;

namespace Saras.eMarking.Domain.Entities
{
    /// <summary>
    /// Class to store all the initial data to application 
    /// </summary>
    public class AppCache : IAppCache
    {
        /// <summary>
        /// Workflow status initial data
        /// </summary>
        private List<WorkflowStatusModel> WorkflowStatusModels { get; set; }
        /// <summary>
        /// Appsetting group initial data
        /// </summary>
        private List<AppsettingGroupModel> AppsettingGroupModels { get; set; }
        /// <summary>
        /// App setting key initial data
        /// </summary>
        private List<AppSettingKeyModel> AppSettingKeyModels { get; set; }

        /// <summary>
        /// Audit master initial data
        /// </summary>
        private EventAuditMasterModel EventAuditMasterModel { get; set; }

        public AppCache()
        {
        }

        /// <summary>
        /// Fill the data to all the list
        /// </summary>
        /// <param name="appCacheService"></param>
        public void GenerateAppCache(IAppCacheService appCacheService)
        {
            AddWorkflowstatusToCache(appCacheService.GetWorkflowstatus());
            AddAppSettingGroupToCache(appCacheService.GetAppsettingGroups());
            AddAppSettingKeyToCache(appCacheService.GetAppSettingKeys());
            AddAuditMastersToCache(appCacheService.GetAuditMasters());
        }

        #region Workflowstatus  
        /// <summary>
        /// Add workflow status data to list
        /// </summary>
        /// <param name="_workflowStatusModels"></param>
        private void AddWorkflowstatusToCache(List<WorkflowStatusModel> _workflowStatusModels)
        {
            WorkflowStatusModels = _workflowStatusModels;
        }

        /// <summary>
        /// Get Workflow status id for given code
        /// </summary>
        /// <param name="workflowStatus"></param>
        /// <param name="workflowType"></param>
        /// <returns></returns>
        public int GetWorkflowStatusId(EnumWorkflowStatus workflowStatus, EnumWorkflowType workflowType)
        {
            int statusid = 0;
            var statusModel = WorkflowStatusModels.FirstOrDefault(a => a.WorkflowCode == StringEnum.GetStringValue(workflowStatus) && a.WorkflowType == workflowType);
            if (statusModel != null)
            {
                statusid = statusModel.WorkflowID;
            }
            return statusid;
        }


        public string GetWorkflowStatusCode(int? WorkflowID, EnumWorkflowType workflowType)
        {
            string statuscode = null;
            var statusModel = WorkflowStatusModels.FirstOrDefault(a => a.WorkflowID == WorkflowID && a.WorkflowType == workflowType);
            if (statusModel != null)
            {
                statuscode = statusModel.WorkflowCode;
            }
            return statuscode;
        }
        #endregion

        #region Appsettings
        /// <summary>
        /// Add app setting key to cache
        /// </summary>
        /// <param name="_AppSettingKeyModels"></param>
        private void AddAppSettingKeyToCache(List<AppSettingKeyModel> _AppSettingKeyModels)
        {
            AppSettingKeyModels = _AppSettingKeyModels;
        }
        /// <summary>
        /// Add app setting group to cache
        /// </summary>
        /// <param name="_AppsettingGroupModels"></param>
        private void AddAppSettingGroupToCache(List<AppsettingGroupModel> _AppsettingGroupModels)
        {
            AppsettingGroupModels = _AppsettingGroupModels;
        }

        /// <summary>
        /// Get app setting key id for given app setting code
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <returns></returns>
        public long GetAppsettingKeyId(EnumAppSettingKey appSettingKey)
        {
            long id = 0;
            var statusModel = AppSettingKeyModels.FirstOrDefault(a => a.AppsettingKey == StringEnum.GetStringValue(appSettingKey));
            if (statusModel != null)
            {
                id = statusModel.AppsettingKeyID;
            }
            return id;
        }

        /// <summary>
        /// Get app setting key group id for given appsetting code
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <returns></returns>
        public long? GetAppsettingKeyGroupId(EnumAppSettingKey appSettingKey)
        {
            long? id = 0;
            var statusModel = AppSettingKeyModels.FirstOrDefault(a => a.AppsettingKey == StringEnum.GetStringValue(appSettingKey));
            if (statusModel != null)
            {
                id = statusModel.SettingGroupID;
            }
            return id;
        }

        /// <summary>
        /// Get app setting group id
        /// </summary>
        /// <param name="appSettingGroup"></param>
        /// <returns></returns>
        public long GetAppsettingGroupId(EnumAppSettingGroup appSettingGroup)
        {
            long id = 0;
            var statusModel = AppsettingGroupModels.FirstOrDefault(a => a.SettingGroupCode == StringEnum.GetStringValue(appSettingGroup));
            if (statusModel != null)
            {
                id = statusModel.SettingGroupID;
            }
            return id;
        }

        #endregion

        #region EventAudit 
        /// <summary>
        /// Add audit master data to list
        /// </summary>
        /// <param name="_eventAuditMasterModel"></param>
        private void AddAuditMastersToCache(EventAuditMasterModel _eventAuditMasterModel)
        {
            EventAuditMasterModel = _eventAuditMasterModel;
        }

        public long GetAuditEventMasterId(AuditTrailEvent auditTrailEvent)
        {
            long statusid = 0;
            var statusModel = EventAuditMasterModel.EventMasters.FirstOrDefault(a => a.EventCode == StringEnum.GetStringValue(auditTrailEvent));
            if (statusModel != null)
            {
                statusid = statusModel.ID;
            }
            return statusid;
        }

        public long GetAuditModuleMasterId(AuditTrailModule auditTrailModule)
        {
            long statusid = 0;
            var statusModel = EventAuditMasterModel.ModuleMasters.FirstOrDefault(a => a.ModuleCode == StringEnum.GetStringValue(auditTrailModule));
            if (statusModel != null)
            {
                statusid = statusModel.ModuleId;
            }
            return statusid;
        }

        public long GetAuditEntityMasterId(AuditTrailEntity auditTrailEntity)
        {
            long statusid = 0;
            var statusModel = EventAuditMasterModel.EntityMasters.FirstOrDefault(a => a.EntityCode == StringEnum.GetStringValue(auditTrailEntity));
            if (statusModel != null)
            {
                statusid = statusModel.EntityId;
            }
            return statusid;
        }
        #endregion
    }
}
