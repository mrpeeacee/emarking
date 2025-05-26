using Saras.eMarking.Inbound.Domain.Models;
using System.Data;
using System.Text;

namespace Saras.eMarking.Inbound.Interfaces.RepositoryInterfaces
{
    public interface IQRLPackRepository
    {
        Task<List<eMarkingSyncUserResponse>> eMarkingQRLpackStatics(DataTable scheduledetails, StringBuilder sbstatus);

    }
}
