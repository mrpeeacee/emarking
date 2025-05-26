using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class UpdatePackageStatisticsRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.PackageStatistics> ListPackageStatisticsField;
        public List<DataContracts.PackageStatistics> ListPackageStatistics
        {
            get
            {
                return this.ListPackageStatisticsField;
            }
            set
            {
                this.ListPackageStatisticsField = value;
            }
        }        
    }
}