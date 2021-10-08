using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DowiezPlBackend.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public ActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            // if (context.Error is DowiezPlException dpe)
            // {
            //     if (dpe.Detail == null)
            //         return Problem(
            //             title: $"{dpe.GetType().Name}: {dpe.Message}",
            //             statusCode: dpe.StatusCode
            //         );
            //     else
            //         return Problem(
            //             title: $"{dpe.GetType().Name}: {dpe.Message}",
            //             detail: $"{dpe.Detail}",
            //             statusCode: dpe.StatusCode
            //         );
            // }

            return Problem();
        }

        [Route("/error-dev")]
        public ActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development" && webHostEnvironment.EnvironmentName != "Server" && webHostEnvironment.EnvironmentName != "VPS")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (context.Error is DbUpdateException dbUpdateException)
            {
                return Problem(
                    title: $"{context.Error.GetType()}: {dbUpdateException.Message}",
                    detail: $"{dbUpdateException.InnerException.GetType()}: {dbUpdateException.InnerException.Message}",
                    statusCode: 400
                );
            }

            if (context.Error.InnerException != null)
            {
                return Problem(
                    title: $"{context.Error.GetType()}: {context.Error.Message}",
                    detail: $"{context.Error.InnerException.GetType()}: {context.Error.InnerException.Message}",
                    statusCode: 500);
            }
            else
            {
                return Problem(
                    title: $"{context.Error.GetType()}: {context.Error.Message}",
                    detail: context.Error.StackTrace,
                    statusCode: 500);
            }
        }
    }
}