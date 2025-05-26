using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class EventAuditArchive
{
    public long ArchiveId { get; set; }

    public long EventAuditId { get; set; }

    public long? EventId { get; set; }

    public DateTime? EventDateTime { get; set; }

    public long? UserId { get; set; }

    public long? ProjectUserRoleId { get; set; }

    public string Status { get; set; }

    public string Remarks { get; set; }

    public long? EntityId { get; set; }

    public long? AssetRef { get; set; }

    public long? ModuleId { get; set; }

    public string Ipaddress { get; set; }

    public string SessionId { get; set; }
}
