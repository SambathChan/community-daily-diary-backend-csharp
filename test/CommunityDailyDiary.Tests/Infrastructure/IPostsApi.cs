using CommunityDailyDiary.Api.Dtos;
using Refit;

namespace CommunityDailyDiary.Tests.Infrastructure;

public interface IPostsApi
{
    [Get("/api/posts")]
    Task<IEnumerable<PostDto>> GetPosts(string date, int offset, int count);

    [Get("/api/posts/{id}")]
    Task<PostDto> GetPost(string id);

    [Post("/api/posts")]
    Task<PostDto> CreatePost([Body] CreatePostDto post);

    [Patch("/api/posts/{id}")]
    Task<int> UpdatePostVote(string id, [Body] UpdatePostVoteDto voteUpdate);
}