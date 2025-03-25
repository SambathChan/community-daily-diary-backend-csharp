using CommunityDailyDiary.Api.Dtos;
using CommunityDailyDiary.Tests.Infrastructure;
using Refit;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CommunityDailyDiary.Tests.Controllers;

public class PostsControllerTests : IntegrationTestBase
{
    public PostsControllerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreatePost_WithValidData_ReturnsCreated()
    {
        var postsApi = CreateApiClient();

        // Arrange
        var createDto = new CreatePostDto(
            Title: "Test Post",
            Body: "Test Content",
            Vote: 0
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/posts", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetPosts_WithPagination_ReturnsCorrectCount()
    {
        var postsApi = CreateApiClient();

        // Arrange
        for (int i = 1; i <= 15; i++)
        {
            var createDto = new CreatePostDto(
                Title: $"Test Post {i}",
                Body: $"Test Content {i}",
                Vote: 0
            );
            await postsApi.CreatePost(createDto);
        }

        // Act
        var returnedPosts = await postsApi.GetPosts($"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}", 5, 5);            

        // Assert
        Assert.NotNull(returnedPosts);
        Assert.Equal(5, returnedPosts.Count());
    }

    [Fact]
    public async Task UpdatePostVote_IncreasesVoteCount()
    {
        var postsApi = CreateApiClient();

        var updateDto = new UpdatePostVoteDto(VoteUp: true);

        // Act
        var post = await postsApi.CreatePost(new CreatePostDto("Test Post", "Test Content", 0));
        var newVote = await postsApi.UpdatePostVote(post._id, updateDto);

        // Assert
        Assert.Equal(1, newVote);

        // Verify through GET as well
        var updatedPost = await postsApi.GetPost(post._id);
        Assert.NotNull(updatedPost);
        Assert.Equal(1, updatedPost.Vote);
    }

    private IPostsApi CreateApiClient()
    {        
        return RestService.For<IPostsApi>(_client);
    }
}