using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "GetNonExecutedScriptsRequest")]
    public class GetNonExecutedScriptsRequest
    {
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
    }
}
