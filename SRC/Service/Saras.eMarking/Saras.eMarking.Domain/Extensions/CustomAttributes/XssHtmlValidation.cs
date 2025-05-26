using Saras.eMarking.Domain.Configuration;
using Saras.eMarking.Domain.Extensions.CustomAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.Extensions
{
    public class XssHtmlValidationAttribute : BaseValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsXssValidationEnabled
                 && XssValidatorConfig != null
                 && !new XssValidator(XssValidatorConfig).ValidateAntiXSS("HTML", Convert.ToString(value), validationContext.ObjectType.FullName + '.' + validationContext.DisplayName))
            {
                return new ValidationResult("Special characters are not allowed!");
            }

            return ValidationResult.Success;
        }
    }

}
