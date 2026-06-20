using ArticleParser.Parsers;
using Common.Constants;
using Common.Models;
using Microsoft.Extensions.Logging;


namespace ArticleParser;

public class ResourceParserFactory : IResourceParserFactory
{
    private readonly ILoggerFactory _loggerFactory;

    private readonly ILogger _logger;
    

    public ResourceParserFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger(nameof(ResourceParserFactory));
    }
    
    
    public IResourceParser CreateResourceParser(DiscoveredArticle resource)
    {
        string resourceType = resource.ResourceType;
        string resourceSource = resource.ResourceSource;

        _logger.LogInformation("Searching for parser for Resource Type {resourceType}, Resource Source {resourceSource}", resourceType, resourceSource);

        if (resourceType == ResourceTypes.Article && resourceSource == ResourceSources.DesiringGod)
        {
            return new Parsers.DesiringGodArticleParser(_loggerFactory.CreateLogger((nameof(DesiringGodArticleParser))));
        }


        throw new NotImplementedException($"No parser implemented for Type: {resourceType}, Source: {resourceSource}");
    }
}