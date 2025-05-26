using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectMarkSchemeTemplateArchive
{
    public long ArchiveId { get; set; }

    public long ProjectMarkSchemeId { get; set; }

    public string SchemeCode { get; set; }

    public string SchemeName { get; set; }

    public decimal? Marks { get; set; }

    public long? ProjectId { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string SchemeDescription { get; set; }

    public bool IsTagged { get; set; }

    /// <summary>
    /// 1--&gt; Question Level 2--&gt; Score Component Level
    /// </summary>
    public byte? MarkingSchemeType { get; set; }

    public bool IsBandExist { get; set; }
}
