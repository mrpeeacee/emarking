using System;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts
{     
    [ServiceContract(Namespace = "http://LicensingAndTransfer.ServiceContracts/2010/01", Name = "IAttendance", SessionMode = SessionMode.Allowed, ProtectionLevel = System.Net.Security.ProtectionLevel.None)]
    public interface IAttendance
    {
        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "PersistDailyScheduleSummary")]
        void PersistDailyScheduleSummary(ServiceContracts.PersistDailyScheduleSummaryRequest request);
    }
}
 