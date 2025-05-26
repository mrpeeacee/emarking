using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Saras.eMarking.Inbound.Interfaces.BusinessInterface;
using Saras.eMarking.Inbound.Services.Model;
using System.Runtime;

namespace Saras.eMarking.Inbound.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : BaseApiController<UsersController>
    {
        private readonly IUsersService _usersService; 
        private readonly AppOptions appSettings;
        public UsersController(IUsersService usersService, ILogger<UsersController> _logger, AppOptions settings) : base(_logger)
        {
            _usersService = usersService;
            appSettings = settings; 
        }

        [Route("api/eMarking/SynceMarkingUser")]
        [HttpPost]
        public async Task<IActionResult> SynceMarkingUser()
        {
            try
            {
                return Ok(await _usersService.SynceMarkingUser());
            }
            catch (Exception ex)
            {
                logger.LogError("Error while SynceMarkingUser:Method Name:SynceMarkingUser()", ex.Message);
                throw;
            }
        } 
    }
}
