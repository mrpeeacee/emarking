using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class WorkflowStatus
{
    public int WorkflowId { get; set; }

    public string WorkflowCode { get; set; }

    public string WorkflowName { get; set; }

    /// <summary>
    /// 1---&gt;Script,  2---&gt;Project, 3--&gt; Categorization, 4--&gt; QIG
    /// </summary>
    public byte? WorkflowType { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<MarkingScriptTimeTracking> MarkingScriptTimeTrackings { get; set; } = new List<MarkingScriptTimeTracking>();

    public virtual ICollection<MpstandardizationQueRespMarkingDetail> MpstandardizationQueRespMarkingDetails { get; set; } = new List<MpstandardizationQueRespMarkingDetail>();

    public virtual ICollection<MpstandardizationScheduleScriptDetail> MpstandardizationScheduleScriptDetails { get; set; } = new List<MpstandardizationScheduleScriptDetail>();

    public virtual ICollection<MpstandardizationSchedule> MpstandardizationSchedules { get; set; } = new List<MpstandardizationSchedule>();

    public virtual ICollection<MpstandardizationScriptMarkingDetail> MpstandardizationScriptMarkingDetails { get; set; } = new List<MpstandardizationScriptMarkingDetail>();

    public virtual ICollection<ProjectUserScript> ProjectUserScripts { get; set; } = new List<ProjectUserScript>();

    public virtual ICollection<ProjectWorkflowStatusTracking> ProjectWorkflowStatusTrackings { get; set; } = new List<ProjectWorkflowStatusTracking>();

    public virtual ICollection<QuestionScoreComponentMarkingDetail> QuestionScoreComponentMarkingDetails { get; set; } = new List<QuestionScoreComponentMarkingDetail>();

    public virtual ICollection<QuestionUserResponseMarkingDetail> QuestionUserResponseMarkingDetails { get; set; } = new List<QuestionUserResponseMarkingDetail>();

    public virtual ICollection<UserScriptMarkingDetail> UserScriptMarkingDetails { get; set; } = new List<UserScriptMarkingDetail>();
}
