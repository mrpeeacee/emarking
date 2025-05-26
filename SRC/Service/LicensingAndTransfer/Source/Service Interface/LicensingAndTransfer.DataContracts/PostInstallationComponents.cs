using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "PostInstallationComponents")]
    public class PostInstallationComponents
    {
        private System.Int64 TestCenterIDField;
        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 0)]
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

        private System.Boolean IsCronDownloadQpackField;
        [DataMember(IsRequired = true, Name = "IsCronDownloadQpack", Order = 1)]
        public System.Boolean IsCronDownloadQpack
        {
            get
            {
                return this.IsCronDownloadQpackField;
            }
            set
            {
                this.IsCronDownloadQpackField = value;

            }

        }

        private System.Boolean IsCronloadQpackField;
        [DataMember(IsRequired = true, Name = "IsCronloadQpack", Order = 2)]
        public System.Boolean IsCronloadQpack
        {
            get
            {
                return this.IsCronloadQpackField;
            }
            set
            {
                this.IsCronloadQpackField = value;
            }
        }

        private System.Boolean IsCronManageScheduleField;
        [DataMember(IsRequired = true, Name = "IsCronManageSchedule", Order = 3)]
        public System.Boolean IsCronManageSchedule
        {
            get
            {
                return this.IsCronManageScheduleField;
            }
            set
            {
                this.IsCronManageScheduleField = value;
            }
        }

        private System.Boolean IsVDMESField;
        [DataMember(IsRequired = true, Name = "IsVDMES", Order = 4)]
        public System.Boolean IsVDMES
        {
            get
            {
                return this.IsVDMESField;
            }
            set
            {
                this.IsVDMESField = value;
            }
        }

        private System.Boolean IsVDTCDashboardField;
        [DataMember(IsRequired = true, Name = "IsVDTCDashboard", Order = 5)]
        public System.Boolean IsVDTCDashboard
        {
            get
            {
                return this.IsVDTCDashboardField;
            }
            set
            {
                this.IsVDTCDashboardField = value;
            }
        }

        private System.Boolean IsVDTestplayerField;
        [DataMember(IsRequired = true, Name = "IsVDTestplayer", Order = 6)]
        public System.Boolean IsVDTestplayer
        {
            get
            {
                return this.IsVDTestplayerField;
            }
            set
            {
                this.IsVDTestplayerField = value;
            }
        }

        private System.Boolean IsFileExistField;
        [DataMember(IsRequired = true, Name = "IsFileExist", Order = 7)]
        public System.Boolean IsFileExist
        {
            get
            {
                return this.IsFileExistField;
            }
            set
            {
                this.IsFileExistField = value;
            }
        }

        private System.Boolean IsEnvironmentVerifiedField;
        [DataMember(IsRequired = true, Name = "IsEnvironmentVerified", Order = 8)]
        public System.Boolean IsEnvironmentVerified
        {
            get
            {
                return this.IsEnvironmentVerifiedField;
            }
            set
            {
                this.IsEnvironmentVerifiedField = value;
            }
        }

        private System.Boolean IsFTPExistsField;
        [DataMember(IsRequired = true, Name = "IsFTPExists", Order = 9)]
        public System.Boolean IsFTPExists
        {
            get
            {
                return this.IsFTPExistsField;
            }
            set
            {
                this.IsFTPExistsField = value;
            }
        }

        private System.Boolean IsTCVerifiedField;
        [DataMember(IsRequired = true, Name = "IsTCVerified", Order = 10)]
        public System.Boolean IsTCVerified
        {
            get
            {
                return this.IsTCVerifiedField;
            }
            set
            {
                this.IsTCVerifiedField = value;
            }
        }

        private System.Int32 ScheduledCountField;
        [DataMember(IsRequired = true, Name = "ScheduledCount", Order = 11)]
        public System.Int32 ScheduledCount
        {
            get
            {
                return this.ScheduledCountField;
            }
            set
            {
                this.ScheduledCountField = value;
            }
        }

        private System.Int32 SubmittedCountField;
        [DataMember(IsRequired = true, Name = "SubmittedCount", Order = 12)]
        public System.Int32 SubmittedCount
        {
            get
            {
                return this.SubmittedCountField;
            }
            set
            {
                this.SubmittedCountField = value;
            }
        }

        private System.Int32 InProgressCountField;
        [DataMember(IsRequired = true, Name = "InProgressCount", Order = 13)]
        public System.Int32 InProgressCount
        {
            get
            {
                return this.InProgressCountField;
            }
            set
            {
                this.InProgressCountField = value;
            }
        }

        private System.DateTime ExtractedDateField;
        [DataMember(IsRequired = true, Name = "ExtractedDate", Order = 14)]
        public System.DateTime ExtractedDate
        {
            get
            {
                return this.ExtractedDateField;
            }
            set
            {
                this.ExtractedDateField = value;
            }
        }

        private System.String SystemDetailsField;
        [DataMember(IsRequired = true, Name = "SystemDetails", Order = 15)]
        public System.String SystemDetails
        {
            get
            {
                return this.SystemDetailsField;
            }
            set
            {
                this.SystemDetailsField = value;
            }
        }

        private System.String Extension1Field;
        [DataMember(IsRequired = true, Name = "Extension1", Order = 16)]
        public System.String Extension1
        {
            get
            {
                return this.Extension1Field;
            }
            set
            {
                this.Extension1Field = value;
            }
        }

        private System.String MacIDField;
        [DataMember(IsRequired = true, Name = "MacID", Order = 17)]
        public System.String MacID
        {
            get
            {
                return MacIDField;
            }
            set
            {
                MacIDField = value;
            }
        }

    }
}
