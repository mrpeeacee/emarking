using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class TcBatchesResponse
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.TcBatches> ListTcBatchesField;
        public List<DataContracts.TcBatches> ListTcBatches
        {
            get
            {
                return this.ListTcBatchesField;
            }
            set
            {
                this.ListTcBatchesField = value;
            }
        }
    }
}
