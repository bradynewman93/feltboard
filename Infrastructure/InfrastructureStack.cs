using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.SQS;
using Constructs;

namespace Infrastructure
{
    public class InfrastructureStack : Stack
    {
        internal InfrastructureStack(Construct scope, string id, IStackProps? props = null) : base(scope, id, props)
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

            var articleDlq = new Queue(this, $"{appEnv}-ArticleParserDLQ", new QueueProps
            {
                QueueName = $"{appEnv}-article-parser-dlq",
                RetentionPeriod = Duration.Days(14)
            });

            var articleQueue = new Queue(this, $"{appEnv}-ArticleParserQueue", new QueueProps
            {
                QueueName = $"{appEnv}-article-parser",
                VisibilityTimeout = Duration.Minutes(6),
                DeadLetterQueue = new DeadLetterQueue
                {
                    Queue = articleDlq,
                    MaxReceiveCount = 3
                }
            });

            var crawlerFunction = new Function(this, $"{appEnv}-DesiringGodCrawler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Handler = "DesiringGodCrawler::DesiringGodCrawler.DesiringGodCrawler_Default_Generated::Default",
                Code = Code.FromAsset("../DesiringGodCrawler/bin/Release/net8.0/linux-x64"),
                MemorySize = 512,
                Timeout = Duration.Minutes(5),
                Environment = new Dictionary<string, string>
                {
                    { "TABLE_NAME", articleTable.TableName },
                    { "QUEUE_URL", articleQueue.QueueUrl }
                },
                Description = "Crawls Desiring God for article URLs and enqueues them for processing"
            });

            var parserFunction = new Function(this, $"{appEnv}-ArticleParser", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Handler = "ArticleParser::ArticleParser.Functions_Default_Generated::Default",
                Code = Code.FromAsset("../ArticleParser/bin/Release/net8.0/linux-x64"),
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
            articleQueue.GrantSendMessages(crawlerFunction);

            parserFunction.AddEventSource(new SqsEventSource(articleQueue, new SqsEventSourceProps
            {
                BatchSize = 1
            }));

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
