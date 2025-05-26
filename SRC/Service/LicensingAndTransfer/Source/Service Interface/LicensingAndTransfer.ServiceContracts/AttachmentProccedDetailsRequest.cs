using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts.Contracts
{
   [MessageContract]
    public class AttachmentProccedDetailsRequest
    {
           [MessageBodyMember(Order=0)]
       private List<DataContracts.AttachmentProcessedDetails> ListAttachmentProcessDetailsField;
           public List<DataContracts.AttachmentProcessedDetails> ListAttachmetnProcessDetails
           {
               get
               {
                   return this.ListAttachmentProcessDetailsField;
               }
               set
               {
                   this.ListAttachmentProcessDetailsField = value;
               }
           }
   }
}
