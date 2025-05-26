using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class UpdateAssessmentStatisticsRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.AssessmentStatistics> ListAssessmentStatisticsField;
        public List<DataContracts.AssessmentStatistics> ListAssessmentStatistics
        {
            get
            {
                return this.ListAssessmentStatisticsField;
            }
            set
            {
                this.ListAssessmentStatisticsField = value;
            }
        }

        [MessageBodyMember(Order = 2)]
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
