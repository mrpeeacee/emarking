using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQigresetArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long? ProjectId { get; set; }

    public long? Qigid { get; set; }

    public long? ResetBy { get; set; }

    public DateTime? ResetDate { get; set; }

    public long? AuthenticateBy { get; set; }

    public DateTime? AuthenticateDate { get; set; }
}
