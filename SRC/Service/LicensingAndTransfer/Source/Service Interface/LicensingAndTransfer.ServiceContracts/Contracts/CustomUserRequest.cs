using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class CustomUserRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.CustomUser> CustomUserField;
        public List<DataContracts.CustomUser> LstCustomUser
        {
            get
            {
                return this.CustomUserField;
            }
            set
            {
                this.CustomUserField = value;
            }
        }
    }
}
