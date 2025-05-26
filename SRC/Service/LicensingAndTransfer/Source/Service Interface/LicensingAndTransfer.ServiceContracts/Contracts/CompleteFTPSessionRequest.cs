using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class CompleteFTPSessionRequest
    {
        [MessageBodyMember(Order = 0)]
        private System.String GuidFTPNameField;
        public System.String GuidFTPName
        {
            get
            {
                return this.GuidFTPNameField;
            }
            set
            {
                this.GuidFTPNameField = value;
            }
        }

        [MessageBodyMember(Order = 1)]
        private System.String MacIDField;
        public System.String MacID
        {
            get
            {
                return this.MacIDField;
            }
            set
            {
                this.MacIDField = value;
            }
        }

        [MessageBodyMember(Order = 2)]
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

        [MessageBodyMember(Order = 3)]
        private DataContracts.Operations OperationField;
        public DataContracts.Operations Operation
        {
            get
            {
                return this.OperationField;
            }
            set
            {
                this.OperationField = value;
            }
        }

        [MessageBodyMember(Order = 4)]
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

        [MessageBodyMember(Order = 5)]
        private LicensingAndTransfer.DataContracts.TransferMedium TransferMediumField;
        public LicensingAndTransfer.DataContracts.TransferMedium TransferMedium
        {
            get
            {
                return this.TransferMediumField;
            }
            set
            {
                this.TransferMediumField = value;
            }
        }
    }
}