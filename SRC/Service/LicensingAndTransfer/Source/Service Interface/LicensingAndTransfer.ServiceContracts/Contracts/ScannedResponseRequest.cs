using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]

    public class ScannedResponseRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.ScannedResponse> ListScannedResponseField;
        public List<DataContracts.ScannedResponse> ListScannedResponse
        {
            get
            {
                return this.ListScannedResponseField;
            }
            set
            {
                this.ListScannedResponseField = value;
            }
        }
    }
}
