using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ExamSeries
{
    public short ExamSeriesId { get; set; }

    public string ExamSeriesCode { get; set; }

    public string ExamSeriesName { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreateDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<ProjectInfo> ProjectInfos { get; set; } = new List<ProjectInfo>();
}
