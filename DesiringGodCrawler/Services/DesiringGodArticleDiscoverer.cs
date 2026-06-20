using Common.Repos;
using Common.Constants;
using Common.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Amazon.SQS;
using Amazon.SQS.Model;
using Common.Util;
using System.Text.Json;

namespace DesiringGodCrawler.Services;

public class DesiringGodArticleDiscoverer : IDesiringGodArticleDiscoverer
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly IArticleTrackingRepository _trackingRepo;
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl;

    private const string BaseArticlePath = "/articles/all";

    private const int MaxPages = 5;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public DesiringGodArticleDiscoverer(
        HttpClient httpClient, 
        ILoggerFactory loggerFactory, 
        IArticleTrackingRepository trackingRepo,
        IAmazonSQS sqsClient)
    {
        _httpClient = httpClient;
        _logger = loggerFactory.CreateLogger(nameof(DesiringGodArticleDiscoverer));
        _trackingRepo = trackingRepo;
        _sqsClient = sqsClient;
        _queueUrl = EnvironmentUtil.EnsureEnvVariable(EnvVariables.QueueUrl);
    }

    private static readonly (string Path, string ResourceType, int MaxPages)[] ResourceSections =
    [
        ("/articles/all", "article", 5),
    ];

    public async Task DiscoverAsync()
    {
        for (int page = 1; page <= MaxPages; page++)
        {
            List<string> articleLinks = await FetchArticleLinks(page);
            
            QueueArticlesForProcessing(articleLinks);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        
    }

    private async Task<List<string>> FetchArticleLinks(int page)
    {
        var url = page == 1 ? BaseArticlePath : $"{BaseArticlePath}?page={page}";

        List<string> articleLinks = new();
        
        HtmlDocument doc;
        try
        {
            var html = await _httpClient.GetStringAsync(url);
            doc = new HtmlDocument();
            doc.LoadHtml(html);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error crawling {Path}", url);
            return articleLinks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error crawling {Path}", url);
            return articleLinks;
        }
        
        var possibleLinks = doc.DocumentNode.SelectNodes("//a[@href]");
        if (possibleLinks.Count == 0) return articleLinks;

        foreach (var link in possibleLinks)
        {
            string href = link.GetAttributeValue("href", string.Empty);
            if (IsResourceUrl(link.GetAttributeValue("href", string.Empty)))
            {
                var absoluteUrl = href.StartsWith("http")
                    ? href
                    : $"https://www.desiringgod.org{href}";
                
                articleLinks.Add(absoluteUrl);
            }
        }
        
        return articleLinks;
    }


    private async Task QueueArticlesForProcessing(List<string> articleLinks)
    {
        foreach (var articleLink in articleLinks)
        {
            if (await _trackingRepo.ExistsAsync(articleLink, ResourceSources.DesiringGod))
            {
                continue;
            };

            var parsedArticle = new DiscoveredArticle()
            {
                ResourceUrl = articleLink,
                ResourceType = ResourceTypes.Article,
                DiscoveredAt = DateTime.UtcNow,
                ResourceSource = ResourceSources.DesiringGod
            };

            SendMessageRequest sendRequest = new()
            {
                QueueUrl = _queueUrl,
                DelaySeconds = Random.Shared.Next(1,20),
                MessageBody = JsonSerializer.Serialize(parsedArticle, _jsonOptions),
            };

            await _sqsClient.SendMessageAsync(sendRequest);
            _trackingRepo.SaveAsync(parsedArticle);
        }
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