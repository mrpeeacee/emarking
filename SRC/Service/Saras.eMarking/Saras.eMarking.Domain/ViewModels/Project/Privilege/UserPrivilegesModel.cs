using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels.Project.Privilege
{
    public class UserPrivilegesModel
    {
        public UserPrivilegesModel()
        {
        }
        public long ID { get; set; }
        [XssTextValidation]
        public string Name { get; set; }
        [XssTextValidation]
        public string RoleCode { get; set; }
        public long ParentID { get; set; }
        [XssTextValidation]
        public string Url { get; set; }
        public int PrivilegeOrder { get; set; }
        [XssTextValidation]
        public string PrivilegeCode { get; set; }
    }
}
