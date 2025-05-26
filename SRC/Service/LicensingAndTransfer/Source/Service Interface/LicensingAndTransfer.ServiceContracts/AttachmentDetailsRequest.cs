using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class AttachmentDetailsRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.AttachmentDetails> ListAttachmentDetailsField;
        public List<DataContracts.AttachmentDetails> ListAttachmentDetails
        {
            get
            {
                return this.ListAttachmentDetailsField;
            }
            set
            {
                this.ListAttachmentDetailsField = value;
            }
        }
    }
}
