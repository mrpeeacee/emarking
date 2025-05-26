namespace Saras.eMarking.Domain.ViewModels.Report
{
    public class MarkerPerformanceStatistics
    {
        public decimal TotalMarkerCount { get; set; }   
        public decimal TotalSchoolCount { get; set; } 
    }
    public class MarkerPerformance
    {
        public string MarkerName { get; set; }
        public string School { get; set; }
        public long TotalScripts { get; set; }
        public long RC1 { get; set; }
        public long RC2 { get; set; }
        public long Adhoc { get; set; }
        public long RealLocated { get; set; }
    }
    public class SchoolDetails
    {
        public long SchoolId { get; set; }
        public string SchoolCode { get; set; }
        public string SchoolName { get; set; }  
    }

    public class MarkerDetails
    {
        public long QigID { get; set; }
        public long ProjectId { get; set; }
        public long ExamYear { get; set; }
        public string MarkerName { get; set; }
        public string SchoolCode { get; set; }

    }
}
