using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using Amazon.CDK;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Constructs;

namespace Infrastructure
{
    public class InfrastructureStack : Stack
    {
        internal InfrastructureStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            string appEnv = ContextDataHelper.GetAppEnvironment(scope);

            var desiringGodCrawlerFunction = new Function(this, $"{appEnv}-DesiringGodCrawler", new FunctionProps()
            {
                Runtime = Runtime.DOTNET_8,
                Handler = "WebCrawler::WebCrawler.DesiringGodCrawler_Default_Generated::Default",
                Code = Code.FromAsset("../WebCrawler/bin/Release/net8.0/linux-x64"),
                MemorySize = 512,
                Timeout = Duration.Minutes(5),
                Environment = new Dictionary<string, string>
                {

                },
                Description = "Crawls Desiring God for article URLs and enqueues them for processing"
            });

            new Rule(this, "DesiringGodCrawlerSchedule", new RuleProps
            {
                Schedule = Schedule.Cron(new CronOptions
                {
                    Minute = "0",
                    Hour = "8",
                    WeekDay = "MON",
                    Month = "*",
                    Year = "*"
                }),
                Targets = new IRuleTarget[]
                {
                    new LambdaFunction(desiringGodCrawlerFunction)
                },
                Description = "Triggers the Desiring God crawler every Monday at 8am UTC"
            });
        }
    }
}
