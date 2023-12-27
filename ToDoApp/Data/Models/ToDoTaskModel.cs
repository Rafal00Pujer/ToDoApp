using System.Diagnostics.CodeAnalysis;

namespace ToDoApp.Data.Models;

public class ToDoTaskModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public DateTime? DueDate { get; set; }

    [MemberNotNullWhen(true, nameof(CompletionDate))]
    public bool IsCompleted { get; set; }

    public DateTime? CompletionDate { get; set; }
}
