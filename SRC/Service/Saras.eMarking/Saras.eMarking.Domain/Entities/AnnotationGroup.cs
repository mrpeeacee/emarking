using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class AnnotationGroup
{
    public int AnnotationGroupId { get; set; }

    public string AnnotationGroupName { get; set; }

    public string AnnotationGroupCode { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime Createddate { get; set; }

    public long? CreatedBy { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
