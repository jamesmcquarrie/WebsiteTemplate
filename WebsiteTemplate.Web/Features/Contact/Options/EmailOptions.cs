using System.ComponentModel.DataAnnotations;

namespace WebsiteTemplate.Web.Features.Contact.Options;

public class EmailOptions
{
    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string Host { get; set; } = string.Empty;
    [Required]
    public int Port { get; set; }
    [Required]
    public string SecureSocketOptions { get; set; } = string.Empty;
}
