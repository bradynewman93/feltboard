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
    
    private readonly ILogger _logger;
    private readonly IArticleDiscoveredHandler _articleDiscoveredHandler;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public Functions(
        ILoggerFactory loggerFactory,
        IArticleDiscoveredHandler articleDiscoveredHandler
        )
    {
        _logger = loggerFactory.CreateLogger(nameof(ArticleParser));
        _articleDiscoveredHandler = articleDiscoveredHandler;
        
    }

    [LambdaFunction]
    public async Task Default(SQSEvent sqsEvent)
    {
        foreach (var record in sqsEvent.Records)
        {
            var article = JsonSerializer.Deserialize<DiscoveredArticle>(record.Body, _jsonOptions)
                ?? throw new InvalidOperationException($"Failed to deserialize SQS message: {record.Body}");

            _logger.LogInformation("Parsing article for url {Url}", article.ResourceUrl);
            
            await _articleDiscoveredHandler.Handler(article);
            
            _logger.LogInformation("Completed parsing resource for url {Url}", article.ResourceUrl);
        }
    }
}
