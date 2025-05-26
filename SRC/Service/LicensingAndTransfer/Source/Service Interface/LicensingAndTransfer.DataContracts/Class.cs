using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "Class")]
   public class Class
    {
        private string _classID;
        private string _name;
        private string _instructors;
        private DateTime _startDate;
        private DateTime _endDate;
        private string _courseBlockID;
        private string _eventid;
        private string _eventName;
        private string _eventCode;
        private string _trainingPartner;
        private DateTime _createdDate;
        private DateTime _lastModifiedDate;

        [DataMember(IsRequired = true, Name = "ClassID", Order = 0)]
        public string ClassID
        {
            get
            {
                return _classID;
            }
            set
            {
                _classID = value;
            }
        }

        [DataMember(IsRequired = true, Name = "Name", Order = 1)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        [DataMember(IsRequired = true, Name = "Instructors", Order = 2)]
        public string Instructors
        {
            get
            {
                return _instructors;
            }
            set
            {
                _instructors = value;
            }
        }

        [DataMember(IsRequired = true, Name = "StartDate", Order = 3)]
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }

        [DataMember(IsRequired = true, Name = "EndDate", Order = 4)]
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
            }
        }

        [DataMember(IsRequired = true, Name = "CourseBlockid", Order = 5)]
        public string CourseBlockid
        {
            get
            {
                return _courseBlockID;
            }
            set
            {
                _courseBlockID = value;
            }
        }

        [DataMember(IsRequired = true, Name = "EventID", Order = 6)]
        public string EventID
        {
            get
            {
                return _eventid;
            }
            set
            {
                _eventid = value;
            }
        }

        [DataMember(IsRequired = true, Name = "EventName", Order = 7)]
        public string EventName
        {
            get
            {
                return _eventName;
            }
            set
            {
                _eventName = value;
            }
        }

        [DataMember(IsRequired = true, Name = "EventCode", Order = 8)]
        public string EventCode
        {
            get
            {
                return _eventCode;
            }
            set
            {
                _eventCode = value;
            }
        }

        [DataMember(IsRequired = true, Name = "TrainingPartner", Order = 9)]
        public string TrainingPartner
        {
            get
            {
                return _trainingPartner;
            }
            set
            {
                _trainingPartner = value;
            }
        }

        [DataMember(IsRequired = true, Name = "CreatedDate", Order = 10)]
        public DateTime CreatedDate
        {
            get
            {
                return _createdDate;
            }
            set
            {
                _createdDate = value;
            }
        }

        [DataMember(IsRequired = true, Name = "LastModifiedDate", Order = 11)]
        public DateTime LastModifiedDate
        {
            get
            {
                return _lastModifiedDate;
            }
            set
            {
                _lastModifiedDate = value;
            }
        }
    }
}
