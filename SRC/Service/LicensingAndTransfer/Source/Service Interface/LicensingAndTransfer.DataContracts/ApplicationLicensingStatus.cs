using System;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "ApplicationLicensingStatus")]
    public class ApplicationLicensingStatus
    {
        [DataMember(IsRequired = true, Name = "LicensingStatusId", Order = 0)]
        public long LicensingStatusId { get; set; }
        [DataMember(IsRequired = true, Name = "AttemptedCount", Order = 1)]
        public string AttemptedCount { get; set; }
        [DataMember(IsRequired = true, Name = "FailureAttemptCount", Order = 2)]
        public string FailureAttemptCount { get; set; }
        [DataMember(IsRequired = true, Name = "ExpiryPeriod", Order = 3)]
        public string ExpiryPeriod { get; set; }
        [DataMember(IsRequired = true, Name = "CurrentStatus", Order = 4)]
        public string CurrentStatus { get; set; }
        [DataMember(IsRequired = true, Name = "RootOrganizationId", Order = 5)]
        public long RootOrganizationId { get; set; }
    }
}
