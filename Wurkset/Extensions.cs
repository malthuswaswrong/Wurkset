namespace Wurkset;

public static class Extensions
{
    public static string ToPath(this long value)
    {
        var tmp = value.ToString().ToCharArray();
        return string.Join('/', tmp);
    }
}
