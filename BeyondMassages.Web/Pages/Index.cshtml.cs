using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BeyondMassagesApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public EmailModel EmailDetails { get; set; } = new EmailModel();
    public bool ShowAlert = false;
    private readonly IEmailService _emailService;

    public IndexModel(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public void OnGet()
    {
        if (TempData["ShowAlert"] != null)
        {
            ShowAlert = (bool)TempData["ShowAlert"]!;
        }

        if (TempData["EmailIsSent"] != null)
        {
            EmailDetails.IsSent = (bool)TempData["EmailIsSent"]!;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            return Page();
        }

        TempData["ShowAlert"] = true;

        try
        {
            EmailDetails.Message = EmailDetails.Message.Replace("\n", "<br>");
            await _emailService.SendEmailAsync(EmailDetails);
            TempData["AlertMessage"] = "Email has been sent!";
        }
        catch (Exception)
        {
            TempData["AlertMessage"] = "Email failed to send";
        }

        TempData["EmailIsSent"] = EmailDetails.IsSent;

        return RedirectToPage();

    }
}