using FluentAssertions;
using Xunit;

namespace game_of_life;

public class Tests
{
    [Fact]
    public void ValidateParsing()
    {
        // Arrange
        const string input = """
                             Generation 1:
                             4 8
                             ........
                             ....*...
                             ...**...
                             ........
                             """;

        // Act
        var generation = Generation.Parse(input);

        // Assert
        generation.Iteration.Should().Be(1);
        generation.Width.Should().Be(8);
        generation.Height.Should().Be(4);
        generation.Grid.Should().HaveCount(3)
            .And.AllSatisfy(x => x.Value.Should().BeTrue())
            .And.ContainKeys((4, 1), (3, 2), (4, 2));
    }
}