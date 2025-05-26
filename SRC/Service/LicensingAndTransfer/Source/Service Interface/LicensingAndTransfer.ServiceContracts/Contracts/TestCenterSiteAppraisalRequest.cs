using System;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts
{
    public class TestCenterSiteAppraisalRequest
    {
        [MessageBodyMember(Order = 0)]
        public String TestCenterMacID { get; set; }
        [MessageBodyMember(Order = 1)]
        public String HostName { get; set; }
        [MessageBodyMember(Order = 2)]
        public String IPAddress { get; set; }
        [MessageBodyMember(Order = 3)]
        public String MACAddress { get; set; }
        [MessageBodyMember(Order = 4)]
        public String OSName { get; set; }
        [MessageBodyMember(Order = 5)]
        public String OSServicePack { get; set; }
        [MessageBodyMember(Order = 6)]
        public String OSVersion { get; set; }
        [MessageBodyMember(Order = 7)]
        public String CPUName { get; set; }
        [MessageBodyMember(Order = 8)]
        public Decimal CPUSpeed { get; set; }
        [MessageBodyMember(Order = 9)]
        public Decimal RAM { get; set; }
        [MessageBodyMember(Order = 10)]
        public String Framework { get; set; }
        [MessageBodyMember(Order = 11)]
        public Decimal HDDFreeSpace { get; set; }
        [MessageBodyMember(Order = 12)]
        public String IISVersion { get; set; }
        [MessageBodyMember(Order = 13)]
        public String IEVersion { get; set; }
        [MessageBodyMember(Order = 14)]
        public String FireFoxVersion { get; set; }
        [MessageBodyMember(Order = 15)]
        public String ChromeVersion { get; set; }
        [MessageBodyMember(Order = 16)]
        public String CenterName { get; set; }
        [MessageBodyMember(Order = 17)]
        public String CenterCode { get; set; }
        [MessageBodyMember(Order = 18)]
        public String CenterAddress { get; set; }
        [MessageBodyMember(Order = 19)]
        public Int64 OrganizationID { get; set; }
        [MessageBodyMember(Order = 20)]
        public TestCenterSiteAppraisalDatabaseDetails DatabaseDetails { get; set; }
    }
        
    public class TestCenterSiteAppraisalDatabaseDetails
    {
        [MessageBodyMember(Order = 0)]
        public String Name { get; set; }
        [MessageBodyMember(Order = 1)]
        public String Instance { get; set; }
        [MessageBodyMember(Order = 2)]
        public String Collation { get; set; }
        [MessageBodyMember(Order = 3)]
        public String Version { get; set; }
    }
}
