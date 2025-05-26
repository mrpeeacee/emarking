using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ExamYear
{
    public byte YearId { get; set; }

    public short Year { get; set; }

    public bool IscurrentYear { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreateDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
