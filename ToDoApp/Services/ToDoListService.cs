using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Context;
using ToDoApp.Data.Entities;
using ToDoApp.Data.Models;

namespace ToDoApp.Services;

internal class ToDoListService(ToDoContext context, IMapper mapper)
{
    public async Task<List<ToDoListModel>> GetToDoListsForUser(string userId)
    {
        var query = context.ToDoLists
            .Include(x => x.Tasks
                .OrderBy(y => y.SortWeight))
            .Where(x => x.UserId == userId);

        var dto = await mapper.ProjectTo<ToDoListModel>(query)
            .ToListAsync();

        return dto;
    }

    public async Task<ToDoListModel> AddToDoList(AddToDoListModel model, string ownerId)
    {
        ArgumentNullException.ThrowIfNull(model);

        var owner = await context.Users.FindAsync(ownerId);

        if (owner is null)
        {
            throw new InvalidOperationException();
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

    public async Task DeleteList(Guid listId)
    {
        var list = await context.ToDoLists.FindAsync(listId);

        if (list is null)
        {
            throw new InvalidOperationException();
        }

        context.ToDoLists.Remove(list);

        await context.SaveChangesAsync();
    }
}
