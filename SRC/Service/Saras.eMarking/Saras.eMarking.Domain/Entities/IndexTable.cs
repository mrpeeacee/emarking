using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class IndexTable
{
    public int Id { get; set; }

    public string TblName { get; set; }

    public string SchemaName { get; set; }

    public long? ObjectId { get; set; }
}
