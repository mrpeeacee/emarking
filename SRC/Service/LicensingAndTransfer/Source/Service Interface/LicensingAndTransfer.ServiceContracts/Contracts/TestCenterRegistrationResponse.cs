using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class TestCenterRegistrationResponse
    {
        [MessageBodyMember(Order = 0)]
        private System.Collections.Generic.List<DataContracts.TestCenter> ResponseField;
        public System.Collections.Generic.List<DataContracts.TestCenter> Response
        {
            get
            {
                return this.ResponseField;
            }
            set
            {
                this.ResponseField = value;
            }
        }
    }

    public class TestCenterTagResponse
    {
        [MessageBodyMember(Order = 0)]
        private DataContracts.TagTestCenterToUser ResponseField;
        public DataContracts.TagTestCenterToUser Response
        {
            get
            {
                return this.ResponseField;
            }
            set
            {
                this.ResponseField = value;
            }
        }
    }
}