using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration.Annotation;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration
{
    public interface IAnnotationSettingRepository
    {
        Task<AnnotationSettingModel> GetQigAnnotationSetting(long QigId, long ProjectId);
        Task<string> UpdateQigAnnotationSetting(long qigId, AnnotationSettingModel objqigconfigmodel, long projectUserRoleID, long projectId);

        Task<string> SaveAnnotationForQIG(long qigId, QigAnnotationDetails qigAnnotationDetails, long projectId, long projectUserRoleID);
        Task<QigAnnotationDetails> GetAnnotationForQIG(long qigId, long projectID, long projectUserRoleID);
    }
}
