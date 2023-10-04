using BeyondMassages.IntegrationTests.Models;
using BeyondMassages.Web.Features.Contact.Common;
using BeyondMassages.Web.Features.Contact.Models;
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
    public async Task OnPostAsync_EmailShouldBeSentSuccessfully()
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

        var serializerOptions = SetToCamelCase();

        //Act
        var result = await _httpClient.PostAsync("/index", formContent);
        var deserializedResponse = await result.Content.ReadFromJsonAsync<EmailResult>(serializerOptions);

        //Assert
        Assert.NotNull(deserializedResponse);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.True(deserializedResponse.IsSent);
        Assert.Equal(StatusMessages.SuccessMessage, deserializedResponse.Message);
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

        var serializerOptions = SetToCamelCase();

        // Act
        var result = await _httpClient.PostAsync("/index", formContent);
        var deserializedResponse = await result.Content.ReadFromJsonAsync<ValidationResponseModel>(serializerOptions);

        // Assert
        Assert.NotNull(deserializedResponse);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.False(deserializedResponse.IsSent);
        Assert.Contains("Please provide a name", deserializedResponse.Message!);
        Assert.Contains("Please provide an email address", deserializedResponse.Message!);
        Assert.Contains("Please provide a subject", deserializedResponse.Message!);
        Assert.Contains("Please provide a message to send", deserializedResponse.Message!);
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

        var serializerOptions = SetToCamelCase();

        // Act
        var result = await _httpClient.PostAsync("/index", formContent);
        var deserializedResponse = await result.Content.ReadFromJsonAsync<ValidationResponseModel>(serializerOptions);

        // Assert
        Assert.NotNull(deserializedResponse);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.False(deserializedResponse.IsSent);
        Assert.Contains("Please provide a valid email address", deserializedResponse.Message!);
    }

    private JsonSerializerOptions SetToCamelCase()
    {
        return new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public void Dispose()
    {
        _factory.Dispose();
        _httpClient.Dispose();
    }
}