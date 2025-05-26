using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class SchoolInfoArchive
{
    public long ArchiveId { get; set; }

    public int SchoolId { get; set; }

    public string SchoolCode { get; set; }

    public string SchoolName { get; set; }

    public string Address { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ProjectId { get; set; }

    public int? ParentId { get; set; }
}
