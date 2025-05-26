using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MarkingScriptTimeTracking
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public long ProjectQuestionId { get; set; }

    public long UserScriptMarkingRefId { get; set; }

    public int WorkFlowStatusId { get; set; }

    public long ProjectUserRoleId { get; set; }

    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// 1 --&gt; View , 2 --&gt; Edit
    /// </summary>
    public short? Mode { get; set; }

    /// <summary>
    /// 1 --&gt; Submit,2 --&gt; Save, 3 --&gt; Cancel , 4 --&gt; Close , 5 --&gt; Navigate
    /// </summary>
    public short? Action { get; set; }

    public TimeOnly? TimeTaken { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectQuestion ProjectQuestion { get; set; }

    public virtual ProjectUserRoleinfo ProjectUserRole { get; set; }

    public virtual ProjectQig Qig { get; set; }

    public virtual UserScriptMarkingDetail UserScriptMarkingRef { get; set; }

    public virtual WorkflowStatus WorkFlowStatus { get; set; }
}
