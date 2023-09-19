using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace BeyondMassages.Web.Pages;

public class ErrorModel : PageModel
{
    public int ResponseStatusCode { get; set; }
    public string? StatusMessage { get; set; }

    public void OnGet(int? statusCode = null)
    {
        ResponseStatusCode = statusCode ?? 0;
        StatusMessage = ResponseStatusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "The page you are looking for could not be found.",
            500 => "Internal Server Error",
            _ => "An unexpected error occurred"
        };
    }
}
