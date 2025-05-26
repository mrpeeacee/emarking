using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "PackageStatistics")]
    public class PackageStatistics
    {
        [DataMember(IsRequired = true, Name = "ID", Order = 0)]
        public System.Int64 ID { get; set; }

        [DataMember(IsRequired = true, Name = "ScheduleID", Order = 1)]
        public System.Int64 ScheduleID { get; set; }

        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 2)]
        public System.Int64 TestCenterID { get; set; }

        [DataMember(IsRequired = true, Name = "PackageType", Order = 3)]
        public System.String PackageType { get; set; }

        [DataMember(IsRequired = true, Name = "GeneratedDate", Order = 4)]
        public System.DateTime GeneratedDate { get; set; }

        [DataMember(IsRequired = true, Name = "TransferredToDataExchangeServer", Order = 5)]
        public System.Boolean TransferredToDataExchangeServer { get; set; }

        [DataMember(IsRequired = true, Name = "TransferredToDataExchangeServerOn", Order = 6)]
        public System.DateTime TransferredToDataExchangeServerOn { get; set; }

        [DataMember(IsRequired = true, Name = "TransferredToTestCenter", Order = 7)]
        public System.Boolean TransferredToTestCenter { get; set; }

        [DataMember(IsRequired = true, Name = "TransferredToTestCenterOn", Order = 8)]
        public System.DateTime TransferredToTestCenterOn { get; set; }

        [DataMember(IsRequired = true, Name = "TransferredToDataCenterDistributed", Order = 9)]
        public System.Boolean TransferredToDataCenterDistributed { get; set; }

        [DataMember(IsRequired = true, Name = "TransferredToDataCenterDistributedOn", Order = 10)]
        public System.DateTime TransferredToDataCenterDistributedOn { get; set; }

        [DataMember(IsRequired = true, Name = "TransferredToDataCenterCentralized", Order = 11)]
        public System.Boolean TransferredToDataCenterCentralized { get; set; }

        [DataMember(IsRequired = true, Name = "TransferredToDataCenterCentralizedOn", Order = 12)]
        public System.DateTime TransferredToDataCenterCentralizedOn { get; set; }

        [DataMember(IsRequired = true, Name = "RecievedFromDataExchangeServer", Order = 13)]
        public System.Int32 RecievedFromDataExchangeServer { get; set; }

        [DataMember(IsRequired = true, Name = "RecievedFromDataExchangeServerOn", Order = 14)]
        public System.DateTime RecievedFromDataExchangeServerOn { get; set; }

        [DataMember(IsRequired = true, Name = "RecievedFromTestCenter", Order = 15)]
        public System.Int32 RecievedFromTestCenter { get; set; }

        [DataMember(IsRequired = true, Name = "RecievedFromTestCenterOn", Order = 16)]
        public System.DateTime RecievedFromTestCenterOn { get; set; }

        [DataMember(IsRequired = true, Name = "RecievedFromDataCenterDistributed", Order = 17)]
        public System.Int32 RecievedFromDataCenterDistributed { get; set; }

        [DataMember(IsRequired = true, Name = "RecievedFromDataCenterDistributedOn", Order = 18)]
        public System.DateTime RecievedFromDataCenterDistributedOn { get; set; }

        [DataMember(IsRequired = true, Name = "RecievedFromDataCenterCentralized", Order = 19)]
        public System.Int32 RecievedFromDataCenterCentralized { get; set; }

        [DataMember(IsRequired = true, Name = "RecievedFromDataCenterCentralizedOn", Order = 20)]
        public System.DateTime RecievedFromDataCenterCentralizedOn { get; set; }

        [DataMember(IsRequired = true, Name = "PackageName", Order = 21)]
        public System.String PackageName { get; set; }

        [DataMember(IsRequired = true, Name = "PackagePassword", Order = 22)]
        public System.String PackagePassword { get; set; }

        [DataMember(IsRequired = true, Name = "PackagePath", Order = 23)]
        public System.String PackagePath { get; set; }

        [DataMember(IsRequired = true, Name = "OrganizationID", Order = 24)]
        public System.Int64 OrganizationID { get; set; }

        [DataMember(IsRequired = true, Name = "OrganizationName", Order = 25)]
        public System.String OrganizationName { get; set; }

        [DataMember(IsRequired = true, Name = "DivisionID", Order = 26)]
        public System.Int64 DivisionID { get; set; }

        [DataMember(IsRequired = true, Name = "DivisionName", Order = 27)]
        public System.String DivisionName { get; set; }

        [DataMember(IsRequired = true, Name = "ProcessID", Order = 28)]
        public System.Int64 ProcessID { get; set; }

        [DataMember(IsRequired = true, Name = "ProcessName", Order = 29)]
        public System.String ProcessName { get; set; }

        [DataMember(IsRequired = true, Name = "EventID", Order = 30)]
        public System.Int64 EventID { get; set; }

        [DataMember(IsRequired = true, Name = "EventName", Order = 31)]
        public System.String EventName { get; set; }

        [DataMember(IsRequired = true, Name = "BatchID", Order = 32)]
        public System.Int64 BatchID { get; set; }

        [DataMember(IsRequired = true, Name = "BatchName", Order = 33)]
        public System.String BatchName { get; set; }

        [DataMember(IsRequired = true, Name = "TestCenterName", Order = 34)]
        public System.String TestCenterName { get; set; }

        [DataMember(IsRequired = true, Name = "ScheduleDate", Order = 35)]
        public System.DateTime ScheduleDate { get; set; }

        [DataMember(IsRequired = true, Name = "ScheduleStartDate", Order = 36)]
        public System.DateTime ScheduleStartDate { get; set; }

        [DataMember(IsRequired = true, Name = "ScheduleEndDate", Order = 37)]
        public System.DateTime ScheduleEndDate { get; set; }

        [DataMember(IsRequired = true, Name = "LeadTimeForQPackDispatchInMinutes", Order = 38)]
        public System.Int32 LeadTimeForQPackDispatchInMinutes { get; set; }

        [DataMember(IsRequired = true, Name = "DeleteQPackAfterExamination", Order = 39)]
        public System.Boolean DeleteQPackAfterExamination { get; set; }

        [DataMember(IsRequired = true, Name = "RPackToBeSentImmediatelyAfterExamination", Order = 40)]
        public System.Boolean RPackToBeSentImmediatelyAfterExamination { get; set; }

        [DataMember(IsRequired = true, Name = "RPackToBeSentAtEOD", Order = 41)]
        public System.Boolean RPackToBeSentAtEOD { get; set; }

        [DataMember(IsRequired = true, Name = "DeleteRPackAfterExamination", Order = 42)]
        public System.Boolean DeleteRPackAfterExamination { get; set; }

        [DataMember(IsRequired = true, Name = "DeleteRPackAtEOD", Order = 43)]
        public System.Boolean DeleteRPackAtEOD { get; set; }

        [DataMember(IsRequired = true, Name = "PackageDeletedStatus", Order = 44)]
        public System.Boolean PackageDeletedStatus { get; set; }

        [DataMember(IsRequired = true, Name = "IsCentralizedPackage", Order = 45)]
        public System.Boolean IsCentralizedPackage { get; set; }

        [DataMember(IsRequired = true, Name = "Extension1", Order = 46)]
        public System.String Extension1 { get; set; }

        [DataMember(IsRequired = true, Name = "Extension2", Order = 47)]
        public System.String Extension2 { get; set; }

        [DataMember(IsRequired = true, Name = "Extension3", Order = 48)]
        public System.String Extension3 { get; set; }

        [DataMember(IsRequired = true, Name = "Extension4", Order = 49)]
        public System.String Extension4 { get; set; }

        [DataMember(IsRequired = true, Name = "Extension5", Order = 50)]
        public System.String Extension5 { get; set; }

        [DataMember(IsRequired = true, Name = "ScheduleDetailID", Order = 51)]
        public System.Int64 ScheduleDetailID { get; set; }

        [DataMember(IsRequired = true, Name = "LoadedDateTestCenter", Order = 52)]
        public System.DateTime LoadedDateTestCenter { get; set; }

        [DataMember(IsRequired = true, Name = "IsPackageGenerated", Order = 53)]
        public System.Boolean IsPackageGenerated { get; set; }

        [DataMember(IsRequired = true, Name = "IsLatest", Order = 54)]
        public System.Boolean IsLatest { get; set; }

        [DataMember(IsRequired = true, Name = "LoadedDateCentralized", Order = 55)]
        public System.DateTime LoadedDateCentralized { get; set; }

        [DataMember(IsRequired = true, Name = "LoadedDateDistributed", Order = 56)]
        public System.DateTime LoadedDateDistributed { get; set; }

        [DataMember(IsRequired = true, Name = "AssessmentID", Order = 57)]
        public System.Int64 AssessmentID { get; set; }

        [DataMember(IsRequired = true, Name = "CandidateCount", Order = 58)]
        public System.Int32 CandidateCount { get; set; }

        [DataMember(IsRequired = true, Name = "ProcessingStatus", Order = 59)]
        public System.Boolean ProcessingStatus { get; set; }

        [DataMember(IsRequired = true, Name = "TestCenterLoadDuration", Order = 60)]
        public System.Int32 TestCenterLoadDuration { get; set; }

        [DataMember(IsRequired = true, Name = "PackageGenerationType", Order = 61)]
        public System.Int16 PackageGenerationType { get; set; }

        [DataMember(IsRequired = true, Name = "IsActivated", Order = 62)]
        public System.Int16 IsActivated { get; set; }
        [DataMember(IsRequired = false, Name = "ExamTypeId", Order = 63)]
        public System.Int64 ExamTypeId { get; set; }
        [DataMember(IsRequired = false, Name = "ExamVersion", Order = 64)]
        public System.Decimal ExamVersion { get; set; }

        [DataMember(IsRequired = true, Name = "IsRecievedEmarkingServer", Order = 65)]
        public System.Boolean IsRecievedEmarkingServer { get; set; }
        [DataMember(IsRequired = true, Name = "RecievedFromEmarkingServerOn", Order = 66)]
        public System.DateTime RecievedFromEmarkingServerOn { get; set; }
        [DataMember(IsRequired = true, Name = "LoadedDateEmarking", Order = 67)]
        public System.DateTime LoadedDateEmarking { get; set; }
    }
}
