using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace LicensingAndTransfer.API.Contracts
{
    [MessageContract]
    public class MediaPackageInfoRequest
    {
        [MessageBodyMember(Order = 0)]
        public string macId { get; set; }

        [MessageBodyMember(Order = 1)]
        public string mediaPkgName { get; set; }

        [MessageBodyMember(Order = 2)]
        public string extnMessage { get; set; }
        [MessageBodyMember(Order = 3)]
        public int TestTypeMasterID { get; set; }
    }
}
