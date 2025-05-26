using System.ServiceModel;
using System;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    public class OrganizationsForTestCenterAppraisalResponse
    {
        [MessageBodyMember(Order = 0)]
        public Dictionary<long, string> OrganizationDetails { get; set; }
        [MessageBodyMember(Order = 1)]
        public Boolean IsTestCenterRegistered { get; set; }
        [MessageBodyMember(Order = 2)]
        public String TestCenterName { get; set; }
        [MessageBodyMember(Order = 3)]
        public String TestCenterCode { get; set; }
        [MessageBodyMember(Order = 4)]
        public String TestCenterAddress { get; set; }
    }
}
