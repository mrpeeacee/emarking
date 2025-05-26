using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ApireportRequestDetailsArchive
{
    public long ArchiveId { get; set; }

    public long RequestDetailId { get; set; }

    public long? RequestId { get; set; }

    public long? SummaryId { get; set; }
}
