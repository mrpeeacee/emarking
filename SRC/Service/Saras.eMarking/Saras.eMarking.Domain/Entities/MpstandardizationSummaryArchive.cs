using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MpstandardizationSummaryArchive
{
    public long ArchiveId { get; set; }

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

    public byte ApprovalStatus { get; set; }

    public bool IsDeleted { get; set; }

    public long? ActionBy { get; set; }

    public DateTime? ActionDate { get; set; }

    public string Remarks { get; set; }
}
