using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class TcVerificationResponse
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.TcVerification> ListTcVerificationField;
        public List<DataContracts.TcVerification> ListTcVerification
        {
            get
            {
                return this.ListTcVerificationField;
            }
            set
            {
                this.ListTcVerificationField = value;
            }
        }
    }
}
