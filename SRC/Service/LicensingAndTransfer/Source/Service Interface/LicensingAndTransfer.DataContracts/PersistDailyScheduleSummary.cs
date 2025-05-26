using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "PersistDailyScheduleSummary")]
    public class PersistDailyScheduleSummary    
    {
        private System.Int64 ScheduleDetailIDField;
        [DataMember(IsRequired = true, Name = "ScheduleDetailID", Order = 0)]
        public System.Int64 ScheduleDetailID
        {
            get { return this.ScheduleDetailIDField; }
            set { this.ScheduleDetailIDField = value; }
        }

        private System.Int32 NoofCandidatesInBatchField;
        [DataMember(IsRequired = true, Name = "NoofCandidatesInBatch", Order = 1)]
        public System.Int32 NoofCandidatesInBatch
        {
            get { return this.NoofCandidatesInBatchField; }
            set { this.NoofCandidatesInBatchField = value; }
        }

        private System.Int32 NoofCandidatesRegisteredField;
        [DataMember(IsRequired = true, Name = "NoofCandidatesRegistered", Order = 2)]
        public System.Int32 NoofCandidatesRegistered
        {
            get { return this.NoofCandidatesRegisteredField; }
            set { this.NoofCandidatesRegisteredField = value; }
        }

        private Int32 NumberOfCandidateStartedExamField;
        [DataMember(IsRequired = true, Name = "NumberOfCandidateStartedExam", Order = 3)]
        public Int32 NumberOfCandidateStartedExam
        {
            get { return this.NumberOfCandidateStartedExamField; }
            set { this.NumberOfCandidateStartedExamField = value; }
        }

        private Int32 NumberOfCandidateCompletedExamField;
        [DataMember(IsRequired = true, Name = "NumberOfCandidateCompletedExam", Order = 4)]
        public Int32 NumberOfCandidateCompletedExam
        {
            get { return this.NumberOfCandidateCompletedExamField; }
            set { this.NumberOfCandidateCompletedExamField = value; }
        }

        private DateTime FirstPersonStartedTimeField;
        [DataMember(IsRequired = true, Name = "FirstPersonStartedTime", Order = 5)]
        public DateTime FirstPersonStartedTime
        {
            get { return this.FirstPersonStartedTimeField; }
            set { this.FirstPersonStartedTimeField = value; }
        }

        private DateTime LastPersonClosedTimeField;
        [DataMember(IsRequired = true, Name = "LastPersonClosedTime", Order = 6)]
        public DateTime LastPersonClosedTime
        {
            get { return this.LastPersonClosedTimeField; }
            set { this.LastPersonClosedTimeField = value; }
        }

        private List<PersistDailyScheduleSummaryForUser> UserListField;
        [DataMember(IsRequired = true, Name = "UserList", Order = 7)]
        public List<PersistDailyScheduleSummaryForUser> UserList
        {
            get
            {
                return this.UserListField;
            }
            set
            {
                this.UserListField = value;
            }
        }
        private System.Int64 ProductIDField;
        [DataMember(IsRequired = true, Name = "ProductID", Order = 8)]
        public System.Int64 ProductID
        {
            get { return this.ProductIDField; }
            set { this.ProductIDField = value; }
        }
        private System.Int64 TestCenterIDField;
        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 9)]
        public System.Int64 TestCenterID
        {
            get { return this.TestCenterIDField; }
            set { this.TestCenterIDField = value; }
        }

        private System.Int64 BatchIDField;
        [DataMember(IsRequired = true, Name = "BatchID", Order = 10)]
        public System.Int64 BatchID
        {
            get { return this.BatchIDField; }
            set { this.BatchIDField = value; }
        }
        private DateTime FirstPersonClosedTimeField;
        [DataMember(IsRequired = true, Name = "FirstPersonClosedTime", Order = 8)]
        public DateTime FirstPersonClosedTime
        {
            get { return this.FirstPersonClosedTimeField; }
            set { this.FirstPersonClosedTimeField = value; }
        }

        private DateTime LastPersonStartedTimeField;
        [DataMember(IsRequired = true, Name = "LastPersonStartedTime", Order = 9)]
        public DateTime LastPersonStartedTime
        {
            get { return this.LastPersonStartedTimeField; }
            set { this.LastPersonStartedTimeField = value; }
        }

        private System.Int64 ScheduleIDField;
        [DataMember(IsRequired = true, Name = "ScheduleID", Order = 10)]
        public System.Int64 ScheduleID
        {
            get { return this.ScheduleIDField; }
            set { this.ScheduleIDField = value; }
        }
        private System.DateTime TestStartDateField;
        [DataMember(IsRequired = false, Name = "TestStartDate", Order = 11)]
        public System.DateTime TestStartDate
        {
            get { return this.TestStartDateField; }
            set { this.TestStartDateField = value; }
        }
        private System.DateTime TestEndDateField;
        [DataMember(IsRequired = false, Name = "TestEndDate", Order = 12)]
        public System.DateTime TestEndDate
        {
            get { return this.TestEndDateField; }
            set { this.TestEndDateField = value; }
        }
    }

    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "PersistDailyScheduleSummaryForUser")]
    public class PersistDailyScheduleSummaryForUser
    {
        private System.Int64 ScheduleUserIDField;
        [DataMember(IsRequired = true, Name = "ScheduleUserID", Order = 0)]
        public System.Int64 ScheduleUserID
        {
            get
            {
                return this.ScheduleUserIDField;
            }
            set
            {
                this.ScheduleUserIDField = value;
            }
        }

        private System.String LoginNameField;
        [DataMember(IsRequired = true, Name = "LoginName", Order = 1)]
        public System.String LoginName
        {
            get
            {
                return this.LoginNameField;
            }
            set
            {
                this.LoginNameField = value;
            }
        }

        private System.Boolean IsRegisteredField;
        [DataMember(IsRequired = true, Name = "IsRegistered", Order = 2)]
        public System.Boolean IsRegistered
        {
            get
            {
                return this.IsRegisteredField;
            }
            set
            {
                this.IsRegisteredField = value;
            }
        }

        private System.Int32 UserResponseCountField;
        [DataMember(IsRequired = true, Name = "UserResponseCount", Order = 2)]
        public System.Int32 UserResonseCount
        {
            get
            {
                return this.UserResponseCountField;
            }
            set
            {
                this.UserResponseCountField = value;
            }
        }

        private System.DateTime StartedDateTimeField;
        [DataMember(IsRequired = true, Name = "StartedDateTime", Order = 3)]
        public System.DateTime StartedDateTime
        {
            get
            {
                return this.StartedDateTimeField;
            }
            set
            {
                this.StartedDateTimeField = value;
            }
        }

        private System.Int32 UserStatusField;
        [DataMember(IsRequired=true,Name ="UserStatus",Order=4)]
        public System.Int32 UserStatus
        {
            get
            {
                return this.UserStatusField;
            }
            set
            {
                this.UserStatusField = value;
            }
        }

        private System.DateTime EndDateTimeField;
        [DataMember(IsRequired = true, Name = "EndDateTime", Order = 5)]
        public System.DateTime EndDateTime
        {
            get
            {
                return this.EndDateTimeField;
            }
            set
            {
                this.EndDateTimeField = value;
            }
        }

        private System.Int32 TimeRemainingField;
        [DataMember(IsRequired = true, Name = "TimeRemaining", Order = 6)]
        public System.Int32 TimeRemaining
        {
            get
            {
                return this.TimeRemainingField;
            }
            set
            {
                this.TimeRemainingField = value;
            }
        }

        private System.String ResponsesField;
        [DataMember(IsRequired = true, Name = "Responses", Order = 7)]
        public System.String Responses
        {
            get
            {
                return this.ResponsesField;
            }
            set
            {
                this.ResponsesField = value;
            }
        }
        private System.Int64 ProductIDField;
        [DataMember(IsRequired = true, Name = "ProductID", Order = 8)]
        public System.Int64 ProductID
        {
            get { return this.ProductIDField; }
            set { this.ProductIDField = value; }
        }
        private System.Int64 TestCenterIDField;
        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 9)]
        public System.Int64 TestCenterID
        {
            get { return this.TestCenterIDField; }
            set { this.TestCenterIDField = value; }
        }
        private System.Int64 BatchIDField;
        [DataMember(IsRequired = false, Name = "BatchID", Order = 10)]
        public Int64 BatchID
        {
            get { return this.BatchIDField; }
            set { this.BatchIDField = value; }
        }
        private System.DateTime SubmittedDateField;
        [DataMember(IsRequired = false, Name = "SubmittedDate", Order = 11)]
        public DateTime SubmittedDate
        {
            get { return SubmittedDateField; }
            set { SubmittedDateField = value; }
        }
        private System.DateTime TestStartDateField;
        [DataMember(IsRequired = false, Name = "TestStartDate", Order = 12)]
        public DateTime TestStartDate
        {
            get { return TestStartDateField; }
            set { TestStartDateField = value; }
        }
        private System.DateTime TestEndDateField;
        [DataMember(IsRequired = false, Name = "TestEndDate", Order = 13)]
        public DateTime TestEndDate
        {
            get { return TestEndDateField; }
            set { TestEndDateField = value; }
        }
    }
}
