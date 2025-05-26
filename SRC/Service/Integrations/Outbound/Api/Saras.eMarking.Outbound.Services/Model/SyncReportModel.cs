namespace Saras.eMarking.Outbound.Services.Model
{
    public class SyncRequestModel
    {

        public string Payload { get; set; }
        public StreamContent FileContent { get; set; }
    }
    public class SyncReportModel
    {
        public int ReportType { get; set; }
        public int Year { get; set; }
        public string FileId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string RootFolder { get; set; } = string.Empty;

    }

    public enum SyncResponseStatus
    {
        Success = 1,
        Error = 2
    }

    public class SyncResponseModel
    {
        public string? Message { get; set; }
        public SyncResponseStatus Status { get; set; } = SyncResponseStatus.Error;
        public string? Content { get; set; }
    }
}
