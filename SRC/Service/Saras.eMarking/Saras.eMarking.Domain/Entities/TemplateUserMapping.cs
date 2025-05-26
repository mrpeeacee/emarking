using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class TemplateUserMapping
{
    public long Id { get; set; }

    public long? ProjectUserRoleId { get; set; }

    public long UserId { get; set; }

    public int? TryOut { get; set; }

    public bool IsMailSent { get; set; }

    public DateTime? SentDateTime { get; set; }

    public long? TemplateId { get; set; }

    public bool IsDeleted { get; set; }
}
