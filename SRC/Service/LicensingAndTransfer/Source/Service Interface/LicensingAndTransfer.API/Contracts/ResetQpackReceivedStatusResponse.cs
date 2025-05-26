using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.API.Contracts
{
    [MessageContract]
    public class ResetQpackReceivedStatusResponse
    {
        [MessageBodyMember(Order = 0)]
        private System.Int32 UpdatedStatusField;
        public System.Int32 UpdatedStatus
        {
            get
            {
                return this.UpdatedStatusField;

            }
            set
            {
                this.UpdatedStatusField = value;
            }
        }
    }
}
