namespace WebCrawler.Util;



public class EnvironmentUtil
{
    public static string EnsureEnvVariable(string key)
    {
        string? value = Environment.GetEnvironmentVariable(key);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("EnvVariable: {} is null or empty.",key);
        }

        return value;
    }
}