using System.Threading;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Interfaces
{
    public interface IApplicationDBContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
