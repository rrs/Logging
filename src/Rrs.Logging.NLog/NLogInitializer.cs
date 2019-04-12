using NLog;
using NLog.Config;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Xml;

namespace Rrs.Logging.NLog
{
    public static class NLogInitializer
    {
        public const string DefaultConfigName = "NLog.config";

        public static void InitializeNLogFromResources(Assembly assembly = null, string nlogResourceName = DefaultConfigName, bool preferFileSystemConfigFile = true, LogLevel overrideLogLevel = null)
        {
            if (preferFileSystemConfigFile && LogManager.Configuration != null) return;
            using (var stream = FindResourceInSpecifiedAssembly(assembly ?? Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly(), nlogResourceName))
            {
                LoadNLogConfigFromStream(stream);
            }

            if (overrideLogLevel != null)
            {
                SetMinLogLevel(overrideLogLevel);
            }
        }

        private static Stream FindResourceInSpecifiedAssembly(Assembly assembly, string nlogResourceName)
        {
            var resourceStream = assembly.GetManifestResourceStreamFromName(nlogResourceName);

            // if the resource stream is null load the default from this project
            return resourceStream ?? Assembly.GetExecutingAssembly().GetManifestResourceStreamFromName(DefaultConfigName);
        }


        private static void LoadNLogConfigFromStream(Stream stream)
        {
            if (stream == null) throw new MissingManifestResourceException();
            using (var reader = new XmlTextReader(stream))
            {
                var configuration = new XmlLoggingConfiguration(reader, null);
                LogManager.Configuration = configuration;
            }
        }

        private static Stream FindResourceInOtherAssemblies(string resourceName)
        {
            var entryAssemblyName = new AssemblyName(Assembly.GetEntryAssembly().GetName().Name);

            var assebliesList = new List<AssemblyName> { entryAssemblyName };

            assebliesList.AddRange(Assembly.GetEntryAssembly().GetReferencedAssemblies());

            var resourceStream = assebliesList.Where(assemblyName => assemblyName.Name != Assembly.GetExecutingAssembly().GetName().Name)
                    .Select(Assembly.Load)
                    .Select(assembly => assembly.GetManifestResourceStreamFromName(resourceName))
                    .FirstOrDefault(stream => stream != null);

            // if the resource stream is still null load the default from this project
            return resourceStream ?? Assembly.GetExecutingAssembly().GetManifestResourceStreamFromName(DefaultConfigName);
        }

        public static void SetMinLogLevel(LogLevel logLevel)
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                rule.EnableLoggingForLevels(logLevel, LogLevel.Fatal);
            }

            LogManager.ReconfigExistingLoggers();
        }
    }
}
