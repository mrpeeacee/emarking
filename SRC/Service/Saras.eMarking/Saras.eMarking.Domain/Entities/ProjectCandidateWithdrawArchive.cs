using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectCandidateWithdrawArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long ProjectId { get; set; }

    public string IndexNumber { get; set; }

    public long ScheduleUserId { get; set; }

    public bool IsDeleted { get; set; }

    public long? WithDrawBy { get; set; }

    public DateTime? WithDrawDate { get; set; }

    public long? UnWithDrawBy { get; set; }

    public DateTime? UnWithDrawDate { get; set; }
}
