using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQigquestionsArchive
{
    public long ArchiveId { get; set; }

    public long QigquestionId { get; set; }

    public long Qigid { get; set; }

    public long ProjectQuestionId { get; set; }

    public long QuestionId { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
