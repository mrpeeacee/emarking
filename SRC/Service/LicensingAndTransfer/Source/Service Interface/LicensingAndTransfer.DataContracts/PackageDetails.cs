using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "PackageDetails")]
    public class PackageDetails
    {
        private System.Int64 ScheduleDetailIDField;
        [DataMember(IsRequired = true, Name = "ScheduleDetailID", Order = 0)]
        public System.Int64 ScheduleDetailID
        {
            get
            {
                return this.ScheduleDetailIDField;
            }
            set
            {
                this.ScheduleDetailIDField = value;
            }
        }

        private System.Int64 CenterIDField;
        [DataMember(IsRequired = true, Name = "CenterID", Order = 1)]
        public System.Int64 CenterID
        {
            get
            {
                return this.CenterIDField;
            }
            set
            {
                this.CenterIDField = value;
            }
        }

        private PackageOperation OperationField;
        [DataMember(IsRequired = true, Name = "Operation", Order = 2)]
        public PackageOperation Operation
        {
            get
            {
                return this.OperationField;
            }
            set
            {
                this.OperationField = value;
            }
        }
    }

    public enum PackageOperation
    {
        None = 0,
        PackageReGenerate = 1,
        PackageReTransfer = 2
    }
}