using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQigquestion
{
    public long QigquestionId { get; set; }

    public long Qigid { get; set; }

    public long ProjectQuestionId { get; set; }

    public long QuestionId { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectQuestion ProjectQuestion { get; set; }

    public virtual ProjectQig Qig { get; set; }
}
