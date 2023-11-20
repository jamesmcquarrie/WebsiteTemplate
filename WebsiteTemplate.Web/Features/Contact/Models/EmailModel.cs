using System.ComponentModel.DataAnnotations;

namespace WebsiteTemplate.Web.Features.Contact.Models;

public class EmailModel
{
    [Required(ErrorMessage = "Please provide a name")]
    public string Name { get; set; } = string.Empty;
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Please provide an email address")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address")]
    public string EmailAddress { get; set; } = string.Empty;
    [Required(ErrorMessage = "Please provide a subject")]
    public string Subject { get; set; } = string.Empty;
    [Required(ErrorMessage = "Please provide a message to send")]
    public string Message { get; set; } = string.Empty;
}
