

using WebCrawler.Models;

public interface IDesiringGodDiscoverer
{
    public Task<List<DiscoveredArticle>> DiscoverAsync();
}