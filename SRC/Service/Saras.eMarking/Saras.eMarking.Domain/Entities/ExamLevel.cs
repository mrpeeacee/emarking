using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ExamLevel
{
    public short ExamLevelId { get; set; }

    public string ExamLevelCode { get; set; }

    public string ExamLevelName { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreateDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<ProjectInfo> ProjectInfos { get; set; } = new List<ProjectInfo>();

    public virtual ICollection<UserToExamLevelMapping> UserToExamLevelMappings { get; set; } = new List<UserToExamLevelMapping>();
}
