using System.ServiceModel;
using System.Collections.Generic;
using LicensingAndTransfer.DataContracts;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class UserRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.User> UserProfilesField;
        /// <summary>
        /// Encrypted Script Data will be sent
        /// </summary>
        public List<DataContracts.User> UserProfile
        {
            get
            {
                return this.UserProfilesField;
            }
            set
            {
                this.UserProfilesField = value;
            }
        }        
    }

    [MessageContract]
    public class RoleRequest 
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.Role> UserRoleField;
        /// <summary>
        /// Encrypted Script Data will be sent
        /// </summary>
        public List<DataContracts.Role> UserRole
        {
            get
            {
                return this.UserRoleField;
            }
            set
            {
                this.UserRoleField = value;
            }
        }    
    }

    [MessageContract]
    public class OrganizationRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.Organization> OrganizationField;

        /// <summary>
        /// Encrypted Script Data will be sent
        /// </summary>
        public List<DataContracts.Organization> Organization
        {
            get
            {
                return this.OrganizationField;
            }
            set
            {
                this.OrganizationField = value;
            }
        }    
    }
}