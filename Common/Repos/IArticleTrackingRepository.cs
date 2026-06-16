namespace Common.Repos;

public interface IArticleTrackingRepository
{
    Task<bool> ExistsAsync(string url, string source);
    Task SaveAsync(string url, string source, string status, DateTimeOffset processedAt);
}
