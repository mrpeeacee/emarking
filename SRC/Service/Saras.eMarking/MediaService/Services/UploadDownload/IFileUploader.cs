using MediaLibrary.Model;

namespace MediaLibrary.Services.UploadDownload
{
    internal interface IFileUploader
    {
        FileUploadResponse? UploadFile(FileUploadRequest fileUploadRequest);
    }

}
