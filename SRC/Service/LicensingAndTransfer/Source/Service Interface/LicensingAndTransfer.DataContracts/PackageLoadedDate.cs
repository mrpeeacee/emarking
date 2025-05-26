using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "PackageLoadedDate")]
    public class PackageLoadedDate
    {
        private System.Int64 ScheduleDetailIDField;
        [DataMember(IsRequired = true, Name = "ScheduleDetailID", Order = 0)]
        public System.Int64 ScheduleDetailID
        {
            get { return this.ScheduleDetailIDField; }
            set { this.ScheduleDetailIDField = value; }
        }

        private System.String PackageTypeField;
        [DataMember(IsRequired = true, Name = "PackageType", Order = 1)]
        public System.String PackageType
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

        private System.DateTime GeneratedDateField;
        [DataMember(IsRequired = true, Name = "GeneratedDate", Order = 2)]
        public System.DateTime GeneratedDate
        {
            get
            {
                return this.GeneratedDateField;
            }
            set
            {
                this.GeneratedDateField = value;
            }
        }

        private System.DateTime LoadedDateTestCenterField;
        [DataMember(IsRequired = true, Name = "LoadedDateTestCenter", Order = 3)]
        public System.DateTime LoadedDateTestCenter
        {
            get
            {
                return this.LoadedDateTestCenterField;
            }
            set
            {
                this.LoadedDateTestCenterField = value;
            }
        }

        private System.DateTime LoadedDateCentralizedField;
        [DataMember(IsRequired = true, Name = "LoadedDateCentralized", Order = 4)]
        public System.DateTime LoadedDateCentralized
        {
            get
            {
                return this.LoadedDateCentralizedField;
            }
            set
            {
                this.LoadedDateCentralizedField = value;
            }
        }

        private System.DateTime LoadedDateDistributedField;
        [DataMember(IsRequired = true, Name = "LoadedDateDistributed", Order = 5)]
        public System.DateTime LoadedDateDistributed
        {
            get
            {
                return this.LoadedDateDistributedField;
            }
            set
            {
                this.LoadedDateDistributedField = value;
            }
        }
        
        private System.String Extension5Field;
        [DataMember(IsRequired = true, Name = "Extension5", Order = 6)]
        public System.String Extension5
        {
            get
            {
                return this.Extension5Field;
            }
            set
            {
                this.Extension5Field = value;
            }
        }

        private System.Int32 TestCenterLoadDurationField;
        [DataMember(IsRequired = true, Name = "TestCenterLoadDuration", Order = 7)]
        public System.Int32 TestCenterLoadDuration
        {
            get
            {
                return this.TestCenterLoadDurationField;
            }
            set
            {
                this.TestCenterLoadDurationField = value;
            }
        }

        private System.Int64 TestCenterIDField;
        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 8)]
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
        private System.DateTime LoadedDateEmarkingCenterField;
        [DataMember(IsRequired = true, Name = "LoadedDateEmarking", Order = 9)]
        public System.DateTime LoadedDateEmarking
        {
            get
            {
                return this.LoadedDateEmarkingCenterField;
            }
            set
            {
                this.LoadedDateEmarkingCenterField = value;
            }
        }
    }
}
