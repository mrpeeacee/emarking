using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ApireportRequestDetail
{
    public long RequestDetailId { get; set; }

    public long? RequestId { get; set; }

    public long? SummaryId { get; set; }
}
