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
        HtmlDocument resourceHtml = await _resourceFetcher.GetArticleHtml(discoveredResouce.ResourceUrl);
        
        
        IResourceParser parser = _parserFactory.CreateResourceParser(discoveredResouce);
        
        ParsedArticle parsedResource = await parser.ParseResource(discoveredResouce,  resourceHtml);
        
        //save into S3
        
        await _articleKnowledgeBase.PersistResource(parsedResource);
        
        //updated resource to status processed
        
        
    }
}