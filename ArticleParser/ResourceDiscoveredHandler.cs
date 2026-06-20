using ArticleParser.Services;
using Common.Models;
using Microsoft.Extensions.Logging;

namespace ArticleParser;

public class ResourceDiscoveredHandler : IArticleDiscoveredHandler
{
    
    private readonly IResourceParserFactory _parserFactory;

    private readonly ILogger _logger;
    
    private readonly IArticleKnowledgeBase _articleKnowledgeBase;

    public ResourceDiscoveredHandler(
        ILoggerFactory loggerFactory,
        IResourceParserFactory parserFactory,
        IArticleKnowledgeBase articleKnowledgeBase
    )
    {
        _logger =  loggerFactory.CreateLogger(nameof(ResourceDiscoveredHandler));
        _parserFactory = parserFactory;
        _articleKnowledgeBase = articleKnowledgeBase;
    }
    
    public Task ParseResource(DiscoveredArticle discoveredResouce)
    {
        throw new NotImplementedException();
    }
}