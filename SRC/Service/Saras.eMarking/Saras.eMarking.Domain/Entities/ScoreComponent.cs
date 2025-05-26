using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ScoreComponent
{
    public long ScoreComponentId { get; set; }

    public string ComponentCode { get; set; }

    public string ComponentName { get; set; }

    public decimal? Marks { get; set; }

    public long? ProjectId { get; set; }

    public bool IsTagged { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<ScoreComponentDetail> ScoreComponentDetails { get; set; } = new List<ScoreComponentDetail>();
}
