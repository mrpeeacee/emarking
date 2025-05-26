using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectUserSchoolMappingArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long? ProjectUserRoleId { get; set; }

    public int? ExemptionSchoolId { get; set; }

    public bool IsSendingSchool { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
