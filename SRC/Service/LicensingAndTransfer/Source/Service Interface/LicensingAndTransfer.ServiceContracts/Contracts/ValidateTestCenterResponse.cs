using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class ValidateTestCenterResponse
    {
        [MessageBodyMember(Order = 0)]
        private System.String StatusCodeField;
        public System.String StatusCode
        {
            get
            {
                return this.StatusCodeField;
            }
            set
            {
                this.StatusCodeField = value;
            }
        }

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
        private System.String MessageField;
        public System.String Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }    
    }
}
