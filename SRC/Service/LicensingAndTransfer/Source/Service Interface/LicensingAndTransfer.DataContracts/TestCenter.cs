using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "TestCenter")]
    public class TestCenter
    {
        [DataMember(IsRequired = true, Name = "ID", Order = 0)]
        public System.Int64 ID { get; set; }
        [DataMember(IsRequired = true, Name = "MacID", Order = 1)]
        public System.String MacID { get; set; }
        [DataMember(IsRequired = true, Name = "CenterName", Order = 2)]
        public System.String CenterName { get; set; }
        [DataMember(IsRequired = true, Name = "CenterCode", Order = 3)]
        public System.String CenterCode { get; set; }
        [DataMember(IsRequired = true, Name = "IsAvailable", Order = 4)]
        public System.Boolean IsAvailable { get; set; }
        [DataMember(IsRequired = true, Name = "AddressID", Order = 5)]
        public System.Int64 AddressID { get; set; }
        [DataMember(IsRequired = true, Name = "CreatedBy", Order = 6)]
        public System.Int64 CreatedBy { get; set; }
        [DataMember(IsRequired = true, Name = "CreatedDate", Order = 7)]
        public System.DateTime CreatedDate { get; set; }
        [DataMember(IsRequired = true, Name = "ModifiedBy", Order = 8)]
        public System.Int64 ModifiedBy { get; set; }
        [DataMember(IsRequired = true, Name = "ModifiedDate", Order = 9)]
        public System.DateTime ModifiedDate { get; set; }
        [DataMember(IsRequired = true, Name = "ParentID", Order = 10)]
        public System.Int64 ParentID { get; set; }
        [DataMember(IsRequired = true, Name = "LocationID", Order = 11)]
        public System.Int64 LocationID { get; set; }

        [DataMember(IsRequired = true, Name = "OrganizationID", Order = 12)]
        public System.Int64 OrganizationID { get; set; }
        [DataMember(IsRequired = false, Name = "IsDeleted", Order = 13)]
        public System.Boolean IsDeleted { get; set; }
        [DataMember(IsRequired = false, Name = "CenterTypeId", Order = 14)]
        public System.Int32 CenterTypeId { get; set; }

        [DataMember(IsRequired = true, Name = "IPAddress", Order = 15)]
        public System.String IPAddress { get; set; }

        [DataMember(IsRequired = true, Name = "Status", Order = 16)]
        public System.String Status { get; set; }

    }

    public class TagTestCenterToUser
    {
        [DataMember(IsRequired = true, Name = "ID", Order = 0)]
        public System.Int64 ID { get; set; }
        [DataMember(IsRequired = true, Name = "UserID", Order = 1)]
        public System.Int64 UserID { get; set; }
        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 2)]
        public System.Int64 TestCenterID { get; set; }
        [DataMember(IsRequired = false, Name = "IsDeleted", Order = 3)]
        public System.Int16 IsDeleted { get; set; }
        [DataMember(IsRequired = false, Name = "IsTransfered", Order = 4)]
        public System.Int16 IsTransfered { get; set; }
    }
}