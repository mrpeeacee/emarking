using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class ResetQpackReceivedStatusRequest
    {
        [MessageBodyMember(Order = 0)]
        private string MacIDField;
        public string MacID
        {
            get
            {
                return this.MacIDField;
            }
            set
            {
                MacIDField = value;
            }
        }
        [MessageBodyMember(Order = 1)]
        private Int64 SidField;
        public Int64 Sid
        {
            get
            {
                return this.SidField;
            }
            set
            {
                SidField = value;
            }
        }
    }
}
