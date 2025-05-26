using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectUserRoleinfo
{
    public long ProjectUserRoleId { get; set; }

    public long ProjectId { get; set; }

    public long UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime? AppointStartDate { get; set; }

    public DateTime? AppointEndDate { get; set; }

    public bool? IsActive { get; set; }

    public bool Isdeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string Remarks { get; set; }

    public long? ReportingTo { get; set; }

    public bool? IsKp { get; set; }

    public int? SendingSchoolId { get; set; }

    public string EmailId { get; set; }

    public string Nric { get; set; }

    public string PhoneNo { get; set; }

    public virtual ICollection<AnnotationTemplate> AnnotationTemplateCreatedByNavigations { get; set; } = new List<AnnotationTemplate>();

    public virtual ICollection<AnnotationTemplateDetail> AnnotationTemplateDetailCreatedByNavigations { get; set; } = new List<AnnotationTemplateDetail>();

    public virtual ICollection<AnnotationTemplateDetail> AnnotationTemplateDetailModifiedByNavigations { get; set; } = new List<AnnotationTemplateDetail>();

    public virtual ICollection<AnnotationTemplate> AnnotationTemplateModifiedByNavigations { get; set; } = new List<AnnotationTemplate>();

    public virtual ICollection<ApireportRequest> ApireportRequests { get; set; } = new List<ApireportRequest>();

    public virtual ICollection<AppSetting> AppSettingCreatedByNavigations { get; set; } = new List<AppSetting>();

    public virtual ICollection<AppSetting> AppSettingModifiedByNavigations { get; set; } = new List<AppSetting>();

    public virtual UserInfo CreatedByNavigation { get; set; }

    public virtual ICollection<EventAudit> EventAudits { get; set; } = new List<EventAudit>();

    public virtual ICollection<MarkSchemeFile> MarkSchemeFileCreatedByNavigations { get; set; } = new List<MarkSchemeFile>();

    public virtual ICollection<MarkSchemeFile> MarkSchemeFileModifiedByNavigations { get; set; } = new List<MarkSchemeFile>();

    public virtual ICollection<MarkingScriptTimeTracking> MarkingScriptTimeTrackings { get; set; } = new List<MarkingScriptTimeTracking>();

    public virtual UserInfo ModifiedByNavigation { get; set; }

    public virtual ICollection<MpstandardizationQueRespMarkingDetail> MpstandardizationQueRespMarkingDetailCreatedByNavigations { get; set; } = new List<MpstandardizationQueRespMarkingDetail>();

    public virtual ICollection<MpstandardizationQueRespMarkingDetail> MpstandardizationQueRespMarkingDetailProjectUserRoles { get; set; } = new List<MpstandardizationQueRespMarkingDetail>();

    public virtual ICollection<MpstandardizationSchedule> MpstandardizationScheduleAssignedByNavigations { get; set; } = new List<MpstandardizationSchedule>();

    public virtual ICollection<MpstandardizationSchedule> MpstandardizationScheduleCreatedByNavigations { get; set; } = new List<MpstandardizationSchedule>();

    public virtual ICollection<MpstandardizationSchedule> MpstandardizationScheduleProjectUserRoles { get; set; } = new List<MpstandardizationSchedule>();

    public virtual ICollection<MpstandardizationScheduleScriptDetail> MpstandardizationScheduleScriptDetails { get; set; } = new List<MpstandardizationScheduleScriptDetail>();

    public virtual ICollection<MpstandardizationScriptMarkingDetail> MpstandardizationScriptMarkingDetailAssignedByNavigations { get; set; } = new List<MpstandardizationScriptMarkingDetail>();

    public virtual ICollection<MpstandardizationScriptMarkingDetail> MpstandardizationScriptMarkingDetailCreatedByNavigations { get; set; } = new List<MpstandardizationScriptMarkingDetail>();

    public virtual ICollection<MpstandardizationScriptMarkingDetail> MpstandardizationScriptMarkingDetailProjectUserRoles { get; set; } = new List<MpstandardizationScriptMarkingDetail>();

    public virtual ICollection<MpstandardizationSummary> MpstandardizationSummaryActionByNavigations { get; set; } = new List<MpstandardizationSummary>();

    public virtual ICollection<MpstandardizationSummary> MpstandardizationSummaryProjectUserRoles { get; set; } = new List<MpstandardizationSummary>();

    public virtual ProjectInfo Project { get; set; }

    public virtual ICollection<ProjectCandidateWithdraw> ProjectCandidateWithdrawUnWithDrawByNavigations { get; set; } = new List<ProjectCandidateWithdraw>();

    public virtual ICollection<ProjectCandidateWithdraw> ProjectCandidateWithdrawWithDrawByNavigations { get; set; } = new List<ProjectCandidateWithdraw>();

    public virtual ICollection<ProjectCenter> ProjectCenterCreatedByNavigations { get; set; } = new List<ProjectCenter>();

    public virtual ICollection<ProjectCenter> ProjectCenterModifiedByNavigations { get; set; } = new List<ProjectCenter>();

    public virtual ICollection<ProjectCenterSchoolMapping> ProjectCenterSchoolMappingCreatedByNavigations { get; set; } = new List<ProjectCenterSchoolMapping>();

    public virtual ICollection<ProjectCenterSchoolMapping> ProjectCenterSchoolMappingModifiedByNavigations { get; set; } = new List<ProjectCenterSchoolMapping>();

    public virtual ICollection<ProjectFile> ProjectFileCreatedByNavigations { get; set; } = new List<ProjectFile>();

    public virtual ICollection<ProjectFile> ProjectFileModifiedByNavigations { get; set; } = new List<ProjectFile>();

    public virtual ICollection<ProjectMarkSchemeBandDetail> ProjectMarkSchemeBandDetailCreatedByNavigations { get; set; } = new List<ProjectMarkSchemeBandDetail>();

    public virtual ICollection<ProjectMarkSchemeBandDetail> ProjectMarkSchemeBandDetailModifiedByNavigations { get; set; } = new List<ProjectMarkSchemeBandDetail>();

    public virtual ICollection<ProjectMarkSchemeQuestion> ProjectMarkSchemeQuestionCreatedByNavigations { get; set; } = new List<ProjectMarkSchemeQuestion>();

    public virtual ICollection<ProjectMarkSchemeQuestion> ProjectMarkSchemeQuestionModifiedByNavigations { get; set; } = new List<ProjectMarkSchemeQuestion>();

    public virtual ICollection<ProjectMarkSchemeTemplate> ProjectMarkSchemeTemplateCreatedByNavigations { get; set; } = new List<ProjectMarkSchemeTemplate>();

    public virtual ICollection<ProjectMarkSchemeTemplate> ProjectMarkSchemeTemplateModifiedByNavigations { get; set; } = new List<ProjectMarkSchemeTemplate>();

    public virtual ICollection<ProjectQig> ProjectQigActionByNavigations { get; set; } = new List<ProjectQig>();

    public virtual ICollection<ProjectQig> ProjectQigCreatedByNavigations { get; set; } = new List<ProjectQig>();

    public virtual ICollection<ProjectQig> ProjectQigModifiedByNavigations { get; set; } = new List<ProjectQig>();

    public virtual ICollection<ProjectQigcenterMapping> ProjectQigcenterMappingCreatedByNavigations { get; set; } = new List<ProjectQigcenterMapping>();

    public virtual ICollection<ProjectQigcenterMapping> ProjectQigcenterMappingModifiedByNavigations { get; set; } = new List<ProjectQigcenterMapping>();

    public virtual ICollection<ProjectQigcenterMapping> ProjectQigcenterMappingRecommendedByNavigations { get; set; } = new List<ProjectQigcenterMapping>();

    public virtual ICollection<ProjectQigquestion> ProjectQigquestionCreatedByNavigations { get; set; } = new List<ProjectQigquestion>();

    public virtual ICollection<ProjectQigquestion> ProjectQigquestionModifiedByNavigations { get; set; } = new List<ProjectQigquestion>();

    public virtual ICollection<ProjectQigreset> ProjectQigresetAuthenticateByNavigations { get; set; } = new List<ProjectQigreset>();

    public virtual ICollection<ProjectQigreset> ProjectQigresetResetByNavigations { get; set; } = new List<ProjectQigreset>();

    public virtual ICollection<ProjectQigscriptsImportEvent> ProjectQigscriptsImportEvents { get; set; } = new List<ProjectQigscriptsImportEvent>();

    public virtual ICollection<ProjectQigteamHierarchy> ProjectQigteamHierarchyCreatedByNavigations { get; set; } = new List<ProjectQigteamHierarchy>();

    public virtual ICollection<ProjectQigteamHierarchy> ProjectQigteamHierarchyModifiedByNavigations { get; set; } = new List<ProjectQigteamHierarchy>();

    public virtual ICollection<ProjectQigteamHierarchy> ProjectQigteamHierarchyProjectUserRoles { get; set; } = new List<ProjectQigteamHierarchy>();

    public virtual ICollection<ProjectQigteamHierarchy> ProjectQigteamHierarchyReportingToNavigations { get; set; } = new List<ProjectQigteamHierarchy>();

    public virtual ICollection<ProjectQuestionChoiceMapping> ProjectQuestionChoiceMappingCreatedByNavigations { get; set; } = new List<ProjectQuestionChoiceMapping>();

    public virtual ICollection<ProjectQuestionChoiceMapping> ProjectQuestionChoiceMappingModifiedByNavigations { get; set; } = new List<ProjectQuestionChoiceMapping>();

    public virtual ICollection<ProjectScheduleCalendar> ProjectScheduleCalendarCreatedByNavigations { get; set; } = new List<ProjectScheduleCalendar>();

    public virtual ICollection<ProjectScheduleCalendar> ProjectScheduleCalendarModifiedByNavigations { get; set; } = new List<ProjectScheduleCalendar>();

    public virtual ICollection<ProjectSchedule> ProjectScheduleCreatedByNavigations { get; set; } = new List<ProjectSchedule>();

    public virtual ICollection<ProjectSchedule> ProjectScheduleModifiedByNavigations { get; set; } = new List<ProjectSchedule>();

    public virtual ICollection<ProjectTeam> ProjectTeamCreatedByNavigations { get; set; } = new List<ProjectTeam>();

    public virtual ICollection<ProjectTeam> ProjectTeamModifiedByNavigations { get; set; } = new List<ProjectTeam>();

    public virtual ICollection<ProjectTeamQig> ProjectTeamQigAssignedByNavigations { get; set; } = new List<ProjectTeamQig>();

    public virtual ICollection<ProjectTeamQig> ProjectTeamQigModifiedByNavigations { get; set; } = new List<ProjectTeamQig>();

    public virtual ICollection<ProjectTeamUserInfo> ProjectTeamUserInfoCreatedByNavigations { get; set; } = new List<ProjectTeamUserInfo>();

    public virtual ICollection<ProjectTeamUserInfo> ProjectTeamUserInfoModifiedByNavigations { get; set; } = new List<ProjectTeamUserInfo>();

    public virtual ICollection<ProjectTeamUserInfo> ProjectTeamUserInfoProjectUserRoles { get; set; } = new List<ProjectTeamUserInfo>();

    public virtual ICollection<ProjectTeamUserInfo> ProjectTeamUserInfoReportingToNavigations { get; set; } = new List<ProjectTeamUserInfo>();

    public virtual ICollection<ProjectUserQuestionResponse> ProjectUserQuestionResponses { get; set; } = new List<ProjectUserQuestionResponse>();

    public virtual ICollection<ProjectUserSchoolMapping> ProjectUserSchoolMappingCreatedByNavigations { get; set; } = new List<ProjectUserSchoolMapping>();

    public virtual ICollection<ProjectUserSchoolMapping> ProjectUserSchoolMappingModifiedByNavigations { get; set; } = new List<ProjectUserSchoolMapping>();

    public virtual ICollection<ProjectUserSchoolMapping> ProjectUserSchoolMappingProjectUserRoles { get; set; } = new List<ProjectUserSchoolMapping>();

    public virtual ICollection<ProjectUserScript> ProjectUserScriptMarkedByNavigations { get; set; } = new List<ProjectUserScript>();

    public virtual ICollection<ProjectUserScript> ProjectUserScriptRecommendedByNavigations { get; set; } = new List<ProjectUserScript>();

    public virtual ICollection<ProjectUserScript> ProjectUserScriptUnRecommendedByNavigations { get; set; } = new List<ProjectUserScript>();

    public virtual ICollection<ProjectWorkflowStatusTracking> ProjectWorkflowStatusTrackings { get; set; } = new List<ProjectWorkflowStatusTracking>();

    public virtual ICollection<QigstandardizationScriptSetting> QigstandardizationScriptSettingCreatedByNavigations { get; set; } = new List<QigstandardizationScriptSetting>();

    public virtual ICollection<QigstandardizationScriptSetting> QigstandardizationScriptSettingModifiedByNavigations { get; set; } = new List<QigstandardizationScriptSetting>();

    public virtual ICollection<QigtoAnnotationTemplateMapping> QigtoAnnotationTemplateMappingCreatedByNavigations { get; set; } = new List<QigtoAnnotationTemplateMapping>();

    public virtual ICollection<QigtoAnnotationTemplateMapping> QigtoAnnotationTemplateMappingModifiedByNavigations { get; set; } = new List<QigtoAnnotationTemplateMapping>();

    public virtual ICollection<QualifyingAssessmentScriptDetail> QualifyingAssessmentScriptDetailCreatedByNavigations { get; set; } = new List<QualifyingAssessmentScriptDetail>();

    public virtual ICollection<QualifyingAssessmentScriptDetail> QualifyingAssessmentScriptDetailModifiedByNavigations { get; set; } = new List<QualifyingAssessmentScriptDetail>();

    public virtual ICollection<QuestionScoreComponentMarkingDetail> QuestionScoreComponentMarkingDetails { get; set; } = new List<QuestionScoreComponentMarkingDetail>();

    public virtual ICollection<QuestionUserResponseMarkingDetail> QuestionUserResponseMarkingDetails { get; set; } = new List<QuestionUserResponseMarkingDetail>();

    public virtual Roleinfo Role { get; set; }

    public virtual ICollection<ScriptCategorizationPool> ScriptCategorizationPoolCreatedByNavigations { get; set; } = new List<ScriptCategorizationPool>();

    public virtual ICollection<ScriptCategorizationPool> ScriptCategorizationPoolModifiedByNavigations { get; set; } = new List<ScriptCategorizationPool>();

    public virtual ICollection<ScriptMarkingPhaseStatusTracking> ScriptMarkingPhaseStatusTrackingActionByNavigations { get; set; } = new List<ScriptMarkingPhaseStatusTracking>();

    public virtual ICollection<ScriptMarkingPhaseStatusTracking> ScriptMarkingPhaseStatusTrackingAssignedToNavigations { get; set; } = new List<ScriptMarkingPhaseStatusTracking>();

    public virtual SchoolInfo SendingSchool { get; set; }

    public virtual ICollection<StandardizationQualifyingAssessment> StandardizationQualifyingAssessmentCreatedByNavigations { get; set; } = new List<StandardizationQualifyingAssessment>();

    public virtual ICollection<StandardizationQualifyingAssessment> StandardizationQualifyingAssessmentModifiedByNavigations { get; set; } = new List<StandardizationQualifyingAssessment>();

    public virtual UserInfo User { get; set; }

    public virtual ICollection<UserResponseFrequencyDistribution> UserResponseFrequencyDistributionCreatedByNavigations { get; set; } = new List<UserResponseFrequencyDistribution>();

    public virtual ICollection<UserResponseFrequencyDistribution> UserResponseFrequencyDistributionModeratedByNavigations { get; set; } = new List<UserResponseFrequencyDistribution>();

    public virtual ICollection<UserResponseFrequencyDistribution> UserResponseFrequencyDistributionModifiedByNavigations { get; set; } = new List<UserResponseFrequencyDistribution>();

    public virtual ICollection<UserScriptMarkingDetail> UserScriptMarkingDetailApprovedByNavigations { get; set; } = new List<UserScriptMarkingDetail>();

    public virtual ICollection<UserScriptMarkingDetail> UserScriptMarkingDetailMarkedByNavigations { get; set; } = new List<UserScriptMarkingDetail>();

    public virtual ICollection<UserScriptMarkingDetail> UserScriptMarkingDetailSelectedByNavigations { get; set; } = new List<UserScriptMarkingDetail>();

    public virtual ICollection<UserStatusTracking> UserStatusTrackingActionByProjectUserRoles { get; set; } = new List<UserStatusTracking>();

    public virtual ICollection<UserStatusTracking> UserStatusTrackingProjectUserRoles { get; set; } = new List<UserStatusTracking>();
}
