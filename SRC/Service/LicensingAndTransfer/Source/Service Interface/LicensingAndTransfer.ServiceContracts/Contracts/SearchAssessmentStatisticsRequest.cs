using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class SearchAssessmentStatisticsRequest
    {
        [MessageBodyMember(Order = 1)]
        private System.String MacIDField;
        public System.String MacID
        {
            get
            {
                return this.MacIDField;
            }
            set
            {
                this.MacIDField = value;
            }
        }    
    }
}
