using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace LicensingAndTransfer.API
{/// <summary>
/// Operation staus
/// </summary>
    public class Status
    {
        private String CodeField;
        /// <summary>
        /// Status Code
        /// </summary>
        public String Code
        {
            get { return this.CodeField; }
            set { this.CodeField = value; }
        }
        private String ReasonField;
        /// <summary>
        /// Reason
        /// </summary>
        public String Reason
        {
            get { return this.ReasonField; }
            set { this.ReasonField = value; }
        }
    }
}