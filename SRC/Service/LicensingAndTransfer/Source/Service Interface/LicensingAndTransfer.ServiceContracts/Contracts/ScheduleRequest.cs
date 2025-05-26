using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class ScheduleRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.ScheduledDetails> ListScheduledDetailsField;
        public List<DataContracts.ScheduledDetails> ListScheduledDetails
        {
            get
            {
                return this.ListScheduledDetailsField;
            }
            set
            {
                this.ListScheduledDetailsField = value;
            }
        }
    }
}
