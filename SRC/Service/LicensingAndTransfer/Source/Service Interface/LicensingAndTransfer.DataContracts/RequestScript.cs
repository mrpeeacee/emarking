using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "RequestScript")]
    public class RequestScript
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


        
        private System.String MacIDField;
                [DataMember(IsRequired = true, Name = "MacID", Order = 0)]
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
                [DataMember(IsRequired = true, Name = "Script", Order = 0)]
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


        
        private ScriptType TypeField;
        [DataMember(IsRequired = true, Name = "Type", Order = 0)]
        public ScriptType Type
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
    public enum ScriptType
    {
        Database=0,      
        FileSystem=1
    }
    
}
