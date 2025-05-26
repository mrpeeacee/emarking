using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public   class GetLogRequestType
    {
        [MessageBodyMember(Order = 0)]
        private DataContracts.GetListOfLogRequest requestField;
        public DataContracts.GetListOfLogRequest request
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
