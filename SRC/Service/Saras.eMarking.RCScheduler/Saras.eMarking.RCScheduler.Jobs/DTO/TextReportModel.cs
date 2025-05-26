using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Saras.eMarking.Domain.ViewModels.Banding
{
    public class TextReportModel
    {
        public TextReportModel()
        {
            Items = new List<string>();
        }
        public long Count { get; set; }
        public long PageIndex { get; set; }
        public long PageSize { get; set; }
        public List<string> Items { get; set; }
        public string FileName { get; set; }
        public int ExamYear { get; set; }
        public DateTime ProcessDate { get; set; }
    }

    public class SyncReportModel
    {
        public int ReportType { get; set; }
        public string FileId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string RootFolder { get; set; } = string.Empty;
        public int Year { get; set; }
        public DateTime ProcessDate { get; set; }
        public StreamContent FileContent { get; set; }
    }
    public enum SyncResponseStatus
    {
        Success = 1,
        Error = 2
    }

    public class SyncResponseModel
    {
        public string Message { get; set; }
        public SyncResponseStatus Status { get; set; } = SyncResponseStatus.Error;
        public string Content { get; set; }
    }

}
