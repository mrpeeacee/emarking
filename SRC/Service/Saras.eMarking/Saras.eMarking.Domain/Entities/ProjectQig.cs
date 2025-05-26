using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQig
{
    public long ProjectQigid { get; set; }

    public string Qigcode { get; set; }

    public string Qigname { get; set; }

    public int? QuestionsType { get; set; }

    public int NoOfQuestions { get; set; }

    public bool IsAllQuestionMandatory { get; set; }

    public int NoofMandatoryQuestion { get; set; }

    public decimal? TotalMarks { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? ProjectId { get; set; }

    public bool IsManualMarkingRequired { get; set; }

    public long? ActionBy { get; set; }

    public DateTime? ActionDate { get; set; }

    /// <summary>
    /// 1---&gt;MCQ,  2---&gt;Composition , 3--&gt; Non-Composition
    /// </summary>
    public byte? Qigtype { get; set; }

    /// <summary>
    /// 1 --&gt; Partial Manual Marking, 2 --&gt; Complete Manual Marking 
    /// </summary>
    public byte? ResponseProcessingType { get; set; }

    public virtual ProjectUserRoleinfo ActionByNavigation { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ICollection<MarkingScriptTimeTracking> MarkingScriptTimeTrackings { get; set; } = new List<MarkingScriptTimeTracking>();

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ICollection<MpstandardizationQueRespMarkingDetail> MpstandardizationQueRespMarkingDetails { get; set; } = new List<MpstandardizationQueRespMarkingDetail>();

    public virtual ICollection<MpstandardizationSchedule> MpstandardizationSchedules { get; set; } = new List<MpstandardizationSchedule>();

    public virtual ICollection<MpstandardizationScriptMarkingDetail> MpstandardizationScriptMarkingDetails { get; set; } = new List<MpstandardizationScriptMarkingDetail>();

    public virtual ICollection<MpstandardizationSummary> MpstandardizationSummaries { get; set; } = new List<MpstandardizationSummary>();

    public virtual ProjectInfo Project { get; set; }

    public virtual ICollection<ProjectQigcenterMapping> ProjectQigcenterMappings { get; set; } = new List<ProjectQigcenterMapping>();

    public virtual ICollection<ProjectQigquestion> ProjectQigquestions { get; set; } = new List<ProjectQigquestion>();

    public virtual ICollection<ProjectQigreset> ProjectQigresets { get; set; } = new List<ProjectQigreset>();

    public virtual ICollection<ProjectQigscriptsImportEvent> ProjectQigscriptsImportEvents { get; set; } = new List<ProjectQigscriptsImportEvent>();

    public virtual ICollection<ProjectQigteamHierarchy> ProjectQigteamHierarchies { get; set; } = new List<ProjectQigteamHierarchy>();

    public virtual ICollection<ProjectTeamQig> ProjectTeamQigs { get; set; } = new List<ProjectTeamQig>();

    public virtual ICollection<ProjectUserScript> ProjectUserScripts { get; set; } = new List<ProjectUserScript>();

    public virtual ICollection<QigstandardizationScriptSetting> QigstandardizationScriptSettings { get; set; } = new List<QigstandardizationScriptSetting>();

    public virtual ICollection<QigtoAnnotationTemplateMapping> QigtoAnnotationTemplateMappings { get; set; } = new List<QigtoAnnotationTemplateMapping>();

    public virtual ICollection<ScriptCategorizationPool> ScriptCategorizationPools { get; set; } = new List<ScriptCategorizationPool>();

    public virtual ICollection<ScriptMarkingPhaseStatusTracking> ScriptMarkingPhaseStatusTrackings { get; set; } = new List<ScriptMarkingPhaseStatusTracking>();

    public virtual ICollection<StandardizationQualifyingAssessment> StandardizationQualifyingAssessments { get; set; } = new List<StandardizationQualifyingAssessment>();

    public virtual ICollection<SummaryProjectUserResultDetail> SummaryProjectUserResultDetails { get; set; } = new List<SummaryProjectUserResultDetail>();

    public virtual ICollection<UserResponseFrequencyDistribution> UserResponseFrequencyDistributions { get; set; } = new List<UserResponseFrequencyDistribution>();
}
