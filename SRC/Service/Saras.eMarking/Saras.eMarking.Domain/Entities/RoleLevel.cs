using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class RoleLevel
{
    public byte RoleLevelId { get; set; }

    public string LevelCode { get; set; }

    public string LevelName { get; set; }

    public short? Order { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
