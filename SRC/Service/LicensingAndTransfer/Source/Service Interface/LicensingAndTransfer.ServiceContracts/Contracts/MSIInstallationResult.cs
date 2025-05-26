using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class MSIInstallationResultRequest
    {

        [MessageBodyMember(Order = 0)]
        public string macId { get; set; }

        [MessageBodyMember(Order = 1)]
        public bool isDeleted { get; set; }

        [MessageBodyMember(Order = 2)]
        public bool isInstalled { get; set; }

        [MessageBodyMember(Order = 3)]
        public string extnMessage { get; set; }

        [MessageBodyMember(Order = 4)]
        public bool isFullMSI { get; set; }
    }
}
