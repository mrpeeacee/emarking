using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

using Saras.eMarking.Domain.Entities.Exceptions;

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Saras.eMarking.API.CustomFilters
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_logger"></param>
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> _logger)
        {
            logger = _logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {

            string referenceId = context.HttpContext.TraceIdentifier;
#if DEBUG
            logger.LogError($"Unhandled error: {context.Exception.GetBaseException().Message} \n Inner Exception: {context.Exception.StackTrace} \n ReferenceId: {referenceId}");
#else
logger.LogError($"Unhandled error: {context.Exception.Message} \n Inner Exception: {context.Exception.StackTrace} \n ReferenceId: {referenceId}");
#endif
            ExceptionMessage ErrObj = new()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                ReferenceId = referenceId
            };
            Exception ExceptionDetails = context.Exception;
            switch (ExceptionDetails)
            {
                case UnauthorizedAccessException:
                    ErrObj.Message = "Unauthorized Access";
                    ErrObj.StatusCode = StatusCodes.Status401Unauthorized;
                    break;
                case TaskCanceledException:
                    ErrObj.Message = "Task Cancellation Exception";
                    ErrObj.StatusCode = StatusCodes.Status307TemporaryRedirect;
                    break;
                case ValidationException:
                    ErrObj.StatusCode = StatusCodes.Status400BadRequest;
                    ErrObj.Message = "Validation Exception";
                    break;
                case NotImplementedException:
                    ErrObj.Message = "Not Implemented Exception.";
                    ErrObj.StatusCode = StatusCodes.Status501NotImplemented;
                    break;
                case ObjectDisposedException:
                    ErrObj.Message = "Object Disposed Exception";
                    ErrObj.StatusCode = StatusCodes.Status302Found;
                    break;
                case AggregateException:
                    ErrObj.Message = "An Aggregate Exception Occured";
                    ErrObj.StatusCode = StatusCodes.Status501NotImplemented;
                    break;
                case OperationCanceledException:
                    ErrObj.Message = "Operation Cancellation Exception";
                    ErrObj.StatusCode = StatusCodes.Status308PermanentRedirect;
                    break;
                case TimeoutException:
                    ErrObj.Message = "Time Out Exception";
                    ErrObj.StatusCode = StatusCodes.Status408RequestTimeout;
                    break;
                default:
                    ErrObj.Message = "Internal Server Error";
                    break;
            }
            ErrObj.Detail = context.Exception.Message;
            context.ExceptionHandled = true;
            context.Result = new ObjectResult(ErrObj)
            {
                StatusCode = ErrObj.StatusCode
            };
        }
    }
}
