using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Inbound.Domain.Models
{
    [MessageContract]
    public class MailModel
    {
        public class EmailUsers
        {
            public long ID { get; set; }
            public string? ToLoginID { get; set; }
            public string? EmailID { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? TemplateBody { get; set; }
            public string? PassPhrase { get; set; }
            public int Year { get; set; }
        }

        public class SendMailResponseModel
        {
            public long ID { get; set; }
            public bool IsMailSent { get; set; }
        }
    }
}
