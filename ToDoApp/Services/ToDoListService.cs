using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Context;
using ToDoApp.Data.Entities;
using ToDoApp.Data.Models;

namespace ToDoApp.Services;

public class ToDoListService(ToDoContext context, IMapper mapper)
{
    public async Task<List<ToDoListModel>> GetToDoListsForUserAsync(string userId)
    {
        var query = context.ToDoLists
            .Include(x => x.Tasks
                .OrderBy(y => y.SortWeight))
            .Where(x => x.UserId == userId);

        var dto = await mapper.ProjectTo<ToDoListModel>(query)
            .ToListAsync();

        return dto;
    }

    public async Task<ToDoListModel> AddToDoListToUserAsync(AddToDoListModel model, string userId)
    {
        ArgumentNullException.ThrowIfNull(model);

        var owner = await context.Users.FindAsync(userId);

        if (owner is null)
        {
            throw new InvalidOperationException($"User with id:{userId} doesn't exist.");
        }

        var toDoListEntity = new ToDoListEntity
        {
            Name = model.Name,
            User = owner
        };

        await context.ToDoLists.AddAsync(toDoListEntity);
        await context.SaveChangesAsync();

        var dto = mapper.Map<ToDoListModel>(toDoListEntity);
        return dto;
    }

    public async Task DeleteToDoListAsync(Guid toDoListId)
    {
        var list = await context.ToDoLists.FindAsync(toDoListId);

        if (list is null)
        {
            throw new InvalidOperationException($"To do list with id:{toDoListId} doesn't exist.");
        }

        context.ToDoLists.Remove(list);

        await context.SaveChangesAsync();
    }
}
