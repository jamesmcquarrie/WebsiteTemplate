using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BeyondMassages.Web.Pages;

public class ErrorModel : PageModel
{
    private readonly ILogger<ErrorModel> _logger;
    public string? ExceptionMessage { get; set; }
    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature != null)
        {
            ExceptionMessage = "We could not complete your request.";
            _logger.LogError(exceptionFeature.Error, "An unhandled exception has occurred");
        }

    }
}
