using Amazon.Lambda.Annotations;
using Amazon.SQS;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Amazon.Extensions.NETCore.Setup;
using AWS.Lambda.Powertools.Logging;
using Common.Repos;
using DesiringGodCrawler.Repos;
using DesiringGodCrawler.Services;

namespace DesiringGodCrawler;

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
        services.AddSingleton<IArticleTrackingRepository, DynamoArticleTrackingRepo>();
        services.AddLogging(b => b.AddPowertoolsLogger());
        services.AddHttpClient<IDesiringGodArticleDiscoverer, DesiringGodArticleDiscoverer>(client =>
        {
            client.BaseAddress = new Uri("https://www.desiringgod.org");
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (compatible; ResearchBot/1.0)");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddAWSService<IAmazonSQS>();
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddSingleton<IArticleTrackingRepository, DynamoArticleTrackingRepo>();
        //services.AddSingleton<IDesiringGodDiscoverer, DesiringGodDiscoverer>();


    }
}
