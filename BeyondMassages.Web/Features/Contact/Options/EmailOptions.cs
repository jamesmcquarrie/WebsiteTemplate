namespace BeyondMassages.Web.Features.Contact.Configuration
{
    public class EmailOptions
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}
