using Endpoint.SharedKernal.ValueObjects;
using System.Collections.Generic;
using System.IO;

namespace Endpoint.SharedKernal.Models
{
    public class Settings
    {
        public bool IsRoot { get; set; } = true;
        public bool IsMicroserviceArchitecture { get; set; } = true;
        public List<string> Projects { get; set; } = new List<string>();
        public string RootDirectory { get; set; }
        public string SolutionName { get; set; }
        public string SolutionFileName { get; set; }
        public string ApiFullPath { get; set; }
        public string RootNamespace { get; set; }
        public string DomainNamespace { get; set; }
        public string ApplicationNamespace { get; set; }
        public string InfrastructureNamespace { get; set; }
        public string ApiNamespace { get; set; }
        public string DomainDirectory { get; set; }
        public string ApplicationDirectory { get; set; }
        public string InfrastructureDirectory { get; set; }
        public string ApiDirectory { get; set; }
        public string UnitTestsDirectory { get; set; }
        public string TestingDirectory { get; set; }
        public List<string> AppDirectories { get; set; } = new List<string>();
        public string BuildingBlocksCoreNamespace { get; set; } = "BuildingBlocks.Core";
        public string BuildingBlocksEventStoreNamespace { get; set; } = "BuildingBlocks.EventStore";
        public string SourceFolder { get; set; } = "src";
        public string TestFolder { get; set; } = "tests";
        public string DbContextName { get; set; }
        public int Port { get; set; } = 5000;
        public int SslPort { get; set; } = 5001;

        public List<string> Resources { get; set; } = new List<string>();

        public Settings(string name, string dbContextName, string resource, string directory, bool isMicroserviceArchitecture = true)
        {
            name = ((Token)name).PascalCase.Replace("-", "_");
            resource = ((Token)resource).PascalCase;
            IsMicroserviceArchitecture = isMicroserviceArchitecture;

            SolutionName = name;
            SolutionFileName = $"{name}.sln";


            var parts = name.Split('.');
            DbContextName = dbContextName ?? $"{parts[parts.Length - 1]}DbContext";

            Resources.Add(resource);
            RootDirectory = $"{directory}{Path.DirectorySeparatorChar}{SolutionName}";
            RootNamespace = SolutionName;
            ApiNamespace = $"{RootNamespace}.Api";
            ApiFullPath = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}{ApiNamespace}{Path.DirectorySeparatorChar}{ApiNamespace}.csproj";
            InfrastructureNamespace = IsMicroserviceArchitecture ? $"{RootNamespace}.Api" : $"{RootNamespace}.Infrastructure";
            DomainNamespace = IsMicroserviceArchitecture ? $"{RootNamespace}.Api" : $"{RootNamespace}.SharedKernal";
            ApplicationNamespace = IsMicroserviceArchitecture ? $"{RootNamespace}.Api" : $"{RootNamespace}.Core";

            var sourceFolder = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}";
            var testsFolder = $"{RootDirectory}{Path.DirectorySeparatorChar}{TestFolder}{Path.DirectorySeparatorChar}";

            if (IsMicroserviceArchitecture)
            {
                ApiDirectory = $"{sourceFolder}{SolutionName}.Api";
                InfrastructureDirectory = $"{sourceFolder}{SolutionName}.Api";
                DomainDirectory = $"{sourceFolder}{SolutionName}.Api";
                ApplicationDirectory = $"{sourceFolder}{SolutionName}.Api";
            }
            else
            {
                ApiDirectory = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}{SolutionName}.Api";
                InfrastructureDirectory = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}{SolutionName}.Infrastructure";
                DomainDirectory = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}{SolutionName}.SharedKernal";
                ApplicationDirectory = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}{SolutionName}.Core";
            }

            UnitTestsDirectory = $"{testsFolder}{RootNamespace}.UnitTests";
            TestingDirectory = $"{testsFolder}{RootNamespace}.Testing";

        }

        public Settings(string name, string dbContextName, List<string> resources, string directory, bool isMicroserviceArchitecture = true)
        {
            name = ((Token)name).PascalCase.Replace("-", "_");

            foreach(var resource in resources)
            {
                Resources.Add(((Token)resource).PascalCase);
            }

            IsMicroserviceArchitecture = isMicroserviceArchitecture;

            SolutionName = name;
            SolutionFileName = $"{name}.sln";


            var parts = name.Split('.');
            DbContextName = dbContextName ?? $"{parts[parts.Length - 1]}DbContext";

            
            RootDirectory = $"{directory}{Path.DirectorySeparatorChar}{SolutionName}";
            RootNamespace = SolutionName;
            ApiNamespace = $"{RootNamespace}.Api";
            ApiFullPath = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}{ApiNamespace}{Path.DirectorySeparatorChar}{ApiNamespace}.csproj";
            InfrastructureNamespace = IsMicroserviceArchitecture ? $"{RootNamespace}.Api" : $"{RootNamespace}.Infrastructure";
            DomainNamespace = IsMicroserviceArchitecture ? $"{RootNamespace}.Api" : $"{RootNamespace}.SharedKernal";
            ApplicationNamespace = IsMicroserviceArchitecture ? $"{RootNamespace}.Api" : $"{RootNamespace}.Core";

            var sourceFolder = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}";
            var testsFolder = $"{RootDirectory}{Path.DirectorySeparatorChar}{TestFolder}{Path.DirectorySeparatorChar}";

            if (IsMicroserviceArchitecture)
            {
                ApiDirectory = $"{sourceFolder}{SolutionName}.Api";
                InfrastructureDirectory = $"{sourceFolder}{SolutionName}.Api";
                DomainDirectory = $"{sourceFolder}{SolutionName}.Api";
                ApplicationDirectory = $"{sourceFolder}{SolutionName}.Api";
            }
            else
            {
                ApiDirectory = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}{SolutionName}.Api";
                InfrastructureDirectory = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}{SolutionName}.Infrastructure";
                DomainDirectory = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}{SolutionName}.SharedKernal";
                ApplicationDirectory = $"{RootDirectory}{Path.DirectorySeparatorChar}{SourceFolder}{Path.DirectorySeparatorChar}{SolutionName}.Core";
            }

            UnitTestsDirectory = $"{testsFolder}{RootNamespace}.UnitTests";
            TestingDirectory = $"{testsFolder}{RootNamespace}.Testing";

        }

        public Settings()
        {

        }

    }
}