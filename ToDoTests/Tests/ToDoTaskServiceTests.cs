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
        var action = async () => await _sut.AddTaskToToDoListAsync(null!);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddTaskToToDoList_InvalidToDoListId_ThrowsException()
    {
        // Arrange
        var model = _fixture.Create<AddToDoTaskModel>();

        // Act
        var action = async () => await _sut.AddTaskToToDoListAsync(model);

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
        var result = await _sut.AddTaskToToDoListAsync(model);

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
        var action = async () => await _sut.UpdateTaskNameAsync(Guid.NewGuid(), string.Empty);

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
        var action = async () => await _sut.UpdateTaskNameAsync(taskId, newName);

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
        await _sut.UpdateTaskNameAsync(task.Id, newName);

        // Assert
        var taskInDatabase = await _context.ToDoTasks.FirstOrDefaultAsync();

        taskInDatabase.Should().NotBeNull();
        taskInDatabase!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task UpdateTaskDueDate_InvalidTaskId_ThrowsException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var newDueDate = _fixture.Create<DateTime>();

        // Act
        var action = async () => await _sut.UpdateTaskDueDateAsync(taskId, newDueDate);

        // Assert
        await action.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage($"To do Task with id:{taskId} doesn't exist.");
    }

    [Fact]
    public async Task UpdateTaskDueDate_ValidTaskIdAndNewDueDate_UpdatesDueDate()
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

        var newDueDate = _fixture.Create<DateTime>();

        // Act
        await _sut.UpdateTaskDueDateAsync(task.Id, newDueDate);

        // Assert
        var taskInDatabase = await _context.ToDoTasks.FirstOrDefaultAsync();

        taskInDatabase.Should().NotBeNull();
        taskInDatabase!.DueDate.Should().Be(newDueDate);
    }

    [Fact]
    public async Task UpdateTaskIsCompleted_InvalidTaskId_ThrowsException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var newIsCompleted = _fixture.Create<bool>();

        // Act
        var action = async () => await _sut.UpdateTaskIsCompletedAsync(taskId, newIsCompleted);

        // Assert
        await action.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage($"To do Task with id:{taskId} doesn't exist.");
    }

    [Fact]
    public async Task UpdateTaskIsCompleted_ValidTaskIdAndFalseIsCompleted_UpdatesIsCompletedAndReturnsNull()
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

        var newIsCompleted = false;

        // Act
        var result = await _sut.UpdateTaskIsCompletedAsync(task.Id, newIsCompleted);

        // Assert
        var taskInDatabase = await _context.ToDoTasks.FirstOrDefaultAsync();

        taskInDatabase.Should().NotBeNull();
        taskInDatabase!.IsCompleted.Should().Be(newIsCompleted);
        taskInDatabase.CompletionDate.Should().BeNull();

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTaskIsCompleted_ValidTaskIdAndTureIsCompleted_UpdatesIsCompletedAndReturnsCurrentDate()
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

        var newIsCompleted = true;
        var currentDate = DateTime.Now;

        // Act
        await _sut.UpdateTaskIsCompletedAsync(task.Id, newIsCompleted);

        // Act
        var result = await _sut.UpdateTaskIsCompletedAsync(task.Id, newIsCompleted);

        // Assert
        var taskInDatabase = await _context.ToDoTasks.FirstOrDefaultAsync();

        taskInDatabase.Should().NotBeNull();
        taskInDatabase!.IsCompleted.Should().Be(newIsCompleted);
        taskInDatabase.CompletionDate.Should().NotBeNull()
            .And.BeCloseTo(currentDate, TimeSpan.FromSeconds(5));

        result.Should().NotBeNull()
            .And.BeCloseTo(currentDate, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task DeleteTask_InvalidTaskId_ThrowsException()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        // Act
        var action = async () => await _sut.DeleteTaskAsync(taskId);

        // Assert
        await action.Should().ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage($"To do Task with id:{taskId} doesn't exist.");
    }

    [Fact]
    public async Task DeleteTask_ValidTaskId_DeletesTask()
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

        // Act
        await _sut.DeleteTaskAsync(task.Id);

        // Assert
        var taskInDatabase = await _context.ToDoTasks.FirstOrDefaultAsync();

        taskInDatabase.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
