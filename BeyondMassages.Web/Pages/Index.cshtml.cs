using BeyondMassages.Web.Features.Contact.Exceptions;
using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

namespace BeyondMassagesApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public EmailModel EmailDetails { get; set; } = new EmailModel();
    public bool ShowAlert => TempData.ContainsKey("ShowAlert") && (bool)TempData["ShowAlert"]!;
    public bool IsSent => TempData.ContainsKey("EmailIsSent") && (bool)TempData["EmailIsSent"]!;
    private readonly IEmailService _emailService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IEmailService emailService, ILogger<IndexModel> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        TempData["ShowAlert"] = true;

        try
        {
            EmailDetails.Message = PrepareMessage(EmailDetails.Message);
            await _emailService.SendEmailAsync(EmailDetails);

            TempData["AlertMessage"] = "Email has been sent!";
            TempData["EmailIsSent"] = true;

            //_logger.LogInformation("Email has been sent successfully.");
        }

        catch (UserFriendlyException ex)
        {
            TempData["AlertMessage"] = ex.Message;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            TempData["AlertMessage"] = "An unexpected error occurred. Please try again later.";
        }

        return RedirectToPage();
    }

    private string PrepareMessage(string message)
    {
        string sanitizedMessage = WebUtility.HtmlEncode(message);

        return sanitizedMessage.Replace("\n", "<br>");
    }
}