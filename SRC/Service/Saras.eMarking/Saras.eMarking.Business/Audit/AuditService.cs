using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Audit;
using Saras.eMarking.Domain.ViewModels.AuditTrail;

namespace Saras.eMarking.Business
{
    public class AuditService : BaseService<AuditService>, IAuditService
    {
        readonly IAuditRepository _auditRepository;
        public AuditService(IAuditRepository auditRepository, ILogger<AuditService> _logger, AppOptions appOptions) : base(_logger, appOptions)
        {
            _auditRepository = auditRepository;
        }
        public void InsertAuditLogs(AuditTrailData auditTrailData)
        {
            if (AppOptions.AppSettings.IsAuditLogEnabled)
            {
                _auditRepository.InsertAuditLogs(auditTrailData);
            }
        }
    }
}
