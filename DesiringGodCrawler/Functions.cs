using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using Common.Constants;
using Common.Repos;
using Common.Util;
using Common.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DesiringGodCrawler;

public class Functions
{
    private readonly ILogger _logger;
    private readonly IAmazonSQS _sqsClient;
    private readonly IDesiringGodDiscoverer _discoverer;
    private readonly IArticleTrackingRepository _trackingRepo;
    private readonly string _queueUrl;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public Functions(ILoggerFactory loggerFactory, IAmazonSQS sqsClient, IDesiringGodDiscoverer discoverer, IArticleTrackingRepository trackingRepo)
    {
        _logger = loggerFactory.CreateLogger(nameof(DesiringGodCrawler));
        _sqsClient = sqsClient;
        _discoverer = discoverer;
        _trackingRepo = trackingRepo;
        _queueUrl = EnvironmentUtil.EnsureEnvVariable(EnvVariables.QueueUrl);
    }

    [LambdaFunction]
    public async Task Default()
    {
        _logger.LogInformation("Starting DesiringGod Discoverer at {time}", DateTime.UtcNow);

        var articles = await _discoverer.DiscoverAsync();

        await EnqueueDiscoveredArticles(articles);

        _logger.LogInformation("Successfully enqueued {count} articles for parsing", articles.Count);
    }

    private async Task EnqueueDiscoveredArticles(List<DiscoveredArticle> articles)
    {
        foreach (var article in articles)
        {
            var response = await _sqsClient.SendMessageAsync(new SendMessageRequest
            {
                DelaySeconds = Random.Shared.Next(1,101),
                QueueUrl = _queueUrl,
                MessageBody = JsonSerializer.Serialize(article, _jsonOptions)
            });

            _logger.LogInformation("Enqueued article {Url} with message ID {MessageId}", article.Url, response.MessageId);

            await _trackingRepo.SaveAsync(article.Url, article.ResourceType, "QUEUED", DateTimeOffset.UtcNow);
        }
    }
}
