namespace MediaLibrary.Model
{
    public class FileDownloadRequest 
    {
        public FileUploadRepo FileUploadRepository { get; set; }
        public LocalMediaRequestModel? LocalMediaRequest { get; set; }
        public CloudMediaRequestModel? CloudMediaRequest { get; set; }
    }

    public class LocalMediaRequestModel
    {
        public string? LocalRepoPath { get; set; }
        public string? FolderPath { get; set; }
    }
}
