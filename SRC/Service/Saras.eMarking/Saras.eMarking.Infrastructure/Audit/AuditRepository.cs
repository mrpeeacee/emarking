using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Audit;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System;
using System.Net;

namespace Saras.eMarking.Infrastructure.Audit
{
    public class AuditRepository : BaseRepository<AuditRepository>, IAuditRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public AuditRepository(ApplicationDbContext context, ILogger<AuditRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }

        public void InsertAuditLogs(AuditTrailData auditTrailData)
        {
            try
            {
                string host = Dns.GetHostName();
                EventAudit entitydata = new()
                {
                    EventAuditId = auditTrailData.EntityId,
                    UserId = auditTrailData.UserId <= 0 ? null : auditTrailData.UserId,                   
                    ProjectUserRoleId = auditTrailData.ProjectUserRoleID > 0 ? auditTrailData.ProjectUserRoleID : null,
                    AssetRef = auditTrailData.AssetRef,
                    SessionId = auditTrailData.SessionId,
                    Ipaddress = Dns.GetHostEntry(host).AddressList[1].ToString(),
                    ModuleId = AppCache.GetAuditModuleMasterId(auditTrailData.Module),
                    EventId = AppCache.GetAuditEventMasterId(auditTrailData.Event),
                    EntityId = AppCache.GetAuditEntityMasterId(auditTrailData.Entity),
                    Status = Convert.ToString((int)auditTrailData.ResponseStatus),
                    EventDateTime = auditTrailData.GetActionDate(),
                    Remarks = auditTrailData.GetRemarks()
                };

                context.EventAudits.Add(entitydata);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"", ex);
            }
        }
    }
}
