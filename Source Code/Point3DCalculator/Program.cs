namespace Point3DCalculator;

public class Point3D
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Point3D()
    {
        X = 0.0;
        Y = 0.0;
        Z = 0.0;
    }

    public Point3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString() =>
        $"({X}, {Y}, {Z})";

    public static Point3D operator +(Point3D a, Point3D b) =>
        new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Point3D operator -(Point3D a, Point3D b) =>
        new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static double operator *(Point3D a, Point3D b) =>
        a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public static Point3D operator *(Point3D a, double k) =>
        new(a.X * k, a.Y * k, a.Z * k);

    public static Point3D operator /(Point3D a, double k)
    {
        if (k.Equals(0.0))
        {
            throw new Exception("Invalid number");
        }

        return new(a.X / k, a.Y / k, a.Z / k);
    }

    public static double Distance(Point3D a, Point3D b) =>
        Math.Sqrt((b.X - a.X) * (b.X - a.X) +
                  (b.Y - a.Y) * (b.Y - a.Y) +
                  (b.Z - a.Z) * (b.Z - a.Z));
}

internal class Program
{
    private static void Main()
    {
        var P1 = new Point3D(1, 2, 3);
        var P2 = new Point3D(4, 5, 6);

        Console.WriteLine($"Given 2 points in space:");
        Console.WriteLine($"- Point P1{P1.ToString()}");
        Console.WriteLine($"- Point P1{P2.ToString()}");

        Point3D A = P1 + P2;
        Console.WriteLine($"1) P1 + P2 = {A.ToString()}");
        Point3D B = P1 - P2;
        Console.WriteLine($"2) P1 - P2 = {B.ToString()}");
        Point3D C = P1 * 5;
        Console.WriteLine($"3) P1 * 5 = {C.ToString()}");
        Point3D D = P1 / 10;
        Console.WriteLine($"4) P1 / 10 = {D.ToString()}");
        double distance = Point3D.Distance(P1, P2);
        Console.WriteLine($"5) Distance from P1 to P2: d(P1, P2) = {distance}");

        Console.ReadLine();
    }
}
