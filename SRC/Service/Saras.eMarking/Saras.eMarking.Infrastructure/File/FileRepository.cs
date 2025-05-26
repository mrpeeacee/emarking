using MediaLibrary.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.File;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.File;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.File
{
    public class FileRepository : BaseRepository<FileRepository>, IFileRepository
    {
        private readonly ApplicationDbContext context;
        public FileRepository(ApplicationDbContext context, ILogger<FileRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        /// <summary>
        /// Add file details to database with repository details.
        /// </summary> 
        /// <returns></returns>
        public string UploadFile(FileUploadResponse fileUploadResponse, long projectId, long ProjectUserRoleID)
        {
            string status;
            try
            {
                if (fileUploadResponse.Status == "Success" || fileUploadResponse.Status == "SUCCESS")
                {
                    string sFileExtension = Path.GetExtension(fileUploadResponse.FileName).ToLower();

                    ProjectFile fileEntity = new()
                    {
                        FilePath = fileUploadResponse.FullFilePath,
                        FileExtention = sFileExtension,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = ProjectUserRoleID,
                        CreatedDate = DateTime.UtcNow,
                        FileName = fileUploadResponse.FileName,
                        ProjectId = projectId,
                        Repository = (byte)fileUploadResponse.FileUploadRepository,
                        FileType = "Document",
                        EntityType = (short)EnumFilesEntityType.TempFile
                    };
                    context.ProjectFiles.Add(fileEntity);
                    context.SaveChanges();

                    status = Convert.ToString(fileEntity.FileId);
                }
                else
                {
                    status = "F001";
                    return status;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while Uploading file for specific Qig:Method Name:QigUsersImportFile() and ProjectID: ProjectID=" + projectId.ToString());
                throw;
            }
            return status;
        }

        /// <summary>
        /// Get project file details for given file id.
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="fileid"></param>
        /// <returns></returns>
        public async Task<FileModel> GetFile(long projectid, long fileid)
        {
            var file = await context.ProjectFiles.Where(msf => msf.FileId == fileid && !msf.IsDeleted && msf.IsActive && msf.ProjectId == projectid).FirstOrDefaultAsync();
            FileModel fileModel = null;
            if (file != null)
            {
                fileModel = new FileModel
                {
                    FileName = file.FileName,
                    Id = file.FileId,
                    FilePath = file.FilePath,
                    FileUploadRepo = (FileUploadRepo)file.Repository
                };
            }
            return fileModel;
        }
    }
}
