using System.ServiceModel;

namespace LicensingAndTransfer.API.Contracts
{
    [MessageContract]
    public class ApplicationLicensingRequest
    {
        [MessageBodyMember(Order = 0)]
        public string MacAddress { get; set; }
    }
}
