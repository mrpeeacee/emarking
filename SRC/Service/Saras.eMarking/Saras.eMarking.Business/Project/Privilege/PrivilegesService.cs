using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Privilege;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Privilege;
using Saras.eMarking.Domain.ViewModels.Project.Privilege;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Privilege
{
    public class PrivilegesService : BaseService<PrivilegesService>, IPrivilegeService
    {
        readonly IPrivilegeRepository _privilegeRepository;
        public PrivilegesService(IPrivilegeRepository privilegeRepository, ILogger<PrivilegesService> _logger) : base(_logger)
        {
            _privilegeRepository = privilegeRepository;
        }


        public async Task<IList<UserPrivilegesModel>> GetUserPrivileges(int Type, long ProjectUserRoleID, long UserId)
        {
            try
            {
                var TrailMarkingScriptQIGs = await _privilegeRepository.GetUserPrivileges(Type, ProjectUserRoleID, UserId);
                return TrailMarkingScriptQIGs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while getting UserPrivileges: Method Name: GetUserPrivileges(): ProjectUserRoleID=" + Convert.ToString(ProjectUserRoleID), "Error while getting UserPrivileges: Method Name: GetUserPrivileges(): UserId=" + UserId.ToString());
                throw;
            }
        }
    }
}
