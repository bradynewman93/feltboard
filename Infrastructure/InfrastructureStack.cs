using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace Infrastructure
{
    public class InfrastructureStack : Stack
    {
        internal InfrastructureStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            string appEnv = ContextDataHelper.GetAppEnvironment(scope);

            var articleTable = new Table(this, $"{appEnv}-ArticleTrackingTable", new TableProps
            {
                TableName = $"{appEnv}-article-tracking",
                PartitionKey = new Attribute { Name = "url", Type = AttributeType.STRING },
                SortKey = new Attribute { Name = "source", Type = AttributeType.STRING },
                BillingMode = BillingMode.PAY_PER_REQUEST,
                RemovalPolicy = RemovalPolicy.RETAIN
            });

            var crawlerFunction = new Function(this, $"{appEnv}-DesiringGodCrawler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Handler = "WebCrawler::WebCrawler.DesiringGodCrawler_Default_Generated::Default",
                Code = Code.FromAsset("../WebCrawler/bin/Release/net8.0/linux-x64"),
                MemorySize = 512,
                Timeout = Duration.Minutes(5),
                Environment = new Dictionary<string, string>
                {
                    { "TABLE_NAME", articleTable.TableName }
                },
                Description = "Crawls Desiring God for article URLs and enqueues them for processing"
            });

            var parserFunction = new Function(this, $"{appEnv}-DesiringGodParser", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Handler = "DesiringGodParser::DesiringGodParser.Functions_Default_Generated::Default",
                Code = Code.FromAsset("../DesiringGodParser/bin/Release/net8.0/linux-x64"),
                MemorySize = 512,
                Timeout = Duration.Minutes(5),
                Environment = new Dictionary<string, string>
                {
                    { "TABLE_NAME", articleTable.TableName }
                },
                Description = "Parses Desiring God articles and saves them to S3"
            });

            articleTable.GrantReadWriteData(crawlerFunction);
            articleTable.GrantReadWriteData(parserFunction);

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
                Targets = new IRuleTarget[] { new LambdaFunction(crawlerFunction) },
                Description = "Triggers the Desiring God crawler every Monday at 8am UTC"
            });
        }
    }
}
