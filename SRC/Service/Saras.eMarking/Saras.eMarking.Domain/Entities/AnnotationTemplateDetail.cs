using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class AnnotationTemplateDetail
{
    public long TemplateDetailId { get; set; }

    public long AnnotationTemplateId { get; set; }

    public int AnnotationToolId { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public virtual AnnotationTemplate AnnotationTemplate { get; set; }

    public virtual AnnotationTool AnnotationTool { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }
}
