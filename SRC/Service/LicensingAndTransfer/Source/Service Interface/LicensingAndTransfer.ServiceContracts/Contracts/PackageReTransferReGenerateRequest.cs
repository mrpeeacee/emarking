using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class PackageReTransferReGenerateRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.PackageDetails> ListPackageDetailsField;
        public List<DataContracts.PackageDetails> ListPackageDetails
        {
            get
            {
                return this.ListPackageDetailsField;
            }
            set
            {
                this.ListPackageDetailsField = value;
            }
        }        
    }
}