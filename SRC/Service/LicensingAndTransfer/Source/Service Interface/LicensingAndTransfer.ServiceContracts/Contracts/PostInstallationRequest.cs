using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class PostInstallationRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.PostInstallationComponents> PostInstallationComponentsField;
        /// <summary>
        /// Encrypted Script Data will be sent
        /// </summary>
        public List<DataContracts.PostInstallationComponents> PostInstallationComponents
        {
            get
            {
                return this.PostInstallationComponentsField;
            }
            set
            {
                this.PostInstallationComponentsField = value;
            }
        }
    }
}
