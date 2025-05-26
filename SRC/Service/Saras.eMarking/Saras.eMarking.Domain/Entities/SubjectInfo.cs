using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class SubjectInfo
{
    public short SubjectId { get; set; }

    public string SubjectCode { get; set; }

    public string SubjectName { get; set; }

    public byte? SubjectType { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreateDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<ProjectInfo> ProjectInfos { get; set; } = new List<ProjectInfo>();
}
