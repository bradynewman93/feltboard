using Common.Models;

namespace ArticleParser;

public interface IArticleDiscoveredHandler
{
    public Task ParseResource(DiscoveredArticle discoveredResouce);
}