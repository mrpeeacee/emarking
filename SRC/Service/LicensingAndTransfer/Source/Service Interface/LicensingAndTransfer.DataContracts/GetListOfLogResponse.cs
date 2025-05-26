using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "GetListOfLogResponse")]
   public class GetListOfLogResponse
    {
       
        private System.Int64 TestCenterLogDetailIDField;
        [DataMember(IsRequired = true, Name = "TestCenterLogDetailID", Order = 0)]
        public System.Int64 TestCenterLogDetailID
        {
            get
            {
                return this.TestCenterLogDetailIDField;
            }
            set
            {
                this.TestCenterLogDetailIDField = value;
            }
        }
        private System.String TestCenterCodeField;
        [DataMember(IsRequired = true, Name = "TestCenterCode", Order = 1)]
        public System.String TestCenterCode
        {
            get
            {
                return this.TestCenterCodeField;
            }
            set
            {
                this.TestCenterCodeField = value;
            }
        }

        private System.DateTime FromDateField;
        [DataMember(IsRequired = true, Name = "FromDate", Order = 2)]
        public System.DateTime FromDate
        {
            get
            {
                return this.FromDateField;
            }
            set
            {
                this.FromDateField = value;
            }
        }

        private System.DateTime TODateField;
        [DataMember(IsRequired = true, Name = "TODate", Order = 3)]
        public System.DateTime TODate
        {
            get
            {
                return this.TODateField;
            }
            set
            {
                this.TODateField = value;
            }
        }

        private System.Boolean ISSystemLogRequiredField;
        [DataMember(IsRequired = true, Name = "ISSystemLogRequired", Order = 4)]
        public System.Boolean ISSystemLogRequired
        {
            get
            {
                return this.ISSystemLogRequiredField;
            }
            set
            {
                this.ISSystemLogRequiredField = value;
            }
        }

        private System.Boolean ISApplicationLogRequiredField;
        [DataMember(IsRequired = true, Name = "ISApplicationLogRequired", Order = 5)]
        public System.Boolean ISApplicationLogRequired
        {
            get
            {
                return this.ISApplicationLogRequiredField;
            }
            set
            {
                this.ISApplicationLogRequiredField = value;
            }
        }

        private System.Boolean ISCronLogRequiredField;
        [DataMember(IsRequired = true, Name = "ISCronLogRequired", Order = 6)]
        public System.Boolean ISCronLogRequired
        {
            get
            {
                return this.ISCronLogRequiredField;
            }
            set
            {
                this.ISCronLogRequiredField = value;
            }
        }
    }
}
