using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace StudyPortal.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ErrorController : ControllerBase
{
    [HttpGet(Name = "GetError")]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (context is not null)
        {
            var stacktrace = context.Error.StackTrace;
            var errorMessage = context.Error.Message;
        }

        return Problem();
    }
}