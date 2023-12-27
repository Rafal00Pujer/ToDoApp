namespace ToDoApp.Data.Entities;

internal class ToDoListEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public UserEntity User { get; set; } = default!;

    public ICollection<ToDoTaskEntity> Tasks { get; set; } = default!;
}
