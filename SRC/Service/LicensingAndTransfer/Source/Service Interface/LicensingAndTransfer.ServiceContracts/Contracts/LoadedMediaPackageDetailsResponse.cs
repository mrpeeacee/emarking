using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class LoadedMediaPackageDetailsResponse
    {
        [MessageBodyMember(Order = 0)]
        private System.String MACIDField;
        public System.String MACID
        {
            get
            {
                return this.MACIDField;
            }
            set
            {
                this.MACIDField = value;
            }
        }
        [MessageBodyMember(Order = 1)]
        private System.Int64 LoadedMediaPackageDetailIDField;
        public System.Int64 LoadedMediaPackageDetailID
        {
            get
            {
                return this.LoadedMediaPackageDetailIDField;
            }
            set
            {
                this.LoadedMediaPackageDetailIDField = value;
            }
        }
        [MessageBodyMember(Order = 2)]
        private System.Int64 TestCenterIDField;
        public System.Int64 TestCenterID
        {
            get
            {
                return this.TestCenterIDField;
            }
            set
            {
                this.TestCenterIDField = value;
            }
        }
        [MessageBodyMember(Order = 3)]
        private System.String TestCenterCodeField;
        public System.String TestCenterCode
        {
            get
            {
                return this.TestCenterCodeField;
            }
            set
            {
                this.TestCenterCodeField = value;
            }
        }
        [MessageBodyMember(Order = 4)]
        private System.String TestCenterNameField;
        public System.String TestCenterName
        {
            get
            {
                return this.TestCenterNameField;
            }
            set
            {
                this.TestCenterNameField = value;
            }
        }
        [MessageBodyMember(Order = 5)]
        private System.String MediaPackageNameField;
        public System.String MediaPackageName
        {
            get
            {
                return this.MediaPackageNameField;
            }
            set
            {
                this.MediaPackageNameField = value;
            }
        }
    }
}