using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class UpdatePackageLoadedDateRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.PackageLoadedDate> ListPackageLoadedDateField;
        public List<DataContracts.PackageLoadedDate> ListPackageLoadedDate
        {
            get
            {
                return this.ListPackageLoadedDateField;
            }
            set
            {
                this.ListPackageLoadedDateField = value;
            }
        }

        [MessageBodyMember(Order = 1)]
        private DataContracts.ServerTypes ServerTypeField;
        public DataContracts.ServerTypes ServerType
        {
            get
            {
                return this.ServerTypeField;
            }
            set
            {
                this.ServerTypeField = value;
            }
        }

     
    }
}
