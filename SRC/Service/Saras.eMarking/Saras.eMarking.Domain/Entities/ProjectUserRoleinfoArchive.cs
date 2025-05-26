using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectUserRoleinfoArchive
{
    public long ArchiveId { get; set; }

    public long ProjectUserRoleId { get; set; }

    public long ProjectId { get; set; }

    public long UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime? AppointStartDate { get; set; }

    public DateTime? AppointEndDate { get; set; }

    public bool? IsActive { get; set; }

    public bool Isdeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string Remarks { get; set; }

    public long? ReportingTo { get; set; }

    public bool? IsKp { get; set; }

    public int? SendingSchoolId { get; set; }

    public string EmailId { get; set; }

    public string Nric { get; set; }

    public string PhoneNo { get; set; }
}
