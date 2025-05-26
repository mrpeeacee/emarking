using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectMarkSchemeTemplate
{
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

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ICollection<MarkSchemeFile> MarkSchemeFiles { get; set; } = new List<MarkSchemeFile>();

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ICollection<ProjectMarkSchemeBandDetail> ProjectMarkSchemeBandDetails { get; set; } = new List<ProjectMarkSchemeBandDetail>();

    public virtual ICollection<ProjectMarkSchemeQuestion> ProjectMarkSchemeQuestions { get; set; } = new List<ProjectMarkSchemeQuestion>();
}
