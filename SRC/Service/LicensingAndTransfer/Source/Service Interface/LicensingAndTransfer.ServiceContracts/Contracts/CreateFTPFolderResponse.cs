using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class CreateFTPSessionResponse
    {
        [MessageBodyMember(Order = 0)]
        private System.String StatusField;
        public System.String Status
        {
            get
            {
                return this.StatusField;
            }
            set
            {
                this.StatusField = value;
            }
        }

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