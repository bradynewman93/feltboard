using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Microsoft.Extensions.Logging;
using Amazon.SQS;
using Amazon.SQS.Model;
using Common.Util;
using System.Security;
using WebCrawler.Models;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WebCrawler;

/// <summary>
/// A collection of sample Lambda functions that provide a REST api for doing simple math calculations. 
/// </summary>
public class DesiringGodCrawler
{

    private readonly ILogger _logger;

    private readonly IAmazonSQS _sqsClient;

    private readonly IDesiringGodDiscoverer _discoverer;
    private readonly string _queueUrl;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public DesiringGodCrawler(ILoggerFactory loggerFactory, IAmazonSQS sqsClient, IDesiringGodDiscoverer discoverer)
    {
        _logger = loggerFactory.CreateLogger(nameof(DesiringGodCrawler));
        _sqsClient = sqsClient;
        _discoverer = discoverer;
        _queueUrl = "test";//"EnvironmentUtil.EnsureEnvVariable(EnvVariables.QueueUrl);
    }

    [LambdaFunction]
    public async Task<List<DiscoveredArticle>> Default()
    {
        _logger.LogInformation("Starting DesiringGod Discoverer at {time}", DateTime.UtcNow);

        List<DiscoveredArticle> result = new();
        result.AddRange(await _discoverer.DiscoverAsync());

        _logger.LogInformation("Successfully enqueued {count} articles for parsing", result.Count);

        return result;

        //await EnqueueDiscoveredArticles(articles);


    }

    private async Task EnqueueDiscoveredArticles(List<DiscoveredArticle> articles)
    {
        const int batchSize = 10;

        for (var i = 0; i < articles.Count; i += batchSize)
        {
            var batch = articles.Skip(i).Take(batchSize).ToList();

            var request = new SendMessageBatchRequest
            {
                QueueUrl = _queueUrl,
                Entries = batch.Select((article, idx) => new SendMessageBatchRequestEntry
                {
                    Id = idx.ToString(),
                    MessageBody = JsonSerializer.Serialize(article, _jsonOptions)
                }).ToList()
            };

            var response = await _sqsClient.SendMessageBatchAsync(request, CancellationToken.None);

            if (response.Failed.Count > 0)
            {
                foreach (var failure in response.Failed)
                {
                    _logger.LogError("Failed to enqueue message {Id}: {Message}", failure.Id, failure.Message);
                }
            }
        }
    }

}