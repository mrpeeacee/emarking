using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class AnnotationTemplate
{
    public long AnnotationTemplateId { get; set; }

    public string AnnotationTemplateName { get; set; }

    public string AnnotationTemplateCode { get; set; }

    public bool? IsDefault { get; set; }

    public bool IsTagged { get; set; }

    public long? OrganizationId { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public virtual ICollection<AnnotationTemplateDetail> AnnotationTemplateDetails { get; set; } = new List<AnnotationTemplateDetail>();

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ICollection<QigtoAnnotationTemplateMapping> QigtoAnnotationTemplateMappings { get; set; } = new List<QigtoAnnotationTemplateMapping>();

    public virtual ICollection<UserScriptMarkingDetail> UserScriptMarkingDetails { get; set; } = new List<UserScriptMarkingDetail>();
}
