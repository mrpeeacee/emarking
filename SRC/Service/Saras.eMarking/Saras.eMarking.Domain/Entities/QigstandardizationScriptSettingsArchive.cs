using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QigstandardizationScriptSettingsArchive
{
    public long ArchiveId { get; set; }

    public long SettingId { get; set; }

    public long? Qigid { get; set; }

    public int? StandardizationScript { get; set; }

    public int? BenchmarkScript { get; set; }

    public int? AdditionalStdScript { get; set; }

    public int? QualityAssuranceScript { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsS1available { get; set; }

    public bool? IsS2available { get; set; }

    public bool? IsS3available { get; set; }

    public DateTime? S1startDate { get; set; }

    public string Remarks { get; set; }
}
