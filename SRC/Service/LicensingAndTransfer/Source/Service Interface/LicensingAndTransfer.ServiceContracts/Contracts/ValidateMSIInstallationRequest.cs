using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class ValidateMSIInstallationRequest
    {
        [MessageBodyMember(Order = 0)]
        private string MacIDField;
        public string MacID
        {
            get
            {
                return this.MacIDField;
            }
            set
            {
                this.MacIDField = value;
            }
        }
    }
}
