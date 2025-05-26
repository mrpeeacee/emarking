namespace Saras.eMarking.Domain.ViewModels.Auth
{
    public class SendMailRequestModel
    {
        public SendMailRequestModel()
        {

        }
        public long QueueID { get; set; }
        public string Status { get; set; }
        public long ID { get; set; }
        public bool IsMailSent { get; set; }


    }

    public class SendMailResponseModel
    {
        public long ID { get; set; }
        public bool IsMailSent { get; set; }


    }

}
