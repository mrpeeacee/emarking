using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ApireportRequestArchive
{
    public long ArchiveId { get; set; }

    public long RequestId { get; set; }

    public long? ProjectId { get; set; }

    public Guid? RequestGuid { get; set; }

    /// <summary>
    /// 1 --&gt; EMS1, 2 --&gt; EMS2,
    /// </summary>
    public short? ReportType { get; set; }

    public int? PageNo { get; set; }

    public int? PageSize { get; set; }

    public bool IsRequestServed { get; set; }

    public DateTime? RequestDate { get; set; }

    public short? RequestOrder { get; set; }

    public string FileName { get; set; }

    public string FilePath { get; set; }

    public DateTime? RequestServedDate { get; set; }

    public long? RequestBy { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsProcessed { get; set; }

    public string Remarks { get; set; }

    public string ErrorMsg { get; set; }
}
