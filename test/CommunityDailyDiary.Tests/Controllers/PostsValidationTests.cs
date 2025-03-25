using CommunityDailyDiary.Api.Dtos;
using CommunityDailyDiary.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CommunityDailyDiary.Tests.Controllers;

public class PostsValidationTests : IntegrationTestBase
{
    public PostsValidationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreatePost_WithInvalidTitle_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreatePostDto(
            Title: "", // Empty title
            Body: "Test Content",
            Vote: 0
        );

        // Act
        var response = await SendRequestAsync("/api/posts", HttpMethod.Post, createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePost_WithTooLongTitle_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreatePostDto(
            Title: new string('x', 251), // Exceeds 250 chars
            Body: "Test Content",
            Vote: 0
        );

        // Act
        var response = await SendRequestAsync("/api/posts", HttpMethod.Post, createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPost_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var invalidId = "invalid-id"; // Invalid ObjectId format

        // Act
        var response = await _client.GetAsync($"/api/posts/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<HttpResponseMessage> SendRequestAsync(string endpoint, HttpMethod method, object? content = null)
    {
        var request = new HttpRequestMessage(method, endpoint);
        if (content != null)
        {
            request.Content = JsonContent.Create(content);
        }
        return await _client.SendAsync(request);
    }
}