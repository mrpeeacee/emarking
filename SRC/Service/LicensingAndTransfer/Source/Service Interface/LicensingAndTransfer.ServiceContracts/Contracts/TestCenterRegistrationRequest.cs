using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class TestCenterRegistrationRequest
    {
        [MessageBodyMember(Order = 0)]
        private System.Collections.Generic.List<DataContracts.TestCenter> RequestField;
        public System.Collections.Generic.List<DataContracts.TestCenter> Request
        {
            get
            {
                return this.RequestField;
            }
            set
            {
                this.RequestField = value;
            }
        }
    }

    public class TestCenterTagRequest
    {
        [MessageBodyMember(Order = 0)]
        private DataContracts.TagTestCenterToUser RequestField;
        public DataContracts.TagTestCenterToUser Request
        {
            get
            {
                return this.RequestField;
            }
            set
            {
                this.RequestField = value;
            }
        }
    }
}