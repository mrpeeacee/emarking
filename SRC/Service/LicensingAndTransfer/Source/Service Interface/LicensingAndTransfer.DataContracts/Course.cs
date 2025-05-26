using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "Course")]
    public  class Course
    {
        private string _courseID;
        private string _courseName;
        private string _courseType;
        private string _courseCode;
        private DateTime _createdDate;
        private DateTime _lastModifiedDate;
         

        [DataMember(IsRequired = true, Name = "CourseID", Order = 0)]
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

        [DataMember(IsRequired = true, Name = "CourseName", Order = 1)]
        public string CourseName
        {
            get
            {
                return _courseName;
            }
            set
            {
                _courseName = value;
            }

        }
        [DataMember(IsRequired = true, Name = "CourseType", Order = 2)]
        public string CourseType
        {
            get
            {
                return _courseType;
            }
            set
            {
                _courseType = value;
            }

        }
        [DataMember(IsRequired = true, Name = "CourseCode", Order = 3)]
        public string CourseCode
        {
            get
            {
                return _courseCode;
            }
            set
            {
                _courseCode = value;
            }

        }
        [DataMember(IsRequired = true, Name = "CreatedDate", Order = 4)]
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

        [DataMember(IsRequired = true, Name = "LastModifiedDate", Order = 5)]
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
