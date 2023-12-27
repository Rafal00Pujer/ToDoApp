using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Entities;

namespace ToDoApp.Data.Context;

internal class ToDoContext(DbContextOptions options) : IdentityDbContext<UserEntity>(options)
{
    public DbSet<ToDoListEntity> ToDoLists { get; set; }

    public DbSet<ToDoTaskEntity> ToDoTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ToDoListEntity>()
            .HasKey(x => x.Id);

        builder.Entity<ToDoListEntity>()
            .HasOne(x => x.User)
            .WithMany(x => x.ToDoLists)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ToDoTaskEntity>()
            .HasKey(x => x.Id);

        builder.Entity<ToDoTaskEntity>()
            .HasOne(x => x.ToDoList)
            .WithMany(x => x.Tasks)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
