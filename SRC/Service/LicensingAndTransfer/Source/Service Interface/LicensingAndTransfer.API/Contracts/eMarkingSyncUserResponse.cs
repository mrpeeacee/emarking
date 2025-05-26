using System.ServiceModel;
using System.Collections.Generic;
using System.Text;

namespace LicensingAndTransfer.API
{
    [MessageContract]
    public class eMarkingSyncUserResponse
    {
        [MessageBodyMember(Order = 0)]
        private System.String StatusCodeField;
        public System.String StatusCode
        {
            get
            {
                return this.StatusCodeField;
            }
            set
            {
                this.StatusCodeField = value;
            }
        }

        [MessageBodyMember(Order = 0)]
        private System.String StatusField;
        public System.String Status
        {
            get
            {
                return this.StatusField;
            }
            set
            {
                this.StatusField = value;
            }
        }

        [MessageBodyMember(Order = 1)]
        private StringBuilder MessageField;
        public StringBuilder Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }

        [MessageBodyMember(Order = 2)]
        private System.String ProjectDataField;
        public System.String ProjectData
        {
            get
            {
                return this.ProjectDataField;
            }
            set
            {
                this.ProjectDataField = value;
            }
        }
        [MessageBodyMember(Order = 3)]
        private System.String ResponseField;
        public System.String ResponseData
        {
            get
            {
                return this.ResponseField;
            }
            set
            {
                this.ResponseField = value;
            }
        }
    }

    public class ProjectInfo
    {
        public string ExamLevel { get; set; }
        public string ExamSeries { get; set; }
        public string ExamYear { get; set; }
        public string ModeofAssessment { get; set; }
        public string PaperNumber { get; set; }
        public string SubjectCode { get; set; }

    }

    public class EmailUsers
    {
        public long ID { get; set; }
        public string ToLoginID { get; set; }
        public string EmailID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TemplateBody { get; set; }

        public string TemplateName { get; set; }
        public string PassPhrase { get; set; }            
        public int Year { get; set; }            
        public string Nric { get; set; }            
    }

    public class SendMailResponseModel
    {
        public long ID { get; set; }
        public bool IsMailSent { get; set; }
    }

    public class UserPassPhraseDetails
    {
        public string PassPhraseText { get; set; }
        public List<EmailUsers> UserDetails { get; set; }
    }
    public  class UserDeactivateDetails
    {
        public string UserID { get; set; }
    }

}
