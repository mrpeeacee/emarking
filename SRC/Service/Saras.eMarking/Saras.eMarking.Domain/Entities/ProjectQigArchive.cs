using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQigArchive
{
    public long ArchiveId { get; set; }

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

    public byte? ResponseProcessingType { get; set; }
}
