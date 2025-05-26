using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class Organization
{
    public long OrganizationId { get; set; }

    public string OrganizationCode { get; set; }

    public string OrganizationName { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<UserToOrganizationMapping> UserToOrganizationMappings { get; set; } = new List<UserToOrganizationMapping>();
}
