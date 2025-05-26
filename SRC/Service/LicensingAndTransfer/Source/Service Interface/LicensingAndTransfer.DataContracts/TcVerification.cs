using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "TcVerification")]
    public class TcVerification
    {
        private string PackageTypeField;
        [DataMember(IsRequired = true, Name = "PackageType", Order = 1)]
        public string PackageType
        {
            get
            {
                return this.PackageTypeField;

            }
            set
            {
                this.PackageTypeField = value;
            }
        }

        private string BatchField;
        [DataMember(IsRequired = true, Name = "Batch", Order = 2)]
        public string Batch
        {
            get
            {
                return this.BatchField;

            }
            set
            {
                this.BatchField = value;
            }
        }


        private System.Int32 RftField;
        [DataMember(IsRequired = true, Name = "Rft", Order = 3)]
        public System.Int32 Rft
        {
            get
            {
                return this.RftField;

            }
            set
            {
                this.RftField = value;
            }
        }

        private DateTime GeneratedDateField;
        [DataMember(IsRequired = true, Name = "GeneratedDate", Order = 4)]
        public DateTime GeneratedDate
        {
            get
            {
                return this.GeneratedDateField;
            }
            set
            {
                GeneratedDateField = value;
            }
        }

        private System.Int32 CandidateCountField;
        [DataMember(IsRequired = true, Name = "CandidateCount", Order = 5)]
        public System.Int32 CandidateCount
        {
            get
            {
                return this.CandidateCountField;
            }
            set
            {
                this.CandidateCountField = value;
            }
        }

        private DateTime ScheduleStartDateField;
        [DataMember(IsRequired = true, Name = "ScheduleStartDate", Order = 6)]
        public DateTime ScheduleStartDate
        {
            get
            {
                return this.ScheduleStartDateField;
            }
            set
            {
                ScheduleStartDateField = value;
            }
        }

        private DateTime ScheduleEndDateField;
        [DataMember(IsRequired = true, Name = "ScheduleEndDate", Order = 7)]
        public DateTime ScheduleEndDate
        {
            get
            {
                return this.ScheduleEndDateField;
            }
            set
            {
                ScheduleEndDateField = value;
            }
        }

        private System.Int64 ScheduleDetailIDField;
        [DataMember(IsRequired = true, Name = "ScheduleDetailID", Order = 8)]
        public System.Int64 ScheduleDetailID
        {
            get
            {
                return this.ScheduleDetailIDField;
            }
            set
            {
                this.ScheduleDetailIDField = value;
            }
        }
    }
}
