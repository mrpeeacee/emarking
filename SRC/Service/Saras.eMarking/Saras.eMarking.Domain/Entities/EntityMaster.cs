using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class EntityMaster
{
    public long EntityId { get; set; }

    public string EntityCode { get; set; }

    public string EntityName { get; set; }

    public string EntityDescription { get; set; }

    public virtual ICollection<EventAudit> EventAudits { get; set; } = new List<EventAudit>();
}
