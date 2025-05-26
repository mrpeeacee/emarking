using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration.Annotation;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration
{
    public interface IAnnotationSettingService
    {
        Task<AnnotationSettingModel> GetQigAnnotationSetting(long qigId, long ProjectId);
        Task<string> UpdateQigAnnotationSetting(long QigId, AnnotationSettingModel objqigconfigmodel, long ProjectUserRoleID, long ProjectId);

        Task<string> SaveAnnotationForQIG(long qigId, QigAnnotationDetails qigAnnotationDetails, long ProjectId, long ProjectUserRoleID);
        Task<QigAnnotationDetails> GetAnnotationForQIG(long qigId, long ProjectID, long ProjectUserRoleID);
    }
}
