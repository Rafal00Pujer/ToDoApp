using AutoFixture;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Entities;
using ToDoApp.Data.Models;

namespace ToDoTests.Tests;

[Collection(TestsConfigs.UsesDatabase)]
public class ToDoListServiceTests : IClassFixture<DatabaseContextFixture>, IClassFixture<MapperFixture>, IDisposable
{
    private readonly ToDoListService _sut;
    private readonly ToDoContext _context;
    private readonly IFixture _fixture;

    public ToDoListServiceTests(DatabaseContextFixture databaseContextFixture, MapperFixture mapperFixture)
    {
        var mapper = mapperFixture.Fixture;

        var fixture = new Fixture();

        var context = databaseContextFixture.Fixture;
        context.Database.EnsureCreated();

        _sut = new ToDoListService(context, mapper);
        _context = context;
        _fixture = fixture;
    }

    [Fact]
    public async Task GetToDoListsForUser_ReturnsAllToDoListsForUser()
    {
        // Arrange
        var user = _fixture.Build<UserEntity>()
            .Without(x => x.ToDoLists)
            .Create();

        var toDoLists = _fixture.Build<ToDoListEntity>()
            .With(x => x.User, user)
            .Without(x => x.Tasks)
            .CreateMany();

        await _context.ToDoLists.AddRangeAsync(toDoLists);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetToDoListsForUserAsync(user.Id);

        // Assert
        result.Should()
            .HaveCount(toDoLists.Count());
        result.Select(x => x.Id)
            .Should()
            .Contain(toDoLists.Select(x => x.Id));
    }

    [Fact]
    public async Task AddToDoListToUser_ModelIsNull_ThorwsException()
    {
        // Arrange

        // Act
        var action = async () => await _sut.AddToDoListToUserAsync(null!, string.Empty);

        // Assert
        await action.Should()
            .ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddToDoListToUser_UserIdIsInvalid_ThrowsException()
    {
        // Arrange
        var model = _fixture.Create<AddToDoListModel>();
        var userId = Guid.NewGuid().ToString();

        // Act
        var action = async () => await _sut.AddToDoListToUserAsync(model, userId);

        // Assert
        await action.Should()
             .ThrowExactlyAsync<InvalidOperationException>()
             .WithMessage($"User with id:{userId} doesn't exist.");
    }

    [Fact]
    public async Task AddToDoListToUser_ValidModelAndUserId_AddsNewToDoList()
    {
        // Arrange
        var user = _fixture.Build<UserEntity>()
            .Without(x => x.ToDoLists)
            .Create();

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var model = _fixture.Create<AddToDoListModel>();

        // Act
        var result = await _sut.AddToDoListToUserAsync(model, user.Id);

        // Assert
        var toDoListInDb = await _context.ToDoLists.FirstOrDefaultAsync();

        toDoListInDb.Should().NotBeNull();
        toDoListInDb!.Name.Should().BeSameAs(model.Name);
        toDoListInDb.UserId.Should().BeSameAs(user.Id);
    }

    [Fact]
    public async Task DeleteToDoList_InvalidToDoListId_ThrowsException()
    {
        // Arrange
        var toDoListId = Guid.NewGuid();

        // Act
        var action = async () => await _sut.DeleteToDoListAsync(toDoListId);

        // Assert
        await action.Should()
            .ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage($"To do list with id:{toDoListId} doesn't exist.");
    }

    [Fact]
    public async Task DeleteToDoList_ValidToDoListId_RemovesToDoList()
    {
        // Arrange
        var user = _fixture.Build<UserEntity>()
            .Without(x => x.ToDoLists)
            .Create();

        var toDoList = _fixture.Build<ToDoListEntity>()
            .With(x => x.User, user)
            .Without(x => x.Tasks)
            .Create();

        await _context.ToDoLists.AddAsync(toDoList);
        await _context.SaveChangesAsync();

        // Act
        await _sut.DeleteToDoListAsync(toDoList.Id);

        // Assert
        var toDoListInDb = await _context.ToDoLists.FirstOrDefaultAsync();

        toDoListInDb.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
