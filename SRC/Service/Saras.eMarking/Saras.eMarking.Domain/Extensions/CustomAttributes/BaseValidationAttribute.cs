using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Saras.eMarking.Domain.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Saras.eMarking.Domain.Extensions.CustomAttributes
{
    public class BaseValidationAttribute : ValidationAttribute
    {
        public static XssValidatorConfig XssValidatorConfig { get; set; }

        public static bool IsXssValidationEnabled { get; set; }

        /// <summary>
        /// Read regex to validate xss in json and assign to static variable.
        /// </summary>
        /// <param name="jsonFilePath"></param>
        public static void SetXssValidatorConfig(IConfiguration configuration, string jsonFilePath)
        {
            string jsonText = File.ReadAllText(jsonFilePath);

            XssValidatorRoot xssValidator = JsonConvert.DeserializeObject<XssValidatorRoot>(jsonText);
            if (xssValidator != null)
            {
                XssValidatorConfig = xssValidator.XssValidatorConfig;
            }

            IsXssValidationEnabled = GetXssValidationEnabled(configuration);

        }

        protected static bool GetXssValidationEnabled(IConfiguration configuration)
        {
            return configuration != null &&
                   !string.IsNullOrEmpty(configuration["AppSettings:IsXssValidationEnabled"]) &&
                   Convert.ToBoolean(configuration["AppSettings:IsXssValidationEnabled"]);
        }
    }
}
