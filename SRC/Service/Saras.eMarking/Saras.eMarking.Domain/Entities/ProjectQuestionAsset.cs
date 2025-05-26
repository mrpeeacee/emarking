using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQuestionAsset
{
    public long Id { get; set; }

    public long? ProjectQuestionId { get; set; }

    /// <summary>
    /// 1 --&gt; Question, 2 --&gt; Passage
    /// </summary>
    public short? AssetType { get; set; }

    public string Path { get; set; }

    public bool IsDeleted { get; set; }

    public long? AssetId { get; set; }

    public string AssetName { get; set; }

    public virtual ProjectQuestion ProjectQuestion { get; set; }
}
