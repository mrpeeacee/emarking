using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class ApplicationLicensingRequest
    {
        [MessageBodyMember(Order = 0)]
        public string MacAddress { get; set; }
    }
}
