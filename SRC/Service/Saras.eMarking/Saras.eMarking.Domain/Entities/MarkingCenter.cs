using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MarkingCenter
{
    public int CenterId { get; set; }

    public string CenterCode { get; set; }

    public string CenterName { get; set; }

    public string Address { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }
}
