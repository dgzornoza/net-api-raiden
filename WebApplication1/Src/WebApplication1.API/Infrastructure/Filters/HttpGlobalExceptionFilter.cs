using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WebApplication1.API.Infrastructure.Filters
{
    /// <summary>
    /// Filter for manage all http global exception
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="env">Web host environment</param>
        /// <param name="logger">Logger</param>
        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            environment = env;
            this.logger = logger;
        }

        /// <summary>
        /// Invoked function for manage Excepcion
        /// </summary>
        /// <param name="context">Exception context</param>
        public void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, $"Error Message --> {context.Exception.Message}");

            // create message and code for specific exception
            int statusCode;
            IDictionary<string, string[]> errors;
            switch (context.Exception)
            {

                case InvalidOperationException _:
                    errors = CreateCommonControlledExceptionError(context.Exception);
                    statusCode = StatusCodes.Status400BadRequest;
                    break;

                case KeyNotFoundException _:
                    errors = CreateCommonControlledExceptionError(context.Exception);
                    statusCode = StatusCodes.Status404NotFound;
                    break;

                default:
                    errors = new Dictionary<string, string[]>()
                    {
                        { "UnknownError", new[] { context.Exception.Message } }
                    };

                    statusCode = StatusCodes.Status500InternalServerError;

                    break;
            }


            // Create problem details
            ValidationProblemDetails problemDetails = new ValidationProblemDetails(errors)
            {
                Title = Properties.Resources.GLOBAL_ERROR_TITLE,
                Status = statusCode
            };

            // Add debug info
            if (environment.IsDevelopment())
            {
                problemDetails.Instance = context.HttpContext.Request.Path;
                problemDetails.Type = $"https://httpstatuses.com/{statusCode}";
                problemDetails.Detail = context.Exception.StackTrace;
                problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
            }

            // Attach to response
            context.Result = new ObjectResult(problemDetails) { StatusCode = statusCode };
            context.HttpContext.Response.StatusCode = statusCode;

            context.ExceptionHandled = true;
        }


        private static Dictionary<string, string[]> CreateCommonControlledExceptionError(Exception exception)
        {
            return new Dictionary<string, string[]>()
            {
                { "ControlledError", new[] { exception.Message } }
            };
        }
    }
}
