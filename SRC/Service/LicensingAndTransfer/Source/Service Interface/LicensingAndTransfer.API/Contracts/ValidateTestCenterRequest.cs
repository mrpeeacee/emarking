using System.ServiceModel;
using System.Collections.Generic;
using System;

namespace LicensingAndTransfer.API
{
    [MessageContract]
    public class SendMailRequestModel
    {
        [MessageBodyMember(Order = 0)]
        private long QueueIdField;
        public long QueueID
        {
            get
            {
                return this.QueueIdField;
            }
            set
            {
                this.QueueIdField = value;
            }
        }

        [MessageBodyMember(Order = 1)]
        private string StatusField;
        public string Status
        {
            get
            {
                return this.StatusField;
            }
            set
            {
                this.StatusField = value;
            }
        }
    }

    public class MailDeactivateModel
    { 
        public Int64 UserWarningDuration { get; set; }
        public Int64 UserDisableDuration { get; set; }

    }

}
