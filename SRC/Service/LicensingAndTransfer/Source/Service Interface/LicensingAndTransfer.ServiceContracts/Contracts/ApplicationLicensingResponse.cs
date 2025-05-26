using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class ApplicationLicensingResponse
    {
        [MessageBodyMember(Order = 0)]
        public List<DataContracts.ApplicationLicensingStatus> LstApplicationLicensingStatus { get; set; }
    }
}
