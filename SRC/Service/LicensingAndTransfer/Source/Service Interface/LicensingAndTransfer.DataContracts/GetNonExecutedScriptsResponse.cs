using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LicensingAndTransfer.DataContracts
{
      [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "GetNonExecutedScriptsResponse")]
    public class GetNonExecutedScriptsResponse
    {
        private System.Int64 TestCenterIDField;
        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 0)]
        public System.Int64 TestCenterID
        {
            get
            {
                return this.TestCenterIDField;
            }
            set
            {
                this.TestCenterIDField = value;
            }
        }
        private System.Int64 IDField;
        [DataMember(IsRequired = true, Name = "ID", Order = 1)]
        public System.Int64 ID
        {
            get
            {
                return this.IDField;
            }
            set
            {
                this.IDField = value;
            }
        }

        private System.String MacIDField;
          [DataMember(IsRequired = true, Name = "MacID", Order = 2)]
        public System.String MacID
        {
            get
            {
                return this.MacIDField;
            }
            set
            {
                this.MacIDField = value;
            }
        }


        private System.String ScriptField;
          [DataMember(IsRequired = true, Name = "Script", Order = 3)]
        public System.String Script
        {
            get
            {
                return this.ScriptField;
            }
            set
            {
                this.ScriptField = value;
            }
        }

        private System.Int32 TypeField;
          [DataMember(IsRequired = true, Name = "Type", Order = 4)]
        public System.Int32 Type
        {
            get
            {
                return this.TypeField;
            }
            set
            {
                this.TypeField = value;
            }
        }
    }
}
