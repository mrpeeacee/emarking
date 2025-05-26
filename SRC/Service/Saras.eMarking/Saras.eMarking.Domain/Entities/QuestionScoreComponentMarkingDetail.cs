using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QuestionScoreComponentMarkingDetail
{
    public long Id { get; set; }

    public long UserScriptMarkingRefId { get; set; }

    public long QuestionUserResponseMarkingRefId { get; set; }

    public long ScoreComponentId { get; set; }

    public decimal? MaxMarks { get; set; }

    public decimal? AwardedMarks { get; set; }

    public long? BandId { get; set; }

    /// <summary>
    /// 1---&gt;Approved,  2---&gt;Ammended, 3---&gt;Rejected
    /// </summary>
    public byte? MarkingStatus { get; set; }

    public int WorkflowStatusId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long MarkedBy { get; set; }

    public DateTime MarkedDate { get; set; }

    public virtual ProjectMarkSchemeBandDetail Band { get; set; }

    public virtual ProjectUserRoleinfo MarkedByNavigation { get; set; }

    public virtual QuestionUserResponseMarkingDetail QuestionUserResponseMarkingRef { get; set; }

    public virtual ProjectQuestionScoreComponent ScoreComponent { get; set; }

    public virtual UserScriptMarkingDetail UserScriptMarkingRef { get; set; }

    public virtual WorkflowStatus WorkflowStatus { get; set; }
}
