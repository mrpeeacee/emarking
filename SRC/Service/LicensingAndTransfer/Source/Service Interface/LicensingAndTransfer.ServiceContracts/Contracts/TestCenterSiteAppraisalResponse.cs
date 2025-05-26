using System.ServiceModel;
using System;

namespace LicensingAndTransfer.ServiceContracts
{
    public class TestCenterSiteAppraisalResponse
    {
        [MessageBodyMember(Order = 0)]
        public String Status { get; set; }
        [MessageBodyMember(Order = 1)]
        public String Message { get; set; }
    }
}
