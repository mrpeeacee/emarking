using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class ETLAppErrorLogsRequest
    {
        [MessageBodyMember(Order = 0)]
        public DateTime logDate { get; set; }

        [MessageBodyMember(Order = 2)]
        public string logLevel { get; set; }

        [MessageBodyMember(Order = 3)]
        public string logger { get; set; }

        [MessageBodyMember(Order = 4)]
        public string message { get; set; }

        [MessageBodyMember(Order = 5)]
        public string exception { get; set; }

        [MessageBodyMember(Order = 6)]
        public string sessionid { get; set; }

        [MessageBodyMember(Order = 7)]
        public string nodeid { get; set; }

        [MessageBodyMember(Order = 8)]
        public string userid { get; set; }

        [MessageBodyMember(Order = 9)]
        public string MACID { get; set; }

    }
}
