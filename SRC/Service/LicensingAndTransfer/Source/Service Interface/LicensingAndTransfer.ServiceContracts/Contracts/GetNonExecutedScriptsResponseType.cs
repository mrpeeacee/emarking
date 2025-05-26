using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
   public class GetNonExecutedScriptsResponseType
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.GetNonExecutedScriptsResponse> ScriptsField;
        /// <summary>
        /// Encrypted Script Data will be sent
        /// </summary>
        public List<DataContracts.GetNonExecutedScriptsResponse> Scripts
        {
            get
            {
                return this.ScriptsField;
            }
            set
            {
                this.ScriptsField = value;
            }
        }        
    }
}
