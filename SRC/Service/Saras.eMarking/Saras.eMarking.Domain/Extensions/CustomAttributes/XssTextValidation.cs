using Saras.eMarking.Domain.Configuration;
using Saras.eMarking.Domain.Extensions.CustomAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.Extensions
{
    /// <summary>
    /// Xss Text Validation Attribute
    /// </summary>
    public class XssTextValidationAttribute : BaseValidationAttribute
    {
        /// <summary>
        /// Validate model class text property for xss scripts
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsXssValidationEnabled
                && XssValidatorConfig != null
                && !new XssValidator(XssValidatorConfig).ValidateAntiXSS("Text", Convert.ToString(value), validationContext.ObjectType.FullName + '.' + validationContext.DisplayName))
            {
                return new ValidationResult("Special characters are not allowed!");
            }

            return ValidationResult.Success;
        }
    }
}
