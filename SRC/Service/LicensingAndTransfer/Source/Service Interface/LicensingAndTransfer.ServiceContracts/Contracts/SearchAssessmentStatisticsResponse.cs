using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class SearchAssessmentStatisticsResponse
    {
        [MessageBodyMember(Order = 1)]
        private List<DataContracts.TestCenetrAssessmentStatistics> ListAssessmentStatisticsField;
        public List<DataContracts.TestCenetrAssessmentStatistics> AssessmentStatistics
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
    }
}
