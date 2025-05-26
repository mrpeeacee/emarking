using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class EventAudit
{
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

    public virtual EntityMaster Entity { get; set; }

    public virtual ModuleMaster Module { get; set; }

    public virtual ProjectUserRoleinfo ProjectUserRole { get; set; }
}
