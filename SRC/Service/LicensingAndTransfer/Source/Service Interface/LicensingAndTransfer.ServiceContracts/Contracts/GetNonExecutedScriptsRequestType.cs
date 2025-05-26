using System;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts
{
        [MessageContract]
    public class GetNonExecutedScriptsRequestType
    {

        [MessageBodyMember(Order = 0)]
        private DataContracts.GetNonExecutedScriptsRequest requestField;
        /// <summary>
        /// Encrypted Script Data will be sent
        /// </summary>
        public DataContracts.GetNonExecutedScriptsRequest request
        {
            get
            {
                return this.requestField;
            }
            set
            {
                this.requestField = value;
            }
        }      
    }
}
