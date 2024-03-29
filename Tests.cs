using FluentAssertions;

namespace game_of_life;

public class Tests
{
    [Fact]
    public void ValidateParsing()
    {
        // Arrange
        const string input = """
                             Generation 1:
                             .*
                             **
                             """;

        // Act
        var generation = Generation.Parse(input);

        // Assert
        generation.Iteration.Should().Be(1);
        generation.Grid.Should().HaveCount(3)
            .And.AllSatisfy(x => x.Value.Should().BeTrue())
            .And.ContainKeys((1, 0), (0, 1), (1, 1));
        generation.ToString().Should().Be("""
                                          Generation 1:
                                          ....
                                          ..*.
                                          .**.
                                          ....
                                          """);
    }
    
    [Fact]
    public void ValidateIteration()
    {
        // Arrange
        const string input = """
                             Generation 1:
                             .*
                             **
                             """;
        
        var generation = Generation.Parse(input);

        // Act
        var nextGeneration = generation.Iterate();

        // Assert
        nextGeneration.ToString().Should().Be("""
                                              Generation 2:
                                              ....
                                              .**.
                                              .**.
                                              ....
                                              """);
    }
    
    [Theory]
    [InlineData("Glider", """
                          Generation 1:
                          .*.
                          ..*
                          ***
                          """)]
    [InlineData("Lol", """
                       Generation 1:
                       *...***.*..
                       *...*.*.*..
                       ***.***.***
                       """)]
    public Task ValidateIterations(string testCase, string input)
    {
        // Arrange
        var generation = Generation.Parse(input);

        // Act
        var generations = new StringBuilder();
        generations.AppendLine(generation.ToString());
        generations.AppendLine();
        foreach (var _ in Enumerable.Range(0, 30))
        {
            generation = generation.Iterate();
            generations.AppendLine(generation.ToString());
            generations.AppendLine();
        }
        
        // Assert
        return Verify(generations.ToString().TrimEnd()).UseParameters(testCase);
    }
}