using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace LicensingAndTransfer.DataContracts
{

  

		

    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "SummaryDetailsResponse")]
    public class SummaryDetailsResponse
    {
        [DataMember(Name = "NoOfPackagesDeliveredTODCC", IsRequired = true, Order = 0)]
        private int NoOfPackagesDeliveredTODCCField;
        public int NoOfPackagesDeliveredTODCC
        {
            get
            {
                return NoOfPackagesDeliveredTODCCField;
            }
            set
            {
                NoOfPackagesDeliveredTODCCField = value;
            }
        }

        [DataMember(Name = "PackagesReachedDCC", IsRequired = true, Order = 1)]
        private int PackagesReachedDCCField;
        public int PackagesReachedDCC
        {
            get
            {
                return PackagesReachedDCCField;
            }

            set
            {
                PackagesReachedDCCField = value;
            }
        }

        [DataMember(Name = "NoOfPackagesLoadedInDCC", IsRequired = true, Order = 2)]
        private int NoOfPackagesLoadedInDCCField;
        public int NoOfPackagesLoadedInDCC
        {
            get
            {
                return NoOfPackagesLoadedInDCCField;
            }

            set
            {
                NoOfPackagesLoadedInDCCField = value;
            }
        }


        [DataMember(Name = "NoOfPackagesDelivered", IsRequired = true, Order = 3)]
        private int NoOfPackagesDeliveredField;
        public int NoOfPackagesDelivered
        {
            get
            {
                return NoOfPackagesDeliveredField;
            }
            set
            {
                NoOfPackagesDeliveredField = value;
            }
        }

        [DataMember(Name = "NoOfPackagesReachedTestCenter", IsRequired = true, Order = 4)]
        private int NoOfPackagesReachedTestCenterField;
        public int NoOfPackagesReachedTestCenter
        {
            get
            {
                return NoOfPackagesReachedTestCenterField;
            }
            set
            {
                NoOfPackagesReachedTestCenterField = value;
            }
        }

        [DataMember(IsRequired = true, Name = "NoOfPackagesLoadedInTestCenter", Order = 5)]
        private int NoOfPackagesLoadedInTestCenterField;
        public int NoOfPackagesLoadedInTestCenter
        {
            get
            {
                return NoOfPackagesLoadedInTestCenterField;
            }
            set
            {
                NoOfPackagesLoadedInTestCenterField = value;
            }
        }

        [DataMember(IsRequired = true, Name = "NoOfPackagesInProgress", Order = 6)]
        private int NoOfPackagesInProgressField;
        public int NoOfPackagesInProgress
        {
            get
            {
                return NoOfPackagesInProgressField;
            }
            set
            {
                NoOfPackagesInProgressField = value;
            }
        }

        [DataMember(IsRequired = true, Name = "NoOfPackagesNotYetStarted", Order = 7)]
        private int NoOfPackagesNotYetStartedField;
        public int NoOfPackagesNotYetStarted
        {
            get
            {
                return NoOfPackagesNotYetStartedField;
            }
            set
            {
                NoOfPackagesNotYetStartedField = value;
            }
        }

        [DataMember(IsRequired = true, Name = "NoOfPackagesFailed", Order = 8)]
        private int NoOfPackagesFailedField;
        public int NoOfPackagesFailed
        {
            get
            {
                return NoOfPackagesFailedField;
            }
            set
            {
                NoOfPackagesFailedField = value;
            }
        }

        [DataMember(IsRequired = true, Name = "NoOfResponsePackagesRecieved", Order = 9)]
        private int NoOfResponsePackagesRecievedField;
        public int NoOfResponsePackagesRecieved
        {
            get
            {
                return NoOfResponsePackagesRecievedField;
            }
            set
            {
                NoOfResponsePackagesRecievedField = value;
            }
        }

        [DataMember(IsRequired = true, Name = "NoOfResponsePackagesLoadedInDCD", Order = 10)]
        private int NoOfResponsePackagesLoadedInDCDField;
        public int NoOfResponsePackagesLoadedInDCD
        {
            get
            {
                return NoOfResponsePackagesLoadedInDCDField;
            }
            set
            {
                NoOfResponsePackagesLoadedInDCDField = value;
            }
        }

        [DataMember(IsRequired = true, Name = "NoofPackagesNotYetReciveInDX", Order = 11)]
        private int NoofPackagesNotYetReciveInDXField;
        public int NoofPackagesNotYetReciveInDX
        {
            get
            {
                return NoofPackagesNotYetReciveInDXField;
            }
            set
            {
                NoofPackagesNotYetReciveInDXField = value;
            }
        }
    

    }
}

