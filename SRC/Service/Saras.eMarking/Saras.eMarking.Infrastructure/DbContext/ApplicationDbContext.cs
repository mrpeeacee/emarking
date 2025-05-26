using Microsoft.EntityFrameworkCore;
using Saras.eMarking.Business.Interfaces;
using Saras.eMarking.Domain.Entities;
namespace Saras.eMarking.Infrastructure
{
    public partial class ApplicationDbContext : DbContext, IApplicationDBContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AnnotationGroup> AnnotationGroups { get; set; }

        public virtual DbSet<AnnotationTemplate> AnnotationTemplates { get; set; }

        public virtual DbSet<AnnotationTemplateArchive> AnnotationTemplateArchives { get; set; }

        public virtual DbSet<AnnotationTemplateDetail> AnnotationTemplateDetails { get; set; }

        public virtual DbSet<AnnotationTemplateDetailsArchive> AnnotationTemplateDetailsArchives { get; set; }

        public virtual DbSet<AnnotationTool> AnnotationTools { get; set; }

        public virtual DbSet<ApireportRequest> ApireportRequests { get; set; }

        public virtual DbSet<ApireportRequestArchive> ApireportRequestArchives { get; set; }

        public virtual DbSet<ApireportRequestDetail> ApireportRequestDetails { get; set; }

        public virtual DbSet<ApireportRequestDetailsArchive> ApireportRequestDetailsArchives { get; set; }

        public virtual DbSet<ApireportRequestHistory> ApireportRequestHistories { get; set; }

        public virtual DbSet<AppSetting> AppSettings { get; set; }

        public virtual DbSet<AppSettingKey> AppSettingKeys { get; set; }

        public virtual DbSet<AppSettingsArchive> AppSettingsArchives { get; set; }

        public virtual DbSet<ApplicationWorkflow> ApplicationWorkflows { get; set; }

        public virtual DbSet<AppsettingGroup> AppsettingGroups { get; set; }

        public virtual DbSet<CourseMovementValidation> CourseMovementValidations { get; set; }

        public virtual DbSet<CourseMovementValidationHistory> CourseMovementValidationHistories { get; set; }

        public virtual DbSet<EntityMaster> EntityMasters { get; set; }

        public virtual DbSet<ErrorHandling> ErrorHandlings { get; set; }

        public virtual DbSet<EventAudit> EventAudits { get; set; }

        public virtual DbSet<EventAuditArchive> EventAuditArchives { get; set; }

        public virtual DbSet<EventMaster> EventMasters { get; set; }

        public virtual DbSet<ExamLevel> ExamLevels { get; set; }

        public virtual DbSet<ExamSeries> ExamSeries { get; set; }

        public virtual DbSet<ExamYear> ExamYears { get; set; }

        public virtual DbSet<IndexTable> IndexTables { get; set; }

        public virtual DbSet<MarkSchemeFile> MarkSchemeFiles { get; set; }

        public virtual DbSet<MarkSchemeFilesArchive> MarkSchemeFilesArchives { get; set; }

        public virtual DbSet<MarkingCenter> MarkingCenters { get; set; }

        public virtual DbSet<MarkingScriptTimeTracking> MarkingScriptTimeTrackings { get; set; }

        public virtual DbSet<MarkingScriptTimeTrackingArchive> MarkingScriptTimeTrackingArchives { get; set; }

        public virtual DbSet<ModeOfAssessment> ModeOfAssessments { get; set; }

        public virtual DbSet<ModuleMaster> ModuleMasters { get; set; }

        public virtual DbSet<MpstandardizationQueRespMarkingDetail> MpstandardizationQueRespMarkingDetails { get; set; }

        public virtual DbSet<MpstandardizationQueRespMarkingDetailsArchive> MpstandardizationQueRespMarkingDetailsArchives { get; set; }

        public virtual DbSet<MpstandardizationSchedule> MpstandardizationSchedules { get; set; }

        public virtual DbSet<MpstandardizationScheduleArchive> MpstandardizationScheduleArchives { get; set; }

        public virtual DbSet<MpstandardizationScheduleScriptDetail> MpstandardizationScheduleScriptDetails { get; set; }

        public virtual DbSet<MpstandardizationScheduleScriptDetailsArchive> MpstandardizationScheduleScriptDetailsArchives { get; set; }

        public virtual DbSet<MpstandardizationScriptMarkingDetail> MpstandardizationScriptMarkingDetails { get; set; }

        public virtual DbSet<MpstandardizationScriptMarkingDetailsArchive> MpstandardizationScriptMarkingDetailsArchives { get; set; }

        public virtual DbSet<MpstandardizationSummary> MpstandardizationSummaries { get; set; }

        public virtual DbSet<MpstandardizationSummaryArchive> MpstandardizationSummaryArchives { get; set; }

        public virtual DbSet<Organization> Organizations { get; set; }

        public virtual DbSet<PassPharse> PassPharses { get; set; }

        public virtual DbSet<Privilege> Privileges { get; set; }

        public virtual DbSet<ProjectArchiveRequest> ProjectArchiveRequests { get; set; }

        public virtual DbSet<ProjectArchiveRequestDetail> ProjectArchiveRequestDetails { get; set; }

        public virtual DbSet<ProjectCandidateWithdraw> ProjectCandidateWithdraws { get; set; }

        public virtual DbSet<ProjectCandidateWithdrawArchive> ProjectCandidateWithdrawArchives { get; set; }

        public virtual DbSet<ProjectCenter> ProjectCenters { get; set; }

        public virtual DbSet<ProjectCenterSchoolMapping> ProjectCenterSchoolMappings { get; set; }

        public virtual DbSet<ProjectCenterSchoolMappingArchive> ProjectCenterSchoolMappingArchives { get; set; }

        public virtual DbSet<ProjectCentersArchive> ProjectCentersArchives { get; set; }

        public virtual DbSet<ProjectCreationJobHistory> ProjectCreationJobHistories { get; set; }

        public virtual DbSet<ProjectFile> ProjectFiles { get; set; }

        public virtual DbSet<ProjectFilesArchive> ProjectFilesArchives { get; set; }

        public virtual DbSet<ProjectInfo> ProjectInfos { get; set; }

        public virtual DbSet<ProjectMarkSchemeBandDetail> ProjectMarkSchemeBandDetails { get; set; }

        public virtual DbSet<ProjectMarkSchemeBandDetailsArchive> ProjectMarkSchemeBandDetailsArchives { get; set; }

        public virtual DbSet<ProjectMarkSchemeQuestion> ProjectMarkSchemeQuestions { get; set; }

        public virtual DbSet<ProjectMarkSchemeQuestionArchive> ProjectMarkSchemeQuestionArchives { get; set; }

        public virtual DbSet<ProjectMarkSchemeTemplate> ProjectMarkSchemeTemplates { get; set; }

        public virtual DbSet<ProjectMarkSchemeTemplateArchive> ProjectMarkSchemeTemplateArchives { get; set; }

        public virtual DbSet<ProjectQig> ProjectQigs { get; set; }

        public virtual DbSet<ProjectQigArchive> ProjectQigArchives { get; set; }

        public virtual DbSet<ProjectQigcenterMapping> ProjectQigcenterMappings { get; set; }

        public virtual DbSet<ProjectQigcenterMappingArchive> ProjectQigcenterMappingArchives { get; set; }

        public virtual DbSet<ProjectQigquestion> ProjectQigquestions { get; set; }

        public virtual DbSet<ProjectQigquestionsArchive> ProjectQigquestionsArchives { get; set; }

        public virtual DbSet<ProjectQigreset> ProjectQigresets { get; set; }

        public virtual DbSet<ProjectQigresetArchive> ProjectQigresetArchives { get; set; }

        public virtual DbSet<ProjectQigscriptsImportEvent> ProjectQigscriptsImportEvents { get; set; }

        public virtual DbSet<ProjectQigscriptsImportEventsArchive> ProjectQigscriptsImportEventsArchives { get; set; }

        public virtual DbSet<ProjectQigteamHierarchy> ProjectQigteamHierarchies { get; set; }

        public virtual DbSet<ProjectQigteamHierarchyArchive> ProjectQigteamHierarchyArchives { get; set; }

        public virtual DbSet<ProjectQuestion> ProjectQuestions { get; set; }

        public virtual DbSet<ProjectQuestionAsset> ProjectQuestionAssets { get; set; }

        public virtual DbSet<ProjectQuestionAssetsArchive> ProjectQuestionAssetsArchives { get; set; }

        public virtual DbSet<ProjectQuestionChoiceMapping> ProjectQuestionChoiceMappings { get; set; }

        public virtual DbSet<ProjectQuestionChoiceMappingArchive> ProjectQuestionChoiceMappingArchives { get; set; }

        public virtual DbSet<ProjectQuestionScoreComponent> ProjectQuestionScoreComponents { get; set; }

        public virtual DbSet<ProjectQuestionScoreComponentsArchive> ProjectQuestionScoreComponentsArchives { get; set; }

        public virtual DbSet<ProjectQuestionsArchive> ProjectQuestionsArchives { get; set; }

        public virtual DbSet<ProjectQuestionsHistory> ProjectQuestionsHistories { get; set; }

        public virtual DbSet<ProjectSchedule> ProjectSchedules { get; set; }

        public virtual DbSet<ProjectScheduleArchive> ProjectScheduleArchives { get; set; }

        public virtual DbSet<ProjectScheduleCalendar> ProjectScheduleCalendars { get; set; }

        public virtual DbSet<ProjectScheduleCalendarArchive> ProjectScheduleCalendarArchives { get; set; }

        public virtual DbSet<ProjectTeam> ProjectTeams { get; set; }

        public virtual DbSet<ProjectTeamQig> ProjectTeamQigs { get; set; }

        public virtual DbSet<ProjectTeamUserInfo> ProjectTeamUserInfos { get; set; }

        public virtual DbSet<ProjectUserQuestionResponse> ProjectUserQuestionResponses { get; set; }

        public virtual DbSet<ProjectUserQuestionResponseAcrchive> ProjectUserQuestionResponseAcrchives { get; set; }

        public virtual DbSet<ProjectUserQuestionResponseArchive> ProjectUserQuestionResponseArchives { get; set; }

        public virtual DbSet<ProjectUserRoleinfo> ProjectUserRoleinfos { get; set; }

        public virtual DbSet<ProjectUserRoleinfoArchive> ProjectUserRoleinfoArchives { get; set; }

        public virtual DbSet<ProjectUserSchoolMapping> ProjectUserSchoolMappings { get; set; }

        public virtual DbSet<ProjectUserSchoolMappingArchive> ProjectUserSchoolMappingArchives { get; set; }

        public virtual DbSet<ProjectUserScript> ProjectUserScripts { get; set; }

        public virtual DbSet<ProjectUserScriptArchive> ProjectUserScriptArchives { get; set; }

        public virtual DbSet<ProjectWorkflow> ProjectWorkflows { get; set; }

        public virtual DbSet<ProjectWorkflowStatusTracking> ProjectWorkflowStatusTrackings { get; set; }

        public virtual DbSet<ProjectWorkflowStatusTrackingArchive> ProjectWorkflowStatusTrackingArchives { get; set; }

        public virtual DbSet<Projectuserscript03072024> Projectuserscript03072024s { get; set; }

        public virtual DbSet<QigstandardizationScriptSetting> QigstandardizationScriptSettings { get; set; }

        public virtual DbSet<QigstandardizationScriptSettingsArchive> QigstandardizationScriptSettingsArchives { get; set; }

        public virtual DbSet<QigtoAnnotationTemplateMapping> QigtoAnnotationTemplateMappings { get; set; }

        public virtual DbSet<QigtoAnnotationTemplateMappingArchive> QigtoAnnotationTemplateMappingArchives { get; set; }

        public virtual DbSet<QrtzBlobTrigger> QrtzBlobTriggers { get; set; }

        public virtual DbSet<QrtzCalendar> QrtzCalendars { get; set; }

        public virtual DbSet<QrtzCronTrigger> QrtzCronTriggers { get; set; }

        public virtual DbSet<QrtzFiredTrigger> QrtzFiredTriggers { get; set; }

        public virtual DbSet<QrtzJobDetail> QrtzJobDetails { get; set; }

        public virtual DbSet<QrtzLock> QrtzLocks { get; set; }

        public virtual DbSet<QrtzPausedTriggerGrp> QrtzPausedTriggerGrps { get; set; }

        public virtual DbSet<QrtzScheduleHistory> QrtzScheduleHistories { get; set; }

        public virtual DbSet<QrtzSchedulerState> QrtzSchedulerStates { get; set; }

        public virtual DbSet<QrtzSimpleTrigger> QrtzSimpleTriggers { get; set; }

        public virtual DbSet<QrtzSimpropTrigger> QrtzSimpropTriggers { get; set; }

        public virtual DbSet<QrtzTrigger> QrtzTriggers { get; set; }

        public virtual DbSet<QualifyingAssessmentScriptDetail> QualifyingAssessmentScriptDetails { get; set; }

        public virtual DbSet<QualifyingAssessmentScriptDetailsArchive> QualifyingAssessmentScriptDetailsArchives { get; set; }

        public virtual DbSet<QuartzRcjobTracking> QuartzRcjobTrackings { get; set; }

        public virtual DbSet<QuestionScoreComponentMarkingDetail> QuestionScoreComponentMarkingDetails { get; set; }

        public virtual DbSet<QuestionScoreComponentMarkingDetailsArchive> QuestionScoreComponentMarkingDetailsArchives { get; set; }

        public virtual DbSet<QuestionUserResponseMarkingDetail> QuestionUserResponseMarkingDetails { get; set; }

        public virtual DbSet<QuestionUserResponseMarkingDetailsArchive> QuestionUserResponseMarkingDetailsArchives { get; set; }

        public virtual DbSet<QuestionUserResponseMarkingImage> QuestionUserResponseMarkingImages { get; set; }

        public virtual DbSet<QuestionUserResponseMarkingImageArchive> QuestionUserResponseMarkingImageArchives { get; set; }

        public virtual DbSet<RoleLevel> RoleLevels { get; set; }

        public virtual DbSet<RoleToPrivilege> RoleToPrivileges { get; set; }

        public virtual DbSet<Roleinfo> Roleinfos { get; set; }

        public virtual DbSet<SchoolInfo> SchoolInfos { get; set; }

        public virtual DbSet<SchoolInfoArchive> SchoolInfoArchives { get; set; }

        public virtual DbSet<ScoreComponent> ScoreComponents { get; set; }

        public virtual DbSet<ScoreComponentDetail> ScoreComponentDetails { get; set; }

        public virtual DbSet<Script> Scripts { get; set; }

        public virtual DbSet<ScriptCategorizationPool> ScriptCategorizationPools { get; set; }

        public virtual DbSet<ScriptCategorizationPoolArchive> ScriptCategorizationPoolArchives { get; set; }

        public virtual DbSet<ScriptCategorizationPoolHistory> ScriptCategorizationPoolHistories { get; set; }

        public virtual DbSet<ScriptMarkingPhaseStatusTracking> ScriptMarkingPhaseStatusTrackings { get; set; }

        public virtual DbSet<ScriptMarkingPhaseStatusTrackingArchive> ScriptMarkingPhaseStatusTrackingArchives { get; set; }

        public virtual DbSet<ScriptMarkingPhaseStatusTrackingHistory> ScriptMarkingPhaseStatusTrackingHistories { get; set; }

        public virtual DbSet<StandardizationQualifyingAssessment> StandardizationQualifyingAssessments { get; set; }

        public virtual DbSet<StandardizationQualifyingAssessmentArchive> StandardizationQualifyingAssessmentArchives { get; set; }

        public virtual DbSet<SubjectInfo> SubjectInfos { get; set; }

        public virtual DbSet<SubjectPaperInfo> SubjectPaperInfos { get; set; }

        public virtual DbSet<SummaryCandidateQuestionComponentMark> SummaryCandidateQuestionComponentMarks { get; set; }

        public virtual DbSet<SummaryCandidateQuestionComponentMarksArchive> SummaryCandidateQuestionComponentMarksArchives { get; set; }

        public virtual DbSet<SummaryProjectUserResultDetail> SummaryProjectUserResultDetails { get; set; }

        public virtual DbSet<SummaryProjectUserResultDetailsArchive> SummaryProjectUserResultDetailsArchives { get; set; }

        public virtual DbSet<TblTopurge> TblTopurges { get; set; }

        public virtual DbSet<Template> Templates { get; set; }

        public virtual DbSet<TemplateUserMapping> TemplateUserMappings { get; set; }

        public virtual DbSet<TemplateUserMappingArchive> TemplateUserMappingArchives { get; set; }

        public virtual DbSet<TimeZone> TimeZones { get; set; }

        public virtual DbSet<UserInfo> UserInfos { get; set; }

        public virtual DbSet<UserInfoArchive> UserInfoArchives { get; set; }

        public virtual DbSet<UserLoginToken> UserLoginTokens { get; set; }

        public virtual DbSet<UserLoginTokenArchive> UserLoginTokenArchives { get; set; }

        public virtual DbSet<UserPwdDetail> UserPwdDetails { get; set; }

        public virtual DbSet<UserPwdDetailsArchive> UserPwdDetailsArchives { get; set; }

        public virtual DbSet<UserResponseFrequencyDistribution> UserResponseFrequencyDistributions { get; set; }

        public virtual DbSet<UserResponseFrequencyDistributionArchive> UserResponseFrequencyDistributionArchives { get; set; }

        public virtual DbSet<UserScriptMarkingDetail> UserScriptMarkingDetails { get; set; }

        public virtual DbSet<UserScriptMarkingDetails03072024> UserScriptMarkingDetails03072024s { get; set; }

        public virtual DbSet<UserScriptMarkingDetailsArchive> UserScriptMarkingDetailsArchives { get; set; }

        public virtual DbSet<UserStatusTracking> UserStatusTrackings { get; set; }

        public virtual DbSet<UserStatusTrackingArchive> UserStatusTrackingArchives { get; set; }

        public virtual DbSet<UserToExamLevelMapping> UserToExamLevelMappings { get; set; }

        public virtual DbSet<UserToExamLevelMappingArchive> UserToExamLevelMappingArchives { get; set; }

        public virtual DbSet<UserToOrganizationMapping> UserToOrganizationMappings { get; set; }

        public virtual DbSet<UserToOrganizationMappingArchive> UserToOrganizationMappingArchives { get; set; }

        public virtual DbSet<UserToRoleMapping> UserToRoleMappings { get; set; }

        public virtual DbSet<UserToRoleMappingArchive> UserToRoleMappingArchives { get; set; }

        public virtual DbSet<UserToTimeZoneMapping> UserToTimeZoneMappings { get; set; }

        public virtual DbSet<UserToTimeZoneMappingArchive> UserToTimeZoneMappingArchives { get; set; }

        public virtual DbSet<Userinfo18122023> Userinfo18122023s { get; set; }

        public virtual DbSet<ValidateCaptcha> ValidateCaptchas { get; set; }

        public virtual DbSet<WorkflowStatus> WorkflowStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnnotationGroup>(entity =>
            {
                entity.ToTable("AnnotationGroup", "Marking");

                entity.HasIndex(e => e.AnnotationGroupCode, "IDX_Marking_AnnotationGroup_AnnotationGroupCode");

                entity.Property(e => e.AnnotationGroupId).HasColumnName("AnnotationGroupID");
                entity.Property(e => e.AnnotationGroupCode).HasMaxLength(50);
                entity.Property(e => e.AnnotationGroupName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Createddate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AnnotationTemplate>(entity =>
            {
                entity.ToTable("AnnotationTemplate", "Marking");

                entity.HasIndex(e => e.AnnotationTemplateCode, "IDX_Marking_AnnotationTemplate_AnnotationTemplateCode");

                entity.Property(e => e.AnnotationTemplateId).HasColumnName("AnnotationTemplateID");
                entity.Property(e => e.AnnotationTemplateCode).HasMaxLength(50);
                entity.Property(e => e.AnnotationTemplateName).HasMaxLength(100);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AnnotationTemplateCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_AnnotationTemplate_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.AnnotationTemplateModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_AnnotationTemplate_ProjectUserRoleinfo1");
            });

            modelBuilder.Entity<AnnotationTemplateArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("AnnotationTemplate_Archive", "Marking");

                entity.HasIndex(e => e.AnnotationTemplateCode, "IDX_Marking_AnnotationTemplate_AnnotationTemplateCode_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AnnotationTemplateCode).HasMaxLength(50);
                entity.Property(e => e.AnnotationTemplateId).HasColumnName("AnnotationTemplateID");
                entity.Property(e => e.AnnotationTemplateName).HasMaxLength(100);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            });

            modelBuilder.Entity<AnnotationTemplateDetail>(entity =>
            {
                entity.HasKey(e => e.TemplateDetailId);

                entity.ToTable("AnnotationTemplateDetails", "Marking");

                entity.HasIndex(e => new { e.AnnotationTemplateId, e.AnnotationToolId, e.Isdeleted }, "IDX_Marking_AnnotationTemplateDetails_AnnotationTemplateID_AnnotationToolID_Isdeleted");

                entity.Property(e => e.TemplateDetailId).HasColumnName("TemplateDetailID");
                entity.Property(e => e.AnnotationTemplateId).HasColumnName("AnnotationTemplateID");
                entity.Property(e => e.AnnotationToolId).HasColumnName("AnnotationToolID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.AnnotationTemplate).WithMany(p => p.AnnotationTemplateDetails)
                    .HasForeignKey(d => d.AnnotationTemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnnotationTemplateDetails_AnnotationTemplate");

                entity.HasOne(d => d.AnnotationTool).WithMany(p => p.AnnotationTemplateDetails)
                    .HasForeignKey(d => d.AnnotationToolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnnotationTemplateDetails_AnnotationTools");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AnnotationTemplateDetailCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_AnnotationTemplateDetails_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.AnnotationTemplateDetailModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_AnnotationTemplateDetails_ProjectUserRoleinfo1");
            });

            modelBuilder.Entity<AnnotationTemplateDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("AnnotationTemplateDetails_Archive", "Marking");

                entity.HasIndex(e => new { e.AnnotationTemplateId, e.AnnotationToolId, e.Isdeleted }, "IDX_Marking_AnnotationTemplateDetails_AnnotationTemplateID_AnnotationToolID_Isdeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AnnotationTemplateId).HasColumnName("AnnotationTemplateID");
                entity.Property(e => e.AnnotationToolId).HasColumnName("AnnotationToolID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.TemplateDetailId).HasColumnName("TemplateDetailID");
            });

            modelBuilder.Entity<AnnotationTool>(entity =>
            {
                entity.ToTable("AnnotationTools", "Marking");

                entity.HasIndex(e => new { e.AnnotationGroupId, e.IsDeleted }, "IDX_Marking_AnnotationTools_AnnotationGroupID_Isdeleted");

                entity.Property(e => e.AnnotationToolId).HasColumnName("AnnotationToolID");
                entity.Property(e => e.AnnotationGroupId).HasColumnName("AnnotationGroupID");
                entity.Property(e => e.AnnotationToolCode).HasMaxLength(50);
                entity.Property(e => e.AnnotationToolName).HasMaxLength(500);
                entity.Property(e => e.AnnotationToolType).HasComment("1-->Icon, 2-->Image, 3-->Comment");
                entity.Property(e => e.AssociatedMark).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ColoorName).HasMaxLength(50);
                entity.Property(e => e.ColorCode).HasMaxLength(50);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsRequiredForDiscrit).HasDefaultValue(true);
                entity.Property(e => e.Modifieddate).HasColumnType("datetime");
                entity.Property(e => e.Path).HasMaxLength(2000);
                entity.Property(e => e.ReferanceAnnotationId).HasColumnName("ReferanceAnnotationID");
            });

            modelBuilder.Entity<ApireportRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId);

                entity.ToTable("APIReportRequest", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.ReportType, e.IsDeleted, e.IsProcessed }, "IDX_Marking_APIReportRequest_ProjectID_ReportType_IsDeleted_IsProcessed");

