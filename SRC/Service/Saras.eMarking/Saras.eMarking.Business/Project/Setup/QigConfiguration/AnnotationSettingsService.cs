using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration.Annotation;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Setup.QigConfiguration
{
    public class AnnotationSettingsService : BaseService<AnnotationSettingsService>, IAnnotationSettingService
    {
        readonly IAnnotationSettingRepository _annotationSettingRepository;

        public AnnotationSettingsService(IAnnotationSettingRepository annotationSettingRepository, ILogger<AnnotationSettingsService> _logger) : base(_logger)
        {
            _annotationSettingRepository = annotationSettingRepository;
        }

        public async Task<AnnotationSettingModel> GetQigAnnotationSetting(long qigId, long ProjectId)
        {
            logger.LogInformation("AnnotationSettingService >> GetQigAnnotationSetting() started");
            try
            {
                return await _annotationSettingRepository.GetQigAnnotationSetting(qigId, ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in AnnotationSettingService page while updating Qig AnnotationSetting: Method Name: GetQigAnnotationSetting()");
                throw;
            }
        }

        public async Task<string> UpdateQigAnnotationSetting(long QigId, AnnotationSettingModel objqigconfigmodel, long ProjectUserRoleID, long ProjectId)
        {
            logger.LogInformation("AnnotationSettingService >> UpdateQigAnnotationSetting() started");
            try
            {
                return await _annotationSettingRepository.UpdateQigAnnotationSetting(QigId, objqigconfigmodel, ProjectUserRoleID, ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QIG AnnotationSettingService page while updating Qig AnnotationSetting: Method Name: UpdateQigAnnotationSetting()");
                throw;
            }
        }

        public async Task<string> SaveAnnotationForQIG(long qigId, QigAnnotationDetails qigAnnotationDetails, long ProjectId, long ProjectUserRoleID)
        {
            logger.LogInformation("AnnotationSettingService >> SaveAnnotationForQIG() started");
            try
            {
                return await _annotationSettingRepository.SaveAnnotationForQIG(qigId, qigAnnotationDetails, ProjectId, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in AnnotationSettingService page while save QIG Annotation :Method Name: SaveAnnotationForQIG() and ProjectID=" + ProjectId + "ProjectUserRoleID=" + ProjectUserRoleID);
                throw;
            }
        }

        public async Task<QigAnnotationDetails> GetAnnotationForQIG(long qigId, long ProjectID, long ProjectUserRoleID)
        {
            logger.LogInformation("AnnotationSettingService >> GetAnnotationForQIG() started");
            try
            {
                return await _annotationSettingRepository.GetAnnotationForQIG(qigId, ProjectID, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in AnnotationSettingService page while getting QIG Annotation :Method Name: GetAnnotationForQIG() and ProjectID=" + ProjectID + "ProjectUserRoleID=" + ProjectUserRoleID);
                throw;
            }
        }
    }
}
