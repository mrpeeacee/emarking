using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QuestionUserResponseMarkingImage
{
    public long Id { get; set; }

    public long? UserScriptMarkingRefId { get; set; }

    public long? QuestionUserResponseMarkingRefId { get; set; }

    public string ImageBase64 { get; set; }

    public bool IsDeleted { get; set; }

    public virtual QuestionUserResponseMarkingDetail QuestionUserResponseMarkingRef { get; set; }

    public virtual UserScriptMarkingDetail UserScriptMarkingRef { get; set; }
}
