using Saras.eMarking.Domain.ViewModels.AuditTrail;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface
{
    public interface IAuditService
    {
        public void InsertAuditLogs(AuditTrailData auditTrailData);
    }
}
