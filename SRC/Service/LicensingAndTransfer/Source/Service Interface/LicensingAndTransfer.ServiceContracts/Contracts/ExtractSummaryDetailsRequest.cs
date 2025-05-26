using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public class ExtractSummaryDetailsRequest
    {
        [MessageBodyMember(Order=0)]
        private DataContracts.SummaryDetailsRequest SummaryRequestField;
        public DataContracts.SummaryDetailsRequest SummaryRequest
        {
            get
            {
                return SummaryRequestField;
            }
            set
            {
                SummaryRequestField = value;
            }
        }
    }
}
