using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.ProjectUsers
{
    public class DeleteUsersModel
    {
        public DeleteUsersModel()
        {
        }

        [Required]
        public long ProjectUserRoleID { get; set; }
        public long QIGID { get; set; }

    }
}
