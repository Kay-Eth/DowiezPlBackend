using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DowiezPlBackend.Controllers
{
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public ActionResult Error() => Problem();

        [Route("/error-dev")]
        public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development" && webHostEnvironment.EnvironmentName != "Server")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            // if (context.Error is DbUpdateException dbUpdateException)
            // {
            //     return Problem(
            //         title: dbUpdateException.Message,
            //         detail: dbUpdateException.InnerException.Message,
            //         statusCode: 400
            //     );
            // }

            return Problem(
                title: context.Error.Message,
                detail: context.Error.InnerException.Message,
                statusCode: 400);
        }
    }
}