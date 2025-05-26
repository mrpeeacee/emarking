using Saras.eMarking.Inbound.Domain.Models;
using System.Data;

namespace Saras.eMarking.Inbound.Interfaces.RepositoryInterfaces
{
    public interface IUsersRepository
    {
        Task<List<eMarkingSyncUserResponse>> SynceMarkingUser(DataTable filteredDT, string? Status, bool IsUserSyncFromDelivery);
    }
}
