using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQuestionsHistory
{
    public long Id { get; set; }

    public long? ProjectQuestionId { get; set; }

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
    /// 1 --&gt; Automatic 2 --&gt; Semi-Automatic 3 --&gt; Open Ended
    /// </summary>
    public byte? MarkingQuestionType { get; set; }

    public bool IsCaseSensitive { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime HistoryCreatedDate { get; set; }

    public virtual ProjectInfo Project { get; set; }
}
