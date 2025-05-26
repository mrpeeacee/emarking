using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.File
{
    public interface IFileService
    {
        string UploadFile(IFormFile files, long projectId, long ProjectUserRoleID);
        HttpResponseMessage Download(long projectid, long fileid, out string filename);
    }
}
