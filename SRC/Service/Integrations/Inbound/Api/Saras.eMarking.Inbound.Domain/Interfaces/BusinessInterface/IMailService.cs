using static Saras.eMarking.Inbound.Domain.Models.MailModel;

namespace Saras.eMarking.Inbound.Domain.Interfaces.BusinessInterface
{
    public interface IMailService
    {
        Task<List<SendMailResponseModel>> EMarkingSendEmail(long? QueueId);

    }
}
