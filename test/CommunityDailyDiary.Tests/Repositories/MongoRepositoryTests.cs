using CommunityDailyDiary.Api.Entities;
using CommunityDailyDiary.Api.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;
using Moq;

namespace CommunityDailyDiary.Tests.Repositories;

public class MongoRepositoryTests
{
    private readonly Mock<IMongoCollection<Post>> _mockCollection;
    private readonly Mock<IMongoDatabase> _mockDb;
    private readonly MongoRepository<Post> _repository;

    public MongoRepositoryTests()
    {
        _mockCollection = new Mock<IMongoCollection<Post>>();
        _mockDb = new Mock<IMongoDatabase>();
        _mockDb.Setup(db => db.GetCollection<Post>(It.IsAny<string>(), null))
               .Returns(_mockCollection.Object);
        
        _repository = new MongoRepository<Post>(_mockDb.Object, "posts");
    }

    [Fact]
    public async Task CreateAsync_WithValidEntity_CallsInsertOne()
    {
        // Arrange
        var post = new Post 
        { 
            Title = "Test Post", 
            Body = "Test Content",
            CreatedAt = DateTime.UtcNow 
        };

        _mockCollection.Setup(c => c.InsertOneAsync(
            It.IsAny<Post>(),
            It.IsAny<InsertOneOptions>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _repository.CreateAsync(post);

        // Assert
        _mockCollection.Verify(c => c.InsertOneAsync(
            It.Is<Post>(p => p == post),
            It.IsAny<InsertOneOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithValidId_ReturnsEntity()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var post = new Post 
        { 
            Id = id,
            Title = "Test Post", 
            Body = "Test Content",
            CreatedAt = DateTime.UtcNow 
        };

        var mockCursor = new Mock<IAsyncCursor<Post>>();
        mockCursor.Setup(c => c.Current).Returns([post]);
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true)
                 .ReturnsAsync(false);

        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.IsAny<FindOptions<Post>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _repository.GetAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WithValidUpdate_CallsUpdateOne()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var update = Builders<Post>.Update.Set(p => p.Vote, 1);
        UpdateResult result = new UpdateResult.Acknowledged(1, 1, null);


        _mockCollection.Setup(c => c.UpdateOneAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.IsAny<UpdateDefinition<Post>>(),
            It.IsAny<UpdateOptions>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(result));

        // Act
        await _repository.UpdateAsync(id, update);

        // Assert
        _mockCollection.Verify(c => c.UpdateOneAsync(
            It.IsAny<FilterDefinition<Post>>(),
            It.Is<UpdateDefinition<Post>>(u => u == update),
            It.IsAny<UpdateOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}