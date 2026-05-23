using CommunityDailyDiary.Api.Dtos;
using CommunityDailyDiary.Api.Entities;
using CommunityDailyDiary.Api.Extensions;
using CommunityDailyDiary.Api.Settings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;

namespace CommunityDailyDiary.Api.Modules;


public class PostModule : IModule
{
    private readonly FilterDefinitionBuilder<Post> filterBuilder = Builders<Post>.Filter;
    private readonly SortDefinitionBuilder<Post> sortBuilder = Builders<Post>.Sort;
    private const string GetPostEndpointName = "GetPost";

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var rateLimitOptions = app.ServiceProvider.GetRequiredService<IOptions<RateLimitSettings>>().Value;

        var group = app.MapGroup("posts")
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                operation.Description = "Group of operations for managing posts.";
                return Task.CompletedTask;
            })
            .WithTags("Posts")
            .RequireRateLimiting(rateLimitOptions.PolicyName);

        group.MapPost("", CreatePostAsync);

        group.MapGet("", GetPostsAsync);

        group.MapGet("/{id}", GetPostByIdAsync)
            .WithName(GetPostEndpointName);

        group.MapPatch("/{id}", UpdatePostVoteAsync);
    }

    private async Task<Results<Ok<int>, NotFound>> UpdatePostVoteAsync(
        [FromServices] DB db,
        [FromServices] HybridCache cache,
        [FromRoute] ObjectId id,
        [FromBody] UpdatePostVoteDto updatePostVote,
        CancellationToken ct)
    {
        var existingPost = await cache.GetOrCreateAsync($"post-{id}", async token =>
        {
            var postDb = await db.Find<Post>().OneAsync(id, token);
            return postDb;
        }, cancellationToken: ct);

        if (existingPost is null)
        {
            return TypedResults.NotFound();
        }

        var value = updatePostVote.VoteUp ? 1 : -1;

        await db.Update<Post>()
            .MatchID(id)
            .Modify(p=>p.Vote, value)
            .ExecuteAsync(ct);

        await cache.RemoveAsync($"post-{id}", ct);

        return TypedResults.Ok(existingPost.Vote + value);
    }

    private async Task<Results<Ok<PostDto>, NotFound>> GetPostByIdAsync(
        [FromServices] DB db,
        [FromServices] HybridCache cache,
        [FromRoute] ObjectId id,
        CancellationToken ct)
    {
        var post = await cache.GetOrCreateAsync($"post-{id}", async token =>
        {
            var postDb = await db.Find<Post>().OneAsync(id, token);
            return postDb;
        }, cancellationToken: ct);

        if(post is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(post.AsDto());
    }

    private async Task<Ok<IEnumerable<PostDto>>> GetPostsAsync([FromServices] DB db,
        [FromQuery] DateTime date, 
        [FromQuery] int offset = 0, 
        [FromQuery] int count = 10)
    {
        var startOfDay = date;
        var endOfDay = startOfDay.AddDays(1);

        var posts = await db.Find<Post>()
            .Match(p => p.CreatedAt >= startOfDay && p.CreatedAt < endOfDay)
            .Sort(p=>p.Vote, Order.Descending)
            .Skip(offset)
            .Limit(count)
            .ExecuteAsync();

        return TypedResults.Ok(posts.Select(p => p.AsDto()));
    }

    private async Task<IResult> CreatePostAsync(
        [FromServices] DB db, 
        [FromBody] CreatePostDto createPostDto)
    {
        var post = new Post
        {
            Title = createPostDto.Title,
            Body = createPostDto.Body,
            CreatedAt = DateTime.UtcNow
        };

        await db.SaveAsync(post);

        return TypedResults.CreatedAtRoute(post.AsDto(), GetPostEndpointName, new { id = post.ID });
    }
}
