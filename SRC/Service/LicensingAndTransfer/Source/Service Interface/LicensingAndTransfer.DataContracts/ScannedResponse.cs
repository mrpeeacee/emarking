using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "ScannedResponse")]
    public class ScannedResponse
    {
        private System.Int64 ScannedResponseIdField;
        [DataMember(IsRequired = true, Name = "ScannedResponseId", Order = 0)]
        public System.Int64 ScannedResponseId
        {
            get
            {
                return this.ScannedResponseIdField;
            }
            set
            {
                this.ScannedResponseIdField = value;
            }
        }

        private System.String ScannedFileNameField;
        [DataMember(IsRequired = true, Name = "FileName", Order = 1)]
        public System.String ScannedFileName
        {
            get
            {
                return this.ScannedFileNameField;
            }
            set
            {
                this.ScannedFileNameField = value;
            }
        }
         
        private System.String ResponseStringField;
        [DataMember(IsRequired = true, Name = "ResponseString", Order = 2)]
        public System.String ResponseString
        {
            get
            {
                return this.ResponseStringField;
            }
            set
            {
                this.ResponseStringField = value;
            }
        }

        private System.DateTime InsertedDateField;
        [DataMember(IsRequired = true, Name = "InsertedDate", Order = 3)]
        public System.DateTime InsertedDate
        {
            get
            {
                return this.InsertedDateField;
            }
            set
            {
                this.InsertedDateField = value;
            }
        }

        private System.Boolean ISProcessedField;
        [DataMember(IsRequired = false, Name = "ISProcessedField", Order = 4)]
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

        private System.String SubjectCodeField;
        [DataMember(IsRequired = true, Name = "SubjectCode", Order = 7)]
        public System.String SubjectCode
        {
            get
            {
                return this.SubjectCodeField;
            }
            set
            {
                this.SubjectCodeField = value;
            }
        }
        private bool ISEncryptedField=false;
        [DataMember(IsRequired = false, Name = "ISEncrypted", Order = 7)]
        public bool ISEncrypted
        {
            get
            {
                return this.ISEncryptedField;
            }
            set
            {
                this.ISEncryptedField = value;
            }
        }
       
        private Int64 PageNumberField = 1;
        [DataMember(IsRequired = false, Name = "PageNumber", Order = 8)]
        public Int64 PageNumber
        {
            get
            {
                return this.PageNumberField;
            }
            set
            {
                this.PageNumberField = value;
            }
        }
        
        private Int64 PageSizeField = 1;
        [DataMember(IsRequired = false, Name = "PageSize", Order = 9)]
        public Int64 PageSize
        {
            get
            {
                return this.PageSizeField;
            }
            set
            {
                this.PageSizeField = value;
            }
        }
        private System.String Extension1Field;
        [DataMember(IsRequired = false, Name = "Extension1", Order = 10)]
        public System.String Extension1
        {
            get
            {
                return this.Extension1Field;
            }
            set
            {
                this.Extension1Field = value;
            }
        }
        private System.String MarkedImagePathField;
        [DataMember(IsRequired = false, Name = "MarkedImagePath", Order = 11)]
        public System.String MarkedImagePath
        {
            get
            {
                return this.MarkedImagePathField;
            }
            set
            {
                this.MarkedImagePathField = value;
            }
        }

        private System.String Extension2Field;
        [DataMember(IsRequired = false, Name = "Extension2", Order = 12)]
        public System.String Extension2
        {
            get
            {
                return this.Extension2Field;
            }
            set
            {
                this.Extension2Field = value;
            }
        }

        private System.Int64 AttachmentDetailIDField;
        [DataMember(IsRequired=false,Name="AttachmentDetailID",Order=13)]
        public System.Int64 AttachmentDetailID
        {
            get
            {
                return AttachmentDetailIDField;
            }
            set
            {
                AttachmentDetailIDField=value;
            }
        }

        private System.String ImageNameField;
        [DataMember(IsRequired = false, Name = "ImageName", Order = 14)]
        public System.String ImageName
        {
            get
            {
                return ImageNameField;
            }
            set
            {
                ImageNameField = value;
            }
        }

        private System.Int16 ISLDSField;//islds is qrcode having scheduleuserid in omr
        [DataMember(IsRequired = false, Name = "ISLDS", Order = 15)]
        public System.Int16 ISLDS
        {
            get
            {
                return this.ISLDSField;
            }
            set
            {
                this.ISLDSField = value;
            }
        }

        private System.Boolean ISLogRequiredField;//log required for request object db
        [DataMember(IsRequired = false, Name = "ISLogRequired", Order = 16)]
        public System.Boolean ISLogRequired
        {
            get
            {
                return this.ISLogRequiredField;
            }
            set
            {
                this.ISLogRequiredField = value;
            }
        }

        private System.Int16 BatchNumberField;//log required for request object db
        [DataMember(IsRequired = false, Name = "BatchNumber", Order = 17)]
        public System.Int16 BatchNumber
        {
            get
            {
                return this.BatchNumberField;
            }
            set
            {
                this.BatchNumberField = value;
            }
        }

        private System.Int64 SequenceNumberField;//log required for request object db
        [DataMember(IsRequired = false, Name = "SequenceNumber", Order = 18)]
        public System.Int64 SequenceNumber
        {
            get
            {
                return this.SequenceNumberField;
            }
            set
            {
                this.SequenceNumberField = value;
            }
        }

        private System.DateTime ExamSeriesDateField;//log required for request object db
        [DataMember(IsRequired = false, Name = "ExamSeriesDate", Order = 19)]
        public System.DateTime ExamSeriesDate
        {
            get
            {
                return this.ExamSeriesDateField;
            }
            set
            {
                this.ExamSeriesDateField = value;
            }
        }

        private System.String SSNField;
        [DataMember(IsRequired =false,Name ="SSN",Order =20 )]
        public System.String SSN
        {

            get
            {
                return this.SSNField;
            }
            set
            {
                this.SSNField = value;
            }
        }

    }
}
