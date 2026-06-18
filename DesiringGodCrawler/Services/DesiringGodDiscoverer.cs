using Common.Repos;
using Common.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace DesiringGodCrawler.Services;

public class DesiringGodDiscoverer : IDesiringGodDiscoverer
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly IArticleTrackingRepository _trackingRepo;

    public DesiringGodDiscoverer(HttpClient httpClient, ILoggerFactory loggerFactory, IArticleTrackingRepository trackingRepo)
    {
        _httpClient = httpClient;
        _logger = loggerFactory.CreateLogger(nameof(DesiringGodDiscoverer));
        _trackingRepo = trackingRepo;
    }

    private static readonly (string Path, string ResourceType, int MaxPages)[] ResourceSections =
    [
        ("/articles/all", "article", 5),
    ];

    public async Task<List<DiscoveredArticle>> DiscoverAsync()
    {
        var discovered = new List<DiscoveredArticle>();

        foreach (var (sectionPath, resourceType, maxPages) in ResourceSections)
        {
            _logger.LogInformation("Discovering {ResourceType} from {Path} (up to {MaxPages} pages)",
                resourceType, sectionPath, maxPages);

            var stopCrawling = false;

            for (var page = 1; page <= maxPages && !stopCrawling; page++)
            {
                var url = page == 1 ? sectionPath : $"{sectionPath}?page={page}";
                var (pageResults, hitKnown) = await CrawlListingPageAsync(url, resourceType);

                if (pageResults.Count == 0)
                {
                    _logger.LogInformation("No results on page {Page} for {Path}, stopping", page, sectionPath);
                    break;
                }

                discovered.AddRange(pageResults);
                _logger.LogInformation("Found {Count} new articles on page {Page} of {Path}",
                    pageResults.Count, page, sectionPath);

                if (hitKnown)
                {
                    _logger.LogInformation("Found previously processed article on page {Page}, stopping crawl", page);
                    stopCrawling = true;
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        var unique = discovered.DistinctBy(a => a.Url).ToList();
        _logger.LogInformation("Discovery complete. {Total} unique URLs found", unique.Count);
        return unique;
    }

    private async Task<(List<DiscoveredArticle> Results, bool HitKnownArticle)> CrawlListingPageAsync(string path, string resourceType)
    {
        var results = new List<DiscoveredArticle>();

        try
        {
            var html = await _httpClient.GetStringAsync(path);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var articleLinks = doc.DocumentNode.SelectNodes("//a[@href]");
            if (articleLinks is null) return (results, false);

            foreach (var link in articleLinks)
            {
                var href = link.GetAttributeValue("href", string.Empty);
                if (!IsResourceUrl(href)) continue;

                var absoluteUrl = href.StartsWith("http")
                    ? href
                    : $"https://www.desiringgod.org{href}";

                if (await _trackingRepo.ExistsAsync(absoluteUrl, resourceType))
                    return (results, true);

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

        return (results, false);
    }

    private static bool IsResourceUrl(string href)
    {
        if (string.IsNullOrWhiteSpace(href)) return false;
        if (href.StartsWith("http") && !href.Contains("desiringgod.org")) return false;
        if (href.Contains('#') || href.Contains("mailto:")) return false;
        if (href.Contains("/articles/all")) return false;

        return href.Contains("/articles/");
    }
}
