using System;
using System.Collections.Generic;
using System.ServiceModel;


namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class ScriptRequestType
    {
        [MessageBodyMember(Order = 0)]
        private DataContracts.RequestScript RequestField;
        public DataContracts.RequestScript Request
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
