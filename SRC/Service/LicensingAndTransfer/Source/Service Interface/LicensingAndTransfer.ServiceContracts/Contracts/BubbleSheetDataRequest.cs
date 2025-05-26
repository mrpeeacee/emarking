using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class BubbleSheetDataRequest
    {
        [MessageBodyMember(Order = 0)]
        private Int64 ScheduleUserIDField;
        public Int64 ScheduleUserID
        {
            get
            {
                return ScheduleUserIDField;
            }
            set
            {
                ScheduleUserIDField = value;
            }
        }
    }
}
