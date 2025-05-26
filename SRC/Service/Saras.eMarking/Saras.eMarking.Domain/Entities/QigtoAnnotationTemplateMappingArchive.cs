using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QigtoAnnotationTemplateMappingArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long Qigid { get; set; }

    public long AnnotationTemplateId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
