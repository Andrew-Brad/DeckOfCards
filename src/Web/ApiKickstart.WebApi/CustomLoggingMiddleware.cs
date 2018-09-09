using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiKickstart.WebApi
{
    //TODO: move to separate Nuget package?
    public class CustomLoggingMiddleware
    {
        //The format section here is respected by the Postgres sink, and in some DB sinks may actually get its own subsection "renderings" in Properties
        //const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMilliseconds:0.0000} ms";
        const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMilliseconds} ms";

        private readonly ILogger<CustomLoggingMiddleware> _logger;
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;

        public CustomLoggingMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next, ILogger<CustomLoggingMiddleware> logger)
        {
            //if (next == null) throw new ArgumentNullException(nameof(next));
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            //_logger.LogTrace("ApiVersion {apiVersion} detected", httpContext.GetRequestedApiVersion().ToString());// is this the best place for this?

            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await _next(httpContext);
                sw.Stop();

                var statusCode = httpContext.Response?.StatusCode;
                _logger.LogInformation(MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, httpContext.Response.StatusCode, sw.Elapsed.TotalMilliseconds);
            }
            // Never caught, because `LogException()` returns false.
            catch (Exception) { }
        }
    }
}
