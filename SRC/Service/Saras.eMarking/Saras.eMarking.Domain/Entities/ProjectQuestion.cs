using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQuestion
{
    public long ProjectQuestionId { get; set; }

    public long ProjectId { get; set; }

    public long QuestionId { get; set; }

    public string QuestionCode { get; set; }

    public long? SectionId { get; set; }

    public int? QuestionOrder { get; set; }

    public int? QuestionType { get; set; }

    public bool IsDeleted { get; set; }

    public string QuestionText { get; set; }

    public decimal? QuestionMarks { get; set; }

    public int? ToleranceLimit { get; set; }

    public bool IsScoreComponentExists { get; set; }

    public decimal? StepValue { get; set; }

    public decimal? QuestionVersion { get; set; }

    public string QuestionGuid { get; set; }

    public string ParentQuestionGuid { get; set; }

    public long? ParentQuestionId { get; set; }

    public string QuestionXml { get; set; }

    public bool IsChildExist { get; set; }

    /// <summary>
    /// 1---&gt;MCQ,  2---&gt;Composition , 3--&gt; Non-Composition
    /// </summary>
    public byte? MarkingQuestionType { get; set; }

    public bool IsCaseSensitive { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? PassageId { get; set; }

    public decimal? PassageVersion { get; set; }

    public string PassageCode { get; set; }

    public string PassageLabel { get; set; }

    public string PassageXml { get; set; }

    /// <summary>
    /// 1 --&gt; Partial Manual Marking, 2 --&gt; Complete Manual Marking
    /// </summary>
    public byte? ResponseProcessingType { get; set; }

    public string DirectionCode { get; set; }

    public bool? IsComposite { get; set; }

    public virtual ICollection<MarkingScriptTimeTracking> MarkingScriptTimeTrackings { get; set; } = new List<MarkingScriptTimeTracking>();

    public virtual ProjectInfo Project { get; set; }

    public virtual ICollection<ProjectMarkSchemeQuestion> ProjectMarkSchemeQuestions { get; set; } = new List<ProjectMarkSchemeQuestion>();

    public virtual ICollection<ProjectQigquestion> ProjectQigquestions { get; set; } = new List<ProjectQigquestion>();

    public virtual ICollection<ProjectQuestionAsset> ProjectQuestionAssets { get; set; } = new List<ProjectQuestionAsset>();

    public virtual ICollection<ProjectQuestionChoiceMapping> ProjectQuestionChoiceMappings { get; set; } = new List<ProjectQuestionChoiceMapping>();

    public virtual ICollection<ProjectQuestionScoreComponent> ProjectQuestionScoreComponents { get; set; } = new List<ProjectQuestionScoreComponent>();

    public virtual ICollection<ProjectUserQuestionResponse> ProjectUserQuestionResponses { get; set; } = new List<ProjectUserQuestionResponse>();

    public virtual ICollection<SummaryProjectUserResultDetail> SummaryProjectUserResultDetails { get; set; } = new List<SummaryProjectUserResultDetail>();

    public virtual ICollection<UserResponseFrequencyDistribution> UserResponseFrequencyDistributionParentQuestions { get; set; } = new List<UserResponseFrequencyDistribution>();

    public virtual ICollection<UserResponseFrequencyDistribution> UserResponseFrequencyDistributionQuestions { get; set; } = new List<UserResponseFrequencyDistribution>();
}
