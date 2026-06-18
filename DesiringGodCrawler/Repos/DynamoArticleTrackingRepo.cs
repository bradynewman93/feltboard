using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Constants;
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

    public async Task SaveAsync(string url, string source, string status, DateTimeOffset processedAt)
    {
        await _dynamo.PutItemAsync(new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "resourceUrl", new AttributeValue(url) },
                { "source", new AttributeValue(source) },
                { "processingStatus", new AttributeValue(status) },
                { "dateProcessed", new AttributeValue(processedAt.ToString("O")) }
            }
        });
    }
}
