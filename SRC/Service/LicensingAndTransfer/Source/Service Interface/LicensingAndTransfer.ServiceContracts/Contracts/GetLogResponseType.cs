using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class GetLogResponseType
    {
        [MessageBodyMember(Order = 0)]
        private DataContracts.GetListOfLogResponse responseField;
        public DataContracts.GetListOfLogResponse Response
        {
            get
            {
                return this.responseField;
            }
            set
            {
                responseField = value;
            }
        }
    }
}
