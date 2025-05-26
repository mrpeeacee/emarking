using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class SearchActivatedStatusForBatchResponse
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
