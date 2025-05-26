using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Saras.eMarking.Inbound.Interfaces.BusinessInterface;
using Saras.eMarking.Inbound.Services.Model;

namespace Saras.eMarking.Inbound.Controllers
{
    [Route("api/projects")]
    public class QRLPackController : BaseApiController<QRLPackController>
    {
        private readonly IQRLPackService _qrlsService;
        private readonly AppSettings appSettings;
        public QRLPackController(IQRLPackService qrlsService, ILogger<QRLPackController> _logger, IOptions<AppSettings> settings) : base(_logger)
        {
            _qrlsService = qrlsService;
            appSettings = settings.Value;
        }

        [Route("QRLpack/validate")]
        [HttpPost]
        public async Task<IActionResult> ValidateQRLpack()
        {
            try
            {
                return Ok(await _qrlsService.eMarkingQRLpackStatics());
            }
            catch (Exception ex)
            {
                logger.LogError("Error while eMarkingQRLpackStatics:Method Name:eMarkingQRLpackStatics()", ex.Message);
                throw;
            }
        }
    }
}
