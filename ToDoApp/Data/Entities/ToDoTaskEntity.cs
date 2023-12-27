using System.Diagnostics.CodeAnalysis;

namespace ToDoApp.Data.Entities;

internal class ToDoTaskEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public int SortWeight { get; set; }

    public DateTime? DueDate { get; set; }

    [MemberNotNullWhen(true, nameof(CompletionDate))]
    public bool IsCompleted { get; set; }

    public DateTime? CompletionDate { get; set; }

    public Guid ToDoListId { get; set; }

    public ToDoListEntity ToDoList { get; set; } = default!;
}
