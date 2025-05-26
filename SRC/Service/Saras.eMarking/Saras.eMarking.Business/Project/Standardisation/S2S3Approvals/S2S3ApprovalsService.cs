using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.S2S3Approvals;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.S2S3Approvals;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.S2S3Approvals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Standardisation.S2S3Approvals
{
    public class S2S3ApprovalsService : BaseService<S2S3ApprovalsService>, IS2S3ApprovalsService
    {
        readonly IS2S3ApprovalsRepository _s2S3ApprovalsRepository;

        public S2S3ApprovalsService(IS2S3ApprovalsRepository s2S3ApprovalsRepository, ILogger<S2S3ApprovalsService> _logger) : base(_logger)
        {
            _s2S3ApprovalsRepository = s2S3ApprovalsRepository;
        }
        public async Task<S2S3ApprovalsModel> GetApprovalStatus(long qigId, long projectId, long projectUserRoleID)
        {
            try
            {
                logger.LogInformation($"S2S3ApprovalsService => GetApprovalStatus() started. QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID}");

                S2S3ApprovalsModel approvalStatus = await _s2S3ApprovalsRepository.GetApprovalStatus(qigId, projectId, projectUserRoleID);

                logger.LogInformation($"S2S3ApprovalsService => GetApprovalStatus() started. QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID}");

                return approvalStatus;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in S2S3ApprovalsService => GetApprovalStatus(). QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID}");
                throw;
            }
        }
        public async Task<List<MarkingPersonal>> GetMarkingPersonal(long qigId, long projectId, long projectUserRoleID, long loginId)
        {
            try
            {
                logger.LogInformation($"S2S3ApprovalsService => GetMarkingPersonal() started. QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID} and LoginId = {loginId}");

                List<MarkingPersonal> approvalStatus = await _s2S3ApprovalsRepository.GetMarkingPersonal(qigId, projectId, projectUserRoleID, loginId);

                logger.LogInformation($"S2S3ApprovalsService => GetMarkingPersonal() started. QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID} and LoginId = {loginId}");

                return approvalStatus;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in S2S3ApprovalsService => GetMarkingPersonal(). QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID} and LoginId = {loginId}");
                throw;
            }
        }
        public async Task<string> scriptApproval(long qigId, long projectId, long projectUserRoleID, long ActionBy, ApprovalDetails approvalDetails)
        {
            string approvalStatus = "";
            try
            {
                logger.LogInformation($"S2S3ApprovalsService => scriptApproval() started. QigId = {qigId} and ProjectId = {projectId} and ProjectUserRoleID = {projectUserRoleID} and ActionBy = {ActionBy} and ApprovalDetails = {approvalDetails}");

                approvalStatus = await _s2S3ApprovalsRepository.scriptApproval(qigId, projectId, projectUserRoleID, ActionBy, approvalDetails);

                logger.LogInformation($"S2S3ApprovalsService => scriptApproval() completed. QigId = {qigId} and ProjectId = {projectId} and ProjectUserRoleID = {projectUserRoleID} and ActionBy = {ActionBy} and ApprovalDetails = {approvalDetails}");

                return approvalStatus;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in S2S3ApprovalsService => scriptApproval(). QigId = {qigId} and ProjectId = {projectId} and ProjectUserRoleID = {projectUserRoleID} and ActionBy = {ActionBy} and ApprovalDetails = {approvalDetails}");
                throw;
            }
        }
        public bool IsEligibleForS2S3LiveMarking(long qigId, long projectId, long projectUserRoleID)
        {
            try
            {
                logger.LogInformation($"S2S3ApprovalsService >> IsEligibleForS2S3LiveMarking() started. QigId = {qigId} and ProjectId = {projectId} and ProjectUserRoleId = {projectUserRoleID}");

                return _s2S3ApprovalsRepository.IsEligibleForS2S3LiveMarking(qigId, projectId, projectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in S2S3ApprovalsService page while getting Script Data :Method Name:IsEligibleForS2S3LiveMarking() and QigID: QigID=" + qigId.ToString());
                throw;
            }
        }
    }
}
