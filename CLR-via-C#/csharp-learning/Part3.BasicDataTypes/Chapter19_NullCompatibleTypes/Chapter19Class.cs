namespace csharp_learning.Part3.BasicDataTypes.Chapter19_NullCompatibleTypes;

public class Chapter19Class
{
    public void Execute()
    {
        Point? p1 = new Point(1, 1);
        Point? p2 = new Point(2, 2);
        Point? p3 = null;

        Console.WriteLine($"Are points equals? {p1 == p2}");
        Console.WriteLine($"Are points equals? {p1 == p3}");


        Int32? n = null;
        Object o = n;
        Console.WriteLine($"o is null = {o == null}");

        n = 5;
        o = n;
        Console.WriteLine($"o's type = {o.GetType()}");
    }
}

public struct Point
{
    public Int32 x, y;

    public Point(Int32 x, Int32 y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(Point p1, Point p2)
    {
        return (p1.x == p2.x) && (p1.y == p2.y);
    }

    public static bool operator !=(Point p1, Point p2)
    {
        return !(p1 == p2);
    }
}
