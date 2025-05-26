using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "CourseBlock")]
    public class CourseBlock
    {
        private string _courseBlockID;
        private string _courseID;
        private string _courseBlockName;
        private string _instructorName;
        private string _location;
        private DateTime _startDate;
        private DateTime _endDate;
        private DateTime _createdDate;
        private DateTime _lastModifiedDate;

        [DataMember(IsRequired = true, Name = "CourseBlockID", Order = 0)]
        public string CourseBlockID
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
        [DataMember(IsRequired = true, Name = "CourseID", Order = 1)]
        public string CourseID
        {
            get
            {
                return _courseID;
            }
            set
            {
                _courseID = value;
            }

        }

        [DataMember(IsRequired = true, Name = "CourseBlockName", Order = 2)]
        public string CourseBlockName
        {
            get
            {
                return _courseBlockName;
            }
            set
            {
                _courseBlockName = value;
            }

        }
        [DataMember(IsRequired = true, Name = "InstructorName", Order = 3)]
        public string InstructorName
        {
            get
            {
                return _instructorName;
            }
            set
            {
                _instructorName = value;
            }

        }
        [DataMember(IsRequired = true, Name = "Location", Order = 4)]
        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }

        }

        [DataMember(IsRequired = true, Name = "StartDate", Order = 5)]
        public DateTime StartDate
        {
            get
            {
                return _startDate;;
            }
            set
            {
                _startDate = value;
            }

        }

        [DataMember(IsRequired = true, Name = "EndDate", Order = 6)]
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
        [DataMember(IsRequired = true, Name = "CreatedDate", Order = 7)]
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

        [DataMember(IsRequired = true, Name = "LastModifiedDate", Order = 8)]
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
