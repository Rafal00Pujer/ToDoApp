using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Components.Pages;
using ToDoApp.Data.Context;
using ToDoApp.Data.Entities;
using ToDoApp.Data.Models;

namespace ToDoApp.Services;

public class ToDoTaskService(ToDoContext context, IMapper mapper)
{
    const int SortWeightInterval = 100_000;

    public async Task<ToDoTaskModel> AddTaskToToDoList(AddToDoTaskModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var toDoList = await context.ToDoLists
            .Include(x => x.Tasks
                .OrderBy(y => y.SortWeight))
            .FirstOrDefaultAsync(x => x.Id == model.ToDoListId);

        if (toDoList is null)
        {
            throw new InvalidOperationException($"To do list with id:{model.ToDoListId} doesn't exist.");
        }

        int sortWeight = SortWeightInterval;

        if (toDoList.Tasks.Count > 0)
        {
            sortWeight += toDoList.Tasks.Last().SortWeight;
        }

        var taskEntity = new ToDoTaskEntity
        {
            Name = model.Name,
            SortWeight = sortWeight
        };

        toDoList.Tasks.Add(taskEntity);

        await context.SaveChangesAsync();

        var result = mapper.Map<ToDoTaskModel>(taskEntity);

        return result;
    }

    public async Task UpdateTaskName(Guid ToDoTaskId, string newName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);

        var task = await context.ToDoTasks.FindAsync(ToDoTaskId);

        if (task is null)
        {
            throw new InvalidOperationException($"To do Task with id:{ToDoTaskId} doesn't exist.");
        }

        task.Name = newName;

        await context.SaveChangesAsync();
    }

    public async Task UpdateTaskDueDate(Guid ToDoTaskId, DateTime? newDueTime)
    {
        var task = await context.ToDoTasks.FindAsync(ToDoTaskId);

        if (task is null)
        {
            throw new InvalidOperationException();
        }

        task.DueDate = newDueTime;

        await context.SaveChangesAsync();
    }

    public async Task<DateTime?> UpdateTaskIsCompleted(Guid ToDoTaskId, bool newIsCompleted)
    {
        var task = await context.ToDoTasks.FindAsync(ToDoTaskId);

        if (task is null)
        {
            throw new InvalidOperationException();
        }

        task.IsCompleted = newIsCompleted;

        if (task.IsCompleted)
        {
            task.CompletionDate = DateTime.Now;
        }
        else
        {
            task.CompletionDate = null;
        }

        await context.SaveChangesAsync();

        return task.CompletionDate;
    }

    public async Task DeleteTask(Guid ToDoTaskId)
    {
        var task = await context.ToDoTasks.FindAsync(ToDoTaskId);

        if (task is null)
        {
            throw new InvalidOperationException();
        }

        context.ToDoTasks.Remove(task);

        await context.SaveChangesAsync();
    }
}
