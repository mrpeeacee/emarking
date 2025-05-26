using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts.Contracts
{
    [MessageContract]
    public  class ExtractAssessmentDetailsResponse
    {
        [MessageBodyMember(Order = 0)]
        private DataContracts.AssessmentDetailsResponse AssessmentDetailsResponseField;
        public DataContracts.AssessmentDetailsResponse AssessmentDetailsResponse
        {
            get
            {
                return this.AssessmentDetailsResponseField;
            }
            set
            {
                this.AssessmentDetailsResponseField = value;
            }
        }
    }
}
