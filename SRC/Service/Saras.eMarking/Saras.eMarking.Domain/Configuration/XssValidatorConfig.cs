using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Saras.eMarking.Domain.Configuration
{
    /// <summary>
    /// Xss Validator Root class to Deserialize json to Object
    /// </summary>
    public class XssValidatorRoot
    {
        public XssValidatorConfig XssValidatorConfig { get; set; }
    }

    /// <summary>
    /// Xss Validator Config model class
    /// </summary>
    public class XssValidatorConfig
    {
        /// <summary>
        /// List of regex to validate xss script in the text
        /// </summary>
        public List<string> TextValidator { get; set; }

        /// <summary>
        /// List of regex to validate xss script in the HTML
        /// </summary>
        public List<string> HtmlValidator { get; set; }

        public List<string> ExceptionalObjects { get; set; }
    }

    /// <summary>
    /// Xss Validator class
    /// </summary>
    public class XssValidator
    {
        private readonly XssValidatorConfig _xssValidatorConfig;
        public XssValidator(XssValidatorConfig xssValidatorConfig)
        {
            _xssValidatorConfig = xssValidatorConfig;
        }

        /// <summary>
        /// Validate anti XSS for HTML and text data
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="inputParameter"></param>
        /// <param name="pageurl"></param>
        /// <returns></returns>
        public bool ValidateAntiXSS(string inputType, string inputParameter, string pageurl = null)
        {
            if (string.IsNullOrEmpty(inputParameter))
                return true;

            if (IsModelClassExist(pageurl))
                return true;

            string xssRegex;
            ///TO validate HTML Page page Request
            if (inputType.ToUpper() != "HTML")
            {
                xssRegex = Convert.ToString(string.Join("", _xssValidatorConfig.TextValidator));
            }
            else
            {
                xssRegex = Convert.ToString(string.Join("", _xssValidatorConfig.HtmlValidator));
            }

            // Following regex convers all the js events and html tags mentioned in followng links.
            if (!string.IsNullOrEmpty(Convert.ToString(_xssValidatorConfig.TextValidator)))
                return !Regex.IsMatch(System.Web.HttpUtility.UrlDecode(inputParameter), xssRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            else
                return true;

        }

        /// <summary>
        /// Check the given Model class property is in Exceptional Objects list
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool IsModelClassExist(string url)
        {
            if (_xssValidatorConfig.ExceptionalObjects == null || _xssValidatorConfig.ExceptionalObjects.Count <= 0)
                return false;

            return _xssValidatorConfig.ExceptionalObjects.Contains(url);
        }

    }
}
