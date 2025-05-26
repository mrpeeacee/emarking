using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;


namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class OMRScannedResponse
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.OMRScannedResponseOuput> ListOMRScannedResponseOutPutField;
        public List<DataContracts.OMRScannedResponseOuput> ListOMRScannedResponseOutPut
        {
            get
            {
                return ListOMRScannedResponseOutPutField;
            }

            set
            {
                ListOMRScannedResponseOutPutField = value;
            }
        }

    }

}
