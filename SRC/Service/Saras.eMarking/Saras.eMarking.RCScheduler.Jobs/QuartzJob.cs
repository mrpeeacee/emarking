namespace Saras.eMarking.RCScheduler.Jobs
{
    public class MarkingQuartzJob
    {
        public string? Name { get; set; }
        public string? Group { get; set; }
        public string? Description { get; set; }
        public bool IsDurable { get; set; }
        public MarkingQuartzTrigger? Trigger { get; set; }
    }
    public class MarkingQuartzTrigger
    {
        public string? Name { get; set; }
        public string? Group { get; set; }
        public string? Description { get; set; }
        public int Interval { get; set; }
        public string? CronExpression { get; set; }
    }
}
