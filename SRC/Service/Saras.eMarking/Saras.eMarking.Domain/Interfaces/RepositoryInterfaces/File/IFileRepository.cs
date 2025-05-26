using MediaLibrary.Model;
using Saras.eMarking.Domain.ViewModels.File;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.File
{
    public interface IFileRepository
    {
        string UploadFile(FileUploadResponse fileUploadResponse, long projectId, long ProjectUserRoleID);
        Task<FileModel> GetFile(long projectid, long fileid);
    }
}
