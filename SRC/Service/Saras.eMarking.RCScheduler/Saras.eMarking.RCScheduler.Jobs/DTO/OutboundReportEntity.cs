
using System;

namespace Saras.eMarking.RCScheduler.Jobs.DTO
{
    public class OutboundReportEntity
    {
        public long ProjectId { get; set; }
        public long RequestedBy { get; set; }
        public long RequestedId { get; set; }
        public string RequestGuid { get; set; }
        public int ReportType { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public bool IsRequestServed { get; set; }
        public int RequestOrder { get; set; }
        //public string FileName { get; set; }
        //public string FilePath { get; set; }
        public long RequestBy { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
