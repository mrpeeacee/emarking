using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace="http://LicensingAndTransfer.DataContracts/2010/01",Name="SummaryDetailRequest")]
    public class SummaryDetailsRequest
    {
        [DataMember(IsRequired = true, Name = "StartDate", Order = 0)]
        private DateTime StartDateField;
        public DateTime StartDate
        {
            get
            {
                return StartDateField;
            }
            set
            {
                StartDateField = value;
            }
        }

        [DataMember(IsRequired = true, Name = "EndDate", Order = 1)]
        private DateTime EndDateField;
        public DateTime EndDate
        {
            get
            {
                return EndDateField;
            }
            set
            {
                EndDateField = value;
            }
        }

        [DataMember(IsRequired = true, Name = "OrganizationID", Order = 2)]
        private Int64 OrganizationIDField;
        public Int64 Organization
        {
            get
            {
                return OrganizationIDField;
            }

        }

        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 3)]
        private Int64 TestCenterIDField;
        public Int64 TestCenterID
        {
            get
            {
                return TestCenterIDField;
            }
            set
            {
                TestCenterIDField = value;
            }
        }

    }
}
