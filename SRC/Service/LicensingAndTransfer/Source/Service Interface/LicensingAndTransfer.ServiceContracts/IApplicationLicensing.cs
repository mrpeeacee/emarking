using System;
using System.ServiceModel;

namespace LicensingAndTransfer.ServiceContracts
{
    [ServiceContract(Namespace = "http://LicensingAndTransfer.ServiceContracts/2010/01", Name = "IApplicationLicensing", SessionMode = SessionMode.Allowed, ProtectionLevel = System.Net.Security.ProtectionLevel.None)]
    public interface IApplicationLicensing
    {
        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "GetApplicationLicensingDetailsForTestCenter")]
        ServiceContracts.ApplicationLicensingResponse GetApplicationLicensingDetailsForTestCenter(ServiceContracts.ApplicationLicensingRequest request);
    }
}
