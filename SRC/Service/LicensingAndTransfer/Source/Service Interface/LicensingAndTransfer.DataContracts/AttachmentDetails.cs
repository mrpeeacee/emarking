using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace LicensingAndTransfer.DataContracts
{
     [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "AttachmentDetails")]
    public class AttachmentDetails
    {
         private System.String AttachmentFileNameField;
        [DataMember(IsRequired = true, Name = "AttachmenFileName", Order = 0)]
         public System.String AttachmentFileName
        {
            get
            {
                return this.AttachmentFileNameField;
            }
            set
            {
                this.AttachmentFileNameField = value;
            }
        }

        private System.Boolean IsProcessedField;
        [DataMember(IsRequired = true, Name = "IsProcessed", Order = 1)]
        public System.Boolean IsProcessed
        {
            get
            {
                return this.IsProcessedField;
            }
            set
            {
                this.IsProcessedField = value;
            }
        }

        private System.String ExtensionField;
        [DataMember(IsRequired = true, Name = "Extension", Order = 1)]
        public System.String Extension
        {
            get
            {
                return this.ExtensionField;
            }
            set
            {
                this.ExtensionField = value;
            }
        }

    }
}
