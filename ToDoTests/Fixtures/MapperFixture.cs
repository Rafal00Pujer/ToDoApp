using ToDoApp.MapperProfile;

namespace ToDoTests.Fixtures;

public class MapperFixture
{
    public IMapper Fixture { get; private set; }

    public MapperFixture()
    {
        var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(ToDoListProfile).Assembly));
        Fixture = config.CreateMapper();
    }
}
