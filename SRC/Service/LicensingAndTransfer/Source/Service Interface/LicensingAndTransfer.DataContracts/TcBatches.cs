using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "TcBatches")]
    public class TcBatches
    {
        private Int64 GroupIDField;
        [DataMember(IsRequired = true, Name = "GroupID", Order = 0)]
        public Int64 GroupID
        {
            get
            {
                return this.GroupIDField;

            }
            set
            {
                this.GroupIDField = value;
            }
        }

        private string GroupNameField;
        [DataMember(IsRequired = true, Name = "GroupName", Order = 1)]
        public string GroupName
        {
            get
            {
                return this.GroupNameField;

            }
            set
            {
                this.GroupNameField = value;
            }
        }

        private DateTime ScheduleDateField;
        [DataMember(IsRequired = true, Name = "ScheduleDate", Order = 2)]
        public DateTime ScheduleDate
        {
            get
            {
                return this.ScheduleDateField;
            }
            set
            {
                ScheduleDateField = value;
            }
        }
    }
}
