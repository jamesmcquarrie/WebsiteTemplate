using WebsiteTemplate.IntegrationTests;
using WebsiteTemplate.Web.Features.Contact.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace BeyondMassages.IntegrationTests.Pages;

public class IndexModelIntegrationTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;

    public IndexModelIntegrationTests()
    {
        _factory = new CustomWebApplicationFactory();
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task OnPostAsync_AttemptEmailSending()
    {
        //Arrange
        var postData = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("EmailDetails.Name", "Test User"),
            new KeyValuePair<string, string>("EmailDetails.EmailAddress", "testuser@testuserdomain.com"),
            new KeyValuePair<string, string>("EmailDetails.Subject", "Test Subject"),
            new KeyValuePair<string, string>("EmailDetails.Message", "Dear Admin,\r\n\r\nThis is a test message.\r\n\r\nMany Thanks,\r\nTest User")
        };

        var formContent = new FormUrlEncodedContent(postData);

        //Act
        var result = await _httpClient.PostAsync("/index", formContent);
        var deserializedResponse = await result.Content.ReadFromJsonAsync<EmailResult>(new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        //Assert
        deserializedResponse.Should().NotBeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task OnPostAsync_EmptyEmailModel_ReturnsValidationErrors()
    {
        // Arrange
        var invalidPostData = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("EmailDetails.Name", ""),
            new KeyValuePair<string, string>("EmailDetails.EmailAddress", ""),
            new KeyValuePair<string, string>("EmailDetails.Subject", ""),
            new KeyValuePair<string, string>("EmailDetails.Message", "")
        };

        var formContent = new FormUrlEncodedContent(invalidPostData);

        // Act
        var result = await _httpClient.PostAsync("/index", formContent);
        var deserializedResponse = await result.Content.ReadFromJsonAsync<InvalidResponseResult>(new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        // Assert
        deserializedResponse.Should().NotBeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        deserializedResponse.IsSent.Should().BeFalse();
        deserializedResponse.Message.Should().Contain("Please provide a name");
        deserializedResponse.Message.Should().Contain("Please provide an email address");
        deserializedResponse.Message.Should().Contain("Please provide a subject");
        deserializedResponse.Message.Should().Contain("Please provide a message to send");
    }

    [Fact]
    public async Task OnPostAsync_InvalidEmailAddress_ReturnsValidationErrors()
    {
        // Arrange
        var invalidPostData = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("EmailDetails.Name", "Test User"),
            new KeyValuePair<string, string>("EmailDetails.EmailAddress", "invalidEmail"),
            new KeyValuePair<string, string>("EmailDetails.Subject", "Test Subject"),
            new KeyValuePair<string, string>("EmailDetails.Message", "Dear Admin,\r\n\r\nThis is a test message.\r\n\r\nMany Thanks,\r\nTest User")
        };

        var formContent = new FormUrlEncodedContent(invalidPostData);

        // Act
        var result = await _httpClient.PostAsync("/index", formContent);
        var deserializedResponse = await result.Content.ReadFromJsonAsync<InvalidResponseResult>(new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        // Assert
        deserializedResponse.Should().NotBeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        deserializedResponse.IsSent.Should().BeFalse();
        deserializedResponse.Message.Should().Contain("Please provide a valid email address");
    }

    public void Dispose()
    {
        _factory.Dispose();
        _httpClient.Dispose();
    }
}