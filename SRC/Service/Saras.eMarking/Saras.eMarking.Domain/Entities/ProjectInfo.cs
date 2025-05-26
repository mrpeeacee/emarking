using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectInfo
{
    public long ProjectId { get; set; }

    public string ProjectCode { get; set; }

    public string ProjectName { get; set; }

    public short? SubjectId { get; set; }

    public short? PaperId { get; set; }

    public short ExamYear { get; set; }

    public short ExamseriesId { get; set; }

    public short ExamLevelId { get; set; }

    public short Moa { get; set; }

    public byte? CreationType { get; set; }

    public long? OrganizationId { get; set; }

    /// <summary>
    /// 0 --&gt; Not Started, 1 --&gt; In- Progress, 2 --&gt; Completed, 3 --&gt; Closed, 4 --&gt; Reopened
    /// </summary>
    public byte ProjectStatus { get; set; }

    public DateTime? ProjectStartDate { get; set; }

    public DateTime? ProjectEndDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreateDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string ProjectInfo1 { get; set; }

    public long? ProductId { get; set; }

    public long? AssessmentId { get; set; }

    public bool IsQuestionXmlexist { get; set; }

    public bool IsScriptImported { get; set; }

    public bool IsArchive { get; set; }

    public DateTime? ArchiveDate { get; set; }

    public virtual ICollection<ApireportRequest> ApireportRequests { get; set; } = new List<ApireportRequest>();

    public virtual ICollection<AppSetting> AppSettings { get; set; } = new List<AppSetting>();

    public virtual ExamLevel ExamLevel { get; set; }

    public virtual ExamSeries Examseries { get; set; }

    public virtual ICollection<MarkingScriptTimeTracking> MarkingScriptTimeTrackings { get; set; } = new List<MarkingScriptTimeTracking>();

    public virtual ModeOfAssessment MoaNavigation { get; set; }

    public virtual ICollection<MpstandardizationQueRespMarkingDetail> MpstandardizationQueRespMarkingDetails { get; set; } = new List<MpstandardizationQueRespMarkingDetail>();

    public virtual ICollection<MpstandardizationSchedule> MpstandardizationSchedules { get; set; } = new List<MpstandardizationSchedule>();

    public virtual ICollection<MpstandardizationScriptMarkingDetail> MpstandardizationScriptMarkingDetails { get; set; } = new List<MpstandardizationScriptMarkingDetail>();

    public virtual ICollection<MpstandardizationSummary> MpstandardizationSummaries { get; set; } = new List<MpstandardizationSummary>();

    public virtual SubjectPaperInfo Paper { get; set; }

    public virtual ICollection<ProjectCandidateWithdraw> ProjectCandidateWithdraws { get; set; } = new List<ProjectCandidateWithdraw>();

    public virtual ICollection<ProjectCenter> ProjectCenters { get; set; } = new List<ProjectCenter>();

    public virtual ICollection<ProjectFile> ProjectFiles { get; set; } = new List<ProjectFile>();

    public virtual ICollection<ProjectMarkSchemeQuestion> ProjectMarkSchemeQuestions { get; set; } = new List<ProjectMarkSchemeQuestion>();

    public virtual ICollection<ProjectMarkSchemeTemplate> ProjectMarkSchemeTemplates { get; set; } = new List<ProjectMarkSchemeTemplate>();

    public virtual ICollection<ProjectQigcenterMapping> ProjectQigcenterMappings { get; set; } = new List<ProjectQigcenterMapping>();

    public virtual ICollection<ProjectQigreset> ProjectQigresets { get; set; } = new List<ProjectQigreset>();

    public virtual ICollection<ProjectQig> ProjectQigs { get; set; } = new List<ProjectQig>();

    public virtual ICollection<ProjectQigscriptsImportEvent> ProjectQigscriptsImportEvents { get; set; } = new List<ProjectQigscriptsImportEvent>();

    public virtual ICollection<ProjectQigteamHierarchy> ProjectQigteamHierarchies { get; set; } = new List<ProjectQigteamHierarchy>();

    public virtual ICollection<ProjectQuestion> ProjectQuestions { get; set; } = new List<ProjectQuestion>();

    public virtual ICollection<ProjectQuestionsHistory> ProjectQuestionsHistories { get; set; } = new List<ProjectQuestionsHistory>();

    public virtual ICollection<ProjectSchedule> ProjectSchedules { get; set; } = new List<ProjectSchedule>();

    public virtual ICollection<ProjectTeamQig> ProjectTeamQigs { get; set; } = new List<ProjectTeamQig>();

    public virtual ICollection<ProjectTeam> ProjectTeams { get; set; } = new List<ProjectTeam>();

    public virtual ICollection<ProjectUserQuestionResponse> ProjectUserQuestionResponses { get; set; } = new List<ProjectUserQuestionResponse>();

    public virtual ICollection<ProjectUserRoleinfo> ProjectUserRoleinfos { get; set; } = new List<ProjectUserRoleinfo>();

    public virtual ICollection<ProjectUserScript> ProjectUserScripts { get; set; } = new List<ProjectUserScript>();

    public virtual ICollection<ScriptCategorizationPool> ScriptCategorizationPools { get; set; } = new List<ScriptCategorizationPool>();

    public virtual ICollection<ScriptMarkingPhaseStatusTracking> ScriptMarkingPhaseStatusTrackings { get; set; } = new List<ScriptMarkingPhaseStatusTracking>();

    public virtual ICollection<StandardizationQualifyingAssessment> StandardizationQualifyingAssessments { get; set; } = new List<StandardizationQualifyingAssessment>();

    public virtual SubjectInfo Subject { get; set; }

    public virtual ICollection<SummaryProjectUserResultDetail> SummaryProjectUserResultDetails { get; set; } = new List<SummaryProjectUserResultDetail>();

    public virtual ICollection<UserResponseFrequencyDistribution> UserResponseFrequencyDistributions { get; set; } = new List<UserResponseFrequencyDistribution>();

    public virtual ICollection<UserScriptMarkingDetail> UserScriptMarkingDetails { get; set; } = new List<UserScriptMarkingDetail>();
}
