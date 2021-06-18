using Employee.API.Infrastructure.DataAccess.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;

namespace Upscript.Services.Employee.API.Middlewares
{
    public static class ExceptionMiddleware
    {
        private static readonly Logging logging = new();

        /// <summary>
        /// This method is to handle the exceptions globally.
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(apperror =>
            apperror.Run(async context =>
            {
                ILogger _logger = (ILogger<Logging>)context.RequestServices.GetService(typeof(ILogger<Logging>));
                string exceptionGUID =Convert.ToString(Guid.NewGuid());

                logging.LogException(_logger, exceptionGUID);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(JsonSerializer.Serialize(new ResponseErrorDetailsDO()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = JsonSerializer.Serialize(context.Features.Get<IExceptionHandlerFeature>().Error.Message),
                    ReferenceID = exceptionGUID
                }));
            }));
        }
    }

    /// <summary>
    /// This class is used for logging.
    /// </summary>
    public class Logging
    {
        public void LogException(ILogger logger, string exceptionGUID)
        {
            string referenceID = $"Reference ID : {exceptionGUID})";
            logger.LogError(referenceID);
        }
    }
}
