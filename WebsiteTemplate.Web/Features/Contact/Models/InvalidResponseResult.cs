namespace WebsiteTemplate.Web.Features.Contact.Models;

public class InvalidResponseResult
{
    public bool IsSent { get; set; }
    public List<string>? Message { get; set; }
}
