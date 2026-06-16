namespace ArticleParser.Util;


public class Sha256Util
{
    public static string GenerateSha256Hash(string input)
    {
        return Uri.TryCreate(input, UriKind.Absolute, out var uri)
        ? $"{uri.Host}{uri.AbsolutePath}".ToLower().Trim('/')
        : input;

    }
}