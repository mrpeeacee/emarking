using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public  class ExtractAssessmentDetailsRequest
    {
        [MessageBodyMember(Order = 0)]
        private DataContracts.AssessmentDetailsRequest AssessmentDetailRequestField;
        public DataContracts.AssessmentDetailsRequest AssessmentField
        {
            get
            {
                return AssessmentDetailRequestField;
            }
            set
            {
                AssessmentDetailRequestField = value;
            }
        }
       
    }
}
