using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQuestionChoiceMappingArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long ProjectQuestionId { get; set; }

    public string ChoiceText { get; set; }

    public decimal? MaxScore { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsCorrect { get; set; }

    public string ChoiceIdentifier { get; set; }

    public bool IsSystemGeneratedOption { get; set; }
}
