using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Standardisation.S1.Recommendation
{
    public class RecPoolsService : BaseService<RecPoolsService>, IRecPoolService
    {
        readonly IRecPoolRepository _recPoolRepository;
        public RecPoolsService(IRecPoolRepository recPoolRepository, ILogger<RecPoolsService> _logger) : base(_logger)
        {
            _recPoolRepository = recPoolRepository;
        }
        public async Task<RecPoolModel> GetRecPoolStastics(long ProjectId, long QigId, UserRole CurrentRole)
        {
            try
            {
                var RecommendationsQIGs = await _recPoolRepository.GetRecPoolStastics(ProjectId, QigId, CurrentRole);
                return RecommendationsQIGs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Recommendations Page: Method Name: GetRecPoolStastics() and QIGID = " + QigId.ToString() + "");
                throw;
            }
        }
        public async Task<IList<RecPoolScriptModel>> GetRecPoolScript(long projectid, long QIGID, UserRole CurrentRole, long ScriptId = 0)
        {
            try
            {
                var RecommendationsQIGs = await _recPoolRepository.GetRecPoolScript(projectid, QIGID, CurrentRole, ScriptId);
                return RecommendationsQIGs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Recommendations Page: Method Name: GetRecPoolScript() and QIGID = " + QIGID.ToString() + "");
                throw;
            }
        }
    }
}
