using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Microsoft.Extensions.Logging;
using DesiringGodParser.Services;
using DesiringGodParser.Models;
using DesiringGodParser.Repos;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DesiringGodParser;

/// <summary>
/// A collection of sample Lambda functions that provide a REST api for doing simple math calculations. 
/// </summary>
public class Functions
{

    private readonly IArticleRetriever _articleRetriever;

    private readonly IArticleParser _articleParser;

    private readonly IArticleRepository _articleRepo;


    private readonly ILogger _logger;

    public Functions(
     ILoggerFactory loggerFactory,
     IArticleParser articleParser,
     IArticleRepository articleRepo,
     IArticleRetriever articleRetriever)
    {
       _logger = loggerFactory.CreateLogger(nameof(DesiringGodParser));
        _articleParser = articleParser;
        _articleRepo = articleRepo;
        _articleRetriever = articleRetriever;
    }

    [LambdaFunction]
    public async Task Default(string articleUrl)
    {
        _logger.LogInformation("parsing article for url {articleUrl}",articleUrl);

        string articleHtml = await _articleRetriever.GetArticleHtml(articleUrl);

        ParsedArticle parsedArticle = _articleParser.ParseArticleHtml(articleUrl, articleHtml);

        await _articleRepo.SaveArticle(parsedArticle);

    }

}