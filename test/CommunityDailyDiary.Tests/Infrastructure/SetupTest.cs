using CommunityDailyDiary.Api.Dtos;
using Refit;
using System.Net.Http.Json;

namespace CommunityDailyDiary.Tests.Infrastructure;

public class SetupTest
{
    private readonly IntegrationTestFactory _factory;

    private SetupTest(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    public static SetupTest Using(IntegrationTestFactory factory)
    {
        return new SetupTest(factory);
    }

    public async Task<PostDto> CreatePost(string title = "Test Post", string body = "Test Content", int vote = 0)
    {
        var client = _factory.CreateClient();
        var postsApi = RestService.For<IPostsApi>(client);
        var createDto = new CreatePostDto(
            Title: title,
            Body: body,
            Vote: vote
        );

        var response = await postsApi.CreatePost(createDto);
        return new PostDto(response._id,response.Title, response.Body, response.Vote, response.CreatedAt);
    }
}