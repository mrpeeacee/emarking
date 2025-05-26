using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "ScheduledDetails")]
    public class ScheduledDetails
    {
        private System.Int64 ScheduleDetailIDField;
        [DataMember(IsRequired = true, Name = "ScheduleDetailID", Order = 1)]
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

        private System.Int32 NumberOfScheduledUsersField;
        [DataMember(IsRequired = true, Name = "NumberOfScheduledUsers", Order = 2)]
        public System.Int32 NumberOfScheduledUsers
        {
            get
            {
                return this.NumberOfScheduledUsersField;
            }
            set
            {
                this.NumberOfScheduledUsersField = value;
            }
        }

        private System.Int32 NumberOfSubmittedUsersField;
        [DataMember(IsRequired = true, Name = "NumberOfSubmittedUsers", Order = 3)]
        public System.Int32 NumberOfSubmittedUsers
        {
            get
            {
                return this.NumberOfSubmittedUsersField;
            }
            set
            {
                this.NumberOfSubmittedUsersField = value;
            }
        }
    }
}
