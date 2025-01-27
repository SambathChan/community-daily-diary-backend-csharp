

using community_daily_diary_api.Dtos;
using community_daily_diary_api.Entities;
using community_daily_diary_api.Extensions;
using community_daily_diary_api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace community_daily_diary_api.Modules
{  

    public class PostModule : IModule
    {
        private readonly FilterDefinitionBuilder<Post> filterBuilder = Builders<Post>.Filter;
        private const string GetPostEndpointName = "GetPost";

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("posts")
                .WithOpenApi()
                .WithTags("Posts");

            group.MapPost("", CreatePostAsync);

            group.MapGet("", GetPostsAsync);

            group.MapGet("/{id}", GetPostByIdAsync)
                .WithName(GetPostEndpointName);

            group.MapPatch("/{id}", UpdatePostVoteAsync);
        }

        private async Task<Results<Ok<int>, NotFound>> UpdatePostVoteAsync(
            [FromServices] IRepository<Post> postsRepository,
            [FromRoute] ObjectId id,
            [FromBody] UpdatePostVoteDto updatePostVote)
        {
            var existingPost = await postsRepository.GetAsync(id);

            if(existingPost is null)
            {
                return TypedResults.NotFound();
            }

            var value = updatePostVote.VoteUp ? 1 : -1;

            UpdateDefinition<Post> update = Builders<Post>.Update.Inc(post => post.Vote, value);

            await postsRepository.UpdateAsync(id, update);

            return TypedResults.Ok(existingPost.Vote + value);
        }

        private async Task<Results<Ok<PostDto>, NotFound>> GetPostByIdAsync(
            [FromServices] IRepository<Post> postsRepository,
            [FromRoute] ObjectId id)
        {
            var post = await postsRepository.GetAsync(id);

            if(post is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(post.AsDto());
        }

        private async Task<Ok<IEnumerable<PostDto>>> GetPostsAsync([FromServices] IRepository<Post> postsRepository,
            [FromQuery] DateTime date, 
            [FromQuery] int offset = 0, 
            [FromQuery] int count = 10)
        {
            FilterDefinition<Post> filter = QueryWithinSingleDay(date);

            var posts = await postsRepository.GetManyAsync(filter, offset, count);

            return TypedResults.Ok(posts.Select(p => p.AsDto()));
        }

        private async Task<IResult> CreatePostAsync(
            [FromServices] IRepository<Post> postsRepository, 
            [FromBody] CreatePostDto createPostDto)
        {
            var post = new Post
            {
                Title = createPostDto.Title,
                Body = createPostDto.Body,
                CreatedAt = DateTime.UtcNow
            };

            await postsRepository.CreateAsync(post);

            return TypedResults.CreatedAtRoute(post.AsDto(), GetPostEndpointName, new { id = post.Id });
        }

        private FilterDefinition<Post> QueryWithinSingleDay(DateTime date)
        {
            return filterBuilder.Gte(entity => entity.CreatedAt, date) &
                   filterBuilder.Lt(entity => entity.CreatedAt, date.AddDays(1));
        }
    }
}
