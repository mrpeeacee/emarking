using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectArchiveRequestDetail
{
    public long Id { get; set; }

    public long ProjectArchiveRequestId { get; set; }

    public string TableName { get; set; }

    public int? TableCountsBeforArchive { get; set; }

    public int? TableCountsAfterArchive { get; set; }

    public int? TableRecordsArchived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ProjectArchiveRequest ProjectArchiveRequest { get; set; }
}
