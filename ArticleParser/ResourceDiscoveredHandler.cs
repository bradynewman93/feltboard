using ArticleParser.Models;
using ArticleParser.Parsers;
using ArticleParser.Services;
using Common.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace ArticleParser;

public class ResourceDiscoveredHandler : IArticleDiscoveredHandler
{
    
    private readonly IResourceParserFactory _parserFactory;

    private readonly ILogger _logger;
    
    private readonly IArticleKnowledgeBase _articleKnowledgeBase;
    
    private readonly IResourceFetcher _resourceFetcher;

    public ResourceDiscoveredHandler(
        ILoggerFactory loggerFactory,
        IResourceParserFactory parserFactory,
        IArticleKnowledgeBase articleKnowledgeBase,
        IResourceFetcher resourceFetcher
    )
    {
        _logger =  loggerFactory.CreateLogger(nameof(ResourceDiscoveredHandler));
        _parserFactory = parserFactory;
        _articleKnowledgeBase = articleKnowledgeBase;
        _resourceFetcher = resourceFetcher;
    }
    
    public async Task Handler(DiscoveredArticle discoveredResouce)
    {
        //check if resource has been handled already or is being handled
        
        //fetch html
        HtmlDocument resourceHtml = await _resourceFetcher.GetArticleHtml(discoveredResouce.ResourceUrl);
        
        //build parser from factory
        
        
        //parse article
        IResourceParser parser = _parserFactory.CreateResourceParser(discoveredResouce);
        
        ParsedArticle parsedResource = await parser.ParseResource(discoveredResouce,  resourceHtml);
        
        //save into S3
        
        //updated resource to status processed
        
        throw new NotImplementedException();
    }
}