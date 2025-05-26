using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectMarkSchemeBandDetailsArchive
{
    public long ArchiveId { get; set; }

    public long BandId { get; set; }

    public long ProjectMarkSchemeId { get; set; }

    public decimal BandFrom { get; set; }

    public decimal BandTo { get; set; }

    public string BandDescription { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string BandCode { get; set; }

    public string BandName { get; set; }
}
