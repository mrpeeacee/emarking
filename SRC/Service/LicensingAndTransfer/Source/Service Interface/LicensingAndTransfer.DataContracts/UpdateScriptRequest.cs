using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "UpdateScriptRequest")]
    public class UpdateScriptRequest
    {
        private System.Int64 IDField;
        [DataMember(IsRequired = true, Name = "ID", Order = 0)]
        public System.Int64 ID
        {
            get { return this.IDField; }
            set { this.IDField = value; }
        }
        private System.String ScriptOutputField;
        [DataMember(IsRequired = true, Name = "ScriptOutput", Order = 1)]
        public System.String ScriptOutput
        {
            get { return this.ScriptOutputField; }
            set { this.ScriptOutputField = value; }
        }

        private System.Int64 TestCenterIDField;
        [DataMember(IsRequired = true, Name = "TestCenterID", Order = 2)]
        public System.Int64 TestCenterID
        {
            get { return this.TestCenterIDField; }
            set { this.TestCenterIDField = value; }
        }
    }
}
