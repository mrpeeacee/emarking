namespace Saras.eMarking.Domain.Entities
{
    public partial class QrtzFiredTriggersHistory
    {
        public long Id { get; set; }
        public string SchedName { get; set; }
        public string TriggerName { get; set; }
        public string EntryId { get; set; }
        public string TriggerGroup { get; set; }
        public long FiredTime { get; set; }
        public string InstanceName { get; set; }
        public long SchedTime { get; set; }
        public string State { get; set; }
        public int Priority { get; set; }
        public string JobGroup { get; set; }
        public string JobName { get; set; }
        public bool? IsNonconcurrent { get; set; }
        public long? RepeatInterval { get; set; }
        public bool? RequestsRecovery { get; set; }
        public int? TimesTriggered { get; set; }
    }
}
