using System;
using System.Collections.Generic;
using System.IO;
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
                    System.Console.WriteLine($"Step type '{typeName}' not found or unable to be loaded.");
                    continue;
                }

                if (!typeof(IEngineStep).GetTypeInfo().IsAssignableFrom(stepType))
                {
                    readingStepFailed = true;
                    System.Console.WriteLine($"Step type '{stepType.AssemblyQualifiedName}' not a valid '{nameof(IEngineStep)}'.");
                }

                if (stepType.GetTypeInfo().GetConstructor(Type.EmptyTypes) == null)
                {
                    readingStepFailed = true;
                    System.Console.WriteLine($"Step type '{stepType.AssemblyQualifiedName}' doesn't provide a parameterless constructor.");
                }

                var stepInstance = (IEngineStep)Activator.CreateInstance(stepType);

                if (!string.IsNullOrEmpty(stepInstance.Name))
                {
                    readingStepFailed = true;
                    System.Console.WriteLine($"Step type '{stepType.AssemblyQualifiedName}' doesn't provide a name.");
                }

                engineSteps.Add(stepInstance);
            }
            
            if (readingStepFailed)
            {
                Environment.Exit(1);
            }

            return new EngineConfiguration(engineSteps);
        }
    }
}
