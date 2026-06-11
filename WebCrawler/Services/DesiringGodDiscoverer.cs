using Microsoft.Extensions.Logging;
using WebCrawler.Models;
using HtmlAgilityPack;

namespace WebCrawler.Services;


public class DesiringGodDiscoverer : IDesiringGodDiscoverer
{

    private readonly HttpClient _httpClient;


    private readonly ILogger _logger;


    public DesiringGodDiscoverer(HttpClient httpClient, ILoggerFactory loggerFactory)
    {
        _httpClient = httpClient;
        _logger = loggerFactory.CreateLogger(nameof(DesiringGodDiscoverer));
    }

   // Listing page configs: (path, resourceType, maxPages)
    // Tune maxPages to control how deep each crawl goes per schedule run.
    private static readonly (string Path, string ResourceType, int MaxPages)[] ResourceSections =
    [
        ("/articles/all",  "article",500),
    ];

    public async Task<List<DiscoveredArticle>> DiscoverAsync()
    {
        var discovered = new List<DiscoveredArticle>();
 
        foreach (var (sectionPath, resourceType, maxPages) in ResourceSections)
        {
            _logger.LogInformation("Discovering {ResourceType} from {Path} (up to {MaxPages} pages)",
                resourceType, sectionPath, maxPages);
 
            for (var page = 1; page <= maxPages; page++)
            {
                var url = page == 1 ? sectionPath : $"{sectionPath}?page={page}";
                var pageResults = await CrawlListingPageAsync(url, resourceType);
 
                if (pageResults.Count == 0)
                {
                    _logger.LogInformation("No results on page {Page} for {Path}, stopping", page, sectionPath);
                    break;
                }
 
                discovered.AddRange(pageResults);
                _logger.LogInformation("Found {Count} articles on page {Page} of {Path}",
                    pageResults.Count, page, sectionPath);
 
                // Be polite — don't hammer the server
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
 
        // Deduplicate by URL
        var unique = discovered
            .DistinctBy(a => a.Url)
            .ToList();
 
        _logger.LogInformation("Discovery complete. {Total} unique URLs found", unique.Count);
        return unique;
    }
 
    private async Task<List<DiscoveredArticle>> CrawlListingPageAsync(string path, string resourceType)
    {
        var results = new List<DiscoveredArticle>();
 
        try
        {
            var html = await _httpClient.GetStringAsync(path);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var articleLinks = doc.DocumentNode.SelectNodes("//a[@href]");

            if (articleLinks is null) return results;
 
            foreach (var link in articleLinks)
            {
                var href = link.GetAttributeValue("href", string.Empty);
 
                if (!IsResourceUrl(href)) continue;
 
                var absoluteUrl = href.StartsWith("http")
                    ? href
                    : $"https://www.desiringgod.org{href}";
 
                results.Add(new DiscoveredArticle
                {
                    Url = absoluteUrl,
                    ResourceType = resourceType,
                    DiscoveredAt = DateTimeOffset.UtcNow
                });
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error crawling {Path}", path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error crawling {Path}", path);
        }
 
        return results;
    }
 
    /// <summary>
    /// Filters out navigation links, external links, and non-resource paths.
    /// DG resource URLs follow patterns like /articles/slug or /sermons/slug.
    /// </summary>
    private static bool IsResourceUrl(string href)
    {
        if (string.IsNullOrWhiteSpace(href)) return false;
        if (href.StartsWith("http") && !href.Contains("desiringgod.org")) return false;
        if (href.Contains('#') || href.Contains("mailto:")) return false;
        if (href.Contains("/articles/all")) return false;

        return href.Contains("/articles/");
    }
}