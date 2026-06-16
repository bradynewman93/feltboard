using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new InfrastructureStack(app, "FeltBoard", new StackProps
            {
                Env = new Amazon.CDK.Environment
                {
                    Region = "us-east-1"
                }
            });
            app.Synth();
        }
    }
}
