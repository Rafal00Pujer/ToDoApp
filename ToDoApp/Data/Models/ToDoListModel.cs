namespace ToDoApp.Data.Models;

public class ToDoListModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public ICollection<ToDoTaskModel> Tasks { get; set; } = default!;
}
