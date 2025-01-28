using CommunityDailyDiary.Api.Dtos;
using CommunityDailyDiary.Api.Entities;

namespace CommunityDailyDiary.Api.Extensions;

public static class DtoMappingExtensions
{
    public static PostDto AsDto(this Post post)
    {
        return new PostDto(post.Id.ToString(), post.Title, post.Body, post.Vote, post.CreatedAt);
    }
}
