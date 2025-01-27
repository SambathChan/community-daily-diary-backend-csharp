using community_daily_diary_api.Dtos;
using community_daily_diary_api.Entities;

namespace community_daily_diary_api.Extensions
{
    public static class DtoMappingExtensions
    {
        public static PostDto AsDto(this Post post)
        {
            return new PostDto(post.Id, post.Title, post.Body, post.Vote, post.CreatedAt);
        }
    }
}
