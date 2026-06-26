using System.Text.Json;
using Microsoft.Extensions.Logging;
using Amazon.S3;
using Amazon.S3.Model;
using ArticleParser.Models;
using Common.Constants;
using Common.Util;

namespace ArticleParser.Services;

public class S3KnowledgeBase : IArticleKnowledgeBase
{
    private readonly IAmazonS3 _s3Client;

    private readonly ILogger _logger;

    private readonly string _s3BucketName;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {

    };
    
    public S3KnowledgeBase(
        IAmazonS3 s3Client,
        ILoggerFactory loggerFactory
    )
    {
        _logger = loggerFactory.CreateLogger<S3KnowledgeBase>();
        _s3Client = s3Client;
        _s3BucketName = EnvironmentUtil.EnsureEnvVariable(EnvVariables.KnowledgeBaseBucketName);
    }
    
    public async Task PersistResource(ParsedArticle parsedResource)
    {
        string urlSlug = UrlHelper.RetrieveUrlSlug(parsedResource.Url);
        string filePath = "/" + parsedResource.Source + "/" + parsedResource.ResourceType + "/" + urlSlug + ".json";

        
        var putObjectRequest = new PutObjectRequest()
        {
            ContentBody = JsonSerializer.Serialize(parsedResource, _jsonSerializerOptions),
            BucketName = _s3BucketName,
            Key = filePath
        };

        var putResult = await _s3Client.PutObjectAsync(putObjectRequest);


        if (putResult.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            _logger.LogError("Unsuccessful put result for S3 {articleUrl} Error Message {}", parsedResource.Url, putResult.HttpStatusCode);
            throw new Exception(
                $"Unsuccessful put result for S3 {parsedResource.Url} Error Message {putResult.HttpStatusCode}");
        }
        
        
    }
}