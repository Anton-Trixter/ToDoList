using Microsoft.EntityFrameworkCore;

namespace ToDoList.Db;

public class ToDoListContext: DbContext
{
    public DbSet<Task> Tasks { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "User ID=postgres;Password=123qwe;Host=localhost;Port=5432;Database=todo;Pooling=true;");
    }
}