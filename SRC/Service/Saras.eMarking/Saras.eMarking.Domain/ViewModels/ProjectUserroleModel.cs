using Saras.eMarking.Domain.Extensions;
using System;

namespace Saras.eMarking.Domain.ViewModels
{
    public class ProjectUserroleModel
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public long ProjectUserRoleID { get; set; }
        public long UserID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<long> ReportingTo { get; set; }
        [XssTextValidation]
        public string RoleCode { get; set; }
    }
}
