using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Context;

namespace ToDoTests.Fixtures;

public class DatabaseContextFixture : IDisposable
{
    private const string ConnectionString = "Server=localhost;Database=ToDoAppTests;Trusted_Connection=True;TrustServerCertificate=True;";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public ToDoContext Fixture { get; private set; }

    public DatabaseContextFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                Fixture = new ToDoContext(new DbContextOptionsBuilder<ToDoContext>()
                    .UseSqlServer(ConnectionString)
                    .Options);

                Fixture.Database.EnsureDeleted();
                //Fixture.Database.EnsureCreated();

                _databaseInitialized = true;
            }
            else
            {
                throw new Exception("Multiple database initializations are not supported.");
            }
        }
    }

    public void Dispose()
    {
        Fixture.Database.EnsureDeleted();
        Fixture.Dispose();
        _databaseInitialized = false;
    }
}
