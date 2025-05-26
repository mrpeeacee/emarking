using System;

namespace Saras.eMarking.Domain.ViewModels.AuditTrail
{
    [Serializable]
    public class LoginAction : IAuditTrail
    {
        public string LoginName { get; set; }
        
    }

    [Serializable]
    public class LoggoutAction : IAuditTrail
    {
        public string Logout { get; set; }
    }
}
