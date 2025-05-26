using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class AnnotationTemplateDetailsArchive
{
    public long ArchiveId { get; set; }

    public long TemplateDetailId { get; set; }

    public long AnnotationTemplateId { get; set; }

    public int AnnotationToolId { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? ModifiedBy { get; set; }
}
