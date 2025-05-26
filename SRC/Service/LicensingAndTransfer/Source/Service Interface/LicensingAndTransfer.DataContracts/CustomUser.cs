using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "CustomUser")]
   public class CustomUser
    {
        private string _userID;
        private string _firstName;
        private string _lastName;
        private string _middleName;
        private string _emailID;
        private string _status;
        private Boolean _lep;
        private string _ell;
        private string _ld;
        private string _preferedLanguage;
        private string _languageFluentin;
        private DateTime _createdDate;
        private DateTime _lastModifiedDate;

        [DataMember(IsRequired = true, Name = "UserID", Order = 0)]
        public string UserID
        {
            get
            {
                return _userID;
            }
            set
            {
                _userID = value;
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

        [DataMember(IsRequired = true, Name = "EmailID", Order = 4)]
        public string Emailid
        {
            get
            {
                return _emailID;
            }
            set
            {
                _emailID = value;
            }
        }

        [DataMember(IsRequired = true, Name = "Status", Order = 5)]
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        [DataMember(IsRequired = true, Name = "LEP", Order = 6)]
        public Boolean LEP
        {
            get
            {
                return _lep;
            }
            set
            {
                _lep = value;
            }
        }

        [DataMember(IsRequired = true, Name = "ELL", Order = 7)]
        public string ELL
        {
            get
            {
                return _ell;
            }
            set
            {
                _ell = value;
            }
        }


        [DataMember(IsRequired = true, Name = "Ld", Order = 8)]
        public string Ld
        {
            get
            {
                return _ld;
            }
            set
            {
                _ld = value;
            }
        }

        [DataMember(IsRequired = true, Name = "PreferedLanguage", Order = 9)]
        public string PreferedLanguage
        {
            get
            {
                return _preferedLanguage;
            }
            set
            {
                _preferedLanguage = value;
            }
        }

        [DataMember(IsRequired = true, Name = "LanguageFluentin", Order = 10)]
        public string LanguageFluentin
        {
            get
            {
                return _languageFluentin;
            }
            set
            {
                _languageFluentin = value;
            }
        }

        [DataMember(IsRequired = true, Name = "CreatedDate", Order = 11)]
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

        [DataMember(IsRequired = true, Name = "LastModifiedDate", Order = 12)]
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
