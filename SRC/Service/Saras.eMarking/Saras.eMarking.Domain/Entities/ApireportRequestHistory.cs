using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ApireportRequestHistory
{
    public long RequestHistoryId { get; set; }

    public long? RequestId { get; set; }

    public DateTime? RequestDate { get; set; }

    public bool IsRequestServed { get; set; }

    public string FileName { get; set; }

    public string FilePath { get; set; }

    public long? RequestBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string Remarks { get; set; }
}
