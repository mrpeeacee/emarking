using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts
{
        [MessageContract]
    public class UpdateScriptsRequestType
    {
        [MessageBodyMember(Order = 0)]
        private DataContracts.UpdateScriptRequest RequestField;
        public DataContracts.UpdateScriptRequest Request
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
