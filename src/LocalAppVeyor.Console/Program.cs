using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LocalAppVeyor.Configuration.Model;
using LocalAppVeyor.Configuration.Reader;
using LocalAppVeyor.Pipeline;
using LocalAppVeyor.Pipeline.Output;
using Microsoft.Extensions.Configuration;

namespace LocalAppVeyor.Console
{
    public class Program
    {
        private static readonly IPipelineOutputter PipelineOutputter = new ConsoleOutputter();

        public static void Main(string[] args)
        {
            var engineConfiguration = TryGetEngineConfigurationOrTerminate(@"C:\Users\JoaoP\Desktop\app");
            var buildConfiguration = TryGetBuildConfigurationOrTerminate(engineConfiguration.RepositoryDirectoryPath);

            var engine = new Engine(
                engineConfiguration,
                buildConfiguration,
                PipelineOutputter);

            engine.Start();

            System.Console.Read();
        }

        private static BuildConfiguration TryGetBuildConfigurationOrTerminate(string repositoryPath)
        {
            var appVeyorYml = Path.Combine(repositoryPath, "appveyor.yml");

            if (!File.Exists(appVeyorYml))
            {
                PipelineOutputter.WriteError($"AppVeyor.yml file not found on repository path. Build aborted.");
                Environment.Exit(1);
            }

            BuildConfiguration configuration = null;

            try
            {
                configuration = new BuildConfigurationYamlReader(@"C:\Users\JoaoP\Desktop\app\appveyor.yml")
                    .GetBuildConfiguration();
            }
            catch (LocalAppVeyorException)
            {
                PipelineOutputter.WriteError($"Error while parsing '{appVeyorYml}' file.");
                PipelineOutputter.WriteError("Build aborted.");
                Environment.Exit(1);
            }

            return configuration;
        }

        private static EngineConfiguration TryGetEngineConfigurationOrTerminate(string repositoryPath)
        {
            // Try infer repository path if one is not provided
            if (!string.IsNullOrEmpty(repositoryPath))
            {
                if (!Directory.Exists(repositoryPath))
                {
                    PipelineOutputter.WriteError($"Repository directory '{repositoryPath}' not found. Build aborted.");
                    Environment.Exit(1);
                }
                else
                {
                    PipelineOutputter.Write($"Using '{repositoryPath}' as the repository path.");
                }
            }
            else
            {
                PipelineOutputter.Write($"Current directory '{repositoryPath}' will be used as repository path.");
                repositoryPath = Directory.GetCurrentDirectory();
            }

            // Try read config file if exists on console path (for steps, etc.)
#if NET451
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
#else
            var exePath = AppContext.BaseDirectory;
#endif
            
            var configFile = Path.Combine(exePath, "LocalAppVeyor.Console.config.json");

            if (!File.Exists(configFile))
            {
                return new EngineConfiguration(repositoryPath, new IEngineStep[0]);
            }

            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile(configFile)
                .Build();

            var stepsAssemblyNames = new List<string>();
            configurationRoot.GetSection("Steps").Bind(stepsAssemblyNames);

            var readingStepFailed = false;
            var engineSteps = new List<IEngineStep>();

            foreach (string typeName in stepsAssemblyNames)
            {
                readingStepFailed = false;

                var stepType = Type.GetType(typeName);

                if (stepType == null)
                {
                    readingStepFailed = true;
                    PipelineOutputter.WriteError($"Step type '{typeName}' not found or unable to be loaded.");
                    continue;
                }

                if (!typeof(IEngineStep).GetTypeInfo().IsAssignableFrom(stepType))
                {
                    readingStepFailed = true;
                    PipelineOutputter.WriteError($"Step type '{stepType.AssemblyQualifiedName}' not a valid '{nameof(IEngineStep)}'.");
                }

                if (stepType.GetTypeInfo().GetConstructor(Type.EmptyTypes) == null)
                {
                    readingStepFailed = true;
                    PipelineOutputter.WriteError($"Step type '{stepType.AssemblyQualifiedName}' doesn't provide a parameterless constructor.");
                }

                var stepInstance = (IEngineStep)Activator.CreateInstance(stepType);

                if (!string.IsNullOrEmpty(stepInstance.Name))
                {
                    readingStepFailed = true;
                    PipelineOutputter.WriteError($"Step type '{stepType.AssemblyQualifiedName}' doesn't provide a name.");
                }

                engineSteps.Add(stepInstance);
            }
            
            if (readingStepFailed)
            {
                Environment.Exit(1);
            }

            // 

            return new EngineConfiguration(repositoryPath, engineSteps);
        }
    }
}
