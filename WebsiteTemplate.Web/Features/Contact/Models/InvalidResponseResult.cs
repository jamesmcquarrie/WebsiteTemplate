namespace WebsiteTemplate.Web.Features.Contact.Models;

public class InvalidResponseResult
{
    public bool IsSent { get; set; }
    public IEnumerable<string> ErrorMessages { get; set; } = new List<string>();
}
