using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.QualityChecks;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.QualityChecks;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Dashboards;
using Saras.eMarking.Domain.ViewModels.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels.Project.QualityChecks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.QualityChecks
{
    public class QualityChecksService : BaseService<QualityChecksService>, IQualityChecksService
    {
        readonly IQualityChecksRepository _qualityChecksRepository;

        public QualityChecksService(IQualityChecksRepository qualityChecksRepository, ILogger<QualityChecksService> _logger) : base(_logger)
        {
            _qualityChecksRepository = qualityChecksRepository;
        }


        public async Task<List<QualityChecksModel>> GetQIGProjectUserReportees(long QigId, long ProjectId, long ProjectUserRoleID)
        {
            logger.LogInformation("Quality Check Service >> GetQIGProjectUserReportees() started");
            try
            {
                return await _qualityChecksRepository.GetQIGProjectUserReportees(QigId, ProjectId, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Quality Check Service page while getting Qig users Data view for specific Qig:Method Name:GetQIGProjectUserReportees() and QigID: QigID=" + QigId.ToString());
                throw;
            }
        }

        public async Task<MarkingOverviewsModel> GetQIGHierarchyLiveMarkingScriptCountDetails(long QigId, long ProjectId, long ProjectUserRoleID)
        {
            logger.LogInformation("Qualtiy Check Service >> GetQIGHierarchyLiveMarkingScriptCountDetails() started");
            try
            {
                return await _qualityChecksRepository.GetQIGHierarchyLiveMarkingScriptCountDetails(QigId, ProjectId, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while getting Qig users Data view for specific Qig:Method Name:GetQIGHierarchyLiveMarkingScriptCountDetails() and QigID: QigID=" + QigId.ToString());
                throw;
            }
        }

        public Task<QualityCheckCountSummary> GetTeamStatistics(TeamStatistics teamStatistics, UserTimeZone userTimeZone, UserRole userRole)
        {
            logger.LogInformation("Qualtiy Check Service >> GetTeamStatistics() started");
            try
            {
                QualityCheckCountSummary qualityCheckCountSummary = new QualityCheckCountSummary();

                teamStatistics.CountOrDetails = 1;

                QualityCheckScript result = _qualityChecksRepository.GetTeamStatistics(teamStatistics, userTimeZone, userRole);
                if (result != null)
                {
                    qualityCheckCountSummary = result.QualityCheckCountSummary;
                }
                return Task.FromResult(qualityCheckCountSummary);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while getting Qig Script Data view for specific Qig:Method Name:GetTeamStatistics() and QigID: QigID=" + teamStatistics.QigId.ToString());
                throw;
            }
        }


        public Task<List<QualityCheckScriptDetailsModel>> GetTeamStatisticsList(TeamStatistics teamStatistics, UserTimeZone userTimeZone, UserRole userRole)
        {
            logger.LogInformation("Qualtiy Check Service >> GetTeamStatisticsList() started");
            try
            {
                teamStatistics.CountOrDetails = 2;
                List<QualityCheckScriptDetailsModel> scriptDetails = new();
                QualityCheckScript result = _qualityChecksRepository.GetTeamStatistics(teamStatistics, userTimeZone, userRole);
                if (result != null)
                {
                    scriptDetails = result.QualityCheckScriptDetailsModel;
                }
                return Task.FromResult(scriptDetails);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while getting Qig Script Data view for specific Qig:Method Name:GetTeamStatisticsList() and QigID: QigID=" + teamStatistics.QigId.ToString());
                throw;
            }
        }




        public async Task<List<QualityCheckScriptDetailsModel>> GetLiveMarkingScriptCountDetails(long QigId, long ProjectId, long ProjectUserRoleID, int scriptPool, long FilterProjectUserRoleID)
        {
            logger.LogInformation("Qualtiy Check Service >> GetLiveMarkingScriptCountDetails() started");
            try
            {
                return await _qualityChecksRepository.GetLiveMarkingScriptCountDetails(QigId, ProjectId, ProjectUserRoleID, scriptPool, FilterProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while getting Qig Script Data view for specific Qig:Method Name:GetLiveMarkingScriptCountDetails() and QigID: QigID=" + QigId.ToString());
                throw;
            }
        }

        public async Task<QualityCheckInitialScriptModel> GetScriptInDetails(long QigId, long ScriptId, long ProjectId, long ProjectUserRoleID, UserTimeZone userTimeZone)
        {
            logger.LogInformation("Qualtiy Check Service >> GetLiveMarkingScriptCountDetails() started");
            try
            {
                return await _qualityChecksRepository.GetScriptInDetails(QigId, ScriptId, ProjectId, ProjectUserRoleID, userTimeZone);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while getting Qig Script Data view for specific Qig:Method Name:GetLiveMarkingScriptCountDetails() and QigID: QigID=" + ScriptId.ToString());
                throw;
            }
        }

        public async Task<string> LiveMarkingScriptApprovalStatus(LivemarkingApproveModel livemarkingApproveModel, long projectId, string roleCode)
        {
            logger.LogInformation("Qualtiy Check Service >> LiveMarkingScriptApprovalStatus() started");
            try
            {
                return await _qualityChecksRepository.LiveMarkingScriptApprovalStatus(livemarkingApproveModel, projectId, roleCode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while approving Script Data :Method Name:LiveMarkingScriptApprovalStatus() and QigID: QigID=" + livemarkingApproveModel.QigID.ToString());
                throw;
            }
        }

        public async Task<QualityCheckCountSummary> GetQualityCheckSummary(long QigId, long ProjectId, long ProjectUserRoleID, long FilterProjectUserRoleID)
        {
            logger.LogInformation("Qualtiy Check Service >> GetQualityCheckSummary() started");
            try
            {
                return await _qualityChecksRepository.GetQualityCheckSummary(QigId, ProjectId, ProjectUserRoleID, FilterProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while getting Script Data :Method Name:GetQualityCheckSummary() and QigID: QigID=" + QigId.ToString());
                throw;
            }
        }

        public bool IsEligibleForLiveMarking(long qigId, long ProjectId, long ProjectUserRoleID)
        {
            logger.LogInformation("Qualtiy Check Service >> GetQualityCheckSummary() started");
            try
            {
                return _qualityChecksRepository.IsEligibleForLiveMarking(qigId, ProjectId, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while getting Script Data :Method Name:GetQualityCheckSummary() and QigID: QigID=" + qigId.ToString());
                throw;
            }
        }

        public async Task<string> CheckedOutScript(LivemarkingApproveModel livemarkingApproveModel, long projectId)
        {
            logger.LogInformation("Qualtiy Check Service >> CheckedOutScript() started");
            try
            {
                return await _qualityChecksRepository.CheckedOutScript(livemarkingApproveModel, projectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while checking Script Data :Method Name:CheckedOutScript() and QigID: QigID=" + livemarkingApproveModel.QigID.ToString());
                throw;
            }
        }

        public async Task<string> AddMarkingRecord(TrialmarkingScriptDetails trialmarkingScriptDetails)
        {
            logger.LogInformation("Qualtiy Check Service >> CheckedOutScript() started");
            try
            {
                return await _qualityChecksRepository.AddMarkingRecord(trialmarkingScriptDetails);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while checking Script Data :Method Name:CheckedOutScript() and QigID: QigID=" + trialmarkingScriptDetails.QigID.ToString());
                throw;
            }
        }

        public async Task<List<Qualitycheckedbyusers>> Getcheckedbyuserslist(long ProjectId, long QigId, long ProjectUserRoleID)
        {

            logger.LogInformation("Qualtiy Check Service >> CheckedOutScript() started");
            try
            {
                return await _qualityChecksRepository.Getcheckedbyuserslist(ProjectId, QigId, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while checking Script Data :Method Name:CheckedOutScript() and QigID: QigID=" + QigId.ToString());
                throw;
            }
        }

        public async Task<string> GetUserStatus(long ProjectId, long qigId, long projectUserRoleId)
        {

            logger.LogInformation("Qualtiy Check Service >> GetUserStatus() started");
            try
            {
                return await _qualityChecksRepository.GetUserStatus(ProjectId, qigId, projectUserRoleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while checking user status Data :Method Name:GetUserStatus() and QigID: QigID=" + qigId.ToString());
                throw;
            }
        }

        public async Task<string> ScriptToBeSubmit(Livepoolscript livepoolscript)
        {
            logger.LogInformation("Qualtiy Check Service >> ScriptToBeSubmit() started");
            try
            {
                return await _qualityChecksRepository.ScriptToBeSubmit(livepoolscript);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while submit script to approved :Method Name:ScriptToBeSubmit() and QigID: QigID=" + livepoolscript.QigID.ToString());
                throw;
            }
        }

        public async Task<string> RevertCheckout(QualityCheckScriptDetailsModel[] scriptsCheckedout, long projectId)
        {
            logger.LogInformation("Qualtiy Check Service >> RevertCheckout() started");
            try
            {
                return await _qualityChecksRepository.RevertCheckout(scriptsCheckedout, projectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while undo checkedout scripts :Method Name:RevertCheckout() " );
                throw;
            }
        }
    }
}
