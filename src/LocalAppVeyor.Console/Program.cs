using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LocalAppVeyor.Configuration.Readers;
using LocalAppVeyor.Pipeline;
using Microsoft.Extensions.Configuration;

namespace LocalAppVeyor.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Remove this when not testing
            Directory.SetCurrentDirectory(@"C:\Users\JoaoP\Desktop\app");

            var engine = new Engine(
                TryGetEngineConfigurationOrTerminate(null),
                new BuildConfigurationYamlReader(@"C:\Users\JoaoP\Desktop\app\appveyor.yml"), 
                new ConsoleOutputter());

            engine.Start();

            System.Console.Read();
        }

        private static EngineConfiguration TryGetEngineConfigurationOrTerminate(string configFileOrDirectoryPath)
        {
            const string baseConfigFile = "LocalAppVeyor.config.json";
            string configFile = null;

            if (string.IsNullOrEmpty(configFileOrDirectoryPath))
            {
#if NET451
                configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, baseConfigFile);
#else
                configFile = Path.Combine(AppContext.BaseDirectory, baseConfigFile);
#endif

                if (!File.Exists(configFile))
                {
                    return EngineConfiguration.Default;
                }
            }
            else if (File.Exists(configFileOrDirectoryPath))
            {
                configFile = configFileOrDirectoryPath;
            }
            else if (Directory.Exists(configFileOrDirectoryPath))
            {
                configFile = Path.Combine(configFileOrDirectoryPath, baseConfigFile);

                if (!File.Exists(configFile))
                {
                    System.Console.WriteLine(
                        $"Invalid LocalAppVeyor console configuration file '{configFileOrDirectoryPath}'.");
                    Environment.Exit(1);
                }
            }

            if (configFile == null)
            {
                System.Console.WriteLine(
                    $"'{configFileOrDirectoryPath}' is not a valid configuration file or directory.");
                Environment.Exit(1);
            }

            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile(configFile)
                .Build();

            var readedSteps = new List<StepConfiguration>();
            configurationRoot.GetSection(nameof(EngineConfiguration.BuildSteps)).Bind(readedSteps);

            var readindStepFailed = false;

            for (var i = 0; i < readedSteps.Count; i++)
            {
                readindStepFailed = false;

                var step = readedSteps[i];

                if (string.IsNullOrEmpty(step.Name))
                {
                    readindStepFailed = true;
                    System.Console.WriteLine($"Step {i} doesn't specify a name.");
                }

                if (string.IsNullOrEmpty(step.TypeName))
                {
                    readindStepFailed = true;
                    System.Console.WriteLine($"Step {i} doesn't specify a type name to be used.");
                }

                if (step.Type == null)
                {
                    readindStepFailed = true;
                    System.Console.WriteLine($"Step type '{step.TypeName}' not found or unable to be loaded.");
                    continue;
                }

                if (!typeof(Step).GetTypeInfo().IsAssignableFrom(step.Type))
                {
                    readindStepFailed = true;
                    System.Console.WriteLine($"Step type '{step.Type.AssemblyQualifiedName}' not a valid '{nameof(Step)}'.");
                }
            }
            
            if (readindStepFailed)
            {
                Environment.Exit(1);
            }

            var steps = new List<StepConfiguration>();

            if (!configurationRoot.GetValue<bool>("SkipAppVeyorSteps"))
            {
                steps = new List<StepConfiguration>
                {
                    new StepConfiguration { Name = "Init", TypeName = "LocalAppVeyor.Pipeline.Steps.AppVeyor.InitStep, LocalAppVeyor" },
                    new StepConfiguration { Name = "CloneFolder", TypeName = "LocalAppVeyor.Pipeline.Steps.AppVeyor.CloneFolderStep, LocalAppVeyor" },
                    new StepConfiguration { Name = "InitEnvironmentVariables", TypeName = "LocalAppVeyor.Pipeline.Steps.AppVeyor.InitEnvironmentVariablesStep, LocalAppVeyor" },
                    new StepConfiguration { Name = "Install", TypeName = "LocalAppVeyor.Pipeline.Steps.AppVeyor.InstallStep, LocalAppVeyor" },
                    new StepConfiguration { Name = "BeforeBuild", TypeName = "LocalAppVeyor.Pipeline.Steps.AppVeyor.BeforeBuildStep, LocalAppVeyor" },
                    //new StepConfiguration { Name = "Build", TypeName = "LocalAppVeyor.Pipeline.Steps.AppVeyor.BuildStep, LocalAppVeyor" }
                };
            }

            foreach (var readedStep in readedSteps)
            {
                if (!string.IsNullOrEmpty(readedStep.Before))
                {
                    var index = steps.FindIndex(s => s.Name == readedStep.Before);

                    if (index >= 0)
                    {
                        steps.Insert(index, readedStep);
                    }
                    else
                    {
                        steps.Add(readedStep);
                    }
                }
                else if (!string.IsNullOrEmpty(readedStep.After))
                {
                    var index = steps.FindIndex(s => s.Name == readedStep.After);

                    if (index >= 0 && index < steps.Count)
                    {
                        steps.Insert(++index, readedStep);
                    }
                    else
                    {
                        steps.Add(readedStep);
                    }
                }
                else
                {
                    steps.Add(readedStep);
                }
            }

            return new EngineConfiguration(
                steps.Select(s => new Pipeline.StepConfiguration(s.Name, s.Type, s.ContinueOnFail)));
        }
    }
}
