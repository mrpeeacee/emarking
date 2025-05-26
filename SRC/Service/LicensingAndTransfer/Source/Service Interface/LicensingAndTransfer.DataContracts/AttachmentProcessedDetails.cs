using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace LicensingAndTransfer.DataContracts
{
   [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "AttachmentProcessedDetails")]
    public class AttachmentProcessedDetails
    {

       private System.Int64 AttachmentDetailIDField;
        [DataMember(IsRequired = false, Name = "AttachmentDetailID", Order = 0)]
       public System.Int64 AttachmentDetailID
        {
            get
            {
                return this.AttachmentDetailIDField;
            }
            set
            {
                this.AttachmentDetailIDField = value;
            }
        }
       private System.Int64 AttachmentIDField;
        [DataMember(IsRequired = true, Name = "AttachmentID", Order = 1)]
        public System.Int64 AttachmentID
        {
            get
            {
                return this.AttachmentIDField;
            }
            set
            {
                this.AttachmentIDField = value;
            }
        }

        private System.String ImageNameField;
        [DataMember(IsRequired = true, Name = "ImageName", Order = 2)]
        public System.String ImageName
        {
            get
            {
                return this.ImageNameField;
            }
            set
            {
                this.ImageNameField = value;
            }
        }
        private System.Byte ImageConvertedStatusField;
        [DataMember(IsRequired = false, Name = "ImageConvertedStatus", Order = 3)]
        public System.Byte ImageConvertedStatus
        {
            get
            {
                return this.ImageConvertedStatusField;
            }
            set
            {
                this.ImageConvertedStatusField = value;
            }
        }
        private System.Byte ImageTransferredStatusField;
        [DataMember(IsRequired = false, Name = "ImageTransferredStatus", Order = 4)]
        public System.Byte ImageTransferredStatus
        {
            get
            {
                return this.ImageTransferredStatusField;
            }
            set
            {
                this.ImageTransferredStatusField = value;
            }
        }

        private System.DateTime ImageTransferedDateField;
        [DataMember(IsRequired = false, Name = "ImageTransferedDate", Order = 5)]
        public System.DateTime ImageTransferedDate
        {
            get
            {
                return this.ImageTransferedDateField;
            }
            set
            {
                this.ImageTransferedDateField = value;
            }
        }

        private System.Byte ImageProcessedStatusField;
        [DataMember(IsRequired = false, Name = "ImageProcessedStatus", Order = 6)]
        public System.Byte ImageProcessedStatus
        {
            get
            {
                return this.ImageProcessedStatusField;
            }
            set
            {
                this.ImageProcessedStatusField = value;
            }
        }

        private System.DateTime ImageProcessedDateField;
        [DataMember(IsRequired = false, Name = "ImageProcessedDate", Order = 7)]
        public System.DateTime ImageProcessedDate
        {
            get
            {
                return this.ImageProcessedDateField;
            }
            set
            {
                this.ImageProcessedDateField = value;
            }
        }
        private System.String AttachmentFileNameField;
        [DataMember(IsRequired = false, Name = "AttachmentFileName", Order = 8)]
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

        private System.String AttachmentDetailStatusField;
        [DataMember(IsRequired = false, Name = "AttachmentDetailStatus", Order = 9)]
        public System.String AttachmentDetailStatus
        {
            get
            {
                return this.AttachmentDetailStatusField;
            }
            set
            {
                this.AttachmentDetailStatusField = value;
            }
        }
    }
}
