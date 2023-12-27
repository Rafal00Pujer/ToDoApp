using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Data.Models;

public class AddToDoListModel
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; }
}
