using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserToRoleMappingArchive
{
    public long ArchiveId { get; set; }

    public long MappingId { get; set; }

    public long? UserId { get; set; }

    public int? RoleId { get; set; }

    public bool IsDeleted { get; set; }

    public long? OrganizationUserId { get; set; }
}
