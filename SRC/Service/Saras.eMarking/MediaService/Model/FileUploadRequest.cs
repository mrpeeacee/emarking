using Microsoft.AspNetCore.Http;

namespace MediaLibrary.Model
{
    public class FileUploadRequest
    {
        public FileUploadRepo FileUploadRepository { get; set; } 
        public IFormFile? File { get; set; } 
        public LocalMediaRequestModel? LocalMediaRequest { get; set; }
        public CloudMediaRequestModel? CloudMediaRequest { get; set; }
    }

    public class FileUploadResponse
    {
        public string? Status { get; set; }
        public string? FileName { get; set; }
        public string? FullFilePath { get; set; }
        public FileUploadRepo FileUploadRepository { get; set; }
        public string? FolderPath { get; internal set; }
    }
    public enum FileUploadRepo
    {
        LocalRepo = 0,
        AwsS3 = 1,
        Azure = 2
    }
}
