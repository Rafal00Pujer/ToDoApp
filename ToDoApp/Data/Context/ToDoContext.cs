using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Entities;

namespace ToDoApp.Data.Context
{
    public class ToDoContext(DbContextOptions<ToDoContext> options) : IdentityDbContext<UserEntity>(options)
    {
    }
}
