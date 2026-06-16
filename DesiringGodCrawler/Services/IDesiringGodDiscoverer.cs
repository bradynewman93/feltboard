

using Common.Models;

public interface IDesiringGodDiscoverer
{
    public Task<List<DiscoveredArticle>> DiscoverAsync();
}