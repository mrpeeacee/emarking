using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class Template
{
    public long TemplateId { get; set; }

    public string TemplateName { get; set; }

    public string TemplateBody { get; set; }

    public long? EventId { get; set; }

    public bool? IsDeleted { get; set; }

    public int? AlertType { get; set; }

    public byte? NoOfTryOut { get; set; }
}
