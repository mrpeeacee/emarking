using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.TrialMarking;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.TrialMarking;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.TrialMarking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Standardisation.S1.TrialMarking
{
    public class TrialMarkingPoolsService : BaseService<TrialMarkingPoolsService>, ITrialMarkingPoolService
    {
        readonly ITrialMarkingPoolRepository _trialMarkingPoolRepository;
        public TrialMarkingPoolsService(ITrialMarkingPoolRepository trialMarkingPoolRepository, ILogger<TrialMarkingPoolsService> _logger) : base(_logger)
        {
            _trialMarkingPoolRepository = trialMarkingPoolRepository;
        }

        public async Task<TrialMarkedQigModel> GetQIGScriptForTrialMark(long ProjectId, long ProjectUserRoleID, long QigId, string filterby, string searchValue, UserRole userRole)
        {
            try
            {
                var TrailMarkingScriptQIGs = await _trialMarkingPoolRepository.GetQIGScriptForTrialMark(ProjectId, ProjectUserRoleID, QigId, filterby,searchValue, userRole);
                return TrailMarkingScriptQIGs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Qig Setup Page: Method Name: GetQIGScriptForTrialMark() and ProjectID = " + ProjectId.ToString() + " and ScriptId = " + QigId.ToString() + " and filterby = " + filterby.ToString());
                throw;
            }
        }
        ////public async Task<string> UpdateTrialMarkWorkFlowStatus(long ProjectId, long ScriptId)
        public async Task<string> UpdateTrialMarkWorkFlowStatus(long ProjectId, TrailmarkingModel trailmarkingModel)
        {
            try
            {
                var TrailMarkingScriptQIGs = await _trialMarkingPoolRepository.UpdateTrialMarkWorkFlowStatus(ProjectId, trailmarkingModel);
                ////var TrailMarkingScriptQIGs = await _trialMarkingPoolRepository.UpdateTrialMarkWorkFlowStatus(ProjectId, ScriptId);
                return TrailMarkingScriptQIGs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Qig Setup Page: Method Name: UpdateTrialMarkWorkFlowStatus() and ProjectID = " + ProjectId.ToString() + " and ScriptId = " + trailmarkingModel.ScriptId.ToString());
                throw;
            }
        }
        public async Task<IList<QuestionBandModel>> GetScriptQuestionBandInformation(long ProjectId, long ScriptId)
        {
            try
            {
                var TrailMarkingScriptQIGs = await _trialMarkingPoolRepository.GetScriptQuestionBandInformation(ProjectId, ScriptId);
                return TrailMarkingScriptQIGs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Qig Setup Page: Method Name: getScriptQuestionBandInformation() and ProjectID = " + ProjectId.ToString() + " and ScriptId = " + ScriptId.ToString());
                throw;
            }
        }
    }
}
