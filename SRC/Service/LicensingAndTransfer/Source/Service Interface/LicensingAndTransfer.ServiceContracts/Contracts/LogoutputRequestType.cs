using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class LogoutputRequestType
    {
        [MessageBodyMember(Order = 0)]
        private DataContracts.LogoutputRequest requestField;
        public DataContracts.LogoutputRequest Request
        {
            get
            {
                return this.requestField;
            }
            set
            {
                this.requestField = value;
            }
        }
    }
}
