using Common.Models;

namespace ArticleParser;

public interface IArticleDiscoveredHandler
{
    public Task Handler(DiscoveredArticle discoveredResouce);
}