using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class ScannedResponse
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.ScannedResponseOutput> ListScannedResponseField;
        public List<DataContracts.ScannedResponseOutput> ListScannedResponse
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
