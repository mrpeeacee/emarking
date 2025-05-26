using System.ServiceModel;
using System.Collections.Generic;
using System;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class PersistDailyScheduleSummaryRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.PersistDailyScheduleSummary> ListPersistDailyScheduleSummaryField;
        public List<DataContracts.PersistDailyScheduleSummary> ListPersistDailyScheduleSummary
        {
            get
            {
                return this.ListPersistDailyScheduleSummaryField;
            }
            set
            {
                this.ListPersistDailyScheduleSummaryField = value;
            }
        }
    }
}