                entity.Property(e => e.RequestId).HasColumnName("RequestID");
                entity.Property(e => e.ErrorMsg).HasMaxLength(2000);
                entity.Property(e => e.FileName).HasMaxLength(250);
                entity.Property(e => e.FilePath).HasMaxLength(2000);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsProcessed).HasDefaultValue(false);
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Remarks).HasMaxLength(2000);
                entity.Property(e => e.ReportType).HasComment("1 --> EMS1, 2 --> EMS2,");
                entity.Property(e => e.RequestDate).HasColumnType("datetime");
                entity.Property(e => e.RequestGuid)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("RequestGUID");
                entity.Property(e => e.RequestServedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Project).WithMany(p => p.ApireportRequests)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_APIReportRequest_ProjectInfo");

                entity.HasOne(d => d.RequestByNavigation).WithMany(p => p.ApireportRequests)
                    .HasForeignKey(d => d.RequestBy)
                    .HasConstraintName("FK_APIReportRequest_ProjectUserRoleinfo");
            });

            modelBuilder.Entity<ApireportRequestArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("APIReportRequest_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.ReportType, e.IsDeleted, e.IsProcessed }, "IDX_Marking_APIReportRequest_ProjectID_ReportType_IsDeleted_IsProcessed_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.ErrorMsg).HasMaxLength(2000);
                entity.Property(e => e.FileName).HasMaxLength(250);
                entity.Property(e => e.FilePath).HasMaxLength(2000);
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Remarks).HasMaxLength(2000);
                entity.Property(e => e.ReportType).HasComment("1 --> EMS1, 2 --> EMS2,");
                entity.Property(e => e.RequestDate).HasColumnType("datetime");
                entity.Property(e => e.RequestGuid).HasColumnName("RequestGUID");
                entity.Property(e => e.RequestId).HasColumnName("RequestID");
                entity.Property(e => e.RequestServedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ApireportRequestDetail>(entity =>
            {
                entity.HasKey(e => e.RequestDetailId);

                entity.ToTable("APIReportRequestDetails", "Marking");

                entity.HasIndex(e => e.RequestId, "IDX_Marking_APIReportRequestDetails_RequestID");

                entity.Property(e => e.RequestDetailId).HasColumnName("RequestDetailID");
                entity.Property(e => e.RequestId).HasColumnName("RequestID");
                entity.Property(e => e.SummaryId).HasColumnName("SummaryID");
            });

            modelBuilder.Entity<ApireportRequestDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("APIReportRequestDetails_Archive", "Marking");

                entity.HasIndex(e => e.RequestId, "IDX_Marking_APIReportRequestDetails_RequestID_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.RequestDetailId).HasColumnName("RequestDetailID");
                entity.Property(e => e.RequestId).HasColumnName("RequestID");
                entity.Property(e => e.SummaryId).HasColumnName("SummaryID");
            });

            modelBuilder.Entity<ApireportRequestHistory>(entity =>
            {
                entity.HasKey(e => e.RequestHistoryId);

                entity.ToTable("APIReportRequestHistory", "Marking");

                entity.Property(e => e.RequestHistoryId).HasColumnName("RequestHistoryID");
                entity.Property(e => e.FileName).HasMaxLength(250);
                entity.Property(e => e.FilePath).HasMaxLength(2000);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.RequestDate).HasColumnType("datetime");
                entity.Property(e => e.RequestId).HasColumnName("RequestID");
            });

            modelBuilder.Entity<AppSetting>(entity =>
            {
                entity.ToTable("AppSettings", "Marking");

                entity.HasIndex(e => new { e.EntityId, e.EntityType, e.AppSettingKeyId, e.Isdeleted }, "IDX_Marking_AppSettings_EntityId_EntityType_AppSettingKeyID_Isdeleted").HasFillFactor(95);

                entity.HasIndex(e => new { e.AppSettingKeyId, e.Isdeleted, e.AppsettingGroupId }, "IX_AppSettings_AppSettingKeyID_Isdeleted_AppsettingGroupID").HasFillFactor(95);

                entity.HasIndex(e => new { e.AppSettingKeyId, e.ProjectId, e.Isdeleted, e.AppsettingGroupId }, "IX_AppSettings_AppSettingKeyID_ProjectID_Isdeleted_AppsettingGroupID").HasFillFactor(95);

                entity.Property(e => e.AppSettingId).HasColumnName("AppSettingID");
                entity.Property(e => e.AppSettingKeyId).HasColumnName("AppSettingKeyID");
                entity.Property(e => e.AppsettingGroupId).HasColumnName("AppsettingGroupID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DefaultValue).HasMaxLength(500);
                entity.Property(e => e.EntityId).HasColumnName("EntityID");
                entity.Property(e => e.EntityType).HasComment("1-->Project, 2-->QIG, 3-->User, 4-->Role, 5.Question");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ReferanceId).HasColumnName("ReferanceID");
                entity.Property(e => e.Value).HasMaxLength(500);
                entity.Property(e => e.ValueType).HasComment("1-->String, 2-->Integer, 3-->Float, 4-->XML, 5-->DateTime,6-->Bit,7-->Int,8-->BigInt");

                entity.HasOne(d => d.AppSettingKey).WithMany(p => p.AppSettings)
                    .HasForeignKey(d => d.AppSettingKeyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppSettings_AppSettingKey");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AppSettingCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_AppSettings_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.AppSettingModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_AppSettings_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.AppSettings)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_AppSettings_ProjectInfo");
            });

            modelBuilder.Entity<AppSettingKey>(entity =>
            {
                entity.ToTable("AppSettingKey", "Marking");

                entity.HasIndex(e => new { e.AppsettingKey1, e.IsDeleted }, "IDX_Marking_AppSettingKey_AppsettingKey_Isdeleted");

                entity.Property(e => e.AppsettingKeyId).HasColumnName("AppsettingKeyID");
                entity.Property(e => e.AppSettingType).HasComment("1-Project, 2- QIG");
                entity.Property(e => e.AppsettingKey1)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("AppsettingKey");
                entity.Property(e => e.AppsettingKeyName).HasMaxLength(50);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
                entity.Property(e => e.ParentAppsettingKeyId).HasColumnName("ParentAppsettingKeyID");
                entity.Property(e => e.SettingGroupId).HasColumnName("SettingGroupID");
            });

            modelBuilder.Entity<AppSettingsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("AppSettings_archive", "Marking");

                entity.HasIndex(e => new { e.EntityId, e.EntityType, e.AppSettingKeyId, e.Isdeleted }, "IDX_Marking_AppSettings_EntityId_EntityType_AppSettingKeyID_Isdeleted_Archive");

                entity.HasIndex(e => new { e.AppSettingKeyId, e.Isdeleted, e.AppsettingGroupId }, "IX_AppSettings_AppSettingKeyID_Isdeleted_AppsettingGroupID_Archive");

                entity.HasIndex(e => new { e.AppSettingKeyId, e.ProjectId, e.Isdeleted, e.AppsettingGroupId }, "IX_AppSettings_AppSettingKeyID_ProjectID_Isdeleted_AppsettingGroupID_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AppSettingId).HasColumnName("AppSettingID");
                entity.Property(e => e.AppSettingKeyId).HasColumnName("AppSettingKeyID");
                entity.Property(e => e.AppsettingGroupId).HasColumnName("AppsettingGroupID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.DefaultValue).HasMaxLength(500);
                entity.Property(e => e.EntityId).HasColumnName("EntityID");
                entity.Property(e => e.EntityType).HasComment("1-->Project, 2-->QIG, 3-->User, 4-->Role, 5.Question");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ReferanceId).HasColumnName("ReferanceID");
                entity.Property(e => e.Value).HasMaxLength(500);
            });

            modelBuilder.Entity<ApplicationWorkflow>(entity =>
            {
                entity.HasKey(e => e.WorkflowId);

                entity.ToTable("ApplicationWorkflow", "Marking");

                entity.Property(e => e.WorkflowId).HasColumnName("WorkflowID");
                entity.Property(e => e.ParentworkflowId).HasColumnName("ParentworkflowID");
                entity.Property(e => e.WorkflowCode)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.WorkflowName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<AppsettingGroup>(entity =>
            {
                entity.HasKey(e => e.SettingGroupId);

                entity.ToTable("AppsettingGroup", "Marking");

                entity.HasIndex(e => new { e.SettingGroupCode, e.IsDeleted }, "IDX_Marking_AppsettingGroup_SettingGroupCode_Isdeleted");

                entity.Property(e => e.SettingGroupId).HasColumnName("SettingGroupID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
                entity.Property(e => e.SettingGroupCode).HasMaxLength(50);
                entity.Property(e => e.SettingGroupName).HasMaxLength(500);
            });

            modelBuilder.Entity<CourseMovementValidation>(entity =>
            {
                entity.ToTable("CourseMovementValidation", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ExamCloseDate).HasColumnType("datetime");
                entity.Property(e => e.IsJobRunRequired).HasDefaultValue(true);
                entity.Property(e => e.IsMpimported).HasColumnName("IsMPImported");
                entity.Property(e => e.JobRunDate).HasColumnType("datetime");
                entity.Property(e => e.JobStatus).HasMaxLength(500);
                entity.Property(e => e.LoadedDataForEmarking).HasColumnType("datetime");
                entity.Property(e => e.MpimportedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("MPImportedDate");
                entity.Property(e => e.MpimportedStatus)
                    .HasMaxLength(500)
                    .HasColumnName("MPImportedStatus");
                entity.Property(e => e.ProjectCreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectCreationStatus).HasMaxLength(500);
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
                entity.Property(e => e.ScriptsImportedDate).HasColumnType("datetime");
                entity.Property(e => e.ScriptsImportedStatus).HasMaxLength(500);
                entity.Property(e => e.ValidationEndDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CourseMovementValidationHistory>(entity =>
            {
                entity.HasKey(e => e.HistoryId);

                entity.ToTable("CourseMovementValidationHistory", "Marking");

                entity.Property(e => e.HistoryId).HasColumnName("HistoryID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ExamCloseDate).HasColumnType("datetime");
                entity.Property(e => e.HistoryCreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.IsMpimported).HasColumnName("IsMPImported");
                entity.Property(e => e.JobRunDate).HasColumnType("datetime");
                entity.Property(e => e.JobStatus).HasMaxLength(500);
                entity.Property(e => e.LoadedDataForEmarking).HasColumnType("datetime");
                entity.Property(e => e.MpimportedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("MPImportedDate");
                entity.Property(e => e.MpimportedStatus)
                    .HasMaxLength(500)
                    .HasColumnName("MPImportedStatus");
                entity.Property(e => e.ProjectCreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectCreationStatus).HasMaxLength(500);
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
                entity.Property(e => e.ScriptsImportedDate).HasColumnType("datetime");
                entity.Property(e => e.ScriptsImportedStatus).HasMaxLength(500);
                entity.Property(e => e.ValidationEndDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<EntityMaster>(entity =>
            {
                entity.HasKey(e => e.EntityId);

                entity.ToTable("EntityMaster", "Marking");

                entity.HasIndex(e => e.EntityCode, "IDX_Marking_EntityMaster_EntityCode");

                entity.Property(e => e.EntityCode).HasMaxLength(50);
                entity.Property(e => e.EntityDescription).HasMaxLength(200);
                entity.Property(e => e.EntityName)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<ErrorHandling>(entity =>
            {
                entity.ToTable("ErrorHandling", "Marking");

                entity.Property(e => e.ErrorHandlingId).HasColumnName("ErrorHandlingID");
                entity.Property(e => e.DatabaseName).HasMaxLength(100);
                entity.Property(e => e.ErrorDetail)
                    .HasMaxLength(4000)
                    .IsUnicode(false);
                entity.Property(e => e.ErrorMessage)
                    .HasMaxLength(4000)
                    .IsUnicode(false);
                entity.Property(e => e.ErrorProcedure)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);
                entity.Property(e => e.ErrorState).HasDefaultValue((short)1);
                entity.Property(e => e.HostName)
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValue("");
                entity.Property(e => e.IsFixed).HasDefaultValue(false);
                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
                entity.Property(e => e.UserName)
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValue("");
            });

            modelBuilder.Entity<EventAudit>(entity =>
            {
                entity.ToTable("EventAudit", "Marking");

                entity.HasIndex(e => new { e.EventId, e.EntityId, e.ModuleId }, "IDX_Marking_EventAudit_EventID_EntityID_ModuleID").HasFillFactor(95);

                entity.Property(e => e.EventAuditId).HasColumnName("EventAuditID");
                entity.Property(e => e.EntityId).HasColumnName("EntityID");
                entity.Property(e => e.EventDateTime)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EventId).HasColumnName("EventID");
                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(255)
                    .HasColumnName("IPAddress");
                entity.Property(e => e.ModuleId).HasColumnName("ModuleID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.SessionId)
                    .HasMaxLength(100)
                    .HasColumnName("SessionID");
                entity.Property(e => e.Status).HasMaxLength(100);
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Entity).WithMany(p => p.EventAudits)
                    .HasForeignKey(d => d.EntityId)
                    .HasConstraintName("FK_EventAudit_EntityMaster");

                entity.HasOne(d => d.Module).WithMany(p => p.EventAudits)
                    .HasForeignKey(d => d.ModuleId)
                    .HasConstraintName("FK_EventAudit_ModuleMaster");

                entity.HasOne(d => d.ProjectUserRole).WithMany(p => p.EventAudits)
                    .HasForeignKey(d => d.ProjectUserRoleId)
                    .HasConstraintName("FK_EventAudit_ProjectUserRoleInfo");
            });

            modelBuilder.Entity<EventAuditArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("EventAudit_archive", "Marking");

                entity.HasIndex(e => new { e.EventId, e.EntityId, e.ModuleId }, "IDX_Marking_EventAudit_EventID_EntityID_ModuleID_Archive").HasFillFactor(95);

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.EntityId).HasColumnName("EntityID");
                entity.Property(e => e.EventAuditId).HasColumnName("EventAuditID");
                entity.Property(e => e.EventDateTime).HasColumnType("datetime");
                entity.Property(e => e.EventId).HasColumnName("EventID");
                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(255)
                    .HasColumnName("IPAddress");
                entity.Property(e => e.ModuleId).HasColumnName("ModuleID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.SessionId)
                    .HasMaxLength(100)
                    .HasColumnName("SessionID");
                entity.Property(e => e.Status).HasMaxLength(100);
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<EventMaster>(entity =>
            {
                entity.ToTable("EventMaster", "Marking");

                entity.HasIndex(e => e.EventCode, "IDX_Marking_EventMaster_EventCode");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.EventCode).HasMaxLength(50);
                entity.Property(e => e.EventType)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ExamLevel>(entity =>
            {
                entity.HasKey(e => e.ExamLevelId).HasName("PK__ExamLeve__00022181458D9965");

                entity.ToTable("ExamLevel", "Marking");

                entity.HasIndex(e => e.ExamLevelCode, "IDX_Marking_ExamLevel_ExamLevelCode");

                entity.Property(e => e.ExamLevelId).HasColumnName("ExamLevelID");
                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ExamLevelCode)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.ExamLevelName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ExamSeries>(entity =>
            {
                entity.HasKey(e => e.ExamSeriesId).HasName("PK__ExamSeri__E5DAEEA09046B150");

                entity.ToTable("ExamSeries", "Marking");

                entity.HasIndex(e => e.ExamSeriesCode, "IDX_Marking_ExamSeries_ExamSeriesCode");

                entity.Property(e => e.ExamSeriesId).HasColumnName("ExamSeriesID");
                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ExamSeriesCode)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.ExamSeriesName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ExamYear>(entity =>
            {
                entity.HasKey(e => e.YearId);

                entity.ToTable("ExamYear", "Marking");

                entity.HasIndex(e => new { e.Year, e.IsDeleted }, "IDX_Marking_ExamYear_Year_IsDeleted");

                entity.Property(e => e.YearId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("YearID");
                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<IndexTable>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__IndexTab__3214EC2752BCA3A0");

                entity.ToTable("IndexTable", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ObjectId).HasColumnName("ObjectID");
                entity.Property(e => e.SchemaName).HasMaxLength(250);
                entity.Property(e => e.TblName)
                    .HasMaxLength(250)
                    .HasColumnName("TblNAME");
            });

            modelBuilder.Entity<MarkSchemeFile>(entity =>
            {
                entity.HasKey(e => e.FileId);

                entity.ToTable("MarkSchemeFiles", "Marking");

                entity.Property(e => e.FileId).HasColumnName("FileID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.FileExtention).HasMaxLength(200);
                entity.Property(e => e.FileName).HasMaxLength(200);
                entity.Property(e => e.FileType).HasMaxLength(200);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MarkSchemeFileCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_MarkSchemeFiles_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.MarkSchemeFileModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_MarkSchemeFiles_ProjectUserRoleinfo1");

                entity.HasOne(d => d.ProjectMarkScheme).WithMany(p => p.MarkSchemeFiles)
                    .HasForeignKey(d => d.ProjectMarkSchemeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MarkSchemeFiles_ProjectMarkSchemeTemplate");
            });

            modelBuilder.Entity<MarkSchemeFilesArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("MarkSchemeFiles_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.FileExtention).HasMaxLength(200);
                entity.Property(e => e.FileId).HasColumnName("FileID");
                entity.Property(e => e.FileName).HasMaxLength(200);
                entity.Property(e => e.FileType).HasMaxLength(200);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<MarkingCenter>(entity =>
            {
                entity.HasKey(e => e.CenterId);

                entity.ToTable("MarkingCenter", "Marking");

                entity.Property(e => e.CenterId).HasColumnName("CenterID");
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.CenterCode).HasMaxLength(50);
                entity.Property(e => e.CenterName).HasMaxLength(100);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<MarkingScriptTimeTracking>(entity =>
            {
                entity.ToTable("MarkingScriptTimeTracking", "Marking");

                entity.HasIndex(e => e.UserScriptMarkingRefId, "IX_MarkingScriptTimeTracking_UserScriptMarkingRefID");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Action).HasComment("1 --> Submit,2 --> Save, 3 --> Cancel , 4 --> Close , 5 --> Navigate");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Mode).HasComment("1 --> View , 2 --> Edit");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
                entity.Property(e => e.WorkFlowStatusId).HasColumnName("WorkFlowStatusID");

                entity.HasOne(d => d.Project).WithMany(p => p.MarkingScriptTimeTrackings)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MarkingScriptTimeTracking_ProjectInfo");

                entity.HasOne(d => d.ProjectQuestion).WithMany(p => p.MarkingScriptTimeTrackings)
                    .HasForeignKey(d => d.ProjectQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MarkingScriptTimeTracking_ProjectQuestions");

                entity.HasOne(d => d.ProjectUserRole).WithMany(p => p.MarkingScriptTimeTrackings)
                    .HasForeignKey(d => d.ProjectUserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MarkingScriptTimeTracking_ProjectUserRoleinfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.MarkingScriptTimeTrackings)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MarkingScriptTimeTracking_ProjectQIG");

                entity.HasOne(d => d.UserScriptMarkingRef).WithMany(p => p.MarkingScriptTimeTrackings)
                    .HasForeignKey(d => d.UserScriptMarkingRefId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MarkingScriptTimeTracking_UserScriptMarkingDetails");

                entity.HasOne(d => d.WorkFlowStatus).WithMany(p => p.MarkingScriptTimeTrackings)
                    .HasForeignKey(d => d.WorkFlowStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MarkingScriptTimeTracking_WorkflowStatus");
            });

            modelBuilder.Entity<MarkingScriptTimeTrackingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("MarkingScriptTimeTracking_Archive", "Marking");

                entity.HasIndex(e => e.UserScriptMarkingRefId, "IX_MarkingScriptTimeTracking_UserScriptMarkingRefID_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Mode).HasComment("1 --> View , 2 --> Edit");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
                entity.Property(e => e.WorkFlowStatusId).HasColumnName("WorkFlowStatusID");
            });

            modelBuilder.Entity<ModeOfAssessment>(entity =>
            {
                entity.HasKey(e => e.Moaid).HasName("PK__ModeOfAs__0F6BD9D1AAF7A321");

                entity.ToTable("ModeOfAssessment", "Marking");

                entity.Property(e => e.Moaid).HasColumnName("MOAID");
                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Moacode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("MOACode");
                entity.Property(e => e.Moaname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("MOAName");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ModuleMaster>(entity =>
            {
                entity.HasKey(e => e.ModuleId);

                entity.ToTable("ModuleMaster", "Marking");

                entity.HasIndex(e => e.ModuleCode, "IDX_Marking_ModuleMaster_ModuleCode");

                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.ModuleCode)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.ModuleName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<MpstandardizationQueRespMarkingDetail>(entity =>
            {
                entity.ToTable("MPStandardizationQueRespMarkingDetails", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AwardedMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DefenetiveMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionResponceId).HasColumnName("ProjectQuestionResponceID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.StandardizationScriptMarkingRefId).HasColumnName("StandardizationScriptMarkingRefID");
                entity.Property(e => e.TotalMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MpstandardizationQueRespMarkingDetailCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_MPStandardizationQueRespMarkingDetails_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.MpstandardizationQueRespMarkingDetails)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationQueRespMarkingDetails_ProjectInfo");

                entity.HasOne(d => d.ProjectQuestionResponce).WithMany(p => p.MpstandardizationQueRespMarkingDetails)
                    .HasForeignKey(d => d.ProjectQuestionResponceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationQueRespMarkingDetails_ProjectUserQuestionResponse");

                entity.HasOne(d => d.ProjectUserRole).WithMany(p => p.MpstandardizationQueRespMarkingDetailProjectUserRoles)
                    .HasForeignKey(d => d.ProjectUserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationQueRespMarkingDetails_ProjectUserRoleinfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.MpstandardizationQueRespMarkingDetails)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationQueRespMarkingDetails_ProjectQIG");

                entity.HasOne(d => d.Script).WithMany(p => p.MpstandardizationQueRespMarkingDetails)
                    .HasForeignKey(d => d.ScriptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationQueRespMarkingDetails_ProjectUserScript");

                entity.HasOne(d => d.StandardizationScriptMarkingRef).WithMany(p => p.MpstandardizationQueRespMarkingDetails)
                    .HasForeignKey(d => d.StandardizationScriptMarkingRefId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationQueRespMarkingDetails_MPStandardizationScriptMarkingDetails");

                entity.HasOne(d => d.WorkflowStatus).WithMany(p => p.MpstandardizationQueRespMarkingDetails)
                    .HasForeignKey(d => d.WorkflowStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationQueRespMarkingDetails_WorkflowStatus");
            });

            modelBuilder.Entity<MpstandardizationQueRespMarkingDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("MPStandardizationQueRespMarkingDetails_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AwardedMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.DefenetiveMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionResponceId).HasColumnName("ProjectQuestionResponceID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.StandardizationScriptMarkingRefId).HasColumnName("StandardizationScriptMarkingRefID");
                entity.Property(e => e.TotalMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");
            });

            modelBuilder.Entity<MpstandardizationSchedule>(entity =>
            {
                entity.HasKey(e => e.StandardizationScheduleId);

                entity.ToTable("MPStandardizationSchedule", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ProjectUserRoleId, e.WorkflowStatusId }, "IDX_Marking_MPStandardizationSchedule_ProjectID_QIGID_ProjectUserRoleID_WorkFlowStatusID");

                entity.Property(e => e.StandardizationScheduleId).HasColumnName("StandardizationScheduleID");
                entity.Property(e => e.AssignedDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.QualifyingAssessmentId).HasColumnName("QualifyingAssessmentID");
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");

                entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.MpstandardizationScheduleAssignedByNavigations)
                    .HasForeignKey(d => d.AssignedBy)
                    .HasConstraintName("FK_MPStandardizationSchedule_ProjectUserRoleinfo1");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MpstandardizationScheduleCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_MPStandardizationSchedule_ProjectUserRoleinfo2");

                entity.HasOne(d => d.Project).WithMany(p => p.MpstandardizationSchedules)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationSchedule_ProjectInfo");

                entity.HasOne(d => d.ProjectUserRole).WithMany(p => p.MpstandardizationScheduleProjectUserRoles)
                    .HasForeignKey(d => d.ProjectUserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationSchedule_ProjectUserRoleinfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.MpstandardizationSchedules)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationSchedule_ProjectQIG");

                entity.HasOne(d => d.QualifyingAssessment).WithMany(p => p.MpstandardizationSchedules)
                    .HasForeignKey(d => d.QualifyingAssessmentId)
                    .HasConstraintName("FK_MPStandardizationSchedule_StandardizationQualifyingAssessment");

                entity.HasOne(d => d.WorkflowStatus).WithMany(p => p.MpstandardizationSchedules)
                    .HasForeignKey(d => d.WorkflowStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationSchedule_WorkflowStatus");
            });

            modelBuilder.Entity<MpstandardizationScheduleArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("MPStandardizationSchedule_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ProjectUserRoleId, e.WorkflowStatusId }, "IDX_Marking_MPStandardizationSchedule_ProjectID_QIGID_ProjectUserRoleID_WorkFlowStatusID_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AssignedDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.QualifyingAssessmentId).HasColumnName("QualifyingAssessmentID");
                entity.Property(e => e.StandardizationScheduleId).HasColumnName("StandardizationScheduleID");
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");
            });

            modelBuilder.Entity<MpstandardizationScheduleScriptDetail>(entity =>
            {
                entity.HasKey(e => e.StandardizationScheduleScriptDetailId);

                entity.ToTable("MPStandardizationScheduleScriptDetails", "Marking");

                entity.HasIndex(e => new { e.StandardizationScheduleId, e.IsCompleted }, "IDX_Marking_MPStandardizationScheduleScriptDetails_StandardizationScheduleID_IsCompleted");

                entity.Property(e => e.StandardizationScheduleScriptDetailId).HasColumnName("StandardizationScheduleScriptDetailID");
                entity.Property(e => e.CategorizationVersion).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ScriptCategorizationPoolId).HasColumnName("ScriptCategorizationPoolID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.StandardizationScheduleId).HasColumnName("StandardizationScheduleID");
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.SubmittedDate).HasColumnType("datetime");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");

                entity.HasOne(d => d.Script).WithMany(p => p.MpstandardizationScheduleScriptDetails)
                    .HasForeignKey(d => d.ScriptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationScheduleScriptDetails_ProjectUserScript");

                entity.HasOne(d => d.StandardizationSchedule).WithMany(p => p.MpstandardizationScheduleScriptDetails)
                    .HasForeignKey(d => d.StandardizationScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationScheduleScriptDetails_MPStandardizationSchedule");

                entity.HasOne(d => d.SubmittedByNavigation).WithMany(p => p.MpstandardizationScheduleScriptDetails)
                    .HasForeignKey(d => d.SubmittedBy)
                    .HasConstraintName("FK_MPStandardizationScheduleScriptDetails_ProjectUserRoleinfo");

                entity.HasOne(d => d.WorkflowStatus).WithMany(p => p.MpstandardizationScheduleScriptDetails)
                    .HasForeignKey(d => d.WorkflowStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationScheduleScriptDetails_WorkflowStatus");
            });

            modelBuilder.Entity<MpstandardizationScheduleScriptDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("MPStandardizationScheduleScriptDetails_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CategorizationVersion).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ScriptCategorizationPoolId).HasColumnName("ScriptCategorizationPoolID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.StandardizationScheduleId).HasColumnName("StandardizationScheduleID");
                entity.Property(e => e.StandardizationScheduleScriptDetailId).HasColumnName("StandardizationScheduleScriptDetailID");
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.SubmittedDate).HasColumnType("datetime");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");
            });

            modelBuilder.Entity<MpstandardizationScriptMarkingDetail>(entity =>
            {
                entity.HasKey(e => e.StandardizationScriptMarkingId);

                entity.ToTable("MPStandardizationScriptMarkingDetails", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ProjectUserRoleId, e.ScriptId, e.WorkflowStatusId }, "IDX_Marking_MPStandardizationScriptMarkingDetails_ProjectID_QIGID_ProjectUserRoleID_ScriptID_WorkflowStatusID");

                entity.Property(e => e.StandardizationScriptMarkingId).HasColumnName("StandardizationScriptMarkingID");
                entity.Property(e => e.AssignedDate).HasColumnType("datetime");
                entity.Property(e => e.AwardedTotalMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DefenitiveScriptMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.StandardizationScheduleId).HasColumnName("StandardizationScheduleID");
                entity.Property(e => e.TotalMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");

                entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.MpstandardizationScriptMarkingDetailAssignedByNavigations)
                    .HasForeignKey(d => d.AssignedBy)
                    .HasConstraintName("FK_MPStandardizationScriptMarkingDetails_ProjectUserRoleinfo1");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MpstandardizationScriptMarkingDetailCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_MPStandardizationScriptMarkingDetails_ProjectUserRoleinfo2");

                entity.HasOne(d => d.Project).WithMany(p => p.MpstandardizationScriptMarkingDetails)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationScriptMarkingDetails_ProjectInfo");

                entity.HasOne(d => d.ProjectUserRole).WithMany(p => p.MpstandardizationScriptMarkingDetailProjectUserRoles)
                    .HasForeignKey(d => d.ProjectUserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationScriptMarkingDetails_ProjectUserRoleinfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.MpstandardizationScriptMarkingDetails)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationScriptMarkingDetails_ProjectQIG");

                entity.HasOne(d => d.Script).WithMany(p => p.MpstandardizationScriptMarkingDetails)
                    .HasForeignKey(d => d.ScriptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationScriptMarkingDetails_ProjectUserScript");

                entity.HasOne(d => d.StandardizationSchedule).WithMany(p => p.MpstandardizationScriptMarkingDetails)
                    .HasForeignKey(d => d.StandardizationScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationScriptMarkingDetails_MPStandardizationSchedule");

                entity.HasOne(d => d.UserScriptMarkingRef).WithMany(p => p.MpstandardizationScriptMarkingDetails)
                    .HasForeignKey(d => d.UserScriptMarkingRefId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationScriptMarkingDetails_UserScriptMarkingDetails");

                entity.HasOne(d => d.WorkflowStatus).WithMany(p => p.MpstandardizationScriptMarkingDetails)
                    .HasForeignKey(d => d.WorkflowStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationScriptMarkingDetails_WorkflowStatus");
            });

            modelBuilder.Entity<MpstandardizationScriptMarkingDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("MPStandardizationScriptMarkingDetails_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AssignedDate).HasColumnType("datetime");
                entity.Property(e => e.AwardedTotalMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.DefenitiveScriptMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.StandardizationScheduleId).HasColumnName("StandardizationScheduleID");
                entity.Property(e => e.StandardizationScriptMarkingId).HasColumnName("StandardizationScriptMarkingID");
                entity.Property(e => e.TotalMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");
            });

            modelBuilder.Entity<MpstandardizationSummary>(entity =>
            {
                entity.HasKey(e => e.StandardizationSummaryId);

                entity.ToTable("MPStandardizationSummary", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ProjectUserRoleId }, "IDX_Marking_MPStandardizationSummary_ProjectID_QIGID_ProjectUserRoleID");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ProjectUserRoleId, e.IsQualifiyingAssementDone, e.IsDeleted }, "IDX_Marking_MPStandardizationSummary_ProjectID_QIGID_ProjectUserRoleID_IsQualifiyingAssementDone_IsDeleted");

                entity.Property(e => e.StandardizationSummaryId).HasColumnName("StandardizationSummaryID");
                entity.Property(e => e.ActionDate).HasColumnType("datetime");
                entity.Property(e => e.ApprovalStatus).HasComment("0 --> Waiting for submission, 1--> Pending, 2--> Rejected, 3 --> Additional standardization Scripts Given,  4 --> Approved 5--> Suspended");
                entity.Property(e => e.ApprovalType).HasComment("1 --> Manual 2 --> Automatic");
                entity.Property(e => e.NoOfAdditionalStdscriptOutOfTolerance).HasColumnName("NoOfAdditionalSTDScriptOutOfTolerance");
                entity.Property(e => e.NoOfAdditionalStdscriptWithinTolerance).HasColumnName("NoOfAdditionalSTDScriptWithinTolerance");
                entity.Property(e => e.NoOfQascriptOutOfTolerance).HasColumnName("NoOfQAScriptOutOfTolerance");
                entity.Property(e => e.NoOfQascriptWithinTolerance).HasColumnName("NoOfQAScriptWithinTolerance");
                entity.Property(e => e.NoOfScriptAdditionalStd).HasColumnName("NoOfScriptAdditionalSTD");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");

                entity.HasOne(d => d.ActionByNavigation).WithMany(p => p.MpstandardizationSummaryActionByNavigations)
                    .HasForeignKey(d => d.ActionBy)
                    .HasConstraintName("FK_MPStandardizationSummary_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.MpstandardizationSummaries)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationSummary_ProjectInfo");

                entity.HasOne(d => d.ProjectUserRole).WithMany(p => p.MpstandardizationSummaryProjectUserRoles)
                    .HasForeignKey(d => d.ProjectUserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationSummary_ProjectUserRoleinfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.MpstandardizationSummaries)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MPStandardizationSummary_ProjectQIG");
            });

            modelBuilder.Entity<MpstandardizationSummaryArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("MPStandardizationSummary_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.ActionDate).HasColumnType("datetime");
                entity.Property(e => e.ApprovalType).HasComment("1 --> Manual 2 --> Automatic");
                entity.Property(e => e.NoOfAdditionalStdscriptOutOfTolerance).HasColumnName("NoOfAdditionalSTDScriptOutOfTolerance");
                entity.Property(e => e.NoOfAdditionalStdscriptWithinTolerance).HasColumnName("NoOfAdditionalSTDScriptWithinTolerance");
                entity.Property(e => e.NoOfQascriptOutOfTolerance).HasColumnName("NoOfQAScriptOutOfTolerance");
                entity.Property(e => e.NoOfQascriptWithinTolerance).HasColumnName("NoOfQAScriptWithinTolerance");
                entity.Property(e => e.NoOfScriptAdditionalStd).HasColumnName("NoOfScriptAdditionalSTD");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.StandardizationSummaryId).HasColumnName("StandardizationSummaryID");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization", "Marking");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
                entity.Property(e => e.OrganizationCode).HasMaxLength(50);
                entity.Property(e => e.OrganizationName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<PassPharse>(entity =>
            {
                entity.ToTable("PassPharse", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.PassPharseCode)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Privilege>(entity =>
            {
                entity.ToTable("Privileges", "Marking");

                entity.Property(e => e.PrivilegeId).HasColumnName("PrivilegeID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsKpprivilege).HasColumnName("IsKPPrivilege");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ParentPrivilegeId).HasColumnName("ParentPrivilegeID");
                entity.Property(e => e.PrivilegeCode)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.PrivilegeDescription).HasMaxLength(2000);
                entity.Property(e => e.PrivilegeLevel).HasMaxLength(100);
                entity.Property(e => e.PrivilegeName)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.PrivilegeType).HasComment("1--> Page 2--> Module 3--> Action 4--> Quick Links");
                entity.Property(e => e.PrivilegeUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("PrivilegeURL");

                entity.HasOne(d => d.ParentPrivilege).WithMany(p => p.InverseParentPrivilege)
                    .HasForeignKey(d => d.ParentPrivilegeId)
                    .HasConstraintName("FK_Privileges_Privileges");
            });

            modelBuilder.Entity<ProjectArchiveRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId);

                entity.ToTable("ProjectArchiveRequest", "Marking");

                entity.Property(e => e.RequestId).HasColumnName("RequestID");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.RequestStatus).HasComment("0 --> In Progress,1 --> Archive Completed  waiting for user confirmation,2 --> User Confirmed,3 --> Archive Completed and Removed the live data,4 --> Error Found");
                entity.Property(e => e.RequestStatusDate).HasColumnType("datetime");
                entity.Property(e => e.RequestedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectArchiveRequestDetail>(entity =>
            {
                entity.ToTable("ProjectArchiveRequestDetails", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ProjectArchiveRequestId).HasColumnName("ProjectArchiveRequestID");
                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.ProjectArchiveRequest).WithMany(p => p.ProjectArchiveRequestDetails)
                    .HasForeignKey(d => d.ProjectArchiveRequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectArchiveRequestDetails_ProjectArchiveRequest");
            });

            modelBuilder.Entity<ProjectCandidateWithdraw>(entity =>
            {
                entity.ToTable("ProjectCandidateWithdraw", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.IndexNumber)
                    .IsRequired()
                    .HasMaxLength(250);
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.UnWithDrawDate).HasColumnType("datetime");
                entity.Property(e => e.WithDrawDate).HasColumnType("datetime");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectCandidateWithdraws)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectCandidateWithdraw_ProjectInfo");

                entity.HasOne(d => d.UnWithDrawByNavigation).WithMany(p => p.ProjectCandidateWithdrawUnWithDrawByNavigations)
                    .HasForeignKey(d => d.UnWithDrawBy)
                    .HasConstraintName("FK_ProjectCandidateWithdraw_ProjectUserRoleinfo");

                entity.HasOne(d => d.WithDrawByNavigation).WithMany(p => p.ProjectCandidateWithdrawWithDrawByNavigations).HasForeignKey(d => d.WithDrawBy);
            });

            modelBuilder.Entity<ProjectCandidateWithdrawArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectCandidateWithdraw_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.IndexNumber)
                    .IsRequired()
                    .HasMaxLength(250);
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.UnWithDrawDate).HasColumnType("datetime");
                entity.Property(e => e.WithDrawDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectCenter>(entity =>
            {
                entity.ToTable("ProjectCenters", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.IsDeleted }, "IX_ProjectCenters_ProjectID_IsDeleted");

                entity.Property(e => e.ProjectCenterId).HasColumnName("ProjectCenterID");
                entity.Property(e => e.CenterCode).HasMaxLength(100);
                entity.Property(e => e.CenterId).HasColumnName("CenterID");
                entity.Property(e => e.CenterName).HasMaxLength(200);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsSelectedForRecommendation).HasDefaultValue(false);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.RecommendationDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectCenterCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectCenters_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectCenterModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectCenters_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectCenters)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectCenters_ProjectInfo");
            });

            modelBuilder.Entity<ProjectCenterSchoolMapping>(entity =>
            {
                entity.ToTable("ProjectCenterSchoolMapping", "Marking");

                entity.HasIndex(e => new { e.ProjectCenterId, e.SchoolId, e.IsDeleted }, "IDX_Marking_ProjectCenterSchoolMapping_ProjectCenterID_SchoolID_IsDeleted");

                entity.HasIndex(e => new { e.SchoolId, e.IsDeleted }, "IDX_Marking_ProjectCenterSchoolMapping_SchoolID_IsDeleted");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectCenterId).HasColumnName("ProjectCenterID");
                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectCenterSchoolMappingCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectCenterSchoolMapping_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectCenterSchoolMappingModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectCenterSchoolMapping_ProjectUserRoleinfo1");

                entity.HasOne(d => d.ProjectCenter).WithMany(p => p.ProjectCenterSchoolMappings)
                    .HasForeignKey(d => d.ProjectCenterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectCenterSchoolMapping_ProjectCenters");

                entity.HasOne(d => d.School).WithMany(p => p.ProjectCenterSchoolMappings)
                    .HasForeignKey(d => d.SchoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectCenterSchoolMapping_SchoolInfo");
            });

            modelBuilder.Entity<ProjectCenterSchoolMappingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectCenterSchoolMapping_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectCenterId).HasColumnName("ProjectCenterID");
                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");
            });

            modelBuilder.Entity<ProjectCentersArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectCenters_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CenterCode).HasMaxLength(100);
                entity.Property(e => e.CenterId).HasColumnName("CenterID");
                entity.Property(e => e.CenterName).HasMaxLength(200);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectCenterId).HasColumnName("ProjectCenterID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.RecommendationDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectCreationJobHistory>(entity =>
            {
                entity.HasKey(e => e.JobId);

                entity.ToTable("ProjectCreationJobHistory", "Marking");

                entity.Property(e => e.JobId).HasColumnName("JobID");
                entity.Property(e => e.JobCode).HasMaxLength(400);
                entity.Property(e => e.JobName).HasMaxLength(400);
                entity.Property(e => e.JobrunDateTime)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Remarks).HasMaxLength(1000);
                entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            });

            modelBuilder.Entity<ProjectFile>(entity =>
            {
                entity.HasKey(e => e.FileId);

                entity.ToTable("ProjectFiles", "Marking");

                entity.Property(e => e.FileId).HasColumnName("FileID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EntityType).HasComment("0--> TEMP Files , 1 --> Markscheme");
                entity.Property(e => e.FileExtention).HasMaxLength(200);
                entity.Property(e => e.FileName).HasMaxLength(200);
                entity.Property(e => e.FileType).HasMaxLength(200);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ParentFileId).HasColumnName("ParentFileID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Repository).HasComment("0--> Local Repo  1--> AWS  2 --> Azure ");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectFileCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectFiles_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectFileModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectFiles_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectFiles)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjectFiles_ProjectInfo");
            });

            modelBuilder.Entity<ProjectFilesArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectFiles_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.EntityType).HasComment("0--> TEMP Files , 1 --> Markscheme");
                entity.Property(e => e.FileExtention).HasMaxLength(200);
                entity.Property(e => e.FileId).HasColumnName("FileID");
                entity.Property(e => e.FileName).HasMaxLength(200);
                entity.Property(e => e.FileType).HasMaxLength(200);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ParentFileId).HasColumnName("ParentFileID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Repository).HasComment("0--> Local Repo  1--> AWS  2 --> Azure ");
            });

            modelBuilder.Entity<ProjectInfo>(entity =>
            {
                entity.HasKey(e => e.ProjectId).HasName("PK__ProjectI__761ABED0AC3CF607");

                entity.ToTable("ProjectInfo", "Marking");

                entity.HasIndex(e => new { e.ProjectCode, e.IsDeleted }, "IDX_Marking_ProjectInfo_ProjectCode_IsDeleted");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ArchiveDate).HasColumnType("datetime");
                entity.Property(e => e.AssessmentId).HasColumnName("AssessmentID");
                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ExamLevelId).HasColumnName("ExamLevelID");
                entity.Property(e => e.ExamseriesId).HasColumnName("ExamseriesID");
                entity.Property(e => e.IsQuestionXmlexist).HasColumnName("IsQuestionXMLExist");
                entity.Property(e => e.Moa).HasColumnName("MOA");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
                entity.Property(e => e.PaperId).HasColumnName("PaperID");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.ProjectCode)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.ProjectEndDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectInfo1)
                    .HasMaxLength(2000)
                    .HasColumnName("ProjectInfo");
                entity.Property(e => e.ProjectName).HasMaxLength(200);
                entity.Property(e => e.ProjectStartDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectStatus).HasComment("0 --> Not Started, 1 --> In- Progress, 2 --> Completed, 3 --> Closed, 4 --> Reopened");
                entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

                entity.HasOne(d => d.ExamLevel).WithMany(p => p.ProjectInfos)
                    .HasForeignKey(d => d.ExamLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectInfo_ExamLevel");

                entity.HasOne(d => d.Examseries).WithMany(p => p.ProjectInfos)
                    .HasForeignKey(d => d.ExamseriesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectInfo_ExamSeries");

                entity.HasOne(d => d.MoaNavigation).WithMany(p => p.ProjectInfos)
                    .HasForeignKey(d => d.Moa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectInfo_ModeOfAssessment");

                entity.HasOne(d => d.Paper).WithMany(p => p.ProjectInfos)
                    .HasForeignKey(d => d.PaperId)
                    .HasConstraintName("FK_ProjectInfo_SubjectPaperInfo");

                entity.HasOne(d => d.Subject).WithMany(p => p.ProjectInfos)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("FK_ProjectInfo_subjectinfo");
            });

            modelBuilder.Entity<ProjectMarkSchemeBandDetail>(entity =>
            {
                entity.HasKey(e => e.BandId);

                entity.ToTable("ProjectMarkSchemeBandDetails", "Marking");

                entity.HasIndex(e => new { e.ProjectMarkSchemeId, e.IsDeleted }, "IDX_Marking_ProjectMarkSchemeBandDetails_ProjectMarkSchemeId_IsDeleted");

                entity.Property(e => e.BandCode).HasMaxLength(50);
                entity.Property(e => e.BandFrom).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.BandName).HasMaxLength(200);
                entity.Property(e => e.BandTo).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectMarkSchemeBandDetailCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectMarkSchemeBandDetails_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectMarkSchemeBandDetailModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectMarkSchemeBandDetails_ProjectUserRoleinfo1");

                entity.HasOne(d => d.ProjectMarkScheme).WithMany(p => p.ProjectMarkSchemeBandDetails)
                    .HasForeignKey(d => d.ProjectMarkSchemeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectMarkSchemeBandDetails_ProjectMarkSchemeTemplate");
            });

            modelBuilder.Entity<ProjectMarkSchemeBandDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectMarkSchemeBandDetails_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.BandCode).HasMaxLength(50);
                entity.Property(e => e.BandFrom).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.BandName).HasMaxLength(200);
                entity.Property(e => e.BandTo).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectMarkSchemeQuestion>(entity =>
            {
                entity.ToTable("ProjectMarkSchemeQuestion", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.ProjectQuestionId, e.ProjectMarkSchemeId, e.ScoreComponentId, e.Isdeleted }, "IDX_Marking_ProjectMarkSchemeQuestion_ProjectID_ProjectQuestionID_ProjectMarkSchemeID_ScoreComponentID_Isdeleted");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectMarkSchemeId).HasColumnName("ProjectMarkSchemeID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.ScoreComponentId).HasColumnName("ScoreComponentID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectMarkSchemeQuestionCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectMarkSchemeQuestion_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectMarkSchemeQuestionModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectMarkSchemeQuestion_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectMarkSchemeQuestions)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectMarkSchemeQuestion_ProjectInfo");

                entity.HasOne(d => d.ProjectMarkScheme).WithMany(p => p.ProjectMarkSchemeQuestions)
                    .HasForeignKey(d => d.ProjectMarkSchemeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectMarkSchemeQuestion_ProjectMarkSchemeTemplate");

                entity.HasOne(d => d.ProjectQuestion).WithMany(p => p.ProjectMarkSchemeQuestions)
                    .HasForeignKey(d => d.ProjectQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectMarkSchemeQuestion_ProjectQuestions");
            });

            modelBuilder.Entity<ProjectMarkSchemeQuestionArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectMarkSchemeQuestion_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectMarkSchemeId).HasColumnName("ProjectMarkSchemeID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.ScoreComponentId).HasColumnName("ScoreComponentID");
            });

            modelBuilder.Entity<ProjectMarkSchemeTemplate>(entity =>
            {
                entity.HasKey(e => e.ProjectMarkSchemeId);

                entity.ToTable("ProjectMarkSchemeTemplate", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.IsDeleted }, "IDX_Marking_ProjectMarkSchemeTemplate_ProjectID_Isdeleted");

                entity.HasIndex(e => new { e.ProjectId, e.SchemeCode, e.MarkingSchemeType, e.IsDeleted }, "IDX_Marking_ProjectMarkSchemeTemplate_ProjectID_SchemeCode_MarkingSchemeType_Isdeleted");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.MarkingSchemeType).HasComment("1--> Question Level 2--> Score Component Level");
                entity.Property(e => e.Marks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.SchemeCode).HasMaxLength(50);
                entity.Property(e => e.SchemeName).HasMaxLength(200);

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectMarkSchemeTemplateCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectMarkSchemeTemplate_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectMarkSchemeTemplateModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectMarkSchemeTemplate_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectMarkSchemeTemplates)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjectMarkSchemeTemplate_ProjectInfo");
            });

            modelBuilder.Entity<ProjectMarkSchemeTemplateArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectMarkSchemeTemplate_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.SchemeCode, e.MarkingSchemeType, e.IsDeleted }, "IDX_Marking_ProjectMarkSchemeTemplate_ProjectID_SchemeCode_MarkingSchemeType_Isdeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.MarkingSchemeType).HasComment("1--> Question Level 2--> Score Component Level");
                entity.Property(e => e.Marks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.SchemeCode).HasMaxLength(50);
                entity.Property(e => e.SchemeName).HasMaxLength(200);
            });

            modelBuilder.Entity<ProjectQig>(entity =>
            {
                entity.ToTable("ProjectQIG", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.IsDeleted }, "IDX_Marking_ProjectQIG_ProjectID_IsDeleted");

                entity.HasIndex(e => new { e.ProjectId, e.Qigcode, e.IsDeleted }, "IDX_Marking_ProjectQIG_ProjectID_QIGCode_Isdeleted");

                entity.Property(e => e.ProjectQigid).HasColumnName("ProjectQIGID");
                entity.Property(e => e.ActionDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.NoofMandatoryQuestion).HasColumnName("NOOfMandatoryQuestion");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigcode)
                    .HasMaxLength(50)
                    .HasColumnName("QIGCode");
                entity.Property(e => e.Qigname)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("QIGName");
                entity.Property(e => e.Qigtype)
                    .HasComment("1--->MCQ,  2--->Composition , 3--> Non-Composition")
                    .HasColumnName("QIGType");
                entity.Property(e => e.ResponseProcessingType).HasComment("1 --> Partial Manual Marking, 2 --> Complete Manual Marking ");
                entity.Property(e => e.TotalMarks).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.ActionByNavigation).WithMany(p => p.ProjectQigActionByNavigations)
                    .HasForeignKey(d => d.ActionBy)
                    .HasConstraintName("FK_ProjectQIG_ProjectUserRoleinfo2");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectQigCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectQIG_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectQigModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectQIG_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectQigs)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjectQIG_ProjectInfo");
            });

            modelBuilder.Entity<ProjectQigArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectQIG_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.IsDeleted }, "IDX_Marking_ProjectQIG_ProjectID_IsDeleted_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigcode, e.IsDeleted }, "IDX_Marking_ProjectQIG_ProjectID_QIGCode_Isdeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.ActionDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.NoofMandatoryQuestion).HasColumnName("NOOfMandatoryQuestion");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQigid).HasColumnName("ProjectQIGID");
                entity.Property(e => e.Qigcode)
                    .HasMaxLength(50)
                    .HasColumnName("QIGCode");
                entity.Property(e => e.Qigname)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("QIGName");
                entity.Property(e => e.Qigtype)
                    .HasComment("1--->MCQ,  2--->Composition , 3--> Non-Composition")
                    .HasColumnName("QIGType");
                entity.Property(e => e.TotalMarks).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<ProjectQigcenterMapping>(entity =>
            {
                entity.HasKey(e => e.ProjectQigcenterId);

                entity.ToTable("ProjectQIGCenterMapping", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ProjectCenterId, e.IsDeleted }, "IDX_Marking_ProjectQIGCenterMapping_ProjectID_QIGID_ProjectCenterID_IsDeleted");

                entity.Property(e => e.ProjectQigcenterId).HasColumnName("ProjectQIGCenterID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectCenterId).HasColumnName("ProjectCenterID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.RecommendedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectQigcenterMappingCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectQIGCenterMapping_ProjectUserRoleinfo1");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectQigcenterMappingModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectQIGCenterMapping_ProjectUserRoleinfo2");

                entity.HasOne(d => d.ProjectCenter).WithMany(p => p.ProjectQigcenterMappings)
                    .HasForeignKey(d => d.ProjectCenterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQIGCenterMapping_ProjectCenters");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectQigcenterMappings)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQIGCenterMapping_ProjectInfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.ProjectQigcenterMappings)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQIGCenterMapping_ProjectQIG");

                entity.HasOne(d => d.RecommendedByNavigation).WithMany(p => p.ProjectQigcenterMappingRecommendedByNavigations)
                    .HasForeignKey(d => d.RecommendedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQIGCenterMapping_ProjectUserRoleinfo");
            });

            modelBuilder.Entity<ProjectQigcenterMappingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectQIGCenterMapping_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ProjectCenterId, e.IsDeleted }, "IDX_Marking_ProjectQIGCenterMapping_ProjectID_QIGID_ProjectCenterID_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectCenterId).HasColumnName("ProjectCenterID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQigcenterId).HasColumnName("ProjectQIGCenterID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.RecommendedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectQigquestion>(entity =>
            {
                entity.HasKey(e => e.QigquestionId);

                entity.ToTable("ProjectQIGQuestions", "Marking");

                entity.HasIndex(e => new { e.Qigid, e.ProjectQuestionId, e.IsDeleted }, "IDX_Marking_ProjectQIGQuestions_QIGID_ProjectQuestionID_IsDeleted");

                entity.Property(e => e.QigquestionId).HasColumnName("QIGQuestionID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectQigquestionCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectQIGQuestions_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectQigquestionModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectQIGQuestions_ProjectUserRoleinfo1");

                entity.HasOne(d => d.ProjectQuestion).WithMany(p => p.ProjectQigquestions)
                    .HasForeignKey(d => d.ProjectQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQIGQuestions_ProjectQuestions");

                entity.HasOne(d => d.Qig).WithMany(p => p.ProjectQigquestions)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQIGQuestions_ProjectQIG");
            });

            modelBuilder.Entity<ProjectQigquestionsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectQIGQuestions_Archive", "Marking");

                entity.HasIndex(e => new { e.Qigid, e.ProjectQuestionId, e.IsDeleted }, "IDX_Marking_ProjectQIGQuestions_QIGID_ProjectQuestionID_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.QigquestionId).HasColumnName("QIGQuestionID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            });

            modelBuilder.Entity<ProjectQigreset>(entity =>
            {
                entity.ToTable("ProjectQIGReset", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AuthenticateDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.ResetDate).HasColumnType("datetime");

                entity.HasOne(d => d.AuthenticateByNavigation).WithMany(p => p.ProjectQigresetAuthenticateByNavigations)
                    .HasForeignKey(d => d.AuthenticateBy)
                    .HasConstraintName("FK_ProjectQIGReset_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectQigresets)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjectQIGReset_ProjectInfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.ProjectQigresets)
                    .HasForeignKey(d => d.Qigid)
                    .HasConstraintName("FK_ProjectQIGReset_ProjectQIG");

                entity.HasOne(d => d.ResetByNavigation).WithMany(p => p.ProjectQigresetResetByNavigations)
                    .HasForeignKey(d => d.ResetBy)
                    .HasConstraintName("FK_ProjectQIGReset_ProjectUserRoleinfo");
            });

            modelBuilder.Entity<ProjectQigresetArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectQIGReset_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AuthenticateDate).HasColumnType("datetime");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.ResetDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectQigscriptsImportEvent>(entity =>
            {
                entity.ToTable("ProjectQIGScriptsImportEvents", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.JobStatus).HasComment("0 -- > Pending 1 -->In-Progree  2 --> Completed 3 --> Failed");
                entity.Property(e => e.ProcessedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.SetUpFinalizedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectQigscriptsImportEvents)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjectQIGScriptsImportEvents_ProjectInfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.ProjectQigscriptsImportEvents)
                    .HasForeignKey(d => d.Qigid)
                    .HasConstraintName("FK_ProjectQIGScriptsImportEvents_ProjectQIG");

                entity.HasOne(d => d.SetUpFinalizedByNavigation).WithMany(p => p.ProjectQigscriptsImportEvents)
                    .HasForeignKey(d => d.SetUpFinalizedBy)
                    .HasConstraintName("FK_ProjectQIGScriptsImportEvents_ProjectUserRoleinfo");
            });

            modelBuilder.Entity<ProjectQigscriptsImportEventsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectQIGScriptsImportEvents_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.JobStatus).HasComment("0 -- > Pending 1 -->In-Progree  2 --> Completed 3 --> Failed");
                entity.Property(e => e.ProcessedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.SetUpFinalizedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectQigteamHierarchy>(entity =>
            {
                entity.ToTable("ProjectQIGTeamHierarchy", "Marking");

                entity.HasIndex(e => new { e.ProjectUserRoleId, e.Isdeleted, e.IsActive, e.IsKp }, "IDX_Marking_ProjectQIGTeamHierarchy_ProjectUserRoleID_IsDeleted_IsActive_IsKP").HasFillFactor(95);

                entity.HasIndex(e => new { e.Qigid, e.ProjectUserRoleId, e.Isdeleted }, "IDX_Marking_ProjectQIGTeamHierarchy_QIGID_ProjectUserRoleID_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => new { e.Qigid, e.ReportingTo, e.Isdeleted }, "IDX_Marking_ProjectQIGTeamHierarchy_QIGID_ReportingTo_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => e.ProjectId, "IX_ProjectQIGTeamHierarchy_ProjectID").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.Isdeleted }, "IX_ProjectQIGTeamHierarchy_ProjectID_Isdeleted").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.IsActive }, "IX_ProjectQIGTeamHierarchy_ProjectID_QIGID_Isdeleted_IsActive").HasFillFactor(95);

                entity.Property(e => e.ProjectQigteamHierarchyId).HasColumnName("ProjectQIGTeamHierarchyID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsKp).HasColumnName("IsKP");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectQigteamHierarchyCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectQIGTeamHierarchy_ProjectUserRoleinfo2");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectQigteamHierarchyModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectQIGTeamHierarchy_ProjectUserRoleinfo3");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectQigteamHierarchies)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQIGTeamHierarchy_ProjectQIGTeamHierarchy");

                entity.HasOne(d => d.ProjectUserRole).WithMany(p => p.ProjectQigteamHierarchyProjectUserRoles)
                    .HasForeignKey(d => d.ProjectUserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQIGTeamHierarchy_ProjectUserRoleinfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.ProjectQigteamHierarchies)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQIGTeamHierarchy_ProjectQIG");

                entity.HasOne(d => d.ReportingToNavigation).WithMany(p => p.ProjectQigteamHierarchyReportingToNavigations)
                    .HasForeignKey(d => d.ReportingTo)
                    .HasConstraintName("FK_ProjectQIGTeamHierarchy_ProjectUserRoleinfo1");
            });

            modelBuilder.Entity<ProjectQigteamHierarchyArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectQIGTeamHierarchy_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectUserRoleId, e.Isdeleted, e.IsActive, e.IsKp }, "IDX_Marking_ProjectQIGTeamHierarchy_ProjectUserRoleID_IsDeleted_IsActive_IsKP_Archive");

                entity.HasIndex(e => new { e.Qigid, e.ProjectUserRoleId, e.Isdeleted }, "IDX_Marking_ProjectQIGTeamHierarchy_QIGID_ProjectUserRoleID_IsDeleted_Archive");

                entity.HasIndex(e => new { e.Qigid, e.ReportingTo, e.Isdeleted }, "IDX_Marking_ProjectQIGTeamHierarchy_QIGID_ReportingTo_IsDeleted_Archive");

                entity.HasIndex(e => e.ProjectId, "IX_ProjectQIGTeamHierarchy_ProjectID_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Isdeleted }, "IX_ProjectQIGTeamHierarchy_ProjectID_Isdeleted_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.IsActive }, "IX_ProjectQIGTeamHierarchy_ProjectID_QIGID_Isdeleted_IsActive_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.IsKp).HasColumnName("IsKP");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQigteamHierarchyId).HasColumnName("ProjectQIGTeamHierarchyID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
            });

            modelBuilder.Entity<ProjectQuestion>(entity =>
            {
                entity.ToTable("ProjectQuestions", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.IsDeleted }, "IDX_Marking_ProjectQuestions_ProjectID_IsDeleted");

                entity.HasIndex(e => new { e.ProjectId, e.QuestionType, e.IsDeleted }, "IDX_Marking_ProjectQuestions_ProjectID_QuestionType_IsDeleted");

                entity.HasIndex(e => e.IsDeleted, "IX_ProjectQuestions_IsDeleted");

                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.DirectionCode).HasMaxLength(250);
                entity.Property(e => e.IsComposite).HasDefaultValue(false);
                entity.Property(e => e.MarkingQuestionType).HasComment("1--->MCQ,  2--->Composition , 3--> Non-Composition");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ParentQuestionGuid)
                    .HasMaxLength(100)
                    .HasColumnName("ParentQuestionGUID");
                entity.Property(e => e.ParentQuestionId).HasColumnName("ParentQuestionID");
                entity.Property(e => e.PassageCode).HasMaxLength(500);
                entity.Property(e => e.PassageId).HasColumnName("PassageID");
                entity.Property(e => e.PassageLabel).HasMaxLength(500);
                entity.Property(e => e.PassageVersion).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.PassageXml)
                    .HasColumnType("ntext")
                    .HasColumnName("PassageXML");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.QuestionCode).HasMaxLength(50);
                entity.Property(e => e.QuestionGuid)
                    .HasMaxLength(100)
                    .HasColumnName("QuestionGUID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
                entity.Property(e => e.QuestionMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.QuestionVersion).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.QuestionXml).HasColumnName("QuestionXML");
                entity.Property(e => e.ResponseProcessingType).HasComment("1 --> Partial Manual Marking, 2 --> Complete Manual Marking");
                entity.Property(e => e.SectionId).HasColumnName("SectionID");
                entity.Property(e => e.StepValue).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectQuestions)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQuestions_ProjectInfo");
            });

            modelBuilder.Entity<ProjectQuestionAsset>(entity =>
            {
                entity.ToTable("ProjectQuestionAssets", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AssetId).HasColumnName("AssetID");
                entity.Property(e => e.AssetName).HasMaxLength(500);
                entity.Property(e => e.AssetType).HasComment("1 --> Question, 2 --> Passage");
                entity.Property(e => e.Path).HasMaxLength(255);
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");

                entity.HasOne(d => d.ProjectQuestion).WithMany(p => p.ProjectQuestionAssets)
                    .HasForeignKey(d => d.ProjectQuestionId)
                    .HasConstraintName("FK_ProjectQuestionAssets_ProjectQuestions");
            });

            modelBuilder.Entity<ProjectQuestionAssetsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectQuestionAssets_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AssetId).HasColumnName("AssetID");
                entity.Property(e => e.AssetName).HasMaxLength(500);
                entity.Property(e => e.AssetType).HasComment("1 --> Question, 2 --> Passage");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Path).HasMaxLength(255);
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
            });

            modelBuilder.Entity<ProjectQuestionChoiceMapping>(entity =>
            {
                entity.ToTable("ProjectQuestionChoiceMapping", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.MaxScore).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectQuestionChoiceMappingCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectQuestionChoiceMapping_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectQuestionChoiceMappingModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectQuestionChoiceMapping_ProjectUserRoleinfo1");

                entity.HasOne(d => d.ProjectQuestion).WithMany(p => p.ProjectQuestionChoiceMappings)
                    .HasForeignKey(d => d.ProjectQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQuestionChoiceMapping_ProjectQuestionChoiceMapping");
            });

            modelBuilder.Entity<ProjectQuestionChoiceMappingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectQuestionChoiceMapping_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.MaxScore).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
            });

            modelBuilder.Entity<ProjectQuestionScoreComponent>(entity =>
            {
                entity.HasKey(e => e.ScoreComponentId);

                entity.ToTable("ProjectQuestionScoreComponents", "Marking");

                entity.HasIndex(e => new { e.ProjectQuestionId, e.IsActive, e.IsDeleted }, "IDX_Marking_ProjectQuestionScoreComponents_ProjectQuestionID_IsActive_IsDeleted");

                entity.Property(e => e.ScoreComponentId).HasColumnName("ScoreComponentID");
                entity.Property(e => e.ComponentCode)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.ComponentDescription).HasMaxLength(2000);
                entity.Property(e => e.ComponentName)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");

                entity.HasOne(d => d.ProjectQuestion).WithMany(p => p.ProjectQuestionScoreComponents)
                    .HasForeignKey(d => d.ProjectQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQuestionScoreComponents_ProjectQuestions");
            });

            modelBuilder.Entity<ProjectQuestionScoreComponentsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectQuestionScoreComponents_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectQuestionId, e.IsActive, e.IsDeleted }, "IDX_Marking_ProjectQuestionScoreComponents_ProjectQuestionID_IsActive_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.ComponentCode)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.ComponentDescription).HasMaxLength(2000);
                entity.Property(e => e.ComponentName)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.ScoreComponentId).HasColumnName("ScoreComponentID");
            });

            modelBuilder.Entity<ProjectQuestionsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectQuestions_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.IsDeleted }, "IDX_Marking_ProjectQuestions_ProjectID_IsDeleted_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.QuestionType, e.IsDeleted }, "IDX_Marking_ProjectQuestions_ProjectID_QuestionType_IsDeleted_Archive");

                entity.HasIndex(e => e.IsDeleted, "IX_ProjectQuestions_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.DirectionCode).HasMaxLength(250);
                entity.Property(e => e.MarkingQuestionType).HasComment("1--->MCQ,  2--->Composition , 3--> Non-Composition");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ParentQuestionGuid)
                    .HasMaxLength(100)
                    .HasColumnName("ParentQuestionGUID");
                entity.Property(e => e.ParentQuestionId).HasColumnName("ParentQuestionID");
                entity.Property(e => e.PassageCode).HasMaxLength(500);
                entity.Property(e => e.PassageId).HasColumnName("PassageID");
                entity.Property(e => e.PassageLabel).HasMaxLength(500);
                entity.Property(e => e.PassageVersion).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.PassageXml)
                    .HasColumnType("ntext")
                    .HasColumnName("PassageXML");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.QuestionCode).HasMaxLength(50);
                entity.Property(e => e.QuestionGuid)
                    .HasMaxLength(100)
                    .HasColumnName("QuestionGUID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
                entity.Property(e => e.QuestionMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.QuestionVersion).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.QuestionXml).HasColumnName("QuestionXML");
                entity.Property(e => e.ResponseProcessingType).HasComment("1 --> Partial Manual Marking, 2 --> Complete Manual Marking");
                entity.Property(e => e.SectionId).HasColumnName("SectionID");
                entity.Property(e => e.StepValue).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<ProjectQuestionsHistory>(entity =>
            {
                entity.ToTable("ProjectQuestionsHistory", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.HistoryCreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.MarkingQuestionType).HasComment("1 --> Automatic 2 --> Semi-Automatic 3 --> Open Ended");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ParentQuestionGuid)
                    .HasMaxLength(100)
                    .HasColumnName("ParentQuestionGUID");
                entity.Property(e => e.ParentQuestionId).HasColumnName("ParentQuestionID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.QuestionCode).HasMaxLength(50);
                entity.Property(e => e.QuestionGuid)
                    .HasMaxLength(100)
                    .HasColumnName("QuestionGUID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
                entity.Property(e => e.QuestionMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.QuestionVersion).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.QuestionXml).HasColumnName("QuestionXML");
                entity.Property(e => e.SectionId).HasColumnName("SectionID");
                entity.Property(e => e.StepValue).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectQuestionsHistories)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectQuestionsHistory_ProjectInfo");
            });

            modelBuilder.Entity<ProjectSchedule>(entity =>
            {
                entity.ToTable("ProjectSchedule", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.IsActive, e.Isdeleted }, "IDX_Marking_ProjectSchedule_ProjectID_IsActive_IsDeleted");

                entity.Property(e => e.ProjectScheduleId).HasColumnName("ProjectScheduleID");
                entity.Property(e => e.ActualEndDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ExpectedEndDate).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.RefProjectScheduleId).HasColumnName("RefProjectScheduleID");
                entity.Property(e => e.ScheduleName).HasMaxLength(50);
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.WorkingDaysConfig).HasMaxLength(2000);

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectScheduleCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectSchedule_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectScheduleModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectSchedule_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectSchedules)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjectSchedule_ProjectInfo");

                entity.HasOne(d => d.RefProjectSchedule).WithMany(p => p.InverseRefProjectSchedule)
                    .HasForeignKey(d => d.RefProjectScheduleId)
                    .HasConstraintName("FK_ProjectSchedule_ProjectSchedule");
            });

            modelBuilder.Entity<ProjectScheduleArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectSchedule_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.IsActive, e.Isdeleted }, "IDX_Marking_ProjectSchedule_ProjectID_IsActive_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.ActualEndDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ExpectedEndDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectScheduleId).HasColumnName("ProjectScheduleID");
                entity.Property(e => e.RefProjectScheduleId).HasColumnName("RefProjectScheduleID");
                entity.Property(e => e.ScheduleName).HasMaxLength(50);
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.WorkingDaysConfig).HasMaxLength(2000);
            });

            modelBuilder.Entity<ProjectScheduleCalendar>(entity =>
            {
                entity.HasKey(e => e.ProjectCalendarId);

                entity.ToTable("ProjectScheduleCalendar", "Marking");

                entity.HasIndex(e => new { e.ProjectScheduleId, e.Isdeleted }, "IDX_Marking_ProjectScheduleCalendar_ProjectScheduleID_IsDeleted");

                entity.Property(e => e.ProjectCalendarId).HasColumnName("ProjectCalendarID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DayType)
                    .HasDefaultValue((byte)1)
                    .HasComment("1-Working;2-Holiday;3-Weekend;4-NotWorking(Others)");
                entity.Property(e => e.EndTime).HasColumnType("datetime");
                entity.Property(e => e.Modifieddate).HasColumnType("datetime");
                entity.Property(e => e.ProjectScheduleId).HasColumnName("ProjectScheduleID");
                entity.Property(e => e.Remarks).HasMaxLength(2000);
                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectScheduleCalendarCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectScheduleCalendar_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectScheduleCalendarModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectScheduleCalendar_ProjectUserRoleinfo1");

                entity.HasOne(d => d.ProjectSchedule).WithMany(p => p.ProjectScheduleCalendars)
                    .HasForeignKey(d => d.ProjectScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectScheduleCalendar_ProjectSchedule");
            });

            modelBuilder.Entity<ProjectScheduleCalendarArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectScheduleCalendar_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectScheduleId, e.Isdeleted }, "IDX_Marking_ProjectScheduleCalendar_ProjectScheduleID_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.DayType).HasComment("1-Working;2-Holiday;3-Weekend;4-NotWorking(Others)");
                entity.Property(e => e.EndTime).HasColumnType("datetime");
                entity.Property(e => e.Modifieddate).HasColumnType("datetime");
                entity.Property(e => e.ProjectCalendarId).HasColumnName("ProjectCalendarID");
                entity.Property(e => e.ProjectScheduleId).HasColumnName("ProjectScheduleID");
                entity.Property(e => e.Remarks).HasMaxLength(2000);
                entity.Property(e => e.StartTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProjectTeam>(entity =>
            {
                entity.HasKey(e => e.ProjectTeamId).HasName("PK__ProjectT__B043C674493E84DF");

                entity.ToTable("ProjectTeam", "Marking");

                entity.Property(e => e.ProjectTeamId).HasColumnName("ProjectTeamID");
                entity.Property(e => e.CenterId).HasColumnName("CenterID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Remarks).HasMaxLength(1000);
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.TeamCode).HasMaxLength(50);
                entity.Property(e => e.TeamName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectTeamCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectTeam_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectTeamModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectTeam_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectTeams)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectTeam_ProjectInfo");
            });

            modelBuilder.Entity<ProjectTeamQig>(entity =>
            {
                entity.HasKey(e => e.TeamQigid);

                entity.ToTable("ProjectTeamQIG", "Marking");

                entity.Property(e => e.TeamQigid).HasColumnName("TeamQIGID");
                entity.Property(e => e.AssignedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.ProjectTeamQigAssignedByNavigations)
                    .HasForeignKey(d => d.AssignedBy)
                    .HasConstraintName("FK_ProjectTeamQIG_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectTeamQigModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectTeamQIG_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectTeamQigs)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectTeamQIG_ProjectInfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.ProjectTeamQigs)
                    .HasForeignKey(d => d.Qigid)
                    .HasConstraintName("FK_ProjectTeamQIG_ProjectQIG");

                entity.HasOne(d => d.Team).WithMany(p => p.ProjectTeamQigs)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectTeamQIG_ProjectTeam");
            });

            modelBuilder.Entity<ProjectTeamUserInfo>(entity =>
            {
                entity.HasKey(e => e.ProjectTeamUserId).HasName("PK__ProjectT__EB2F78A6E5BFF698");

                entity.ToTable("ProjectTeamUserInfo", "Marking");

                entity.Property(e => e.ProjectTeamUserId).HasColumnName("ProjectTeamUserID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsPromotedUser).HasDefaultValue(false);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectTeamId).HasColumnName("ProjectTeamID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Remarks).HasMaxLength(1000);
                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectTeamUserInfoCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectTeamUserInfo_ProjectUserRoleinfo2");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectTeamUserInfoModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectTeamUserInfo_ProjectUserRoleinfo3");

                entity.HasOne(d => d.ProjectTeam).WithMany(p => p.ProjectTeamUserInfos)
                    .HasForeignKey(d => d.ProjectTeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectTeamUserInfo_ProjectTeam");

                entity.HasOne(d => d.ProjectUserRole).WithMany(p => p.ProjectTeamUserInfoProjectUserRoles)
                    .HasForeignKey(d => d.ProjectUserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectTeamUserInfo_ProjectUserRoleinfo");

                entity.HasOne(d => d.ReportingToNavigation).WithMany(p => p.ProjectTeamUserInfoReportingToNavigations)
                    .HasForeignKey(d => d.ReportingTo)
                    .HasConstraintName("FK_ProjectTeamUserInfo_ProjectUserRoleinfo1");
            });

            modelBuilder.Entity<ProjectUserQuestionResponse>(entity =>
            {
                entity.ToTable("ProjectUserQuestionResponse", "Marking");

                entity.HasIndex(e => new { e.Isdeleted, e.IsNullResponse }, "IDX_Marking_ProjectUserQuestionResponse_Isdeleted_IsNullResponse").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.ScriptId, e.Isdeleted }, "IDX_Marking_ProjectUserQuestionResponse_ProjectID_ScriptID_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.ScriptId, e.ProjectQuestionId, e.Isdeleted }, "IDX_Marking_ProjectUserQuestionResponse_ProjectID_ScriptID_ProjectQuestionID_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => new { e.ScriptId, e.ProjectQuestionId, e.Isdeleted }, "IDX_Marking_ProjectUserQuestionResponse_ScriptID_ProjectQuestionID_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => e.Isdeleted, "IDX_ScriptID_BandID").HasFillFactor(95);

                entity.Property(e => e.ProjectUserQuestionResponseId).HasColumnName("ProjectUserQuestionResponseID");
                entity.Property(e => e.FinalizedMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.MarkedDate).HasColumnType("datetime");
                entity.Property(e => e.MarkedType).HasComment("1--> Auto , 2--> Moderated , 3 --> Manual, 4-> Post Live Marking Moderation");
                entity.Property(e => e.MaxScore).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.ProjectUserQuestionResponseGuid)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("ProjectUserQuestionResponseGUID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
                entity.Property(e => e.ResponsePath).HasMaxLength(2000);
                entity.Property(e => e.ResponseType).HasComment("1-->Text, 2-->Audio, 3-->Video, 4-->Documents");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.UserResponseId).HasColumnName("UserResponseID");

                entity.HasOne(d => d.MarkedByNavigation).WithMany(p => p.ProjectUserQuestionResponses)
                    .HasForeignKey(d => d.MarkedBy)
                    .HasConstraintName("FK_ProjectUserQuestionResponse_ProjectUserRoleinfo");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectUserQuestionResponses)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectUserQuestionResponse_ProjectInfo");

                entity.HasOne(d => d.ProjectQuestion).WithMany(p => p.ProjectUserQuestionResponses)
                    .HasForeignKey(d => d.ProjectQuestionId)
                    .HasConstraintName("FK_ProjectUserQuestionResponse_ProjectQuestions");

                entity.HasOne(d => d.RecommendedBandNavigation).WithMany(p => p.ProjectUserQuestionResponses)
                    .HasForeignKey(d => d.RecommendedBand)
                    .HasConstraintName("FK_ProjectUserQuestionResponse_ProjectMarkSchemeBandDetails");

                entity.HasOne(d => d.Script).WithMany(p => p.ProjectUserQuestionResponses)
                    .HasForeignKey(d => d.ScriptId)
                    .HasConstraintName("FK_ProjectUserQuestionResponse_ProjectUserScript");
            });

            modelBuilder.Entity<ProjectUserQuestionResponseAcrchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectUserQuestionResponse_Acrchive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.FinalizedMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.MarkedDate).HasColumnType("datetime");
                entity.Property(e => e.MaxScore).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.ProjectUserQuestionResponseGuid).HasColumnName("ProjectUserQuestionResponseGUID");
                entity.Property(e => e.ProjectUserQuestionResponseId).HasColumnName("ProjectUserQuestionResponseID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
                entity.Property(e => e.ResponsePath).HasMaxLength(2000);
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.UserResponseId).HasColumnName("UserResponseID");
            });

            modelBuilder.Entity<ProjectUserQuestionResponseArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectUserQuestionResponse_Archive", "Marking");

                entity.HasIndex(e => new { e.Isdeleted, e.IsNullResponse }, "IDX_Marking_ProjectUserQuestionResponse_Isdeleted_IsNullResponse_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.ScriptId, e.Isdeleted }, "IDX_Marking_ProjectUserQuestionResponse_ProjectID_ScriptID_IsDeleted_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.ScriptId, e.ProjectQuestionId, e.Isdeleted }, "IDX_Marking_ProjectUserQuestionResponse_ProjectID_ScriptID_ProjectQuestionID_IsDeleted_Archive");

                entity.HasIndex(e => new { e.ScriptId, e.ProjectQuestionId, e.Isdeleted }, "IDX_Marking_ProjectUserQuestionResponse_ScriptID_ProjectQuestionID_IsDeleted_Archive");

                entity.HasIndex(e => e.Isdeleted, "IDX_ScriptID_BandID_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.FinalizedMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.MarkedDate).HasColumnType("datetime");
                entity.Property(e => e.MaxScore).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.ProjectUserQuestionResponseGuid).HasColumnName("ProjectUserQuestionResponseGUID");
                entity.Property(e => e.ProjectUserQuestionResponseId).HasColumnName("ProjectUserQuestionResponseID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
                entity.Property(e => e.ResponsePath).HasMaxLength(2000);
                entity.Property(e => e.ResponseType).HasComment("1-->Text, 2-->Audio, 3-->Video, 4-->Documents");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.UserResponseId).HasColumnName("UserResponseID");
            });

            modelBuilder.Entity<ProjectUserRoleinfo>(entity =>
            {
                entity.HasKey(e => e.ProjectUserRoleId).HasName("PK__ProjectU__B371A121252C81E4");

                entity.ToTable("ProjectUserRoleinfo", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.RoleId, e.IsActive, e.Isdeleted }, "IDX_Marking_ProjectUserRoleinfo_ProjectID_RoleID_IsActive_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.UserId, e.RoleId, e.IsActive, e.Isdeleted }, "IDX_Marking_ProjectUserRoleinfo_ProjectID_UserID_RoleID_IsActive_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => new { e.UserId, e.IsActive, e.Isdeleted }, "IDX_Marking_ProjectUserRoleinfo_UserID_IsActive_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => e.Isdeleted, "IX_ProjectUserRoleinfo_Isdeleted_ProjectID_UserID").HasFillFactor(95);

                entity.HasIndex(e => new { e.UserId, e.Isdeleted }, "IX_ProjectUserRoleinfo_UserID_Isdeleted_RoleID").HasFillFactor(95);

                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.AppointEndDate).HasColumnType("datetime");
                entity.Property(e => e.AppointStartDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EmailId)
                    .HasMaxLength(320)
                    .HasColumnName("EmailID");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsKp).HasColumnName("IsKP");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Nric)
                    .HasMaxLength(100)
                    .HasColumnName("NRIC");
                entity.Property(e => e.PhoneNo).HasMaxLength(50);
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Remarks).HasMaxLength(1000);
                entity.Property(e => e.RoleId).HasColumnName("RoleID");
                entity.Property(e => e.SendingSchoolId).HasColumnName("SendingSchoolID");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectUserRoleinfoCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectUserRoleinfo_UserInfo1");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectUserRoleinfoModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectUserRoleinfo_UserInfo2");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectUserRoleinfos)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectUserRoleinfo_ProjectInfo");

                entity.HasOne(d => d.Role).WithMany(p => p.ProjectUserRoleinfos)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectUserRoleinfo_Roleinfo");

                entity.HasOne(d => d.SendingSchool).WithMany(p => p.ProjectUserRoleinfos)
                    .HasForeignKey(d => d.SendingSchoolId)
                    .HasConstraintName("FK_ProjectUserRoleInfo_SchoolInfo");

                entity.HasOne(d => d.User).WithMany(p => p.ProjectUserRoleinfoUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectUserRoleinfo_UserInfo");
            });

            modelBuilder.Entity<ProjectUserRoleinfoArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId).HasName("PK__ProjectU__33A73E772C611880");

                entity.ToTable("ProjectUserRoleinfo_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.RoleId, e.IsActive, e.Isdeleted }, "IDX_Marking_ProjectUserRoleinfo_ProjectID_RoleID_IsActive_IsDeleted_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.UserId, e.RoleId, e.IsActive, e.Isdeleted }, "IDX_Marking_ProjectUserRoleinfo_ProjectID_UserID_RoleID_IsActive_IsDeleted_Archive");

                entity.HasIndex(e => new { e.UserId, e.IsActive, e.Isdeleted }, "IDX_Marking_ProjectUserRoleinfo_UserID_IsActive_IsDeleted_Archive");

                entity.HasIndex(e => e.Isdeleted, "IX_ProjectUserRoleinfo_Isdeleted_ProjectID_UserID_Archive");

                entity.HasIndex(e => new { e.UserId, e.Isdeleted }, "IX_ProjectUserRoleinfo_UserID_Isdeleted_RoleID_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AppointEndDate).HasColumnType("datetime");
                entity.Property(e => e.AppointStartDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.EmailId)
                    .HasMaxLength(320)
                    .HasColumnName("EmailID");
                entity.Property(e => e.IsKp).HasColumnName("IsKP");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Nric)
                    .HasMaxLength(100)
                    .HasColumnName("NRIC");
                entity.Property(e => e.PhoneNo).HasMaxLength(50);
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Remarks).HasMaxLength(1000);
                entity.Property(e => e.RoleId).HasColumnName("RoleID");
                entity.Property(e => e.SendingSchoolId).HasColumnName("SendingSchoolID");
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<ProjectUserSchoolMapping>(entity =>
            {
                entity.ToTable("ProjectUserSchoolMapping", "Marking");

                entity.HasIndex(e => new { e.ProjectUserRoleId, e.ExemptionSchoolId, e.IsDeleted }, "IDX_Marking_ProjectUserSchoolMapping_ProjectUserRoleID_ExemptionSchoolID_IsDeleted");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ExemptionSchoolId).HasColumnName("ExemptionSchoolID");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectUserSchoolMappingCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectUserSchoolMapping_ProjectUserRoleinfo1");

                entity.HasOne(d => d.ExemptionSchool).WithMany(p => p.ProjectUserSchoolMappings)
                    .HasForeignKey(d => d.ExemptionSchoolId)
                    .HasConstraintName("FK_ProjectUserSchoolMapping_SchoolInfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ProjectUserSchoolMappingModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ProjectUserSchoolMapping_ProjectUserRoleinfo2");

                entity.HasOne(d => d.ProjectUserRole).WithMany(p => p.ProjectUserSchoolMappingProjectUserRoles)
                    .HasForeignKey(d => d.ProjectUserRoleId)
                    .HasConstraintName("FK_ProjectUserSchoolMapping_ProjectUserRoleinfo");
            });

            modelBuilder.Entity<ProjectUserSchoolMappingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectUserSchoolMapping_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectUserRoleId, e.ExemptionSchoolId, e.IsDeleted }, "IDX_Marking_ProjectUserSchoolMapping_ProjectUserRoleID_ExemptionSchoolID_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ExemptionSchoolId).HasColumnName("ExemptionSchoolID");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
            });

            modelBuilder.Entity<ProjectUserScript>(entity =>
            {
                entity.HasKey(e => e.ScriptId);

                entity.ToTable("ProjectUserScript", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.DownloadedBy }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_DownloadedBy");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.ProjectCenterId }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_ProjectCenterID").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.ScheduleUserId }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_ScheduleUserID").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.ScriptType }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_ScriptType").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.WorkflowStatusId }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_workflowStatusID").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.WorkflowStatusId, e.DownloadedBy }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_workflowStatusID_DownloadedBy");

                entity.HasIndex(e => e.ProjectId, "IX_ProjectUserScript_ProjectID").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.IsRecommended }, "IX_ProjectUserScript_ProjectID_QIGID_Isdeleted_IsRecommended").HasFillFactor(95);

                entity.HasIndex(e => new { e.Qigid, e.Isdeleted }, "IX_ProjectUserScript_QIGID_Isdeleted").HasFillFactor(95);

                entity.HasIndex(e => new { e.Qigid, e.Isdeleted, e.ProjectCenterId, e.WorkflowStatusId }, "IX_ProjectUserScript_QIGID_Isdeleted_ProjectCenterID_WorkflowStatusID").HasFillFactor(95);

                entity.HasIndex(e => new { e.Qigid, e.Isdeleted, e.WorkflowStatusId }, "IX_ProjectUserScript_QIGID_Isdeleted_WorkflowStatusID").HasFillFactor(95);

                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.IsRecommended).HasDefaultValue(false);
                entity.Property(e => e.MarkedDate).HasColumnType("datetime");
                entity.Property(e => e.MarkedType).HasComment("1--> Auto , 2--> Moderated , 3 --> Manual, 4-> Post Live Marking Moderation");
                entity.Property(e => e.ProjectCenterId).HasColumnName("ProjectCenterID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.RecommendedDate).HasColumnType("datetime");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.ScriptGuid)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("ScriptGUID");
                entity.Property(e => e.ScriptName).HasMaxLength(50);
                entity.Property(e => e.ScriptType).HasComment("1-> Null Response, 2-> Partial Response,3-> Complete Response");
                entity.Property(e => e.TotalMarksAwarded).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.TotalMaxMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.UnRecommendedDate).HasColumnType("datetime");
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");

                entity.HasOne(d => d.MarkedByNavigation).WithMany(p => p.ProjectUserScriptMarkedByNavigations)
                    .HasForeignKey(d => d.MarkedBy)
                    .HasConstraintName("FK_ProjectUserScript_ProjectUserRoleinfo1");

                entity.HasOne(d => d.ProjectCenter).WithMany(p => p.ProjectUserScripts)
                    .HasForeignKey(d => d.ProjectCenterId)
                    .HasConstraintName("FK_ProjectUserScript_ProjectCenters");

                entity.HasOne(d => d.Project).WithMany(p => p.ProjectUserScripts)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectUserScript_ProjectInfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.ProjectUserScripts)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectUserScript_ProjectQIG");

                entity.HasOne(d => d.RecommendedByNavigation).WithMany(p => p.ProjectUserScriptRecommendedByNavigations)
                    .HasForeignKey(d => d.RecommendedBy)
                    .HasConstraintName("FK_ProjectUserScript_ProjectUserRoleinfo");

                entity.HasOne(d => d.UnRecommendedByNavigation).WithMany(p => p.ProjectUserScriptUnRecommendedByNavigations)
                    .HasForeignKey(d => d.UnRecommendedBy)
                    .HasConstraintName("FK_ProjectUserScript_ProjectUserRoleinfo2");

                entity.HasOne(d => d.WorkflowStatus).WithMany(p => p.ProjectUserScripts)
                    .HasForeignKey(d => d.WorkflowStatusId)
                    .HasConstraintName("FK_ProjectUserScript_WorkflowStatus");
            });

            modelBuilder.Entity<ProjectUserScriptArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectUserScript_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.DownloadedBy }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_DownloadedBy_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.ProjectCenterId }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_ProjectCenterID_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.ScheduleUserId }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_ScheduleUserID_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.ScriptType }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_ScriptType_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.WorkflowStatusId }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_workflowStatusID_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.WorkflowStatusId, e.DownloadedBy }, "IDX_Marking_ProjectUserScript_ProjectID_QIGID_IsDeleted_workflowStatusID_DownloadedBy_Archive");

                entity.HasIndex(e => e.ProjectId, "IX_ProjectUserScript_ProjectID_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.Isdeleted, e.IsRecommended }, "IX_ProjectUserScript_ProjectID_QIGID_Isdeleted_IsRecommended_Archive");

                entity.HasIndex(e => new { e.Qigid, e.Isdeleted }, "IX_ProjectUserScript_QIGID_Isdeleted_Archive");

                entity.HasIndex(e => new { e.Qigid, e.Isdeleted, e.ProjectCenterId, e.WorkflowStatusId }, "IX_ProjectUserScript_QIGID_Isdeleted_ProjectCenterID_WorkflowStatusID_Archive");

                entity.HasIndex(e => new { e.Qigid, e.Isdeleted, e.WorkflowStatusId }, "IX_ProjectUserScript_QIGID_Isdeleted_WorkflowStatusID_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.MarkedDate).HasColumnType("datetime");
                entity.Property(e => e.MarkedType).HasComment("1--> Auto , 2--> Moderated , 3 --> Manual, 4-> Post Live Marking Moderation");
                entity.Property(e => e.ProjectCenterId).HasColumnName("ProjectCenterID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.RecommendedDate).HasColumnType("datetime");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.ScriptGuid).HasColumnName("ScriptGUID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.ScriptName).HasMaxLength(50);
                entity.Property(e => e.ScriptType).HasComment("1-> Null Response, 2-> Partial Response,3-> Complete Response");
                entity.Property(e => e.TotalMarksAwarded).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.TotalMaxMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.UnRecommendedDate).HasColumnType("datetime");
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");
            });

            modelBuilder.Entity<ProjectWorkflow>(entity =>
            {
                entity.ToTable("ProjectWorkflow", "Marking");

                entity.Property(e => e.ProjectWorkflowId).HasColumnName("ProjectWorkflowID");
                entity.Property(e => e.ApplicationWorkflowId).HasColumnName("ApplicationWorkflowID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.DisabledDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            });

            modelBuilder.Entity<ProjectWorkflowStatusTracking>(entity =>
            {
                entity.HasKey(e => e.ProjectWorkflowTrackingId);

                entity.ToTable("ProjectWorkflowStatusTracking", "Marking");

                entity.HasIndex(e => new { e.EntityId, e.EntityType, e.WorkflowStatusId, e.IsDeleted }, "IDX_Marking_ProjectWorkflowStatusTracking_EntityID_EntityType_WorkflowStatusID_IsDeleted");

                entity.Property(e => e.ProjectWorkflowTrackingId).HasColumnName("ProjectWorkflowTrackingID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EntityId).HasColumnName("EntityID");
                entity.Property(e => e.EntityType).HasComment("1-->Project, 2-->QIG, 3-->User, 4-->Role, 5.Question");
                entity.Property(e => e.ProcessStatus).HasComment("1-->Started, 2-->InProgress, 3-->Completed, 4-->OnHold, 5--> Closure");
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectWorkflowStatusTrackings)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ProjectWorkflowStatusTracking_ProjectUserRoleinfo");

                entity.HasOne(d => d.WorkflowStatus).WithMany(p => p.ProjectWorkflowStatusTrackings)
                    .HasForeignKey(d => d.WorkflowStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectWorkflowStatusTracking_WorkflowStatus");
            });

            modelBuilder.Entity<ProjectWorkflowStatusTrackingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ProjectWorkflowStatusTracking_Archive", "Marking");

                entity.HasIndex(e => new { e.EntityId, e.EntityType, e.WorkflowStatusId, e.IsDeleted }, "IDX_Marking_ProjectWorkflowStatusTracking_EntityID_EntityType_WorkflowStatusID_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.EntityId).HasColumnName("EntityID");
                entity.Property(e => e.EntityType).HasComment("1-->Project, 2-->QIG, 3-->User, 4-->Role, 5.Question");
                entity.Property(e => e.ProjectWorkflowTrackingId).HasColumnName("ProjectWorkflowTrackingID");
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");
            });

            modelBuilder.Entity<Projectuserscript03072024>(entity =>
            {
                entity.HasKey(e => e.ScriptId)
                    .HasName("PK_ScriptID")
                    .IsClustered(false);

                entity.ToTable("projectuserscript_03072024", "Marking");

                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.MarkedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectCenterId).HasColumnName("ProjectCenterID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.RecommendedDate).HasColumnType("datetime");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.ScriptGuid).HasColumnName("ScriptGUID");
                entity.Property(e => e.ScriptName).HasMaxLength(50);
                entity.Property(e => e.TotalMarksAwarded).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.TotalMaxMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.UnRecommendedDate).HasColumnType("datetime");
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");
            });

            modelBuilder.Entity<QigstandardizationScriptSetting>(entity =>
            {
                entity.HasKey(e => e.SettingId);

                entity.ToTable("QIGStandardizationScriptSettings", "Marking");

                entity.HasIndex(e => new { e.Qigid, e.Isdeleted }, "IDX_Marking_QIGStandardizationScriptSettings_QIGID_IsDeleted");

                entity.Property(e => e.SettingId).HasColumnName("SettingID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsS1available)
                    .HasDefaultValue(true)
                    .HasColumnName("IsS1Available");
                entity.Property(e => e.IsS2available).HasColumnName("IsS2Available");
                entity.Property(e => e.IsS3available).HasColumnName("IsS3Available");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.S1startDate)
                    .HasColumnType("datetime")
                    .HasColumnName("S1StartDate");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QigstandardizationScriptSettingCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_QIGStandardizationScriptSettings_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.QigstandardizationScriptSettingModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_QIGStandardizationScriptSettings_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Qig).WithMany(p => p.QigstandardizationScriptSettings)
                    .HasForeignKey(d => d.Qigid)
                    .HasConstraintName("FK_QIGStandardizationScriptSettings_ProjectQIG");
            });

            modelBuilder.Entity<QigstandardizationScriptSettingsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId).HasName("PK_[QIGStandardizationScriptSettings_Archive");

                entity.ToTable("QIGStandardizationScriptSettings_Archive", "Marking");

                entity.HasIndex(e => new { e.Qigid, e.Isdeleted }, "IDX_Marking_QIGStandardizationScriptSettings_QIGID_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.IsS1available).HasColumnName("IsS1Available");
                entity.Property(e => e.IsS2available).HasColumnName("IsS2Available");
                entity.Property(e => e.IsS3available).HasColumnName("IsS3Available");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.S1startDate)
                    .HasColumnType("datetime")
                    .HasColumnName("S1StartDate");
                entity.Property(e => e.SettingId).HasColumnName("SettingID");
            });

            modelBuilder.Entity<QigtoAnnotationTemplateMapping>(entity =>
            {
                entity.ToTable("QIGToAnnotationTemplateMapping", "Marking");

                entity.HasIndex(e => new { e.Qigid, e.AnnotationTemplateId, e.IsActive, e.IsDeleted }, "IDX_Marking_QIGToAnnotationTemplateMapping_QIGID_AnnotationTemplateID_IsActive_IsDeleted");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AnnotationTemplateId).HasColumnName("AnnotationTemplateID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");

                entity.HasOne(d => d.AnnotationTemplate).WithMany(p => p.QigtoAnnotationTemplateMappings)
                    .HasForeignKey(d => d.AnnotationTemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QIGToAnnotationTemplateMapping_AnnotationTemplate");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QigtoAnnotationTemplateMappingCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_QIGToAnnotationTemplateMapping_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.QigtoAnnotationTemplateMappingModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_QIGToAnnotationTemplateMapping_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Qig).WithMany(p => p.QigtoAnnotationTemplateMappings)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QIGToAnnotationTemplateMapping_ProjectQIG");
            });

            modelBuilder.Entity<QigtoAnnotationTemplateMappingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("QIGToAnnotationTemplateMapping_Archive", "Marking");

                entity.HasIndex(e => new { e.Qigid, e.AnnotationTemplateId, e.IsActive, e.IsDeleted }, "IDX_Marking_QIGToAnnotationTemplateMapping_QIGID_AnnotationTemplateID_IsActive_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AnnotationTemplateId).HasColumnName("AnnotationTemplateID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
            });

            modelBuilder.Entity<QrtzBlobTrigger>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.ToTable("QRTZ_BLOB_TRIGGERS", "Quartz");

                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.TriggerName)
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_NAME");
                entity.Property(e => e.TriggerGroup)
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_GROUP");
                entity.Property(e => e.BlobData)
                    .HasColumnType("image")
                    .HasColumnName("BLOB_DATA");
            });

            modelBuilder.Entity<QrtzCalendar>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.CalendarName });

                entity.ToTable("QRTZ_CALENDARS", "Quartz");

                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.CalendarName)
                    .HasMaxLength(200)
                    .HasColumnName("CALENDAR_NAME");
                entity.Property(e => e.Calendar)
                    .IsRequired()
                    .HasColumnType("image")
                    .HasColumnName("CALENDAR");
            });

            modelBuilder.Entity<QrtzCronTrigger>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.ToTable("QRTZ_CRON_TRIGGERS", "Quartz");

                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.TriggerName)
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_NAME");
                entity.Property(e => e.TriggerGroup)
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_GROUP");
                entity.Property(e => e.CronExpression)
                    .IsRequired()
                    .HasMaxLength(120)
                    .HasColumnName("CRON_EXPRESSION");
                entity.Property(e => e.TimeZoneId)
                    .HasMaxLength(80)
                    .HasColumnName("TIME_ZONE_ID");

                entity.HasOne(d => d.QrtzTrigger).WithOne(p => p.QrtzCronTrigger)
                    .HasForeignKey<QrtzCronTrigger>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                    .HasConstraintName("FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QrtzFiredTrigger>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.EntryId });

                entity.ToTable("QRTZ_FIRED_TRIGGERS", "Quartz");

                entity.HasIndex(e => new { e.SchedName, e.InstanceName, e.RequestsRecovery }, "IDX_QRTZ_FT_INST_JOB_REQ_RCVRY");

                entity.HasIndex(e => new { e.SchedName, e.JobGroup }, "IDX_QRTZ_FT_JG");

                entity.HasIndex(e => new { e.SchedName, e.JobName, e.JobGroup }, "IDX_QRTZ_FT_J_G");

                entity.HasIndex(e => new { e.SchedName, e.TriggerGroup }, "IDX_QRTZ_FT_TG");

                entity.HasIndex(e => new { e.SchedName, e.InstanceName }, "IDX_QRTZ_FT_TRIG_INST_NAME");

                entity.HasIndex(e => new { e.SchedName, e.TriggerName, e.TriggerGroup }, "IDX_QRTZ_FT_T_G");

                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.EntryId)
                    .HasMaxLength(95)
                    .HasColumnName("ENTRY_ID");
                entity.Property(e => e.FiredTime).HasColumnName("FIRED_TIME");
                entity.Property(e => e.InstanceName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("INSTANCE_NAME");
                entity.Property(e => e.IsNonconcurrent).HasColumnName("IS_NONCONCURRENT");
                entity.Property(e => e.JobGroup)
                    .HasMaxLength(150)
                    .HasColumnName("JOB_GROUP");
                entity.Property(e => e.JobName)
                    .HasMaxLength(150)
                    .HasColumnName("JOB_NAME");
                entity.Property(e => e.Priority).HasColumnName("PRIORITY");
                entity.Property(e => e.RequestsRecovery).HasColumnName("REQUESTS_RECOVERY");
                entity.Property(e => e.SchedTime).HasColumnName("SCHED_TIME");
                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(16)
                    .HasColumnName("STATE");
                entity.Property(e => e.TriggerGroup)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_GROUP");
                entity.Property(e => e.TriggerName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_NAME");
            });

            modelBuilder.Entity<QrtzJobDetail>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.JobName, e.JobGroup });

                entity.ToTable("QRTZ_JOB_DETAILS", "Quartz");

                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.JobName)
                    .HasMaxLength(150)
                    .HasColumnName("JOB_NAME");
                entity.Property(e => e.JobGroup)
                    .HasMaxLength(150)
                    .HasColumnName("JOB_GROUP");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .HasColumnName("DESCRIPTION");
                entity.Property(e => e.IsDurable).HasColumnName("IS_DURABLE");
                entity.Property(e => e.IsNonconcurrent).HasColumnName("IS_NONCONCURRENT");
                entity.Property(e => e.IsUpdateData).HasColumnName("IS_UPDATE_DATA");
                entity.Property(e => e.JobClassName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("JOB_CLASS_NAME");
                entity.Property(e => e.JobData)
                    .HasColumnType("image")
                    .HasColumnName("JOB_DATA");
                entity.Property(e => e.RequestsRecovery).HasColumnName("REQUESTS_RECOVERY");
            });

            modelBuilder.Entity<QrtzLock>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.LockName });

                entity.ToTable("QRTZ_LOCKS", "Quartz");

                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.LockName)
                    .HasMaxLength(40)
                    .HasColumnName("LOCK_NAME");
            });

            modelBuilder.Entity<QrtzPausedTriggerGrp>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerGroup });

                entity.ToTable("QRTZ_PAUSED_TRIGGER_GRPS", "Quartz");

                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.TriggerGroup)
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_GROUP");
            });

            modelBuilder.Entity<QrtzScheduleHistory>(entity =>
            {
                entity.ToTable("QRTZ_SCHEDULE_HISTORY", "Quartz");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");
                entity.Property(e => e.JobClass)
                    .HasMaxLength(150)
                    .HasColumnName("JOB_CLASS");
                entity.Property(e => e.JobGroup)
                    .HasMaxLength(150)
                    .HasColumnName("JOB_GROUP");
                entity.Property(e => e.JobName)
                    .HasMaxLength(150)
                    .HasColumnName("JOB_NAME");
                entity.Property(e => e.NextFireTime).HasColumnName("NEXT_FIRE_TIME");
                entity.Property(e => e.PrevFireTime).HasColumnName("PREV_FIRE_TIME");
                entity.Property(e => e.ProjectId).HasColumnName("PROJECT_ID");
                entity.Property(e => e.QigId).HasColumnName("QIG_ID");
                entity.Property(e => e.RcType).HasColumnName("RC_TYPE");
                entity.Property(e => e.RepeatInterval).HasColumnName("REPEAT_INTERVAL");
                entity.Property(e => e.SchedName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.StartTime).HasColumnName("START_TIME");
                entity.Property(e => e.TimesTriggered).HasColumnName("TIMES_TRIGGERED");
                entity.Property(e => e.TriggerGroup)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_GROUP");
                entity.Property(e => e.TriggerName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_NAME");
            });

            modelBuilder.Entity<QrtzSchedulerState>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.InstanceName });

                entity.ToTable("QRTZ_SCHEDULER_STATE", "Quartz");

                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.InstanceName)
                    .HasMaxLength(200)
                    .HasColumnName("INSTANCE_NAME");
                entity.Property(e => e.CheckinInterval).HasColumnName("CHECKIN_INTERVAL");
                entity.Property(e => e.LastCheckinTime).HasColumnName("LAST_CHECKIN_TIME");
            });

            modelBuilder.Entity<QrtzSimpleTrigger>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.ToTable("QRTZ_SIMPLE_TRIGGERS", "Quartz");

                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.TriggerName)
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_NAME");
                entity.Property(e => e.TriggerGroup)
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_GROUP");
                entity.Property(e => e.RepeatCount).HasColumnName("REPEAT_COUNT");
                entity.Property(e => e.RepeatInterval).HasColumnName("REPEAT_INTERVAL");
                entity.Property(e => e.TimesTriggered).HasColumnName("TIMES_TRIGGERED");

                entity.HasOne(d => d.QrtzTrigger).WithOne(p => p.QrtzSimpleTrigger)
                    .HasForeignKey<QrtzSimpleTrigger>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                    .HasConstraintName("FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QrtzSimpropTrigger>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.ToTable("QRTZ_SIMPROP_TRIGGERS", "Quartz");

                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.TriggerName)
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_NAME");
                entity.Property(e => e.TriggerGroup)
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_GROUP");
                entity.Property(e => e.BoolProp1).HasColumnName("BOOL_PROP_1");
                entity.Property(e => e.BoolProp2).HasColumnName("BOOL_PROP_2");
                entity.Property(e => e.DecProp1)
                    .HasColumnType("numeric(13, 4)")
                    .HasColumnName("DEC_PROP_1");
                entity.Property(e => e.DecProp2)
                    .HasColumnType("numeric(13, 4)")
                    .HasColumnName("DEC_PROP_2");
                entity.Property(e => e.IntProp1).HasColumnName("INT_PROP_1");
                entity.Property(e => e.IntProp2).HasColumnName("INT_PROP_2");
                entity.Property(e => e.LongProp1).HasColumnName("LONG_PROP_1");
                entity.Property(e => e.LongProp2).HasColumnName("LONG_PROP_2");
                entity.Property(e => e.StrProp1)
                    .HasMaxLength(512)
                    .HasColumnName("STR_PROP_1");
                entity.Property(e => e.StrProp2)
                    .HasMaxLength(512)
                    .HasColumnName("STR_PROP_2");
                entity.Property(e => e.StrProp3)
                    .HasMaxLength(512)
                    .HasColumnName("STR_PROP_3");

                entity.HasOne(d => d.QrtzTrigger).WithOne(p => p.QrtzSimpropTrigger)
                    .HasForeignKey<QrtzSimpropTrigger>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                    .HasConstraintName("FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QrtzTrigger>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.ToTable("QRTZ_TRIGGERS", "Quartz");

                entity.HasIndex(e => new { e.SchedName, e.CalendarName }, "IDX_QRTZ_T_C");

                entity.HasIndex(e => new { e.SchedName, e.TriggerGroup }, "IDX_QRTZ_T_G");

                entity.HasIndex(e => new { e.SchedName, e.JobName, e.JobGroup }, "IDX_QRTZ_T_J");

                entity.HasIndex(e => new { e.SchedName, e.JobGroup }, "IDX_QRTZ_T_JG");

                entity.HasIndex(e => new { e.SchedName, e.NextFireTime }, "IDX_QRTZ_T_NEXT_FIRE_TIME");

                entity.HasIndex(e => new { e.SchedName, e.MisfireInstr, e.NextFireTime }, "IDX_QRTZ_T_NFT_MISFIRE");

                entity.HasIndex(e => new { e.SchedName, e.TriggerState, e.NextFireTime }, "IDX_QRTZ_T_NFT_ST");

                entity.HasIndex(e => new { e.SchedName, e.MisfireInstr, e.NextFireTime, e.TriggerState }, "IDX_QRTZ_T_NFT_ST_MISFIRE");

                entity.HasIndex(e => new { e.SchedName, e.MisfireInstr, e.NextFireTime, e.TriggerGroup, e.TriggerState }, "IDX_QRTZ_T_NFT_ST_MISFIRE_GRP");

                entity.HasIndex(e => new { e.SchedName, e.TriggerGroup, e.TriggerState }, "IDX_QRTZ_T_N_G_STATE");

                entity.HasIndex(e => new { e.SchedName, e.TriggerName, e.TriggerGroup, e.TriggerState }, "IDX_QRTZ_T_N_STATE");

                entity.HasIndex(e => new { e.SchedName, e.TriggerState }, "IDX_QRTZ_T_STATE");

                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
                entity.Property(e => e.TriggerName)
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_NAME");
                entity.Property(e => e.TriggerGroup)
                    .HasMaxLength(150)
                    .HasColumnName("TRIGGER_GROUP");
                entity.Property(e => e.CalendarName)
                    .HasMaxLength(200)
                    .HasColumnName("CALENDAR_NAME");
                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .HasColumnName("DESCRIPTION");
                entity.Property(e => e.EndTime).HasColumnName("END_TIME");
                entity.Property(e => e.JobData)
                    .HasColumnType("image")
                    .HasColumnName("JOB_DATA");
                entity.Property(e => e.JobGroup)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("JOB_GROUP");
                entity.Property(e => e.JobName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("JOB_NAME");
                entity.Property(e => e.MisfireInstr).HasColumnName("MISFIRE_INSTR");
                entity.Property(e => e.NextFireTime).HasColumnName("NEXT_FIRE_TIME");
                entity.Property(e => e.PrevFireTime).HasColumnName("PREV_FIRE_TIME");
                entity.Property(e => e.Priority).HasColumnName("PRIORITY");
                entity.Property(e => e.StartTime).HasColumnName("START_TIME");
                entity.Property(e => e.TriggerState)
                    .IsRequired()
                    .HasMaxLength(16)
                    .HasColumnName("TRIGGER_STATE");
                entity.Property(e => e.TriggerType)
                    .IsRequired()
                    .HasMaxLength(8)
                    .HasColumnName("TRIGGER_TYPE");

                entity.HasOne(d => d.QrtzJobDetail).WithMany(p => p.QrtzTriggers)
                    .HasForeignKey(d => new { d.SchedName, d.JobName, d.JobGroup })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS");
            });

            modelBuilder.Entity<QualifyingAssessmentScriptDetail>(entity =>
            {
                entity.HasKey(e => e.QassessmentScriptId);

                entity.ToTable("QualifyingAssessmentScriptDetails", "Marking");

                entity.HasIndex(e => new { e.QualifyingAssessmentId, e.ScriptCategorizationPoolId, e.IsSelected, e.IsDeleted }, "IDX_Marking_QualifyingAssessmentScriptDetails_QualifyingAssessmentID_ScriptCategorizationPoolID_IsSelected_IsDeleted");

                entity.Property(e => e.QassessmentScriptId).HasColumnName("QAssessmentScriptID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsSelected).HasDefaultValue(true);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.QualifyingAssessmentId).HasColumnName("QualifyingAssessmentID");
                entity.Property(e => e.ScriptCategorizationPoolId).HasColumnName("ScriptCategorizationPoolID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QualifyingAssessmentScriptDetailCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_QualifyingAssessmentScriptDetails_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.QualifyingAssessmentScriptDetailModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_QualifyingAssessmentScriptDetails_ProjectUserRoleinfo1");

                entity.HasOne(d => d.QualifyingAssessment).WithMany(p => p.QualifyingAssessmentScriptDetails)
                    .HasForeignKey(d => d.QualifyingAssessmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QualifyingAssessmentScriptDetails_StandardizationQualifyingAssessment");

                entity.HasOne(d => d.ScriptCategorizationPool).WithMany(p => p.QualifyingAssessmentScriptDetails)
                    .HasForeignKey(d => d.ScriptCategorizationPoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QualifyingAssessmentScriptDetails_ScriptCategorizationPool");
            });

            modelBuilder.Entity<QualifyingAssessmentScriptDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("QualifyingAssessmentScriptDetails_Archive", "Marking");

                entity.HasIndex(e => new { e.QualifyingAssessmentId, e.ScriptCategorizationPoolId, e.IsSelected, e.IsDeleted }, "IDX_Marking_QualifyingAssessmentScriptDetails_QualifyingAssessmentID_ScriptCategorizationPoolID_IsSelected_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.QassessmentScriptId).HasColumnName("QAssessmentScriptID");
                entity.Property(e => e.QualifyingAssessmentId).HasColumnName("QualifyingAssessmentID");
                entity.Property(e => e.ScriptCategorizationPoolId).HasColumnName("ScriptCategorizationPoolID");
            });

            modelBuilder.Entity<QuartzRcjobTracking>(entity =>
            {
                entity.ToTable("QuartzRCJobTracking", "Quartz");

                entity.HasIndex(e => new { e.ProjectId, e.JobRunDateTime }, "IDX_Quartz_QuartzRCJobTracking_ProjectID_JobRunDateTime").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.Qigid }, "IDX_Quartz_QuartzRCJobTracking_ProjectID_QIGID").HasFillFactor(95);

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.JobGroup)
                    .HasMaxLength(150)
                    .HasColumnName("JOB_GROUP");
                entity.Property(e => e.JobGuid)
                    .HasMaxLength(200)
                    .HasColumnName("JobGUID");
                entity.Property(e => e.JobName)
                    .HasMaxLength(150)
                    .HasColumnName("JOB_NAME");
                entity.Property(e => e.JobRunDateTime).HasColumnType("datetime");
                entity.Property(e => e.JobStatus).HasComment("1-> Success, 2-> Failure");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.Rctype).HasColumnName("RCType");
                entity.Property(e => e.SamplingRate).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.SchedName)
                    .HasMaxLength(100)
                    .HasColumnName("SCHED_NAME");
            });

            modelBuilder.Entity<QuestionScoreComponentMarkingDetail>(entity =>
            {
                entity.ToTable("QuestionScoreComponentMarkingDetails", "Marking");

                entity.HasIndex(e => new { e.UserScriptMarkingRefId, e.QuestionUserResponseMarkingRefId, e.ScoreComponentId, e.IsActive, e.IsDeleted }, "IDX_Marking_QuestionScoreComponentMarkingDetails_USMarkingRefID_QRMarkingRefID_ScoreComponentID_IsActive_IsDeleted");

                entity.HasIndex(e => new { e.QuestionUserResponseMarkingRefId, e.ScoreComponentId }, "IX_QuestionScoreComponentMarkingDetails_QuestionUserResponseMarkingRefID_ScoreComponentID");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AwardedMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.BandId).HasColumnName("BandID");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.MarkedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.MarkingStatus).HasComment("1--->Approved,  2--->Ammended, 3--->Rejected");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.QuestionUserResponseMarkingRefId).HasColumnName("QuestionUserResponseMarkingRefID");
                entity.Property(e => e.ScoreComponentId).HasColumnName("ScoreComponentID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");

                entity.HasOne(d => d.Band).WithMany(p => p.QuestionScoreComponentMarkingDetails)
                    .HasForeignKey(d => d.BandId)
                    .HasConstraintName("FK_QuestionScoreComponentMarkingDetails_ProjectMarkSchemeBandDetails");

                entity.HasOne(d => d.MarkedByNavigation).WithMany(p => p.QuestionScoreComponentMarkingDetails)
                    .HasForeignKey(d => d.MarkedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionScoreComponentMarkingDetails_ProjectUserRoleinfo");

                entity.HasOne(d => d.QuestionUserResponseMarkingRef).WithMany(p => p.QuestionScoreComponentMarkingDetails)
                    .HasForeignKey(d => d.QuestionUserResponseMarkingRefId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionScoreComponentMarkingDetails_QuestionUserResponseMarkingDetails");

                entity.HasOne(d => d.ScoreComponent).WithMany(p => p.QuestionScoreComponentMarkingDetails)
                    .HasForeignKey(d => d.ScoreComponentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionScoreComponentMarkingDetails_ProjectQuestionScoreComponents");

                entity.HasOne(d => d.UserScriptMarkingRef).WithMany(p => p.QuestionScoreComponentMarkingDetails)
                    .HasForeignKey(d => d.UserScriptMarkingRefId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionScoreComponentMarkingDetails_UserScriptMarkingDetails");

                entity.HasOne(d => d.WorkflowStatus).WithMany(p => p.QuestionScoreComponentMarkingDetails)
                    .HasForeignKey(d => d.WorkflowStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionScoreComponentMarkingDetails_WorkflowStatus");
            });

            modelBuilder.Entity<QuestionScoreComponentMarkingDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("QuestionScoreComponentMarkingDetails_Archive", "Marking");

                entity.HasIndex(e => new { e.UserScriptMarkingRefId, e.QuestionUserResponseMarkingRefId, e.ScoreComponentId, e.IsActive, e.IsDeleted }, "IDX_Marking_QuestionScoreComponentMarkingDetails_USMarkingRefID_QRMarkingRefID_ScoreComponentID_IsActive_IsDeleted_Archive");

                entity.HasIndex(e => new { e.QuestionUserResponseMarkingRefId, e.ScoreComponentId }, "IX_QuestionScoreComponentMarkingDetails_QuestionUserResponseMarkingRefID_ScoreComponentID_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AwardedMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.BandId).HasColumnName("BandID");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.MarkedDate).HasColumnType("datetime");
                entity.Property(e => e.MarkingStatus).HasComment("1--->Approved,  2--->Ammended, 3--->Rejected");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.QuestionUserResponseMarkingRefId).HasColumnName("QuestionUserResponseMarkingRefID");
                entity.Property(e => e.ScoreComponentId).HasColumnName("ScoreComponentID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
                entity.Property(e => e.WorkflowStatusId).HasColumnName("WorkflowStatusID");
            });

            modelBuilder.Entity<QuestionUserResponseMarkingDetail>(entity =>
            {
                entity.ToTable("QuestionUserResponseMarkingDetails", "Marking");

                entity.HasIndex(e => new { e.ScriptId, e.MarkedBy, e.IsActive, e.IsDeleted }, "IDX_Marking_QuestionUserResponseMarkingDetails_ScriptID_MarkedBy_IsActive_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => e.UserScriptMarkingRefId, "IDX_Marking_QuestionUserResponseMarkingDetails_UserScriptMarkingRefID");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.BandId).HasColumnName("BandID");
                entity.Property(e => e.CandidateId).HasColumnName("CandidateID");
                entity.Property(e => e.Comments).HasMaxLength(2000);
                entity.Property(e => e.MarkedType).HasComment("1--> Auto , 2--> Moderated , 3 --> Manual, 4-> Post Live Marking Moderation");
                entity.Property(e => e.Markeddate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.MarkingStatus).HasComment("1--->Approved,  2--->Ammended, 3--->Rejected");
                entity.Property(e => e.Marks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ProjectQuestionResponseId).HasColumnName("ProjectQuestionResponseID");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
                entity.Property(e => e.WorkflowstatusId).HasColumnName("WorkflowstatusID");

                entity.HasOne(d => d.Band).WithMany(p => p.QuestionUserResponseMarkingDetails)
                    .HasForeignKey(d => d.BandId)
                    .HasConstraintName("FK_QuestionUserResponseMarkingDetails_ProjectMarkSchemeBandDetails");

                entity.HasOne(d => d.MarkedByNavigation).WithMany(p => p.QuestionUserResponseMarkingDetails)
                    .HasForeignKey(d => d.MarkedBy)
                    .HasConstraintName("FK_QuestionUserResponseMarkingDetails_ProjectUserRoleinfo");

                entity.HasOne(d => d.ProjectQuestionResponse).WithMany(p => p.QuestionUserResponseMarkingDetails)
                    .HasForeignKey(d => d.ProjectQuestionResponseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionUserResponseMarkingDetails_ProjectUserQuestionResponse");

                entity.HasOne(d => d.Script).WithMany(p => p.QuestionUserResponseMarkingDetails)
                    .HasForeignKey(d => d.ScriptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionUserResponseMarkingDetails_ProjectUserScript");

                entity.HasOne(d => d.UserScriptMarkingRef).WithMany(p => p.QuestionUserResponseMarkingDetails)
                    .HasForeignKey(d => d.UserScriptMarkingRefId)
                    .HasConstraintName("FK_QuestionUserResponseMarkingDetails_UserScriptMarkingDetails");

                entity.HasOne(d => d.Workflowstatus).WithMany(p => p.QuestionUserResponseMarkingDetails)
                    .HasForeignKey(d => d.WorkflowstatusId)
                    .HasConstraintName("FK_QuestionUserResponseMarkingDetails_WorkflowStatus");
            });

            modelBuilder.Entity<QuestionUserResponseMarkingDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("QuestionUserResponseMarkingDetails_Archive", "Marking");

                entity.HasIndex(e => new { e.ScriptId, e.MarkedBy, e.IsActive, e.IsDeleted }, "IDX_Marking_QuestionUserResponseMarkingDetails_ScriptID_MarkedBy_IsActive_IsDeleted_Archive");

                entity.HasIndex(e => e.UserScriptMarkingRefId, "IDX_Marking_QuestionUserResponseMarkingDetails_UserScriptMarkingRefID_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.BandId).HasColumnName("BandID");
                entity.Property(e => e.CandidateId).HasColumnName("CandidateID");
                entity.Property(e => e.Comments).HasMaxLength(2000);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Markeddate).HasColumnType("datetime");
                entity.Property(e => e.MarkingStatus).HasComment("1--->Approved,  2--->Ammended, 3--->Rejected");
                entity.Property(e => e.Marks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ProjectQuestionResponseId).HasColumnName("ProjectQuestionResponseID");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
                entity.Property(e => e.WorkflowstatusId).HasColumnName("WorkflowstatusID");
            });

            modelBuilder.Entity<QuestionUserResponseMarkingImage>(entity =>
            {
                entity.ToTable("QuestionUserResponseMarkingImage", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.QuestionUserResponseMarkingRefId).HasColumnName("QuestionUserResponseMarkingRefID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");

                entity.HasOne(d => d.QuestionUserResponseMarkingRef).WithMany(p => p.QuestionUserResponseMarkingImages)
                    .HasForeignKey(d => d.QuestionUserResponseMarkingRefId)
                    .HasConstraintName("FK_QuestionUserResponseMarkingImage_QuestionUserResponseMarkingDetails");

                entity.HasOne(d => d.UserScriptMarkingRef).WithMany(p => p.QuestionUserResponseMarkingImages)
                    .HasForeignKey(d => d.UserScriptMarkingRefId)
                    .HasConstraintName("FK_QuestionUserResponseMarkingImage_UserScriptMarkingDetails");
            });

            modelBuilder.Entity<QuestionUserResponseMarkingImageArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("QuestionUserResponseMarkingImage_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.QuestionUserResponseMarkingRefId).HasColumnName("QuestionUserResponseMarkingRefID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
            });

            modelBuilder.Entity<RoleLevel>(entity =>
            {
                entity.HasKey(e => e.RoleLevelId).HasName("PK__RoleLeve__934E77A98B4BD0AA");

                entity.ToTable("RoleLevel", "Marking");

                entity.Property(e => e.RoleLevelId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("RoleLevelID");
                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.LevelCode)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.LevelName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RoleToPrivilege>(entity =>
            {
                entity.ToTable("RoleToPrivileges", "Marking");

                entity.HasIndex(e => new { e.RoleId, e.PrivilegeId, e.IsDeleted }, "IDX_Marking_RoleToPrivileges_RoleID_PrivilegeID_IsDeleted");

                entity.Property(e => e.RoleToPrivilegeId).HasColumnName("RoleToPrivilegeID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.PrivilegeId).HasColumnName("PrivilegeID");
                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Privilege).WithMany(p => p.RoleToPrivileges)
                    .HasForeignKey(d => d.PrivilegeId)
                    .HasConstraintName("FK_RoleToPrivileges_Privileges");

                entity.HasOne(d => d.Role).WithMany(p => p.RoleToPrivileges)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleToPrivileges_Roleinfo");
            });

            modelBuilder.Entity<Roleinfo>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.ToTable("Roleinfo", "Marking");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsChildExist).HasDefaultValue(false);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ParentRoleId).HasColumnName("ParentRoleID");
                entity.Property(e => e.RoleCode)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.RoleGroup).HasComment("1-> Application, 2-> Project");
                entity.Property(e => e.RoleLevelId).HasColumnName("RoleLevelID");
                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SchoolInfo>(entity =>
            {
                entity.HasKey(e => e.SchoolId);

                entity.ToTable("SchoolInfo", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.IsDeleted }, "IDX_Marking_SchoolInfo_ProjectID_IsDeleted").HasFillFactor(95);

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ParentId).HasColumnName("ParentID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.SchoolCode).HasMaxLength(50);
                entity.Property(e => e.SchoolName).HasMaxLength(100);
            });

            modelBuilder.Entity<SchoolInfoArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("SchoolInfo_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.IsDeleted }, "IDX_Marking_SchoolInfo_ProjectID_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ParentId).HasColumnName("ParentID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.SchoolCode).HasMaxLength(50);
                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");
                entity.Property(e => e.SchoolName).HasMaxLength(100);
            });

            modelBuilder.Entity<ScoreComponent>(entity =>
            {
                entity.ToTable("ScoreComponent", "Marking");

                entity.Property(e => e.ScoreComponentId).HasColumnName("ScoreComponentID");
                entity.Property(e => e.ComponentCode).HasMaxLength(50);
                entity.Property(e => e.ComponentName).HasMaxLength(200);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Marks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            });

            modelBuilder.Entity<ScoreComponentDetail>(entity =>
            {
                entity.HasKey(e => e.ComponentDetailId);

                entity.ToTable("ScoreComponentDetails", "Marking");

                entity.Property(e => e.ComponentDetailId).HasColumnName("ComponentDetailID");
                entity.Property(e => e.ComponentCode).HasMaxLength(50);
                entity.Property(e => e.ComponentName).HasMaxLength(200);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.Marks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ScoreComponentId).HasColumnName("ScoreComponentID");

                entity.HasOne(d => d.ScoreComponent).WithMany(p => p.ScoreComponentDetails)
                    .HasForeignKey(d => d.ScoreComponentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ScoreComponentDetails_ScoreComponent");
            });

            modelBuilder.Entity<Script>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Scripts__3214EC270D08A238");

                entity.ToTable("Scripts", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Script1).HasColumnName("Script");
                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<ScriptCategorizationPool>(entity =>
            {
                entity.ToTable("ScriptCategorizationPool", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.IsDeleted }, "IDX_Marking_ScriptCategorizationPool_ProjectID_QIGID_IsDeleted");

                entity.Property(e => e.ScriptCategorizationPoolId).HasColumnName("ScriptCategorizationPoolID");
                entity.Property(e => e.CategorizationVersion).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.FinalizedMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.PoolType).HasComment("1-->Standardization Script, 2-->Adtnal.Standardization Script, 3-->BenchMark Script");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ScriptCategorizationPoolCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_ScriptCategorizationPool_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ScriptCategorizationPoolModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_ScriptCategorizationPool_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.ScriptCategorizationPools)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ScriptCategorizationPool_ProjectInfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.ScriptCategorizationPools)
                    .HasForeignKey(d => d.Qigid)
                    .HasConstraintName("FK_ScriptCategorizationPool_ProjectQIG");

                entity.HasOne(d => d.Script).WithMany(p => p.ScriptCategorizationPools)
                    .HasForeignKey(d => d.ScriptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ScriptCategorizationPool_ProjectUserScript");

                entity.HasOne(d => d.UserScriptMarkingRef).WithMany(p => p.ScriptCategorizationPools)
                    .HasForeignKey(d => d.UserScriptMarkingRefId)
                    .HasConstraintName("FK_ScriptCategorizationPool_UserScriptMarkingDetails");
            });

            modelBuilder.Entity<ScriptCategorizationPoolArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ScriptCategorizationPool_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.IsDeleted }, "IDX_Marking_ScriptCategorizationPool_ProjectID_QIGID_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CategorizationVersion).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.FinalizedMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.PoolType).HasComment("1-->Standardization Script, 2-->Adtnal.Standardization Script, 3-->BenchMark Script");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.ScriptCategorizationPoolId).HasColumnName("ScriptCategorizationPoolID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
            });

            modelBuilder.Entity<ScriptCategorizationPoolHistory>(entity =>
            {
                entity.ToTable("ScriptCategorizationPoolHistory", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.IsDeleted }, "IDX_Marking_ScriptCategorizationPoolHistory_ProjectID_QIGID_IsDeleted");

                entity.HasIndex(e => new { e.ScriptCategorizationPoolId, e.CategorizationVersion }, "IDX_Marking_ScriptCategorizationPoolHistory_ScriptCategorizationPoolID_CategorizationVersion");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CategorizationVersion).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.FinalizedMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.ScriptCategorizationPoolId).HasColumnName("ScriptCategorizationPoolID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
            });

            modelBuilder.Entity<ScriptMarkingPhaseStatusTracking>(entity =>
            {
                entity.HasKey(e => e.PhaseStatusTrackingId);

                entity.ToTable("ScriptMarkingPhaseStatusTracking", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ActionBy, e.IsActive, e.IsDeleted }, "IDX_Marking_ScriptMarkingPhaseStatusTracking_ProjectID_QIGID_ActionBy_IsActive_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ActionBy, e.IsRcjobRun, e.IsActive, e.IsDeleted }, "IDX_Marking_ScriptMarkingPhaseStatusTracking_ProjectID_QIGID_ActionBy_IsRCJobRun_IsActive_IsDeleted").HasFillFactor(95);

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ScriptId, e.Phase, e.Status, e.IsDeleted }, "IDX_Marking_ScriptMarkingPhaseStatusTracking_ProjectID_QIGID_ScriptID_Phase_Status_IsDeleted");

                entity.HasIndex(e => new { e.Qigid, e.IsDeleted, e.ScriptInitiatedBy }, "IDX_Marking_ScriptMarkingPhaseStatusTracking_QIGID_IsDeleted_ScriptInitiatedBy").HasFillFactor(95);

                entity.HasIndex(e => new { e.Qigid, e.IsDeleted, e.Status }, "IDX_Marking_ScriptMarkingPhaseStatusTracking_QIGID_IsDeleted_Status");

                entity.HasIndex(e => e.IsActive, "IX_ScriptMarkingPhaseStatusTracking_IsActive");

                entity.HasIndex(e => new { e.IsActive, e.IsDeleted, e.IsScriptFinalised }, "IX_ScriptMarkingPhaseStatusTracking_IsActive_IsDeleted_IsScriptFinalised");

                entity.HasIndex(e => new { e.Phase, e.Status }, "IX_ScriptMarkingPhaseStatusTracking_Phase_Status");

                entity.HasIndex(e => new { e.ScriptId, e.IsActive }, "IX_ScriptMarkingPhaseStatusTracking_ScriptID_IsActive");

                entity.HasIndex(e => new { e.ScriptId, e.IsActive, e.IsDeleted }, "IX_ScriptMarkingPhaseStatusTracking_ScriptID_IsActive_IsDeleted");

                entity.HasIndex(e => new { e.ScriptId, e.IsActive, e.IsDeleted, e.IsScriptFinalised }, "IX_ScriptMarkingPhaseStatusTracking_ScriptID_IsActive_IsDeleted_IsScriptFinalised");

                entity.HasIndex(e => new { e.ScriptId, e.Phase, e.Status }, "IX_ScriptMarkingPhaseStatusTracking_ScriptID_Phase_Status");

                entity.Property(e => e.PhaseStatusTrackingId).HasColumnName("PhaseStatusTrackingID");
                entity.Property(e => e.ActionDate).HasColumnType("datetime");
                entity.Property(e => e.AssignedToDateTime).HasColumnType("datetime");
                entity.Property(e => e.GracePeriodEndDateTime).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsRcjobRun).HasColumnName("IsRCJobRun");
                entity.Property(e => e.Phase).HasComment("1 --> Live Marking, 2 --> RC - 1, 3 --> RC - 2, 4 --> Ad-hoc, 5 --> Escalate");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.RcjobRunDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("RCJobRunDateTime");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.Status).HasComment("1 --> Downloaded, 2 --> In Progress, 3 --> Submitted, 4 --> In RC Pool, 5 --> Approved, 6 --> Re-Marking, 7 --> RE-Submitted, 8 --> Escalate, 9 --> Invalidate and Re-Mark, 10 --> Invalidate and Live Pool, 11 --> Return to Live Pool");
                entity.Property(e => e.TotalAwardedMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");

                entity.HasOne(d => d.ActionByNavigation).WithMany(p => p.ScriptMarkingPhaseStatusTrackingActionByNavigations)
                    .HasForeignKey(d => d.ActionBy)
                    .HasConstraintName("FK_ScriptMarkingPhaseStatusTracking_ProjectUserRoleinfo");

                entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.ScriptMarkingPhaseStatusTrackingAssignedToNavigations)
                    .HasForeignKey(d => d.AssignedTo)
                    .HasConstraintName("FK_ScriptMarkingPhaseStatusTracking_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.ScriptMarkingPhaseStatusTrackings)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ScriptMarkingPhaseStatusTracking_ProjectInfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.ScriptMarkingPhaseStatusTrackings)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ScriptMarkingPhaseStatusTracking_ProjectQIG");

                entity.HasOne(d => d.Script).WithMany(p => p.ScriptMarkingPhaseStatusTrackings)
                    .HasForeignKey(d => d.ScriptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ScriptMarkingPhaseStatusTracking_ProjectUserScript");

                entity.HasOne(d => d.UserScriptMarkingRef).WithMany(p => p.ScriptMarkingPhaseStatusTrackings)
                    .HasForeignKey(d => d.UserScriptMarkingRefId)
                    .HasConstraintName("FK_ScriptMarkingPhaseStatusTracking_UserScriptMarkingDetails");
            });

            modelBuilder.Entity<ScriptMarkingPhaseStatusTrackingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("ScriptMarkingPhaseStatusTracking_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ActionBy, e.IsActive, e.IsDeleted }, "IDX_Marking_ScriptMarkingPhaseStatusTracking_ProjectID_QIGID_ActionBy_IsActive_IsDeleted_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ActionBy, e.IsRcjobRun, e.IsActive, e.IsDeleted }, "IDX_Marking_ScriptMarkingPhaseStatusTracking_ProjectID_QIGID_ActionBy_IsRCJobRun_IsActive_IsDeleted_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ScriptId, e.Phase, e.Status, e.IsDeleted }, "IDX_Marking_ScriptMarkingPhaseStatusTracking_ProjectID_QIGID_ScriptID_Phase_Status_IsDeleted_Archive");

                entity.HasIndex(e => new { e.Qigid, e.IsDeleted, e.ScriptInitiatedBy }, "IDX_Marking_ScriptMarkingPhaseStatusTracking_QIGID_IsDeleted_ScriptInitiatedBy_Archive");

                entity.HasIndex(e => new { e.Qigid, e.IsDeleted, e.Status }, "IDX_Marking_ScriptMarkingPhaseStatusTracking_QIGID_IsDeleted_Status_Archive");

                entity.HasIndex(e => e.IsActive, "IX_ScriptMarkingPhaseStatusTracking_IsActive_Archive");

                entity.HasIndex(e => new { e.IsActive, e.IsDeleted, e.IsScriptFinalised }, "IX_ScriptMarkingPhaseStatusTracking_IsActive_IsDeleted_IsScriptFinalised_Archive");

                entity.HasIndex(e => new { e.Phase, e.Status }, "IX_ScriptMarkingPhaseStatusTracking_Phase_Status_Archive");

                entity.HasIndex(e => new { e.ScriptId, e.IsActive }, "IX_ScriptMarkingPhaseStatusTracking_ScriptID_IsActive_Archive");

                entity.HasIndex(e => new { e.ScriptId, e.IsActive, e.IsDeleted }, "IX_ScriptMarkingPhaseStatusTracking_ScriptID_IsActive_IsDeleted_Archive");

                entity.HasIndex(e => new { e.ScriptId, e.IsActive, e.IsDeleted, e.IsScriptFinalised }, "IX_ScriptMarkingPhaseStatusTracking_ScriptID_IsActive_IsDeleted_IsScriptFinalised_Archive");

                entity.HasIndex(e => new { e.ScriptId, e.Phase, e.Status }, "IX_ScriptMarkingPhaseStatusTracking_ScriptID_Phase_Status_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.ActionDate).HasColumnType("datetime");
                entity.Property(e => e.AssignedToDateTime).HasColumnType("datetime");
                entity.Property(e => e.GracePeriodEndDateTime).HasColumnType("datetime");
                entity.Property(e => e.IsRcjobRun).HasColumnName("IsRCJobRun");
                entity.Property(e => e.Phase).HasComment("1 --> Live Marking, 2 --> RC - 1, 3 --> RC - 2, 4 --> Ad-hoc, 5 --> Escalate");
                entity.Property(e => e.PhaseStatusTrackingId).HasColumnName("PhaseStatusTrackingID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.RcjobRunDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("RCJobRunDateTime");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.TotalAwardedMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
            });

            modelBuilder.Entity<ScriptMarkingPhaseStatusTrackingHistory>(entity =>
            {
                entity.ToTable("ScriptMarkingPhaseStatusTrackingHistory", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ActionBy, e.IsActive, e.IsDeleted }, "IDX_Marking_ScriptMarkingPhaseStatusTrackingHistory_ProjectID_QIGID_ActionBy_IsActive_IsDeleted");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ActionDate).HasColumnType("datetime");
                entity.Property(e => e.AssignedToDateTime).HasColumnType("datetime");
                entity.Property(e => e.GracePeriodEndDateTime).HasColumnType("datetime");
                entity.Property(e => e.HistoryCreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsRcjobRun).HasColumnName("IsRCJobRun");
                entity.Property(e => e.Phase).HasComment("1 --> Live Marking, 2 --> RC - 1, 3 --> RC - 2, 4 --> Ad-hoc, 5 --> Escalate");
                entity.Property(e => e.PhaseStatusTrackingId).HasColumnName("PhaseStatusTrackingID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.RcjobRunDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("RCJobRunDateTime");
                entity.Property(e => e.RecordType).HasComment("1-->JOB , 2-->APPLICATION");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.Status).HasComment("1 --> Downloaded, 2 --> In Progress, 3 --> Submitted, 4 --> In RC Pool, 5 --> Approved, 6 --> Re-Marking, 7 --> RE-Submitted, 8 --> Escalate, 9 --> Invalidate and Re-Mark, 10 --> Invalidate and Live Pool, 11 --> Return to Live Pool");
                entity.Property(e => e.TotalAwardedMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
            });

            modelBuilder.Entity<StandardizationQualifyingAssessment>(entity =>
            {
                entity.HasKey(e => e.QualifyingAssessmentId);

                entity.ToTable("StandardizationQualifyingAssessment", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.IsTagged, e.IsActive, e.IsDeleted }, "IDX_Marking_StandardizationQualifyingAssessment_ProjectID_QIGID_IsTagged_IsActive_IsDeleted");

                entity.Property(e => e.QualifyingAssessmentId).HasColumnName("QualifyingAssessmentID");
                entity.Property(e => e.ApprovalType).HasComment("1-->Manual, 2-->Automatic");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.RefQualifyingAssessmentId).HasColumnName("RefQualifyingAssessmentID");
                entity.Property(e => e.ScriptPresentationType).HasComment("1-->Sequential, 2-->Random");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StandardizationQualifyingAssessmentCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_StandardizationQualifyingAssessment_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.StandardizationQualifyingAssessmentModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_StandardizationQualifyingAssessment_ProjectUserRoleinfo1");

                entity.HasOne(d => d.Project).WithMany(p => p.StandardizationQualifyingAssessments)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StandardizationQualifyingAssessment_ProjectInfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.StandardizationQualifyingAssessments)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StandardizationQualifyingAssessment_ProjectQIG");

                entity.HasOne(d => d.RefQualifyingAssessment).WithMany(p => p.InverseRefQualifyingAssessment)
                    .HasForeignKey(d => d.RefQualifyingAssessmentId)
                    .HasConstraintName("FK_StandardizationQualifyingAssessment_StandardizationQualifyingAssessment1");
            });

            modelBuilder.Entity<StandardizationQualifyingAssessmentArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("StandardizationQualifyingAssessment_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.IsTagged, e.IsActive, e.IsDeleted }, "IDX_Marking_StandardizationQualifyingAssessment_ProjectID_QIGID_IsTagged_IsActive_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.QualifyingAssessmentId).HasColumnName("QualifyingAssessmentID");
                entity.Property(e => e.RefQualifyingAssessmentId).HasColumnName("RefQualifyingAssessmentID");
                entity.Property(e => e.ScriptPresentationType).HasComment("1-->Sequential, 2-->Random");
            });

            modelBuilder.Entity<SubjectInfo>(entity =>
            {
                entity.HasKey(e => e.SubjectId).HasName("PK__SubjectI__AC1BA388713F2098");

                entity.ToTable("SubjectInfo", "Marking");

                entity.HasIndex(e => new { e.SubjectCode, e.IsDeleted }, "IDX_Marking_SubjectInfo_SubjectCode_IsDeleted");

                entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.SubjectCode)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.SubjectName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SubjectPaperInfo>(entity =>
            {
                entity.HasKey(e => e.PaperId).HasName("PK__SubjectP__AB86126B3C1A5630");

                entity.ToTable("SubjectPaperInfo", "Marking");

                entity.HasIndex(e => new { e.PaperCode, e.IsDeleted }, "IDX_Marking_SubjectPaperInfo_PaperCode_IsDeleted");

                entity.Property(e => e.PaperId).HasColumnName("PaperID");
                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.PaperCode)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.PaperName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SummaryCandidateQuestionComponentMark>(entity =>
            {
                entity.ToTable("SummaryCandidateQuestionComponentMarks", "Marking");

                entity.HasIndex(e => new { e.ParentQuestionId, e.IsDeleted, e.IsSelectedFromOptionality }, "IDX_Marking_SummaryCandidateQuestionComponentMarks_projectID_ParentQID_IsDeltd_Optionality");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AssessmentId).HasColumnName("AssessmentID");
                entity.Property(e => e.AssessmentVersion).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.BandId).HasColumnName("BandID");
                entity.Property(e => e.ComponentAwardedMarks).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.ComponentMaxMarks).HasColumnType("decimal(6, 2)");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.IsNullResponse).HasDefaultValue(false);
                entity.Property(e => e.IsSelectedFromOptionality).HasDefaultValue(false);
                entity.Property(e => e.MarkSchemeId).HasColumnName("MarkSchemeID");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.ParentQuestionId).HasColumnName("ParentQuestionID");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
                entity.Property(e => e.QuestionVersion).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.ScoreComponentId).HasColumnName("ScoreComponentID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.TotalAwardedMarks).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
            });

            modelBuilder.Entity<SummaryCandidateQuestionComponentMarksArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("SummaryCandidateQuestionComponentMarks_archive", "Marking");

                entity.HasIndex(e => new { e.ParentQuestionId, e.IsDeleted, e.IsSelectedFromOptionality }, "IDX_Marking_SummaryCandidateQuestionComponentMarks_projectID_ParentQID_IsDeltd_Optionality_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AssessmentId).HasColumnName("AssessmentID");
                entity.Property(e => e.AssessmentVersion).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.BandId).HasColumnName("BandID");
                entity.Property(e => e.ComponentAwardedMarks).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.ComponentMaxMarks).HasColumnType("decimal(6, 2)");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.MarkSchemeId).HasColumnName("MarkSchemeID");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.ParentQuestionId).HasColumnName("ParentQuestionID");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
                entity.Property(e => e.QuestionVersion).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.ScoreComponentId).HasColumnName("ScoreComponentID");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.TotalAwardedMarks).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.UserScriptMarkingRefId).HasColumnName("UserScriptMarkingRefID");
            });

            modelBuilder.Entity<SummaryProjectUserResultDetail>(entity =>
            {
                entity.ToTable("SummaryProjectUserResultDetails", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.ProjectQuestionId, e.ReportType, e.ScheduleUserId }, "IDX_Marking_SummaryProjectUserResultDetails_ProjectID_ProjectQuestionID_ReportType_ScheduleUserID");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid }, "IDX_Marking_SummaryProjectUserResultDetails_ProjectID_QIGID");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AssessmentId).HasColumnName("AssessmentID");
                entity.Property(e => e.AssessmentVersion).HasColumnType("decimal(5, 1)");
                entity.Property(e => e.ContentMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsSynced).HasDefaultValue(false);
                entity.Property(e => e.LanguageMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
                entity.Property(e => e.QuestionVersion).HasColumnType("decimal(5, 1)");
                entity.Property(e => e.ReportType).HasComment("1 --> EMS1, 2 --> EMS2,");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");
                entity.Property(e => e.TotalMarks).HasColumnType("decimal(8, 2)");

                entity.HasOne(d => d.Project).WithMany(p => p.SummaryProjectUserResultDetails)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SummaryProjectUserResultDetails_SummaryProjectUserResultDetails");

                entity.HasOne(d => d.ProjectQuestion).WithMany(p => p.SummaryProjectUserResultDetails)
                    .HasForeignKey(d => d.ProjectQuestionId)
                    .HasConstraintName("FK_SummaryProjectUserResultDetails_ProjectQuestions");

                entity.HasOne(d => d.Qig).WithMany(p => p.SummaryProjectUserResultDetails)
                    .HasForeignKey(d => d.Qigid)
                    .HasConstraintName("FK_SummaryProjectUserResultDetails_ProjectQIG");
            });

            modelBuilder.Entity<SummaryProjectUserResultDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("SummaryProjectUserResultDetails_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.ProjectQuestionId, e.ReportType, e.ScheduleUserId }, "IDX_Marking_SummaryProjectUserResultDetails_ProjectID_ProjectQuestionID_ReportType_ScheduleUserID_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid }, "IDX_Marking_SummaryProjectUserResultDetails_ProjectID_QIGID_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AssessmentId).HasColumnName("AssessmentID");
                entity.Property(e => e.AssessmentVersion).HasColumnType("decimal(5, 1)");
                entity.Property(e => e.ContentMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.LanguageMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(8, 2)");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.ProjectQuestionId).HasColumnName("ProjectQuestionID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
                entity.Property(e => e.QuestionVersion).HasColumnType("decimal(5, 1)");
                entity.Property(e => e.ReportType).HasComment("1 --> EMS1, 2 --> EMS2,");
                entity.Property(e => e.ScheduleUserId).HasColumnName("ScheduleUserID");
                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");
                entity.Property(e => e.TotalMarks).HasColumnType("decimal(8, 2)");
            });

            modelBuilder.Entity<TblTopurge>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TblTOPur__3214EC27DBAE11C8");

                entity.ToTable("TblTOPurge", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ObjectId).HasColumnName("ObjectID");
                entity.Property(e => e.SchemaName).HasMaxLength(250);
                entity.Property(e => e.TblName)
                    .HasMaxLength(250)
                    .HasColumnName("TblNAME");
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.ToTable("Template", "Marking");

                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");
                entity.Property(e => e.EventId).HasColumnName("EventID");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.TemplateName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<TemplateUserMapping>(entity =>
            {
                entity.ToTable("TemplateUserMapping", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.SentDateTime).HasColumnType("datetime");
                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<TemplateUserMappingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("TemplateUserMapping_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.SentDateTime).HasColumnType("datetime");
                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<TimeZone>(entity =>
            {
                entity.ToTable("TimeZone", "Marking");

                entity.HasIndex(e => new { e.TimeZoneCode, e.IsActive, e.IsDeleted }, "IDX_Marking_TimeZone_TimeZoneCode_IsActive_IsDeleted");

                entity.Property(e => e.TimeZoneId).HasColumnName("TimeZoneID");
                entity.Property(e => e.BaseUtcoffsetInMin).HasColumnName("BaseUTCOffsetInMin");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.TimeZoneCode)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.TimeZoneName)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK_Marking_UserInfo");

                entity.ToTable("UserInfo", "Marking");

                entity.HasIndex(e => new { e.LoginId, e.IsDeleted, e.IsBlock, e.IsDisable }, "IDX_Marking_UserInfo_LoginID_IsDeleted_IsDisable");

                entity.HasIndex(e => new { e.LoginId, e.Password, e.IsDeleted, e.IsBlock }, "IDX_Marking_UserInfo_LoginID_Password_IsDeleted_IsBlock");

                entity.HasIndex(e => new { e.LoginId, e.Password, e.IsDeleted, e.IsBlock, e.IsFirstTimeLoggedIn }, "IDX_Marking_UserInfo_LoginID_Password_IsDeleted_IsBlock_IsFirstTimeLoggedIn");

                entity.HasIndex(e => e.IsDeleted, "IX_UserInfo_IsDeleted");

                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EmailId)
                    .HasMaxLength(320)
                    .HasColumnName("EmailID");
                entity.Property(e => e.FirstName).HasMaxLength(250);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsApprove).HasDefaultValue(true);
                entity.Property(e => e.IsFirstTimeLoggedIn).HasDefaultValue(true);
                entity.Property(e => e.LastFailedAttempt).HasColumnType("datetime");
                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
                entity.Property(e => e.LastLogoutDate).HasColumnType("datetime");
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.LoginId)
                    .HasMaxLength(320)
                    .HasColumnName("LoginID");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Nric)
                    .HasMaxLength(100)
                    .HasColumnName("NRIC");
                entity.Property(e => e.PassPharaseId).HasColumnName("PassPharaseID");
                entity.Property(e => e.Password).HasMaxLength(50);
                entity.Property(e => e.PasswordLastModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.PhoneNumber).HasMaxLength(50);
                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.HasOne(d => d.PassPharase).WithMany(p => p.UserInfos)
                    .HasForeignKey(d => d.PassPharaseId)
                    .HasConstraintName("FK_UserInfo_PassPhrase");

                entity.HasOne(d => d.School).WithMany(p => p.UserInfos)
                    .HasForeignKey(d => d.SchoolId)
                    .HasConstraintName("FK_UserInfo_SchoolInfo");
            });

            modelBuilder.Entity<UserInfoArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId).HasName("PK_Marking_UserInfo_Archive");

                entity.ToTable("UserInfo_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.EmailId)
                    .HasMaxLength(320)
                    .HasColumnName("EmailID");
                entity.Property(e => e.FirstName).HasMaxLength(250);
                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
                entity.Property(e => e.LastLogoutDate).HasColumnType("datetime");
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.LoginId)
                    .HasMaxLength(320)
                    .HasColumnName("LoginID");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Nric)
                    .HasMaxLength(100)
                    .HasColumnName("NRIC");
                entity.Property(e => e.PassPharaseId).HasColumnName("PassPharaseID");
                entity.Property(e => e.Password).HasMaxLength(50);
                entity.Property(e => e.PasswordLastModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.PhoneNumber).HasMaxLength(50);
                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserLoginToken>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.ToTable("UserLoginToken", "Marking");

                entity.HasIndex(e => new { e.UserId, e.IsExpired, e.Expires }, "IDX_Marking_UserID_IsExpired_Expires");

                entity.HasIndex(e => new { e.RefreshToken, e.IsDeleted }, "IDX_Marking_UserLoginToken_RefreshToken_IsDeleted");

                entity.HasIndex(e => new { e.UserId, e.IsDeleted }, "IDX_Marking_UserLoginToken_UserID_Isdeleted");

                entity.HasIndex(e => new { e.UserId, e.LoginId }, "IDX_Marking_UserLoginToken_UserID_LoginID");

                entity.Property(e => e.TokenId).HasColumnName("TokenID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Expires).HasColumnType("datetime");
                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(50)
                    .HasColumnName("IPAddress");
                entity.Property(e => e.IsActive).HasDefaultValue(false);
                entity.Property(e => e.LoginId)
                    .HasMaxLength(320)
                    .HasColumnName("LoginID");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.RefreshToken).HasMaxLength(200);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(200);
                entity.Property(e => e.Revoked).HasColumnType("datetime");
                entity.Property(e => e.SessionId).HasMaxLength(100);
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserLoginTokenArchive>(entity =>
            {
                entity.ToTable("UserLoginToken_Archive", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Expires).HasColumnType("datetime");
                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(50)
                    .HasColumnName("IPAddress");
                entity.Property(e => e.IsActive).HasDefaultValue(false);
                entity.Property(e => e.LoginId)
                    .HasMaxLength(320)
                    .HasColumnName("LoginID");
                entity.Property(e => e.RefreshToken).HasMaxLength(200);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(200);
                entity.Property(e => e.Revoked).HasColumnType("datetime");
                entity.Property(e => e.SessionId).HasMaxLength(100);
                entity.Property(e => e.TokenId).HasColumnName("TokenID");
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserPwdDetail>(entity =>
            {
                entity.ToTable("UserPwdDetails", "Marking");

                entity.Property(e => e.UserPwdDetailId).HasColumnName("UserPwdDetailID");
                entity.Property(e => e.ActivationEnddate).HasColumnType("datetime");
                entity.Property(e => e.ActivationStartdate).HasColumnType("datetime");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User).WithMany(p => p.UserPwdDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserPwdDetails_UserInfo");
            });

            modelBuilder.Entity<UserPwdDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("UserPwdDetails_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.ActivationEnddate).HasColumnType("datetime");
                entity.Property(e => e.ActivationStartdate).HasColumnType("datetime");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.UserPwdDetailId).HasColumnName("UserPwdDetailID");
            });

            modelBuilder.Entity<UserResponseFrequencyDistribution>(entity =>
            {
                entity.ToTable("UserResponseFrequencyDistribution", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ParentQuestionId, e.IsDeleted }, "IDX_Marking_UserResponseFrequencyDistribution_ProjectID_QIGID_ParentQuestionID_IsDeleted");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AutoMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.AwardedMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DiscrepancyStatus).HasComment("1 --> Resolved , 2 --> Accepeted");
                entity.Property(e => e.MarkingType).HasComment("1--> Auto , 2--> Moderated , 3 --> Manual");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.ModeratedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ParentQuestionId).HasColumnName("ParentQuestionID");
                entity.Property(e => e.PercentageDistribution).HasColumnType("decimal(14, 2)");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.QuestionGuid)
                    .HasMaxLength(100)
                    .HasColumnName("QuestionGUID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserResponseFrequencyDistributionCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_UserResponseFrequencyDistribution_ProjectUserRoleinfo1");

                entity.HasOne(d => d.ModeratedByNavigation).WithMany(p => p.UserResponseFrequencyDistributionModeratedByNavigations)
                    .HasForeignKey(d => d.ModeratedBy)
                    .HasConstraintName("FK_UserResponseFrequencyDistribution_ProjectUserRoleinfo");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.UserResponseFrequencyDistributionModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_UserResponseFrequencyDistribution_ProjectUserRoleinfo2");

                entity.HasOne(d => d.ParentQuestion).WithMany(p => p.UserResponseFrequencyDistributionParentQuestions)
                    .HasForeignKey(d => d.ParentQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserResponseFrequencyDistribution_ProjectQuestions");

                entity.HasOne(d => d.Project).WithMany(p => p.UserResponseFrequencyDistributions)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserResponseFrequencyDistribution_ProjectInfo");

                entity.HasOne(d => d.Qig).WithMany(p => p.UserResponseFrequencyDistributions)
                    .HasForeignKey(d => d.Qigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserResponseFrequencyDistribution_ProjectQIG");

                entity.HasOne(d => d.Question).WithMany(p => p.UserResponseFrequencyDistributionQuestions)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserResponseFrequencyDistribution_ProjectQuestions1");
            });

            modelBuilder.Entity<UserResponseFrequencyDistributionArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("UserResponseFrequencyDistribution_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectId, e.Qigid, e.ParentQuestionId, e.IsDeleted }, "IDX_Marking_UserResponseFrequencyDistribution_ProjectID_QIGID_ParentQuestionID_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AutoMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.AwardedMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.MarkingType).HasComment("1--> Auto , 2--> Moderated , 3 --> Manual");
                entity.Property(e => e.MaxMarks).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.ModeratedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.ParentQuestionId).HasColumnName("ParentQuestionID");
                entity.Property(e => e.PercentageDistribution).HasColumnType("decimal(14, 2)");
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
                entity.Property(e => e.Qigid).HasColumnName("QIGID");
                entity.Property(e => e.QuestionGuid)
                    .HasMaxLength(100)
                    .HasColumnName("QuestionGUID");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            });

            modelBuilder.Entity<UserScriptMarkingDetail>(entity =>
            {
                entity.ToTable("UserScriptMarkingDetails", "Marking");

                entity.HasIndex(e => new { e.ScriptId, e.ProjectId, e.WorkFlowStatusId, e.MarkedBy, e.IsActive, e.IsDeleted }, "IDX_Marking_UserScriptMarkingDetails_ScriptID_ProjectId_WorkFlowStatusID_MarkedBy_IsActive_IsDeleted");

                entity.HasIndex(e => new { e.MarkedBy, e.IsDeleted, e.IsActive }, "IX_UserScriptMarkingDetails_MarkedBy_IsDeleted_IsActive");

                entity.HasIndex(e => e.ProjectId, "IX_UserScriptMarkingDetails_ProjectId");

                entity.HasIndex(e => new { e.ProjectId, e.IsDeleted, e.IsActive, e.WorkFlowStatusId }, "IX_UserScriptMarkingDetails_ProjectId_IsDeleted_IsActive_WorkFlowStatusID");

                entity.HasIndex(e => new { e.ProjectId, e.ScriptMarkingStatus, e.IsDeleted, e.IsActive, e.WorkFlowStatusId }, "IX_UserScriptMarkingDetails_ProjectId_ScriptMarkingStatus_IsDeleted_IsActive_WorkFlowStatusID");

                entity.HasIndex(e => new { e.WorkFlowStatusId, e.IsDeleted }, "IX_UserScriptMarkingDetails_WorkFlowStatusID_IsDeleted");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.AnnotationTemplateId).HasColumnName("AnnotationTemplateID");
                entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
                entity.Property(e => e.MarkedDate).HasColumnType("datetime");
                entity.Property(e => e.MarkedType).HasComment("1--> Auto , 2--> Moderated , 3 --> Manual, 4-> Post Live Marking Moderation");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.ScriptMarkingStatus).HasComment("1-->In Progress, 2-->Completed");
                entity.Property(e => e.SelectedDate).HasColumnType("datetime");
                entity.Property(e => e.TotalMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.WorkFlowStatusId).HasColumnName("WorkFlowStatusID");

                entity.HasOne(d => d.AnnotationTemplate).WithMany(p => p.UserScriptMarkingDetails)
                    .HasForeignKey(d => d.AnnotationTemplateId)
                    .HasConstraintName("FK_UserScriptMarkingDetails_AnnotationTemplate");

                entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.UserScriptMarkingDetailApprovedByNavigations)
                    .HasForeignKey(d => d.ApprovedBy)
                    .HasConstraintName("FK_UserScriptMarkingDetails_ProjectUserRoleinfo2");

                entity.HasOne(d => d.MarkedByNavigation).WithMany(p => p.UserScriptMarkingDetailMarkedByNavigations)
                    .HasForeignKey(d => d.MarkedBy)
                    .HasConstraintName("FK_UserScriptMarkingDetails_ProjectUserRoleinfo");

                entity.HasOne(d => d.Project).WithMany(p => p.UserScriptMarkingDetails)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserScriptMarkingDetails_ProjectInfo");

                entity.HasOne(d => d.Script).WithMany(p => p.UserScriptMarkingDetails)
                    .HasForeignKey(d => d.ScriptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserScriptMarkingDetails_ProjectUserScript");

                entity.HasOne(d => d.SelectedByNavigation).WithMany(p => p.UserScriptMarkingDetailSelectedByNavigations)
                    .HasForeignKey(d => d.SelectedBy)
                    .HasConstraintName("FK_UserScriptMarkingDetails_ProjectUserRoleinfo1");

                entity.HasOne(d => d.WorkFlowStatus).WithMany(p => p.UserScriptMarkingDetails)
                    .HasForeignKey(d => d.WorkFlowStatusId)
                    .HasConstraintName("FK_UserScriptMarkingDetails_WorkflowStatus");
            });

            modelBuilder.Entity<UserScriptMarkingDetails03072024>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("UserScriptMarkingDetails_03072024", "Marking");

                entity.HasIndex(e => e.MarkedDate, "IX_TABLE1_partitioncol").IsClustered();

                entity.Property(e => e.AnnotationTemplateId).HasColumnName("AnnotationTemplateID");
                entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");
                entity.Property(e => e.MarkedDate).HasColumnType("datetime");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.SelectedDate).HasColumnType("datetime");
                entity.Property(e => e.TotalMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.WorkFlowStatusId).HasColumnName("WorkFlowStatusID");
            });

            modelBuilder.Entity<UserScriptMarkingDetailsArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("UserScriptMarkingDetails_Archive", "Marking");

                entity.HasIndex(e => new { e.ScriptId, e.ProjectId, e.WorkFlowStatusId, e.MarkedBy, e.IsActive, e.IsDeleted }, "IDX_Marking_UserScriptMarkingDetails_ScriptID_ProjectId_WorkFlowStatusID_MarkedBy_IsActive_IsDeleted_Archive");

                entity.HasIndex(e => new { e.MarkedBy, e.IsDeleted, e.IsActive }, "IX_UserScriptMarkingDetails_MarkedBy_IsDeleted_IsActive_Archive");

                entity.HasIndex(e => e.ProjectId, "IX_UserScriptMarkingDetails_ProjectId_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.IsDeleted, e.IsActive, e.WorkFlowStatusId }, "IX_UserScriptMarkingDetails_ProjectId_IsDeleted_IsActive_WorkFlowStatusID_Archive");

                entity.HasIndex(e => new { e.ProjectId, e.ScriptMarkingStatus, e.IsDeleted, e.IsActive, e.WorkFlowStatusId }, "IX_UserScriptMarkingDetails_ProjectId_ScriptMarkingStatus_IsDeleted_IsActive_WorkFlowStatusID_Archive");

                entity.HasIndex(e => new { e.WorkFlowStatusId, e.IsDeleted }, "IX_UserScriptMarkingDetails_WorkFlowStatusID_IsDeleted_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.AnnotationTemplateId).HasColumnName("AnnotationTemplateID");
                entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.MarkedDate).HasColumnType("datetime");
                entity.Property(e => e.ScriptId).HasColumnName("ScriptID");
                entity.Property(e => e.ScriptMarkingStatus).HasComment("1-->In Progress, 2-->Completed");
                entity.Property(e => e.SelectedDate).HasColumnType("datetime");
                entity.Property(e => e.TotalMarks).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.WorkFlowStatusId).HasColumnName("WorkFlowStatusID");
            });

            modelBuilder.Entity<UserStatusTracking>(entity =>
            {
                entity.HasKey(e => e.UserTrackingId);

                entity.ToTable("UserStatusTracking", "Marking");

                entity.HasIndex(e => new { e.ProjectUserRoleId, e.StatusLevel, e.Status }, "IDX_Marking_UserStatusTracking_ProjectUserRoleID_StatusLevel_Status").HasFillFactor(95);

                entity.HasIndex(e => new { e.UserId, e.StatusLevel, e.Status }, "IDX_Marking_UserStatusTracking_UserID_StatusLevel_Status");

                entity.Property(e => e.UserTrackingId).HasColumnName("UserTrackingID");
                entity.Property(e => e.ActionByProjectUserRoleId).HasColumnName("ActionByProjectUserRoleID");
                entity.Property(e => e.ActionByUserId).HasColumnName("ActionByUserID");
                entity.Property(e => e.ActionDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Remarks).HasMaxLength(2000);
                entity.Property(e => e.Status).HasComment("1 --> Disable, 2 --> Map, 3 --> Un-Map, 4 --> Block,5 --> Unblock,6 --> Active,7 --> InActive,8--> Remove,9 --> Promotion,10-->Suspended,11--> Resume, 12 -> Tag, 13 -> Untag, 14 -> Retag, 15 -> PasswordReset");
                entity.Property(e => e.StatusLevel).HasComment("1--> Application Level , 2 --> Project Level");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.ActionByProjectUserRole).WithMany(p => p.UserStatusTrackingActionByProjectUserRoles)
                    .HasForeignKey(d => d.ActionByProjectUserRoleId)
                    .HasConstraintName("FK_UserStatusTracking_ProjectUserRoleinfo1");

                entity.HasOne(d => d.ActionByUser).WithMany(p => p.UserStatusTrackingActionByUsers)
                    .HasForeignKey(d => d.ActionByUserId)
                    .HasConstraintName("FK_UserStatusTracking_UserInfo1");

                entity.HasOne(d => d.ProjectUserRole).WithMany(p => p.UserStatusTrackingProjectUserRoles)
                    .HasForeignKey(d => d.ProjectUserRoleId)
                    .HasConstraintName("FK_UserStatusTracking_ProjectUserRoleinfo");

                entity.HasOne(d => d.User).WithMany(p => p.UserStatusTrackingUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserStatusTracking_UserInfo");
            });

            modelBuilder.Entity<UserStatusTrackingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("UserStatusTracking_Archive", "Marking");

                entity.HasIndex(e => new { e.ProjectUserRoleId, e.StatusLevel, e.Status }, "IDX_Marking_UserStatusTracking_ProjectUserRoleID_StatusLevel_Status_Archive");

                entity.HasIndex(e => new { e.UserId, e.StatusLevel, e.Status }, "IDX_Marking_UserStatusTracking_UserID_StatusLevel_Status_Archive");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.ActionByProjectUserRoleId).HasColumnName("ActionByProjectUserRoleID");
                entity.Property(e => e.ActionByUserId).HasColumnName("ActionByUserID");
                entity.Property(e => e.ActionDate).HasColumnType("datetime");
                entity.Property(e => e.ProjectUserRoleId).HasColumnName("ProjectUserRoleID");
                entity.Property(e => e.Remarks).HasMaxLength(2000);
                entity.Property(e => e.Status).HasComment("1 --> Disable, 2 --> Map, 3 --> Un-Map, 4 --> Block,5 --> Unblock,6 --> Active,7 --> InActive,8--> Remove,9 --> Promotion,10-->Suspended,11--> Resume, 12 -> Tag, 13 -> Untag, 14 -> Retag, 15 -> PasswordReset");
                entity.Property(e => e.StatusLevel).HasComment("1--> Application Level , 2 --> Project Level");
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.UserTrackingId).HasColumnName("UserTrackingID");
            });

            modelBuilder.Entity<UserToExamLevelMapping>(entity =>
            {
                entity.HasKey(e => e.UserToLevelMappingId);

                entity.ToTable("UserToExamLevelMapping", "Marking");

                entity.HasIndex(e => new { e.UserId, e.ExamLevelId, e.IsDeleted }, "IDX_Marking_UserToExamLevelMapping_UserID_ExamLevelID_IsDeleted");

                entity.Property(e => e.UserToLevelMappingId).HasColumnName("UserToLevelMappingID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ExamLevelId).HasColumnName("ExamLevelID");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserToExamLevelMappingCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_UserToExamLevelMapping_UserInfo1");

                entity.HasOne(d => d.ExamLevel).WithMany(p => p.UserToExamLevelMappings)
                    .HasForeignKey(d => d.ExamLevelId)
                    .HasConstraintName("FK_UserToExamLevelMapping_ExamLevel");

                entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.UserToExamLevelMappingModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_UserToExamLevelMapping_UserInfo2");

                entity.HasOne(d => d.User).WithMany(p => p.UserToExamLevelMappingUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserToExamLevelMapping_UserInfo");
            });

            modelBuilder.Entity<UserToExamLevelMappingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("UserToExamLevelMapping_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ExamLevelId).HasColumnName("ExamLevelID");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.UserToLevelMappingId).HasColumnName("UserToLevelMappingID");
            });

            modelBuilder.Entity<UserToOrganizationMapping>(entity =>
            {
                entity.HasKey(e => e.OrganizationUserId);

                entity.ToTable("UserToOrganizationMapping", "Marking");

                entity.HasIndex(e => new { e.UserId, e.OrganizationId, e.IsDeleted }, "IDX_Marking_UserToOrganizationMapping_UserID_OrganizationID_IsDeleted");

                entity.Property(e => e.OrganizationUserId).HasColumnName("OrganizationUserID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Organization).WithMany(p => p.UserToOrganizationMappings)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_UserToOrganizationMapping_Organization");

                entity.HasOne(d => d.User).WithMany(p => p.UserToOrganizationMappings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserToOrganizationMapping_UserInfo");
            });

            modelBuilder.Entity<UserToOrganizationMappingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("UserToOrganizationMapping_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
                entity.Property(e => e.OrganizationUserId).HasColumnName("OrganizationUserID");
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserToRoleMapping>(entity =>
            {
                entity.HasKey(e => e.MappingId);

                entity.ToTable("UserToRoleMapping", "Marking");

                entity.HasIndex(e => new { e.UserId, e.RoleId, e.IsDeleted }, "IDX_Marking_UserToRoleMapping_UserID_RoleID_IsDeleted");

                entity.Property(e => e.MappingId).HasColumnName("MappingID");
                entity.Property(e => e.OrganizationUserId).HasColumnName("OrganizationUserID");
                entity.Property(e => e.RoleId).HasColumnName("RoleID");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.OrganizationUser).WithMany(p => p.UserToRoleMappings)
                    .HasForeignKey(d => d.OrganizationUserId)
                    .HasConstraintName("FK_UserToRoleMapping_UserToOrganizationMapping");

                entity.HasOne(d => d.Role).WithMany(p => p.UserToRoleMappings)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_UserToRoleMapping_Roleinfo");

                entity.HasOne(d => d.User).WithMany(p => p.UserToRoleMappings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserToRoleMapping_UserInfo");
            });

            modelBuilder.Entity<UserToRoleMappingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("UserToRoleMapping_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.MappingId).HasColumnName("MappingID");
                entity.Property(e => e.OrganizationUserId).HasColumnName("OrganizationUserID");
                entity.Property(e => e.RoleId).HasColumnName("RoleID");
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserToTimeZoneMapping>(entity =>
            {
                entity.HasKey(e => e.TimeZoneMappingId);

                entity.ToTable("UserToTimeZoneMapping", "Marking");

                entity.HasIndex(e => new { e.UserId, e.TimeZoneId, e.IsActive, e.IsDeleted }, "IDX_Marking_UserToTimeZoneMapping_UserID_TimeZoneID_IsActive_IsDeleted");

                entity.Property(e => e.TimeZoneMappingId).HasColumnName("TimeZoneMappingID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.TimeZoneId).HasColumnName("TimeZoneID");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.TimeZone).WithMany(p => p.UserToTimeZoneMappings)
                    .HasForeignKey(d => d.TimeZoneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserToTimeZoneMapping_TimeZone");

                entity.HasOne(d => d.User).WithMany(p => p.UserToTimeZoneMappings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserToTimeZoneMapping_UserInfo");
            });

            modelBuilder.Entity<UserToTimeZoneMappingArchive>(entity =>
            {
                entity.HasKey(e => e.ArchiveId);

                entity.ToTable("UserToTimeZoneMapping_Archive", "Marking");

                entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.TimeZoneId).HasColumnName("TimeZoneID");
                entity.Property(e => e.TimeZoneMappingId).HasColumnName("TimeZoneMappingID");
                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<Userinfo18122023>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("Userinfo18122023", "Marking");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.EmailId)
                    .HasMaxLength(320)
                    .HasColumnName("EmailID");
                entity.Property(e => e.FirstName).HasMaxLength(250);
                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
                entity.Property(e => e.LastLogoutDate).HasColumnType("datetime");
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.LoginId)
                    .HasMaxLength(320)
                    .HasColumnName("LoginID");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Nric)
                    .HasMaxLength(100)
                    .HasColumnName("NRIC");
                entity.Property(e => e.PassPharaseId).HasColumnName("PassPharaseID");
                entity.Property(e => e.Password).HasMaxLength(50);
                entity.Property(e => e.PasswordLastModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.PhoneNumber).HasMaxLength(50);
                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");
                entity.Property(e => e.UserId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("UserID");
            });

            modelBuilder.Entity<ValidateCaptcha>(entity =>
            {
                entity.ToTable("ValidateCaptcha", "Marking");

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Captcha).HasMaxLength(80);
                entity.Property(e => e.CaptchaGuid)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("Captcha_GUID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ValidatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<WorkflowStatus>(entity =>
            {
                entity.HasKey(e => e.WorkflowId).HasName("PK_Workflow");

                entity.ToTable("WorkflowStatus", "Marking");

                entity.HasIndex(e => new { e.WorkflowCode, e.WorkflowType, e.IsDeleted }, "IDX_Marking_WorkflowStatus_WorkflowCode_WorkflowType_IsDeleted");

                entity.Property(e => e.WorkflowId).HasColumnName("WorkflowID");
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getutcdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.WorkflowCode).HasMaxLength(50);
                entity.Property(e => e.WorkflowName).HasMaxLength(200);
                entity.Property(e => e.WorkflowType).HasComment("1--->Script,  2--->Project, 3--> Categorization, 4--> QIG");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
