using Saras.eMarking.Inbound.Domain.Models;
using static Saras.eMarking.Inbound.Domain.Models.MailModel;

namespace Saras.eMarking.Inbound.Domain.Interfaces.RepositoryInterfaces
{
    public interface IMailRepository
    {
        Task<List<SendMailResponseModel>> EMarkingSendEmail(List<EmailUsers> UsersList);

    }
}
