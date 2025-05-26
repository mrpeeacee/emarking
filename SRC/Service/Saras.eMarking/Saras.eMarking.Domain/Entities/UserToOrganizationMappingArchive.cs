using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserToOrganizationMappingArchive
{
    public long ArchiveId { get; set; }

    public long OrganizationUserId { get; set; }

    public long? UserId { get; set; }

    public long? OrganizationId { get; set; }

    public bool? IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
