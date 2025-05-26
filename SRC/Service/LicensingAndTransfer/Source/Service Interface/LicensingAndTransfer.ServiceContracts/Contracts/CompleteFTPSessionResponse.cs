using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class CompleteFTPSessionResponse
    {
        [MessageBodyMember(Order = 1)]
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