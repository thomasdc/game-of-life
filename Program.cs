// https://codingdojo.org/kata/GameOfLife/

Console.WriteLine(Generation.Parse(File.ReadAllText("input.txt")));

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
}
