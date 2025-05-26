using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "Batch")]
    public class Batch    
    {
        private System.Int64 ScheduleDetailIDField;
        [DataMember(IsRequired = true, Name = "ScheduleDetailID", Order = 0)]
        public System.Int64 ScheduleDetailID
        {
            get { return this.ScheduleDetailIDField; }
            set { this.ScheduleDetailIDField = value; }
        }

        private System.Int64 UserIDField;
        [DataMember(IsRequired = true, Name = "UserID", Order = 1)]
        public System.Int64 UserID
        {
            get { return this.UserIDField; }
            set { this.UserIDField = value; }
        }

        private System.Int64 TestCenterIDField;
        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 2)]
        public System.Int64 TestCenterID
        {
            get
            {
                return this.TestCenterIDField;
            }
            set
            {
                this.TestCenterIDField = value;
            }
        }

        private System.DateTime PreviousEndDateField;
        [DataMember(IsRequired = true, Name = "PreviousEndDate", Order = 3)]
        public System.DateTime PreviousEndDate
        {
            get { return this.PreviousEndDateField; }
            set { this.PreviousEndDateField = value; }
        }

        private System.DateTime PresentEndDateField;
        [DataMember(IsRequired = true, Name = "PresentEndDate", Order = 4)]
        public System.DateTime PresentEndDate
        {
            get { return this.PresentEndDateField; }
            set { this.PresentEndDateField = value; }
        }

        private System.Boolean IsNotifiedField;
        [DataMember(IsRequired = true, Name = "IsNotified", Order = 5)]
        public System.Boolean IsNotified
        {
            get { return this.IsNotifiedField; }
            set { this.IsNotifiedField = value; }
        }

    }
}
