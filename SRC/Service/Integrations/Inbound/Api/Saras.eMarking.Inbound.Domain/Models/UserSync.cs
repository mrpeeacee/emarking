using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Inbound.Domain.Models
{
    [MessageContract]
    public class eMarkingSyncUserResponse
    {
        [MessageBodyMember(Order = 0)]
        private System.String? StatusCodeField;
        public System.String? StatusCode
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
        private System.String? StatusField;
        public System.String? Status
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
        private StringBuilder? MessageField;
        public StringBuilder? Message
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
        private System.String? ProjectDataField;
        public System.String? ProjectData
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
        private System.String? ResponseField;
        public System.String? ResponseData
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

        public class ProjectInfo
        {
            public string? ExamLevel { get; set; }
            public string? ExamSeries { get; set; }
            public string? ExamYear { get; set; }
            public string? ModeofAssessment { get; set; }
            public string? PaperNumber { get; set; }
            public string? SubjectCode { get; set; }

        }

    }
}
