using Amazon.CDK;

namespace Infrastructure
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            _ = new InfrastructureStack(app, "FeltBoard", new StackProps
            {
                Env = new Environment
                {
                    Region = "us-east-1"
                }
            });
            app.Synth();
        }
    }
}
