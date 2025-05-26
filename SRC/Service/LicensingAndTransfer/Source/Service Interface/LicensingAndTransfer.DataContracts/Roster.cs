using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "Roster")]
    public class Roster
    {
        private string _rosterID;
        private string _classID;
        private string _status;
        private string _userID;
        private string _EmailID;
        private DateTime _createdDate;
        private DateTime _lastModifiedDate;

        [DataMember(IsRequired = true, Name = "RosterID", Order = 0)]
        public string RosterID
        {
            get
            {
                return _rosterID;
            }
            set
            {
                _rosterID = value;
            }

        }

        [DataMember(IsRequired = true, Name = "ClassID", Order = 1)]
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

        [DataMember(IsRequired = true, Name = "Status", Order = 2)]
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
        [DataMember(IsRequired = true, Name = "UserID", Order = 3)]
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
