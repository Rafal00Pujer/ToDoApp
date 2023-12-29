using Microsoft.EntityFrameworkCore;
using ToDoApp.Components.Pages;

namespace ToDoTests.Tests;

[Collection(TestsConfigs.UsesDatabase)]
public class ToDoTaskServiceTests : IClassFixture<DatabaseContextFixture>, IClassFixture<MapperFixture>, IDisposable
{
    private readonly ToDoTaskService _sut;
    private readonly ToDoContext _context;
    private readonly IFixture _fixture;

    public ToDoTaskServiceTests(DatabaseContextFixture databaseContextFixture, MapperFixture mapperFixture)
    {
        var mapper = mapperFixture.Fixture;

        var fixture = new Fixture();

        var context = databaseContextFixture.Fixture;
        context.Database.EnsureCreated();

        _sut = new ToDoTaskService(context, mapper);
        _context = context;
        _fixture = fixture;
    }

    [Fact]
    public async Task AddTaskToToDoList_ModelIsNull_ThrowsException()
    {
        // Arrange

        // Act
        var action = async () => await _sut.AddTaskToToDoList(null!);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddTaskToToDoList_InvalidToDoListId_ThrowsException()
    {
        // Arrange
        var model = _fixture.Create<AddToDoTaskModel>();

        // Act
        var action = async () => await _sut.AddTaskToToDoList(model);

        // Assert
        await action.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage($"To do list with id:{model.ToDoListId} doesn't exist.");
    }

    [Fact]
    public async Task AddTaskToToDoList_ValidModel_AddsNewTaskToToDoList()
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

        var model = _fixture.Build<AddToDoTaskModel>()
            .With(x => x.ToDoListId, toDoList.Id)
            .Create();

        // Act
        var result = await _sut.AddTaskToToDoList(model);

        // Assert
        var taskInDatabase = await _context.ToDoTasks.FirstOrDefaultAsync();

        taskInDatabase.Should().NotBeNull();
        taskInDatabase!.ToDoListId.Should().Be(toDoList.Id);

        result.Id.Should().Be(taskInDatabase.Id);

        result.Name.Should().Be(taskInDatabase.Name);
        result.Name.Should().Be(model.Name);
    }

    [Fact]
    public async Task UpdateTaskName_InvalidNewName_ThrowsException()
    {
        // Arrange

        // Act
        var action = async () => await _sut.UpdateTaskName(Guid.NewGuid(), string.Empty);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateTaskName_InvalidTaskId_ThrowsException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var newName = _fixture.Create<string>();

        // Act
        var action = async () => await _sut.UpdateTaskName(taskId, newName);

        // Assert
        await action.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage($"To do Task with id:{taskId} doesn't exist.");
    }

    [Fact]
    public async Task UpdateTaskName_ValidTaskIdAndNewName_UpdatesTaskName()
    {
        // Arrange
        var user = _fixture.Build<UserEntity>()
            .Without(x => x.ToDoLists)
            .Create();

        var toDoList = _fixture.Build<ToDoListEntity>()
            .With(x => x.User, user)
            .Without(x => x.Tasks)
            .Create();

        var task = _fixture.Build<ToDoTaskEntity>()
            .With(x => x.ToDoList, toDoList)
            .Create();

        await _context.ToDoTasks.AddAsync(task);
        await _context.SaveChangesAsync();

        var newName = _fixture.Create<string>();

        // Act
        await _sut.UpdateTaskName(task.Id, newName);

        // Assert
        var taskInDatabase = await _context.ToDoTasks.FirstOrDefaultAsync();

        taskInDatabase.Should().NotBeNull();
        taskInDatabase!.Name.Should().Be(newName);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
