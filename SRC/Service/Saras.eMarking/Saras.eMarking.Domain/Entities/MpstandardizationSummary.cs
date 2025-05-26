using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MpstandardizationSummary
{
    public long StandardizationSummaryId { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public long ProjectUserRoleId { get; set; }

    public bool IsStandardizationDone { get; set; }

    public bool IsPracticeCompleted { get; set; }

    public int? NoOfScriptPracticed { get; set; }

    public int? NoOfPracticeScriptWithinTolerance { get; set; }

    public int? NoOfPracticeScriptOutOfTolerance { get; set; }

    public bool IsQualifiyingAssementDone { get; set; }

    public int? NoOfScriptQualifingAssessment { get; set; }

    public int? NoOfQascriptWithinTolerance { get; set; }

    public int? NoOfQascriptOutOfTolerance { get; set; }

    public bool IsAdditionalStdAssessmentTaken { get; set; }

    public int? NoOfScriptAdditionalStd { get; set; }

    public int? NoOfAdditionalStdscriptWithinTolerance { get; set; }

    public int? NoOfAdditionalStdscriptOutOfTolerance { get; set; }

    /// <summary>
    /// 1 --&gt; Manual 2 --&gt; Automatic
    /// </summary>
    public byte? ApprovalType { get; set; }

    /// <summary>
    /// 0 --&gt; Waiting for submission, 1--&gt; Pending, 2--&gt; Rejected, 3 --&gt; Additional standardization Scripts Given,  4 --&gt; Approved 5--&gt; Suspended
    /// </summary>
    public byte ApprovalStatus { get; set; }

    public bool IsDeleted { get; set; }

    public long? ActionBy { get; set; }

    public DateTime? ActionDate { get; set; }

    public string Remarks { get; set; }

    public virtual ProjectUserRoleinfo ActionByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectUserRoleinfo ProjectUserRole { get; set; }

    public virtual ProjectQig Qig { get; set; }
}
