using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ModuleMaster
{
    public long ModuleId { get; set; }

    public string ModuleCode { get; set; }

    public string ModuleName { get; set; }

    public long? ParentId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<EventAudit> EventAudits { get; set; } = new List<EventAudit>();
}
