using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Data.Models;

public class AddToDoTaskModel
{
    public string Name { get; set; } = default!;

    public Guid ToDoListId { get; set; }
}
