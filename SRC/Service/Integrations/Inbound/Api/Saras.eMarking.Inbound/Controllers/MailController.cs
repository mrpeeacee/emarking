using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Saras.eMarking.Inbound.Business.Services;
using Saras.eMarking.Inbound.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Inbound.Infrastructure.Mail;
using Saras.eMarking.Inbound.Interfaces.BusinessInterface;
using Saras.eMarking.Inbound.Services.Model;
using Saras.eMarking.Inbound.Services.Services;
using static Saras.eMarking.Inbound.Domain.Models.MailModel;

namespace Saras.eMarking.Inbound.Controllers
{
    [Route("api/mail")]
    [ApiController]
    public class MailController : BaseApiController<MailController>
    {
        private readonly IMailService _mailService;
        private readonly AppOptions appSettings;
        public MailController(IMailService mailService, ILogger<MailController> _logger, AppOptions settings) : base(_logger)
        {
            _mailService = mailService;
            appSettings = settings;
        }

        [Route("send/{queue-id}")]
        [HttpPost]
        public async Task<IActionResult> SendMail(long? QueueId)
        {
            try
            {
                return Ok(await _mailService.EMarkingSendEmail(QueueId));
            }
            catch (Exception ex)
            {
                logger.LogError("Error while EMarkingSendEmail:Method Name:EMarkingSendEmail()", ex.Message);
                throw;
            }
        }
    }
}
