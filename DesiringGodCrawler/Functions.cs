using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;
using AWS.Lambda.Powertools.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DesiringGodCrawler;

public class Functions
{
    private readonly ILogger _logger;
    private readonly IDesiringGodArticleDiscoverer _articleDiscoverer;
    

    public Functions(ILoggerFactory loggerFactory, IDesiringGodArticleDiscoverer articleDiscoverer)
    {
        _logger = loggerFactory.CreateLogger(nameof(DesiringGodCrawler));
        _articleDiscoverer = articleDiscoverer;
    }

    [LambdaFunction,Logging]
    public async Task Default()
    {
        _logger.LogInformation("Starting DesiringGod Discoverer at {time}", DateTime.UtcNow);
        await _articleDiscoverer.DiscoverAsync();
        
        _logger.LogInformation("Finished DesiringGod Discoverer at {time}", DateTime.UtcNow);
    }
}
