using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ModeOfAssessment
{
    public short Moaid { get; set; }

    public string Moacode { get; set; }

    public string Moaname { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreateDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<ProjectInfo> ProjectInfos { get; set; } = new List<ProjectInfo>();
}
