using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserResponseFrequencyDistributionArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public long ParentQuestionId { get; set; }

    public long QuestionId { get; set; }

    public string QuestionGuid { get; set; }

    public string ResponseText { get; set; }

    public int? TotalNoOfCandidates { get; set; }

    public int? NoOfCandidatesAnswered { get; set; }

    public decimal? PercentageDistribution { get; set; }

    public decimal? MaxMarks { get; set; }

    public decimal? AwardedMarks { get; set; }

    /// <summary>
    /// 1--&gt; Auto , 2--&gt; Moderated , 3 --&gt; Manual
    /// </summary>
    public byte? MarkingType { get; set; }

    public long? ModeratedBy { get; set; }

    public DateTime? ModeratedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public decimal? AutoMarks { get; set; }

    public string Remarks { get; set; }

    public bool IsDiscrepancyExist { get; set; }

    public byte? DiscrepancyStatus { get; set; }
}
