using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ScoreComponentDetail
{
    public long ComponentDetailId { get; set; }

    public long ScoreComponentId { get; set; }

    public string ComponentCode { get; set; }

    public string ComponentName { get; set; }

    public decimal? Marks { get; set; }

    public byte? ComponentOrder { get; set; }

    public bool? IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ScoreComponent ScoreComponent { get; set; }
}
