namespace Geo_Wall_E;

static class Resources
{
    //Punto Aleatorio
    private static Random random = new Random();
    private static int CoordinateLimit = 2000;
    public static void SetCoordinateLimit(int coordinateLimit)
    {
        CoordinateLimit = Math.Abs(coordinateLimit);
    }
    public static int RandomCoordinates()
    {
        return Math.Abs((random.Next() % (2 * CoordinateLimit + 1)) - CoordinateLimit);
    }

    public static int RandomCoordinates(Random r)
    {
        return (r.Next() % (2 * CoordinateLimit + 1)) - CoordinateLimit;
    }

    public static Object.Point RandomPoint(Random r)
    {
        return new Object.Point(new Object.String(), new Object.Number(RandomCoordinates(r)), new Object.Number(RandomCoordinates(r)), new Object.String(), Colors.BLACK);
    }

    //Comparacion de DOS valores reales
    public static int Compare(float x, float y)
    {
        if (float.IsNegative(x - y))
        {
            return -1;
        }
        if (Is0(x - y))
        {
            return 0;
        }
        return 1;
    }

    //Comprobacion aproximada del valor nulo
    public static bool Is0(float x)
    {
        if (x <= ZeroLimit)
        {
            return true;
        }
        return false;
    }

    public static float ZeroLimit = 1e-6f;

    public static string LoadFile(string path)
    {
        path = Path.GetFullPath(path);

        if (!Path.Exists(path))
        {
            throw new FileNotFoundException(path);
        }

        return System.Text.Encoding.Default.GetString(File.ReadAllBytes(path));
    }

    public static void PrintTokens(List<Token> tokens, TextWriter outputStream, string file = "")
    {
        outputStream.WriteLine($"Scanner output {file} : Begin");
        outputStream.WriteLine("{");
        foreach (Token t in tokens) outputStream.WriteLine("\t" + t);
        outputStream.WriteLine("}");
        outputStream.WriteLine("Scanner output : End");
    }

    public static bool IsDraweable(Object Object1)
    {
        switch (Object1.Type)
        {
            case ObjectsTypes.ARC:
                return true;
            case ObjectsTypes.CIRCLE:
                return true;
            case ObjectsTypes.LINE:
                return true;
            case ObjectsTypes.SEGMENT:
                return true;
            case ObjectsTypes.RAY:
                return true;
            case ObjectsTypes.POINT:
                return true;
            default:
                return false;
        }
    }
}