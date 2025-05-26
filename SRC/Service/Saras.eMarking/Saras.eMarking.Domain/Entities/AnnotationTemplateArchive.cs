using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class AnnotationTemplateArchive
{
    public long ArchiveId { get; set; }

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
}
