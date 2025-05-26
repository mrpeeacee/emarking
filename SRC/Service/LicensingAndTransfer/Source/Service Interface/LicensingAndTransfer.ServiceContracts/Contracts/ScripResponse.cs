using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class ScriptResponse
    {
        [MessageBodyMember(Order = 0)]
        private System.String ScriptField;
        /// <summary>
        /// Encrypted Script Data will be sent
        /// </summary>
        public System.String Script
        {
            get
            {
                return this.ScriptField;
            }
            set
            {
                this.ScriptField = value;
            }
        }        
    }
}