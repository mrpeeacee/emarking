using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectMarkSchemeBandDetail
{
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

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectMarkSchemeTemplate ProjectMarkScheme { get; set; }

    public virtual ICollection<ProjectUserQuestionResponse> ProjectUserQuestionResponses { get; set; } = new List<ProjectUserQuestionResponse>();

    public virtual ICollection<QuestionScoreComponentMarkingDetail> QuestionScoreComponentMarkingDetails { get; set; } = new List<QuestionScoreComponentMarkingDetail>();

    public virtual ICollection<QuestionUserResponseMarkingDetail> QuestionUserResponseMarkingDetails { get; set; } = new List<QuestionUserResponseMarkingDetail>();
}
