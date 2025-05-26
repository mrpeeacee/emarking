using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "ScannedResponseOutput")]
    public class ScannedResponseOutput
    {


        private System.Boolean ISProcessedField;
        [DataMember(IsRequired = true, Name = "ISProcessedField", Order = 4)]
        public System.Boolean ISProcessed
        {
            get
            {
                return this.ISProcessedField;
            }
            set
            {
                this.ISProcessedField = value;
            }

        }

        private System.String SheetIDField;
        [DataMember(IsRequired = true, Name = "SheetID", Order = 5)]
        public System.String SheetID
        {
            get
            {
                return this.SheetIDField;
            }
            set
            {
                this.SheetIDField = value;
            }
        }

        private System.String LoginNameField;
        [DataMember(IsRequired = true, Name = "LoginName", Order = 6)]
        public System.String LoginName
        {
            get
            {
                return this.LoginNameField;
            }
            set
            {
                this.LoginNameField = value;
            }
        }


    }
}
