using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Context;
using ToDoApp.Data.Entities;
using ToDoApp.Data.Models;

namespace ToDoApp.Services;

internal class ToDoTaskService(ToDoContext context, IMapper mapper)
{
    public async Task<ToDoTaskModel> AddTaskToList(AddToDoTaskModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var toDoList = await context.ToDoLists
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == model.ToDoListId);

        if (toDoList is null)
        {
            throw new InvalidOperationException();
        }

        var taskEntity = new ToDoTaskEntity
        {
            Name = model.Name
        };

        toDoList.Tasks.Add(taskEntity);

        await context.SaveChangesAsync();

        var result = mapper.Map<ToDoTaskModel>(taskEntity);

        return result;
    }
}
