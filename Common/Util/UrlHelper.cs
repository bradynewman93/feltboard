namespace Common.Util;

public static class UrlHelper
{
    public static string RetrieveUrlSlug(string url)
    {
        var urlSlug = url.Split("/").Last();
        return urlSlug;
    }
}