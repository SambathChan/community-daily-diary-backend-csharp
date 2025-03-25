using CommunityDailyDiary.Api.Entities;
using CommunityDailyDiary.Api.Extensions;
using MongoDB.Bson;
using Xunit;

namespace CommunityDailyDiary.Tests.Dtos;

public class DtoValidationTests
{   
    [Fact]
    public void PostDto_MappedFromEntity_HasCorrectValues()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var createdAt = DateTime.UtcNow;
        var post = new Post
        {
            Id = id,
            Title = "Test Title",
            Body = "Test Body",
            Vote = 1,
            CreatedAt = createdAt
        };

        // Act
        var dto = post.AsDto();

        // Assert
        Assert.Equal(id.ToString(), dto._id);
        Assert.Equal(post.Title, dto.Title);
        Assert.Equal(post.Body, dto.Body);
        Assert.Equal(post.Vote, dto.Vote);
        Assert.Equal(post.CreatedAt, dto.CreatedAt);
    }
}