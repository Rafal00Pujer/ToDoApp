namespace ToDoTests.Tests;

public class MapperTests(MapperFixture mapper) : IClassFixture<MapperFixture>
{
    private readonly IMapper _mapper = mapper.Fixture;

    [Fact]
    public void ValidateMapperProfiles()
    {
        var action = _mapper.ConfigurationProvider.AssertConfigurationIsValid;

        action.Should().NotThrow();
    }
}
