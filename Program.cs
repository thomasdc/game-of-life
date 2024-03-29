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
    IReadOnlyDictionary<(int X, int Y), bool> Grid)
{
    public static Generation Parse(string input)
    {
        var lines = input.Split("\n");
        var iteration = int.Parse(Regex.Match(lines[0], @"\s(?<iter>\d+):").Groups["iter"].ValueSpan);
        var height = lines.Length - 1;
        var width = lines[1].Trim().Length;
        var grid = (
            from y in Enumerable.Range(0, height)
            from x in Enumerable.Range(0, width)
            where lines[y + 1][x] is '*'
            select ((x, y), true)
        ).ToDictionary();
        return new Generation(iteration, grid);
    }

    public Generation Iterate()
    {
        var bounds = Grid.Bounds();
        var nextGrid = (
            from y in Enumerable.Range(bounds.Y.Min - 1, bounds.Y.Length + 2)
            from x in Enumerable.Range(bounds.X.Min - 1, bounds.X.Length + 2)
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
        return new Generation(Iteration + 1, nextGrid);
    }

    public override string ToString()
    {
        var bounds = Grid.Bounds();
        return new StringBuilder()
            .AppendLine($"Generation {Iteration}:")
            .AppendLine(string.Join("\r\n",
                from y in Enumerable.Range(bounds.Y.Min - 1, bounds.Y.Length + 2)
                select string.Concat(
                    from x in Enumerable.Range(bounds.X.Min - 1, bounds.X.Length + 2)
                    select Grid.GetValueOrDefault((x, y)) ? '*' : '.')))
            .ToString()
            .TrimEnd();
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

    public static ((int Min, int Length) Y, (int Min, int Length) X) Bounds(this IReadOnlyDictionary<(int X, int Y), bool> grid)
    {
        var aliveCells = grid.Where(_ => _.Value).Select(_ => _.Key).ToArray();
        var minY = aliveCells.Min(_ => _.Y);
        var minX = aliveCells.Min(_ => _.X);
        return (
            (minY, aliveCells.Max(_ => _.Y) - minY + 1),
            (minX, aliveCells.Max(_ => _.X) - minX + 1)
        );
    }
}
