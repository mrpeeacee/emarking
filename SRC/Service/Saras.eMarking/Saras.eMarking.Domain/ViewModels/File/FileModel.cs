using MediaLibrary.Model;

namespace Saras.eMarking.Domain.ViewModels.File
{
    public class FileModel
    {
        public FileModel()
        {
        }
        public long Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public FileUploadRepo FileUploadRepo { get; set; } = 0;
    }
}
