// https://codingdojo.org/kata/GameOfLife/

var generation = Generation.Parse(File.ReadAllText("input.txt"));

do
{
    Console.WriteLine(generation);
    Console.ReadLine();
    generation = generation.Iterate();
} while (true);

public record Generation(
    int Iteration, 
    int Width, 
    int Height, 
    IReadOnlyDictionary<(int X, int Y), bool> Grid)
{
    public static Generation Parse(string input)
    {
        var lines = input.Split("\n");
        var iteration = int.Parse(Regex.Match(lines[0], @"\s(?<iter>\d+):").Groups["iter"].ValueSpan);
        var dimension = Regex.Match(lines[1], @"(?<height>\d+)\s(?<width>\d+)");
        var height = int.Parse(dimension.Groups["height"].ValueSpan);
        var width = int.Parse(dimension.Groups["width"].ValueSpan);
        var grid = (
            from y in Enumerable.Range(0, height)
            from x in Enumerable.Range(0, width)
            where lines[y + 2][x] is '*'
            select ((x, y), true)
        ).ToDictionary();
        return new Generation(iteration, width, height, grid);
    }

    public Generation Iterate()
    {
        var nextGrid = (
            from y in Enumerable.Range(0, Height)
            from x in Enumerable.Range(0, Width)
            let liveNeighbourCount = (x, y).Neighbours().Count(n => Grid.GetValueOrDefault((n.X, n.Y)))
            let alive = Grid.GetValueOrDefault((x, y))
            let nextState = (alive, liveNeighbourCount) switch
            {
                (true, < 2) => false,   // 1. Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
                (true, > 3) => false,   // 2. Any live cell with more than three live neighbours dies, as if by overcrowding.
                (true, 2 or 3) => true, // 3. Any live cell with two or three live neighbours lives on to the next generation.
                (false, 3) => true,     // 4. Any dead cell with exactly three live neighbours becomes a live cell.
                (_, _) => alive 
            }
            select ((x, y), nextState)
        ).ToDictionary();
        return this with { Iteration = Iteration + 1, Grid = nextGrid };
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Generation {Iteration}:");
        builder.AppendLine($"{Height} {Width}");
        for (var y = 0; y < Height; y++)
        {
            var innerBuilder = new StringBuilder();
            for (var x = 0; x < Width; x++)
            {
                innerBuilder.Append(Grid.GetValueOrDefault((x, y)) ? '*' : '.');
            }

            builder.AppendLine(innerBuilder.ToString());
        }
        
        return builder.ToString().TrimEnd();
    }
}

public static class Util
{
    private static readonly (int xDiff, int yDiff)[] NeighbourVectors = (
        from x in new[] { -1, 0, 1 }
        from y in new[] { -1, 0, 1 }
        where (x, y) is not (0, 0)
        select (x, y)
    ).ToArray();

    public static (int X, int Y)[] Neighbours(this (int X, int Y) cell) => (
            from diff in NeighbourVectors
            select (cell.X + diff.xDiff, cell.Y + diff.yDiff))
        .ToArray();
}
