using Amazon.Lambda.Annotations;
using DesiringGodParser.Services;
using DesiringGodParser.Repos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Amazon.Extensions.NETCore.Setup;

namespace DesiringGodParser;

[Amazon.Lambda.Annotations.LambdaStartup]
public class Startup
{

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddDefaultAWSOptions(new AWSOptions
        {
            Region = Amazon.RegionEndpoint.USEast1
        });

        //services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddLogging(b => b.AddConsole());
        services.AddHttpClient<IArticleRetriever, DesiringGodRetriever>(client =>
        {
            client.BaseAddress = new Uri("https://www.desiringgod.org");
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (compatible; ResearchBot/1.0)");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddSingleton<IArticleParser, DesiringGodArticleParser>();
        services.AddSingleton<IArticleRepository, StubRepository>();

    }
}
