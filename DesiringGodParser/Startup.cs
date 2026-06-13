using Amazon.Lambda.Annotations;
using DesiringGodParser.Services;
using DesiringGodParser.Repos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DesiringGodParser;
using Amazon.Extensions.NETCore.Setup;

[Amazon.Lambda.Annotations.LambdaStartup]
public class Startup
{
    /// <summary>
    /// Services for Lambda functions can be registered in the services dependency injection container in this method. 
    ///
    /// The services can be injected into the Lambda function through the containing type's constructor or as a
    /// parameter in the Lambda function using the FromService attribute. Services injected for the constructor have
    /// the lifetime of the Lambda compute container. Services injected as parameters are created within the scope
    /// of the function invocation.
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {

        services.AddDefaultAWSOptions(new AWSOptions
        {
            Region = Amazon.RegionEndpoint.USEast1
        });

        services.AddSingleton<ILoggerFactory, LoggerFactory>();
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
