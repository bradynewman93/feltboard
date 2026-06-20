using System.Diagnostics;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Constants;
using Common.Models;
using Common.Repos;
using Common.Util;

namespace DesiringGodCrawler.Repos;

public class DynamoArticleTrackingRepo : IArticleTrackingRepository
{
    private readonly IAmazonDynamoDB _dynamo;
    private readonly string _tableName;

    public DynamoArticleTrackingRepo(IAmazonDynamoDB dynamo)
    {
        _dynamo = dynamo;
        _tableName = EnvironmentUtil.EnsureEnvVariable(EnvVariables.TableName);
    }

    public async Task<bool> ExistsAsync(string url, string source)
    {
        var response = await _dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "resourceUrl", new AttributeValue(url) },
                { "source", new AttributeValue(source) }
            },
            ProjectionExpression = "resourceUrl"
        });

        return response.IsItemSet;
    }

    public async Task<DiscoveredArticle> SaveAsync(DiscoveredArticle article)
    {
        await _dynamo.PutItemAsync(new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "resourceUrl", new AttributeValue(article.ResourceUrl) },
                { "source", new AttributeValue(article.ResourceSource) },
                { "processingStatus", new AttributeValue(article.ProcessingStatus) },
                { "resourceType", new AttributeValue(article.ResourceType) },
                { "dateDiscovered", new AttributeValue(article.DiscoveredAt.ToString("O")) },
            }
        });

        return article;
    }
}
