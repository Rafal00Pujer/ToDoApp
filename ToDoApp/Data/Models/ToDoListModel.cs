namespace ToDoApp.Data.Models;

internal class ToDoListModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public ICollection<ToDoTaskModel> Tasks { get; set; } = default!;
}
