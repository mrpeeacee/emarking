using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Business
{
    public class QigService : BaseService<QigService>, IQigService
    {
        readonly IQigRepository _qigRepository;
        public QigService(IQigRepository qigRepository, ILogger<QigService> _logger) : base(_logger)
        {
            _qigRepository = qigRepository;
        }

        /// <summary>
        /// Method to get all qig for specific project
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns>Returns the All QIGs data for specific project</returns>
        public async Task<IList<QigModel>> GetAllQIGs(long ProjectId)
        {
            try
            {
                var activity = await _qigRepository.GetAllQIGs(ProjectId);
                if (activity != null && activity.Count > 0)
                {
                    foreach (var act in activity)
                    {
                        if (act.AnnotationSetting != null)
                        {
                            act.AnnotationSetting = (List<AppSettingModel>)act.AnnotationSetting.BuildAppKeyTree();
                        }

                        if (act.RandomCheckSettings != null)
                        {
                            act.RandomCheckSettings = (List<AppSettingModel>)act.RandomCheckSettings.BuildAppKeyTree();
                        }
                    }

                }
                return activity;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigService Page: Method Name: GetAllQIGs() and ProjectID = " + ProjectId.ToString() + "");
                throw;
            }
        }

        /// <summary>
        /// Method to update the given Project Team QIG, Annotation and RC settings
        /// </summary>
        /// <param name="objQigModel"></param>
        /// <param name="objProjectTeamsIdsModel"></param>
        ///// <param name="objrandomCheckSettingsModel"></param>
        /// <returns></returns>

        public async Task<bool> UpdateQigSetting(QigModel objQigModel, long projectId, long ProjectUserRoleID)
        {
            try
            {
                return await _qigRepository.UpdateQigSetting(objQigModel, projectId, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigService Page: Method Name: UpdateQigSetting() and ProjectID = " + projectId.ToString() + "");
                throw;
            }
        }

        /// <summary>
        /// Get all Qig Questions
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="QigId"></param>
        /// <returns></returns>
        public async Task<IList<QigQuestionModel>> GetAllQigQuestions(long ProjectId, long QigId)
        {
            try
            {
                return await _qigRepository.GetAllQigQuestions(ProjectId, QigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigService Page: Method Name: GetAllQigQuestions() and ProjectID = " + ProjectId.ToString() + ", QigId = " + QigId.ToString() + "");
                throw;
            }
        }

         async Task<List<WorkflowStatusTrackingModel>> IQigService.GetQigWorkflowTracking(long projectId, long entityid, EnumAppSettingEntityType entitytype)
        {
            try
            {
                return await _qigRepository.GetQigWorkflowTracking(projectId, entityid, entitytype);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigService while getting qigworkflowstatus : Method Name: GetQigWorkflowTracking() and  EntityID = " + entityid.ToString() + "EntityType = " + entitytype + "");
                throw;
            }
        }
        public async Task<IList<UserQigModel>> GetQIGs(long ProjectId, long ProjectUserRoleID, bool? iskp,long? Qigtype)
        {
            try
            {
                IList<UserQigModel> TrailMarkingScriptQIGs = await _qigRepository.GetQIGs(ProjectId, ProjectUserRoleID, Qigtype);
                if (TrailMarkingScriptQIGs != null && TrailMarkingScriptQIGs.Count > 0)
                {
                    if (iskp != null)
                    {
                        if (iskp == true)
                        {
                            TrailMarkingScriptQIGs = TrailMarkingScriptQIGs.Where(a => a.IsKp).ToList();
                        }
                        else
                        {
                            TrailMarkingScriptQIGs = TrailMarkingScriptQIGs.Where(a => !a.IsKp).ToList();
                        }
                    }
                    TrailMarkingScriptQIGs = TrailMarkingScriptQIGs.OrderBy(a => a.QigId).ToList();
                }
                return TrailMarkingScriptQIGs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigService Page: Method Name: GetQIGs() and ProjectID = " + ProjectId.ToString() + " and ProjectUserRoleID = " + Convert.ToString(ProjectUserRoleID));
                throw;
            }
        }

    }
}
