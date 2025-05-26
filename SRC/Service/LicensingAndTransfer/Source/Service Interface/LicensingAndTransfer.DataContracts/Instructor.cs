using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "Instructor")]
    public class Instructor
    {
        private string _instructorID;
        private string _firstName;
        private string _lastName;
        private string _middleName;
        private string _EmailID;
        private DateTime _createdDate;
        private DateTime _lastModifiedDate;


        [DataMember(IsRequired = true, Name = "InstructorID", Order = 0)]
        public string InstructorID
        {
            get
            {
                return _instructorID;
            }
            set
            {
                _instructorID = value;
            }

        }

        [DataMember(IsRequired = true, Name = "FirstName", Order = 1)]
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
            }

        }

        [DataMember(IsRequired = true, Name = "LastName", Order = 2)]
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
            }

        }

        [DataMember(IsRequired = true, Name = "MiddleName", Order = 3)]
        public string MiddleName
        {
            get
            {
                return _middleName;
            }
            set
            {
                _middleName = value;
            }

        }

        [DataMember(IsRequired = true, Name = "Emailid", Order = 4)]
        public string EmailID
        {
            get
            {
                return _EmailID;
            }
            set
            {
                _EmailID = value;
            }

        }

        [DataMember(IsRequired = true, Name = "CreatedDate", Order = 5)]
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

        [DataMember(IsRequired = true, Name = "LastModifiedDate", Order = 6)]
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
