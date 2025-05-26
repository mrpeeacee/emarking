using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace LicensingAndTransfer.API.Contracts
{
    [MessageContract]
    public class TestCenterDeatil
    {
        public string CenterID { get; set; }
        public string CenterType { get; set; }
        public string CenterName { get; set; }

    }

    public class TestCenterRegister
    {
        public string MacID { get; set; }      
        public int VenueID { get; set; }
        public string IpAddress { get; set; }
        public string CenterCode { get; set; }
        public int CenterTypeId { get; set; }
        public string StatusCode { get; set; }
        public int SchoolID { get; set; }
        public int ExamCenterID { get; set; }
    }
}