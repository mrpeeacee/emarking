namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdTwo.Practice
{
    public class PracticeEnableModel
    {
        public bool IsPracticeEnable { get; set; }
        public bool IsQualifyingEnable { get; set; }
        public bool IsS1Enable { get; set; }
        public bool IsS2Enable { get; set; }
        public bool IsS3Enable { get; set; }
        public int? S1Completed { get; set; }
        public bool IsLiveMarkingStart { get; set; }
    }
}
