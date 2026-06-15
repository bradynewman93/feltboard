using Amazon.Lambda.Annotations;
using DesiringGodParser.Services;
using DesiringGodParser.Repos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Amazon.Extensions.NETCore.Setup;
using System.Net.Http;
using System.Net;
using Amazon.S3;

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
         "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
     client.DefaultRequestHeaders.Add("Accept",
         "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
     client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
     client.Timeout = TimeSpan.FromSeconds(30);
 })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.GZip
                               | DecompressionMethods.Deflate
                               | DecompressionMethods.Brotli
    });

        services.AddSingleton<IArticleParser, DesiringGodArticleParser>();
        services.AddSingleton<IArticleRepository, StubRepository>();
        services.AddAWSService<IAmazonS3>();


    }
}
