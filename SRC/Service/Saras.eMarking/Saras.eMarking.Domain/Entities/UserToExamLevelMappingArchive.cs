using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserToExamLevelMappingArchive
{
    public long ArchiveId { get; set; }

    public long UserToLevelMappingId { get; set; }

    public long? UserId { get; set; }

    public short? ExamLevelId { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
