using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ValidateCaptcha
{
    public long Id { get; set; }

    public Guid CaptchaGuid { get; set; }

    public string Captcha { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsValidated { get; set; }

    public DateTime? ValidatedDate { get; set; }
}
