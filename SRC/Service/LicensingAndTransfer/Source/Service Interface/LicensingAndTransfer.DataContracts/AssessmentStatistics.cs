using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "AssessmentStatistics")]
    public class AssessmentStatistics
    {
        private System.Int64 AssessmentIDField;
        [DataMember(IsRequired = true, Name = "AssessmentID", Order = 0)]
        public System.Int64 AssessmentID
        {
            get
            {
                return this.AssessmentIDField;
            }
            set
            {
                this.AssessmentIDField = value;
            }
        }

        private System.String AssessmentNameField;
        [DataMember(IsRequired = true, Name = "AssessmentName", Order = 1)]
        public System.String AssessmentName
        {
            get
            {
                return this.AssessmentNameField;
            }
            set
            {
                this.AssessmentNameField = value;
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

        private System.Int32 TransferredToDXField;
        [DataMember(IsRequired = true, Name = "TransferredToDX", Order = 3)]
        public System.Int32 TransferredToDX
        {
            get
            {
                return this.TransferredToDXField;
            }
            set
            {
                this.TransferredToDXField = value;
            }
        }

        private System.DateTime TransferredToDXOnField;
        [DataMember(IsRequired = true, Name = "TransferredToDXOn", Order = 4)]
        public System.DateTime TransferredToDXOn
        {
            get
            {
                return this.TransferredToDXOnField;
            }
            set
            {
                this.TransferredToDXOnField = value;
            }
        }

        private System.Int32 RecievedFromDXField;
        [DataMember(IsRequired = true, Name = "RecievedFromDX", Order = 5)]
        public System.Int32 RecievedFromDX
        {
            get
            {
                return this.RecievedFromDXField;
            }
            set
            {
                this.RecievedFromDXField = value;
            }
        }

        private System.DateTime RecievedFromDXOnField;
        [DataMember(IsRequired = true, Name = "RecievedFromDXOn", Order = 6)]
        public System.DateTime RecievedFromDXOn
        {
            get
            {
                return this.RecievedFromDXOnField;
            }
            set
            {
                this.RecievedFromDXOnField = value;
            }
        }

        private System.String PackageNameField;
        [DataMember(IsRequired = true, Name = "PackageName", Order = 7)]
        public System.String PackageName
        {
            get
            {
                return this.PackageNameField;
            }
            set
            {
                this.PackageNameField = value;
            }
        }

        private System.String PackagePasswordField;
        [DataMember(IsRequired = true, Name = "PackagePassword", Order = 8)]
        public System.String PackagePassword
        {
            get
            {
                return this.PackagePasswordField;
            }
            set
            {
                this.PackagePasswordField = value;
            }
        }

        private System.String StatusDescriptionField;
        [DataMember(IsRequired = true, Name = "StatusDescription", Order = 9)]
        public System.String StatusDescription
        {
            get
            {
                return this.StatusDescriptionField;
            }
            set
            {
                this.StatusDescriptionField = value;
            }
        }

        private System.Boolean IsPackageGeneratedField;
        [DataMember(IsRequired = true, Name = "IsPackageGenerated", Order = 10)]
        public System.Boolean IsPackageGenerated
        {
            get
            {
                return this.IsPackageGeneratedField;
            }
            set
            {
                this.IsPackageGeneratedField = value;
            }
        }

        private System.Boolean IsLatestField;
        [DataMember(IsRequired = true, Name = "IsLatest", Order = 11)]
        public System.Boolean IsLatest
        {
            get
            {
                return this.IsLatestField;
            }
            set
            {
                this.IsLatestField = value;
            }
        }
    }

    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "TestCenterAssessmentPacks")]
    public class TestCenterAssessmentPacks
    {
        private System.Int64 AssessmentIDField;
        [DataMember(IsRequired = true, Name = "AssessmentID", Order = 0)]
        public System.Int64 AssessmentID
        {
            get
            {
                return this.AssessmentIDField;
            }
            set
            {
                this.AssessmentIDField = value;
            }
        }

        private System.Int64 TestCenterIDField;
        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 1)]
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

        private System.Int32 TransferredToTCField;
        [DataMember(IsRequired = true, Name = "TransferredToTC", Order = 2)]
        public System.Int32 TransferredToTC
        {
            get
            {
                return this.TransferredToTCField;
            }
            set
            {
                this.TransferredToTCField = value;
            }
        }

        private System.DateTime TransferredToTCOnField;
        [DataMember(IsRequired = true, Name = "TransferredToTCOn", Order = 3)]
        public System.DateTime TransferredToTCOn
        {
            get
            {
                return this.TransferredToTCOnField;
            }
            set
            {
                this.TransferredToTCOnField = value;
            }
        }

        private System.Int32 RecievedFromTCField;
        [DataMember(IsRequired = true, Name = "RecievedFromTC", Order = 4)]
        public System.Int32 RecievedFromTC
        {
            get
            {
                return this.RecievedFromTCField;
            }
            set
            {
                this.RecievedFromTCField = value;
            }
        }

        private System.DateTime RecievedFromTCOnField;
        [DataMember(IsRequired = true, Name = "RecievedFromTCOn", Order = 5)]
        public System.DateTime RecievedFromTCOn
        {
            get
            {
                return this.RecievedFromTCOnField;
            }
            set
            {
                this.RecievedFromTCOnField = value;
            }
        }

        private System.String StatusDescriptionField;
        [DataMember(IsRequired = true, Name = "StatusDescription", Order = 6)]
        public System.String StatusDescription
        {
            get
            {
                return this.StatusDescriptionField;
            }
            set
            {
                this.StatusDescriptionField = value;
            }
        }

        private System.DateTime LoadedDateField;
        [DataMember(IsRequired = true, Name = "LoadedDate", Order = 7)]
        public System.DateTime LoadedDate
        {
            get
            {
                return this.LoadedDateField;
            }
            set
            {
                this.LoadedDateField = value;
            }
        }

        private System.Int32 LeadTimeForDispatchInMinutesField;
        [DataMember(IsRequired = true, Name = "LeadTimeForDispatchInMinutes", Order = 8)]
        public System.Int32 LeadTimeForDispatchInMinutes
        {
            get
            {
                return this.LeadTimeForDispatchInMinutesField;
            }
            set
            {
                this.LeadTimeForDispatchInMinutesField = value;
            }
        }

        private System.Int64 ScheduleDetailIDField;
        [DataMember(IsRequired = true, Name = "ScheduleDetailID", Order = 9)]
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

    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "TestCenetrAssessmentStatistics")]
    public class TestCenetrAssessmentStatistics
    {
        private System.Int64 AssessmentIDField;
        [DataMember(IsRequired = true, Name = "AssessmentID", Order = 0)]
        public System.Int64 AssessmentID
        {
            get
            {
                return this.AssessmentIDField;
            }
            set
            {
                this.AssessmentIDField = value;
            }
        }

        private System.String AssessmentNameField;
        [DataMember(IsRequired = true, Name = "AssessmentName", Order = 1)]
        public System.String AssessmentName
        {
            get
            {
                return this.AssessmentNameField;
            }
            set
            {
                this.AssessmentNameField = value;
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

        private System.Int32 TransferredToDXField;
        [DataMember(IsRequired = true, Name = "TransferredToDX", Order = 3)]
        public System.Int32 TransferredToDX
        {
            get
            {
                return this.TransferredToDXField;
            }
            set
            {
                this.TransferredToDXField = value;
            }
        }

        private System.DateTime TransferredToDXOnField;
        [DataMember(IsRequired = true, Name = "TransferredToDXOn", Order = 4)]
        public System.DateTime TransferredToDXOn
        {
            get
            {
                return this.TransferredToDXOnField;
            }
            set
            {
                this.TransferredToDXOnField = value;
            }
        }

        private System.Int32 RecievedFromDXField;
        [DataMember(IsRequired = true, Name = "RecievedFromDX", Order = 5)]
        public System.Int32 RecievedFromDX
        {
            get
            {
                return this.RecievedFromDXField;
            }
            set
            {
                this.RecievedFromDXField = value;
            }
        }

        private System.DateTime RecievedFromDXOnField;
        [DataMember(IsRequired = true, Name = "RecievedFromDXOn", Order = 6)]
        public System.DateTime RecievedFromDXOn
        {
            get
            {
                return this.RecievedFromDXOnField;
            }
            set
            {
                this.RecievedFromDXOnField = value;
            }
        }

        private System.String PackageNameField;
        [DataMember(IsRequired = true, Name = "PackageName", Order = 7)]
        public System.String PackageName
        {
            get
            {
                return this.PackageNameField;
            }
            set
            {
                this.PackageNameField = value;
            }
        }

        private System.String PackagePasswordField;
        [DataMember(IsRequired = true, Name = "PackagePassword", Order = 8)]
        public System.String PackagePassword
        {
            get
            {
                return this.PackagePasswordField;
            }
            set
            {
                this.PackagePasswordField = value;
            }
        }

        private System.Int32 LeadTimeForDispatchInMinutesField;
        [DataMember(IsRequired = true, Name = "LeadTimeForDispatchInMinutes", Order = 9)]
        public System.Int32 LeadTimeForDispatchInMinutes
        {
            get
            {
                return this.LeadTimeForDispatchInMinutesField;
            }
            set
            {
                this.LeadTimeForDispatchInMinutesField = value;
            }
        }

        private System.String StatusDescriptionField;
        [DataMember(IsRequired = true, Name = "StatusDescription", Order = 10)]
        public System.String StatusDescription
        {
            get
            {
                return this.StatusDescriptionField;
            }
            set
            {
                this.StatusDescriptionField = value;
            }
        }

        private System.Boolean IsPackageGeneratedField;
        [DataMember(IsRequired = true, Name = "IsPackageGenerated", Order = 11)]
        public System.Boolean IsPackageGenerated
        {
            get
            {
                return this.IsPackageGeneratedField;
            }
            set
            {
                this.IsPackageGeneratedField = value;
            }
        }

        private System.Boolean IsLatestField;
        [DataMember(IsRequired = true, Name = "IsLatest", Order = 12)]
        public System.Boolean IsLatest
        {
            get
            {
                return this.IsLatestField;
            }
            set
            {
                this.IsLatestField = value;
            }
        }

        private System.Int64 TestCenterIDField;
        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 13)]
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

        private System.Int32 TransferredToTCField;
        [DataMember(IsRequired = true, Name = "TransferredToTC", Order = 14)]
        public System.Int32 TransferredToTC
        {
            get
            {
                return this.TransferredToTCField;
            }
            set
            {
                this.TransferredToTCField = value;
            }
        }

        private System.DateTime TransferredToTCOnField;
        [DataMember(IsRequired = true, Name = "TransferredToTCOn", Order = 15)]
        public System.DateTime TransferredToTCOn
        {
            get
            {
                return this.TransferredToTCOnField;
            }
            set
            {
                this.TransferredToTCOnField = value;
            }
        }

        private System.Int32 RecievedFromTCField;
        [DataMember(IsRequired = true, Name = "RecievedFromTC", Order = 16)]
        public System.Int32 RecievedFromTC
        {
            get
            {
                return this.RecievedFromTCField;
            }
            set
            {
                this.RecievedFromTCField = value;
            }
        }

        private System.DateTime RecievedFromTCOnField;
        [DataMember(IsRequired = true, Name = "RecievedFromTCOn", Order = 17)]
        public System.DateTime RecievedFromTCOn
        {
            get
            {
                return this.RecievedFromTCOnField;
            }
            set
            {
                this.RecievedFromTCOnField = value;
            }
        }

        private System.DateTime LoadedDateField;
        [DataMember(IsRequired = true, Name = "LoadedDate", Order = 18)]
        public System.DateTime LoadedDate
        {
            get
            {
                return this.LoadedDateField;
            }
            set
            {
                this.LoadedDateField = value;
            }
        }

        private System.Int64 ScheduleDetailIDField;
        [DataMember(IsRequired = true, Name = "ScheduleDetailID", Order = 19)]
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
