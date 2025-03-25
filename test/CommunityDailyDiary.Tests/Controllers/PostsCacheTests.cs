using CommunityDailyDiary.Api.Dtos;
using CommunityDailyDiary.Api.Entities;
using CommunityDailyDiary.Tests.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;
using Refit;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CommunityDailyDiary.Tests.Controllers;

public class PostsCacheTests : IntegrationTestBase
{
    public PostsCacheTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetPost_WhenCached_ReturnsFromCache()
    {
        // Arrange
        var postsApi = CreateApiClient();
        var createDto = new CreatePostDto(
            Title: "Cache Test Post",
            Body: "Test Content",
            Vote: 0
        );

        // Act - First call should cache
        var post = await postsApi.CreatePost(createDto);
        var response1 = await _client.GetAsync($"/api/posts/{post._id}");
        
        // Delete from DB to verify cache hit
        await _database.GetCollection<Post>(nameof(Post)).DeleteOneAsync(p => p.Id == new ObjectId(post._id));
        
        // Act - Second call should come from cache
        var response2 = await _client.GetAsync($"/api/posts/{post._id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
    }

    [Fact]
    public async Task UpdatePost_InvalidatesCacheAndReturnsUpdatedValue()
    {
        // Arrange
        var postsApi = CreateApiClient();
        var createDto = new CreatePostDto(
            Title: "Cache Test Post",
            Body: "Test Content",
            Vote: 0
        );

        var post = await postsApi.CreatePost(createDto);

        // Cache the initial value
        await postsApi.GetPost(post._id);

        // Act - Update should invalidate cache
        var response = await _client.PatchAsJsonAsync($"/api/posts/{post._id}", new { VoteUp = true });
        var newVote = await response.Content.ReadFromJsonAsync<int>();

        // Get updated value
        var updatedPost = await postsApi.GetPost(post._id);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, newVote);
        Assert.NotNull(updatedPost);
        Assert.Equal(1, updatedPost.Vote);
    }

    private IPostsApi CreateApiClient()
    {        
        return RestService.For<IPostsApi>(_client);
    }
}