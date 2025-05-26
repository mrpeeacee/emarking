using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.AuditModels
{
    public class EventAuditMasterModel
    {
        public List<EventMasterModel> EventMasters { get; set; }
        public List<ModuleMasterModel> ModuleMasters { get; set; }
        public List<EntityMasterModel> EntityMasters { get; set; }
    }
    public class EventMasterModel
    {
        public long ID { get; set; }
        public string EventCode { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public bool IsManualDriven { get; set; }
    }
    public class ModuleMasterModel
    {
        public long ModuleId { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public long? ParentId { get; set; }
    }
    public class EntityMasterModel
    {
        public long EntityId { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string EntityDescription { get; set; }
    }
}
