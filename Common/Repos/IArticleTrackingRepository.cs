using Common.Models;

namespace Common.Repos;

public interface IArticleTrackingRepository
{
    public Task<bool> ExistsAsync(string url, string source);
    public Task<DiscoveredArticle> SaveAsync(DiscoveredArticle article);
}
