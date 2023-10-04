using System.ComponentModel.DataAnnotations;

namespace BeyondMassages.Web.Features.Contact.Options;

public class EmailOptions
{
    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string IntermediaryEmailAddress { get; set; } = string.Empty;
    [Required]
    public string Host { get; set; } = string.Empty;
    [Required]
    public int Port { get; set; }
    [Required]
    public string SecureSocketOptions { get; set; } = string.Empty;
}
