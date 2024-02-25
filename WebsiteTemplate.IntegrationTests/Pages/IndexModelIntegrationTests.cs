using WebsiteTemplate.IntegrationTests;
using WebsiteTemplate.Web.Features.Contact.Models;
using WebsiteTemplate.Web.Features.Contact.Constants;
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
        var formData = new Dictionary<string, string>
        {
            { "EmailDetails.Name", "Test User" },
            { "EmailDetails.EmailAddress", "testuser@testuserdomain.com"},
            { "EmailDetails.Subject", "Test Subject" },
            { "EmailDetails.Message", "Dear Admin,\r\n\r\nThis is a test message.\r\n\r\nMany Thanks,\r\nTest User" }
        };

        var formContent = new FormUrlEncodedContent(formData);

        //Act
        var result = await _httpClient.PostAsync("/index", formContent);
        var deserializedResponse = await result.Content.ReadFromJsonAsync<EmailResult>(new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        //Assert
        deserializedResponse.IsSent.Should().BeTrue();
        deserializedResponse.Message.Should().Be(EmailStatusMessages.SuccessMessage);
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task OnPostAsync_EmptyEmailModel_ReturnsValidationErrors()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "EmailDetails.Name", "" },
            { "EmailDetails.EmailAddress", ""},
            { "EmailDetails.Subject", "" },
            { "EmailDetails.Message", "" }
        };

        var formContent = new FormUrlEncodedContent(formData);

        // Act
        var result = await _httpClient.PostAsync("/index", formContent);
        var deserializedResponse = await result.Content.ReadFromJsonAsync<InvalidResponseResult>(new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        deserializedResponse.IsSent.Should().BeFalse();
        deserializedResponse.ErrorMessages.Should().Contain("Please provide a name");
        deserializedResponse.ErrorMessages.Should().Contain("Please provide an email address");
        deserializedResponse.ErrorMessages.Should().Contain("Please provide a subject");
        deserializedResponse.ErrorMessages.Should().Contain("Please provide a message to send");
    }

    [Fact]
    public async Task OnPostAsync_InvalidEmailAddress_ReturnsValidationErrors()
    {
        // Arrange
        var formData = new Dictionary<string, string>
        {
            { "EmailDetails.Name", "Test User" },
            { "EmailDetails.EmailAddress", "invalid email" },
            { "EmailDetails.Subject", "Test Subject" },
            { "EmailDetails.Message", "Dear Admin,\r\n\r\nThis is a test message.\r\n\r\nMany Thanks,\r\nTest User" }
        };

        var formContent = new FormUrlEncodedContent(formData);

        // Act
        var result = await _httpClient.PostAsync("/index", formContent);
        var deserializedResponse = await result.Content.ReadFromJsonAsync<InvalidResponseResult>(new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        deserializedResponse.IsSent.Should().BeFalse();
        deserializedResponse.ErrorMessages.Should().Contain("Please provide a valid email address");
    }

    public void Dispose()
    {
        _factory.Dispose();
        _httpClient.Dispose();
    }
}