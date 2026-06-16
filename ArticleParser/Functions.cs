using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using ArticleParser.Models;
using ArticleParser.Repos;
using ArticleParser.Services;
using Common.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ArticleParser;

public class Functions
{
    private readonly IArticleRetriever _articleRetriever;
    private readonly IArticleParser _articleParser;
    private readonly IArticleRepository _articleRepo;
    private readonly ILogger _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public Functions(
        ILoggerFactory loggerFactory,
        IArticleParser articleParser,
        IArticleRepository articleRepo,
        IArticleRetriever articleRetriever)
    {
        _logger = loggerFactory.CreateLogger(nameof(ArticleParser));
        _articleParser = articleParser;
        _articleRepo = articleRepo;
        _articleRetriever = articleRetriever;
    }

    [LambdaFunction]
    public async Task Default(SQSEvent sqsEvent)
    {
        foreach (var record in sqsEvent.Records)
        {
            var article = JsonSerializer.Deserialize<DiscoveredArticle>(record.Body, _jsonOptions)
                ?? throw new InvalidOperationException($"Failed to deserialize SQS message: {record.Body}");

            _logger.LogInformation("Parsing article for url {Url}", article.Url);

            string articleHtml = await _articleRetriever.GetArticleHtml(article.Url);
            ParsedArticle parsedArticle = _articleParser.ParseArticleHtml(article.Url, articleHtml);
            await _articleRepo.SaveArticle(parsedArticle);
        }
    }
}
