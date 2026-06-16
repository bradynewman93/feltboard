using Amazon.S3;
using Amazon.S3.Model;
using Common.Util;
using Common.Constants;
using DesiringGodParser.Models;
using System.Text.Json;

namespace DesiringGodParser.Repos;

public class S3ArticleRepo : IArticleRepository
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3ArticleRepo(IAmazonS3 s3)
    {
        _s3Client = s3;
        _bucketName = EnvironmentUtil.EnsureEnvVariable(EnvVariables.S3BucketName);
    }

    public async Task<ParsedArticle> SaveArticle(ParsedArticle article)
    {

        string slug = ExtractSlug(article.Url);
        var key = $"articles/{article.Source}/{slug}.json";


        var json = JsonSerializer.Serialize(article, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            ContentBody = json,
            ContentType = "application/json"
        };

        await _s3Client.PutObjectAsync(request);

        article.Key = slug;
        return article;
    }

    private static string ExtractSlug(string url)
    {
        var path = new Uri(url).AbsolutePath.TrimEnd('/');
        var slug = path.Split('/').Last();
        return Path.GetFileNameWithoutExtension(slug);
    }
}