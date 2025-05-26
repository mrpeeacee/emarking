using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class AnnotationTool
{
    public int AnnotationToolId { get; set; }

    public string AnnotationToolName { get; set; }

    public string AnnotationToolCode { get; set; }

    /// <summary>
    /// 1--&gt;Icon, 2--&gt;Image, 3--&gt;Comment
    /// </summary>
    public byte? AnnotationToolType { get; set; }

    public int? AnnotationGroupId { get; set; }

    public bool? Isdefault { get; set; }

    public decimal? AssociatedMark { get; set; }

    public bool? IsRequiredForDiscrit { get; set; }

    public int? ReferanceAnnotationId { get; set; }

    public string ColorCode { get; set; }

    public string ColoorName { get; set; }

    public string Path { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? Modifieddate { get; set; }

    public long? ModifiedBy { get; set; }

    public virtual ICollection<AnnotationTemplateDetail> AnnotationTemplateDetails { get; set; } = new List<AnnotationTemplateDetail>();
}
