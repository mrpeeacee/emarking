using Saras.eMarking.Domain.ViewModels.AuditTrail;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Audit
{
    public interface IAuditRepository
    {
        public void InsertAuditLogs(AuditTrailData auditTrailData);
    }
}
