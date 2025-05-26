using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace LicensingAndTransfer.API.Controllers
{
    public class eMarkingController : ApiController
    {

        [Route("api/eMarking/SynceMarkingUser")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "ValidateTestCenter is successful.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Supplied input is invalid")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Authorization has been denied for this request.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Entity details not found in the system. Refer response body for details.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error.")]
        public List<eMarkingSyncUserResponse> SynceMarkingUser([FromBody] SendMailRequestModel request)
        {
            List<eMarkingSyncUserResponse> eMarkingSyncUserResponses = (new eMarkingFactory()).SynceMarkingUser();


            bool IsReGenerateDefaultPwd = Convert.ToBoolean(ConfigurationManager.AppSettings["IsReGenerateDefaultPwd"]);
            if (IsReGenerateDefaultPwd)
            {
                new eMarkingFactory().ReGenerateDefaultPwd();
            }


            return eMarkingSyncUserResponses;
        }

        [Route("api/eMarking/eMarkingQRLpackStatics")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "ValidateTestCenter is successful.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Supplied input is invalid")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Authorization has been denied for this request.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Entity details not found in the system. Refer response body for details.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error.")]
        public async Task<List<eMarkingSyncUserResponse>> eMarkingQRLpackStatics([FromBody] SendMailRequestModel request)
        {
            List<eMarkingSyncUserResponse> eMarkingSyncUserResponses = new eMarkingQpackRpackStatics().eMarkingQRLpackStatics();

            return eMarkingSyncUserResponses;
        }

        [Route("api/eMarking/SendEmail")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "ValidateTestCenter is successful.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Supplied input is invalid")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Authorization has been denied for this request.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Entity details not found in the system. Refer response body for details.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error.")]
        public async Task<List<SendMailResponseModel>> SendEmail([FromBody] SendMailRequestModel request)
        {
            long? QueueID = null;
            if (request != null && request.QueueID > 0)
            {
                QueueID = request.QueueID;
            }

            List<SendMailResponseModel> sendMailResponseModels = new eMarkingSendEmail().EMarkingSendEmail(QueueID);

            return sendMailResponseModels;
        }


        [Route("api/eMarking/notify")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Description = "ValidateTestCenter is successful.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Supplied input is invalid")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Authorization has been denied for this request.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Entity details not found in the system. Refer response body for details.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error.")]
        public Task<List<MailDeactivateModel>> SendEmailtoDeactivate()
        {
         
            List<MailDeactivateModel> sendMailResponseModels = new SendEmailtoDeactivate().sendEmailtoDeactivate();

            return Task.FromResult(sendMailResponseModels);
        }
    }
}