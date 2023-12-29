using Microsoft.AspNetCore.Identity;

namespace ToDoApp.Data.Entities;

// Add profile data for application users by adding properties to the ApplicationUser class
public class UserEntity : IdentityUser
{
    public ICollection<ToDoListEntity> ToDoLists { get; set; } = default!;
}
