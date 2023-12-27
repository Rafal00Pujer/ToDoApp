using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Data.Models;

internal class AddToDoTaskModel
{
    public string Name { get; set; } = default!;

    public Guid ToDoListId { get; set; }
}
