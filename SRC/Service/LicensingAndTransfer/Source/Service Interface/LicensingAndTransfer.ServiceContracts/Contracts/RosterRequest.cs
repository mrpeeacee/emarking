using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
   public  class RosterRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.Roster> LstRosterField;
        public List<DataContracts.Roster> LstRoster
        {
            get
            {
                return LstRosterField;
            }
            set
            {
                LstRosterField = value;
            }
        }

    }
}
