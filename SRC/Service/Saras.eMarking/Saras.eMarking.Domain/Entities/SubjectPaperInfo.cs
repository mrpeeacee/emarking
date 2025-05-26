using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class SubjectPaperInfo
{
    public short PaperId { get; set; }

    public string PaperCode { get; set; }

    public string PaperName { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreateDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<ProjectInfo> ProjectInfos { get; set; } = new List<ProjectInfo>();
}
