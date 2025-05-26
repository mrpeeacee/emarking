using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Setup
{
    public class ProjectClosureService : BaseService<ProjectClosureService>, IProjectClosureService
    {
        readonly IProjectClosureRepository _projectClosureRepository;
        public ProjectClosureService(IProjectClosureRepository projectClosureRepository, ILogger<ProjectClosureService> _logger) : base(_logger)
        {
            _projectClosureRepository = projectClosureRepository;
        }

        public async Task<ProjectClosureModel> GetProjectClosure(long ProjectId)
        {
            logger.LogInformation($"ProjectClosureService >> GetProjectClosure() started. ProjectId = {ProjectId}");
            try
            {
                ProjectClosureModel res = await _projectClosureRepository.GetProjectClosure(ProjectId);

                logger.LogDebug($"ProjectClosureService >> GetProjectClosure() completed. ProjectId = {ProjectId}");

                return res;
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureService while getting Project Schedule details for specific project : Method Name : GetProjectClosure(). ProjectId = {ProjectId}");
                throw;
            }
        }
        public async Task<string> UpdateProjectClosure(long ProjectId, long ProjectUserRoleId, ProjectClosureModel closuremodel)
        {
            logger.LogInformation($"ProjectClosureService > UpdateProjectClosure() started. ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");
            try
            {
                string res;
                if (await ValidationAsync(ProjectId))
                {
                    if (closuremodel.Remarks != null || closuremodel.Remarks != "")
                    {
                        if (closuremodel.Remarks.Length <= 250)
                        {
                            res = await _projectClosureRepository.UpdateProjectClosure(ProjectId, ProjectUserRoleId, closuremodel);
                        }
                        else
                        {
                            res = "RL001";
                        }

                    }
                    else
                    {
                        res = "RM001";
                    }
                }
                else
                {
                    res = "QI001";
                }
                logger.LogDebug($"ProjectClosureService > UpdateProjectClosure() completed. ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");

                return res;
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureService while getting Project Schedule details for specific project : Method Name : UpdateProjectClosure(). ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");
                throw;
            }
        }
        public async Task<string> UpdateProjectReopen(long ProjectId, long ProjectUserRoleId, ProjectClosureModel closuremodel)
        {
            logger.LogInformation($"ProjectClosureService > UpdateProjectReopen() started. ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");
            try
            {

                string res;
                if (closuremodel.ReopenRemarks != null || closuremodel.ReopenRemarks != "")
                {
                    if (closuremodel.ReopenRemarks.Length <= 250)
                    {
                        res = await _projectClosureRepository.UpdateProjectReopen(ProjectId, ProjectUserRoleId, closuremodel);
                    }
                    else
                    {
                        res = "RL001";
                    }

                }
                else
                {
                    res = "RM001";
                }
                logger.LogDebug($"ProjectClosureService >> UpdateProjectReopen() completed. ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");

                return res;
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureService while getting Project Schedule details for specific project : Method Name : UpdateProjectReopen(). ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");
                throw;
            }
        }
        public async Task<ProjectClosureModel> CheckDiscrepancy(long ProjectId, long? projectquestionId = null)
        {
            logger.LogInformation($"ProjectClosureService >> CheckDiscrepancy() started. ProjectId = {ProjectId} and ProjectQuestionId = {projectquestionId}");
            try
            {
                ProjectClosureModel res = await _projectClosureRepository.CheckDiscrepancy(ProjectId, projectquestionId);

                logger.LogDebug($"ProjectClosureService >> CheckDiscrepancy() completed. ProjectId = {ProjectId} and ProjectQuestionId = {projectquestionId}");

                return res;
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureController while getting Project Schedule details for specific project : Method Name : CheckDiscrepancy(). ProjectId = {ProjectId} and ProjectQuestionId = {projectquestionId}");
                throw;
            }
        }
        private async Task<bool> ValidationAsync(long ProjectId)
        {
            var closuremodel = await _projectClosureRepository.GetProjectClosure(ProjectId);

            bool resp = true;

            if (closuremodel.QigModels.Any(tot => tot.TotalScriptCount <= 0) ||
                closuremodel.QigModels.Any(man => man.ManualMarkingCount <= 0) ||
                closuremodel.QigModels.Any(sub => sub.SubmittedScriptCount <= 0) ||
                closuremodel.QigModels.Any(ssma => ssma.ManualMarkingCount != ssma.SubmittedScriptCount) ||
                closuremodel.QigModels.Any(liv => liv.LivePoolScriptCount != 0) ||
                closuremodel.QigModels.Any(rc1 => rc1.Rc1UnApprovedCount != 0) ||
                (closuremodel.QigModels.Any(ex => ex.RC2Exists > 0) &&
                    (closuremodel.QigModels.Any(rc2 => rc2.Rc2UnApprovedCount != 0) ||
                    closuremodel.QigModels.Any(sam => sam.ToBeSampledForRC2 != 0))) ||
                closuremodel.QigModels.Any(cos => cos.CheckedOutScripts != 0))
            {
                resp = false;
            }

            return resp;
        }


        /// <summary>
        /// Clear all the scripts which are not picked to rc job and move it to adhoc check 
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <param name="qigid"></param>
        /// <returns></returns>
        public async Task<string> ClearPendingScripts(long ProjectId, long ProjectUserRoleId, long qigid)
        {
            logger.LogInformation($"ProjectClosureService >> ClearPendingScripts() started. ProjectId = {ProjectId} and ProjectQuestionId = {ProjectUserRoleId}, qigid {qigid}");
            try
            {
                ProjectClosureModel projectDetails = await _projectClosureRepository.GetProjectClosure(ProjectId);

                string res = "VALERR";
                if (projectDetails != null && IsAvailableForClear(projectDetails, qigid))
                {
                    res = await _projectClosureRepository.ClearPendingScripts(ProjectId, ProjectUserRoleId, qigid);
                }
                logger.LogDebug($"ProjectClosureService >> ClearPendingScripts() completed. ProjectId = {ProjectId} and ProjectQuestionId = {ProjectUserRoleId}, qigid {qigid}");

                return res;
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureController while getting Project Schedule details for specific project : Method Name : ClearPendingScripts(). ProjectId = {ProjectId} and ProjectQuestionId = {ProjectUserRoleId}, qigid {qigid}");
                throw;
            }
        }

        private static bool IsAvailableForClear(ProjectClosureModel projectDetails, long qigid)
        {
            if (projectDetails?.QigModels == null)
            {
                return false;
            }

            ProjectClosureQigModel qig = projectDetails.QigModels.Find(a => a.QigId == qigid);

            return qig != null && qig.TotalSubmitted > 0 && qig.ManualMarkingCount == qig.TotalSubmitted && qig.SubmittedScriptCount != qig.TotalSubmitted;
        }

    }
}
