using System;
using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [ServiceContract(Namespace = "http://LicensingAndTransfer.ServiceContracts/2010/01", Name = "ILicensingAndTransfer", SessionMode = SessionMode.Allowed, ProtectionLevel = System.Net.Security.ProtectionLevel.None)]
    public interface ILicensingAndTransfer
    {
        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "TestCenterRegistratiom")]
        LicensingAndTransfer.ServiceContracts.TestCenterRegistrationResponse TestCenterRegistration(LicensingAndTransfer.ServiceContracts.TestCenterRegistrationRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "TagUserToTestCenter")]
        LicensingAndTransfer.ServiceContracts.TestCenterTagResponse TagUserToTestCenter(LicensingAndTransfer.ServiceContracts.TestCenterTagRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "CreateFTPSession")]
        ServiceContracts.CreateFTPSessionResponse CreateFTPSession(ServiceContracts.CreateFTPSessionRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "CompleteFTPSession")]
        ServiceContracts.CompleteFTPSessionResponse CompleteFTPSession(ServiceContracts.CompleteFTPSessionRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "PackageReTransferOrReGenerate")]
        void PackageReTransferOrReGenerate(ServiceContracts.PackageReTransferReGenerateRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpdatePackageStatistics")]
        void UpdatePackageStatistics(ServiceContracts.UpdatePackageStatisticsRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpdatePackageLoadedDate")]
        void UpdatePackageLoadedDate(ServiceContracts.UpdatePackageLoadedDateRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "ValidateTestCenter")]
        ServiceContracts.ValidateTestCenterResponse ValidateTestCenter(ServiceContracts.ValidateTestCenterRequest request);

        //[OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "PersistDailyScheduleSummary")]
        //void PersistDailyScheduleSummary(ServiceContracts.PersistDailyScheduleSummaryRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "GetDXUTCDateTime")]
        ServiceContracts.DXUTCDateTimeResponse GetDXUTCDateTime();

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "SynchronizeDBData")]
        ServiceContracts.ScriptResponse SynchronizeDBData(ServiceContracts.ScriptRequestType request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpdateBatchEndTime")]
        void UpdateBatchEndTime(ServiceContracts.BatchEndTimeUpdateRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "ManageUser")]
        void ManageUser(ServiceContracts.UserRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "ManageRole")]
        void ManageRole(ServiceContracts.RoleRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "ManageOrganization")]
        void ManageOrganization(ServiceContracts.OrganizationRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "SynchronizeCandidates")]
        ServiceContracts.ScheduleResponse SynchronizeCandidates(ServiceContracts.ScheduleRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpsertCandidateDetails")]
        void UpsertCandidateDetails(ServiceContracts.ScheduleRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpdateAssessmentStatistics")]
        void UpdateAssessmentStatistics(ServiceContracts.UpdateAssessmentStatisticsRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpdateTestCenterAssessmentPacks")]
        void UpdateTestCenterAssessmentPacks(ServiceContracts.UpdateTestCenterAssessmentPacksRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "SearchAssessmentStatistics")]
        ServiceContracts.SearchAssessmentStatisticsResponse SearchAssessmentStatistics(ServiceContracts.SearchAssessmentStatisticsRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "ResetQPackTransferForTestCenter")]
        void ResetQPackTransferForTestCenter(string MacID, List<long> ScheduleIDs);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "GetScripts")]
        ServiceContracts.GetNonExecutedScriptsResponseType GetScripts(GetNonExecutedScriptsRequestType request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpdateScripts")]
        void UpdateScripts(UpdateScriptsRequestType request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "CheckQPackInFTP")]
        bool CheckQPackInFTP(string FileName);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "DownloadFile")]
        HttpFileTransferResponse FileTransfer(HttpFileTransferRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UploadFile")]
        void UploadFile(HttpFileTransferRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "CheckApplicationLicense")]
        String CheckApplicationLicense(String request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpdateTestCenterSiteAppraisal")]
        TestCenterSiteAppraisalResponse UpdateTestCenterSiteAppraisal(TestCenterSiteAppraisalRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "GetOrganizationsForTestCenterAppraisal")]
        OrganizationsForTestCenterAppraisalResponse GetOrganizationsForTestCenterAppraisal(String MacAddress);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpdatePostInstallationStatus")]
        void UpdatePostInstallationStatus(ServiceContracts.Contracts.PostInstallationRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "SearchActivatedStatusForBatch")]
        ServiceContracts.Contracts.SearchActivatedStatusForBatchResponse SearchActivatedStatusForBatch(ServiceContracts.Contracts.SearchActivatedStatusForBatchRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "PersistScannedResponse")]
        ServiceContracts.Contracts.ScannedResponse PersistScannedResponse(ServiceContracts.Contracts.ScannedResponseRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "GetTestCenterBatch")]
        ServiceContracts.Contracts.TcBatchesResponse GetTestCenterBatch(ServiceContracts.Contracts.TcBatchesRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "QpackRpackDetailsForTcVerification")]
        ServiceContracts.Contracts.TcVerificationResponse QpackRpackDetailsForTcVerification(ServiceContracts.Contracts.TcVerificationRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "ResetTcQpackReceivedStatus")]
        ServiceContracts.Contracts.ResetQpackReceivedStatusResponse ResetTcQpackReceivedStatus(ServiceContracts.Contracts.ResetQpackReceivedStatusRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "ExtractBubbleSheetData")]
        ServiceContracts.Contracts.BubbleSheetDataResponse ExtractBubbleSheetData(ServiceContracts.Contracts.BubbleSheetDataRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "PersistOfflineScannedResponse")]
        ServiceContracts.Contracts.OMRScannedResponse PersistOfflineScannedResponse(ServiceContracts.Contracts.ScannedResponseRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "ExtractAssessemntDetails")]
        ServiceContracts.Contracts.ExtractAssessmentDetailsResponse ExtractAssessmentDetails(ServiceContracts.Contracts.ExtractAssessmentDetailsRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "PersistOfflineScannedResponseForMobile")]
        void PersistOfflineScannedResponseForMobile(string response);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "VerifyEmailID")]
        int VerifyEmailID(string EmailID);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "VerifyUser")]
        int VerifyUser(string LoginName, string Password, bool IsEncrypted = true);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "PersistOMRResponse")]
        void PersistOMRResponse(ServiceContracts.Contracts.ScannedResponseRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "DoOMRResponseProcessing")]
        void DoOMRResponseProcessing();

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "PersistPaperBasedResponseForMIIClient")]
        void PersistPaperBasedResponseForMIIClient(ServiceContracts.Contracts.ScannedResponseRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "DoOMRResponseProcessingForMIIClient")]
        void DoOMRResponseProcessingForMIIClient();

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpsertAttachmentTransferredState")]
        void UpsertAttachmentTransferredState(ServiceContracts.Contracts.AttachmentDetailsRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpdateAttachmentProcessedDetails")]
        void UpdateAttachmentProcessedDetails(ServiceContracts.Contracts.AttachmentProccedDetailsRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpdateMobileBasedOMRPath")]
        void UpdateMobileBasedOMRPath(System.Data.DataTable tblOMR);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpsertCourse")]
        ServiceContracts.CourseResponse UpsertCourse(ServiceContracts.CourseRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpsertCourseBlock")]
        ServiceContracts.CourseBlockResponse UpsertCourseBlock(ServiceContracts.CourseBlockRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpsertClass")]
        ServiceContracts.ClassResponse UpsertClass(ServiceContracts.ClassRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpsertCustomUser")]
        ServiceContracts.CustomUserResponse UpsertCustomUser(ServiceContracts.CustomUserRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpsertInstructor")]
        ServiceContracts.InstructorResponse UpsertInstructor(ServiceContracts.InstrucotrRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpsertRoster")]
        ServiceContracts.RosterResponse UpsertRoster(ServiceContracts.RosterRequest request);
        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "PersistOfflineScannedResponseForNYCS")]
        ServiceContracts.Contracts.OMRScannedResponse PersistOfflineScannedResponseForNYCS(ServiceContracts.Contracts.ScannedResponseRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "ETLAppErrorLogs")]
        void ETLAppErrorLogs(ServiceContracts.Contracts.ETLAppErrorLogsRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "MSIInstallationResult")]
        void MSIInstallationResult(ServiceContracts.Contracts.MSIInstallationResultRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "ValidateMSIInstallation")]
        ServiceContracts.ValidateMSIInstallationResponse ValidateMSIInstallation(ServiceContracts.ValidateMSIInstallationRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "PersistMediaLoadedDetails")]
        void PersistMediaLoadedDetails(ServiceContracts.Contracts.MediaPackageInfoRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "FetchMediaLoadedDetails")]
        List<ServiceContracts.LoadedMediaPackageDetailsResponse> FetchMediaLoadedDetails(string MacID);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "ValidateTestCenterType")]
        ServiceContracts.ValidateTestCenterTypeResponse ValidateTestCenterType(ServiceContracts.ValidateTestCenterTypeRequest request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "GetLogRequest")]
        ServiceContracts.GetLogResponseType GetLogRequest(ServiceContracts.GetLogRequestType request);

        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "UpdateLogResponse")]
        void  UpdateLogResponse(ServiceContracts.LogoutputRequestType  request);
        
    }
}
