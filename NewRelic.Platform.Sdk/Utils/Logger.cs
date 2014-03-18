using System;
using System.Globalization;
using System.IO;
using NewRelic.Platform.Sdk.Configuration;
using NLog.Config;
using NLog.Targets;
using NLogger = NLog.Logger;
using NLogLevel = NLog.LogLevel;
using NLogManager = NLog.LogManager;

namespace NewRelic.Platform.Sdk.Utils
{
    public sealed class Logger
    {
        private NLogger nLogger;
        private static bool Configured = false;
        private static object ConfigureLock = new object();

        private Logger(string className)
        {
            this.nLogger = NLogManager.GetLogger(className);
        }

        public void Debug(string format, params object[] items)
        {
            this.nLogger.Debug(format, items);
        }

        public void Info(string format, params object[] items)
        {
            this.nLogger.Info(format, items);
        }

        public void Warn(string format, params object[] items)
        {
            this.nLogger.Warn(format, items);
        }

        public void Error(string format, params object[] items)
        {
            this.nLogger.Error(format, items);
        }

        public void Fatal(string format, params object[] items)
        {
            this.nLogger.Fatal(format, items);
        }

        public static Logger GetLogger(string className)
        {
            return GetLogger(className, null);
        }

        private static void Configure(INewRelicConfig config)
        {
            config = config ?? NewRelicConfig.Instance;
            LoggingConfiguration loggingConfiguration = new LoggingConfiguration();
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget { Name = "Console" };
            loggingConfiguration.AddTarget("Console", consoleTarget);
            loggingConfiguration.LoggingRules.Add(new LoggingRule("*", ToNLogLevel(config.LogLevel), consoleTarget));

            if (config.LogFilePath.IsValidString() && config.LogFileName.IsValidString())
            {
                long archiveAboveSize = config.LogLimitInKiloBytes == 0 ? long.MaxValue : config.LogLimitInKiloBytes * 1024;
                FileTarget fileTarget = new FileTarget
                {
                    KeepFileOpen = true,
                    ConcurrentWrites = false,
                    FileName = Path.Combine(config.LogFilePath, config.LogFileName),
                    MaxArchiveFiles = 1,
                    ArchiveAboveSize = archiveAboveSize,
                    Name = "File",
                };

                loggingConfiguration.AddTarget("File", fileTarget);
                loggingConfiguration.LoggingRules.Add(new LoggingRule("*", ToNLogLevel(config.LogLevel), fileTarget));
            }

            NLogManager.Configuration = loggingConfiguration;
        }

        private static NLogLevel ToNLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return NLogLevel.Debug;
                case LogLevel.Error:
                    return NLogLevel.Error;
                case LogLevel.Fatal:
                    return NLogLevel.Fatal;
                case LogLevel.Info:
                    return NLogLevel.Info;
                case LogLevel.Warn:
                    return NLogLevel.Warn;
                default:
                    throw new ArgumentException(string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} is not a valid LogLevel",
                        level.ToString()));
            }
        }

        // methods in this region should only be called directly by tests
        #region CalledByTests

        internal static Logger GetLogger(string className, INewRelicConfig config)
        {
            if (!Configured || config != null)
            {
                lock (ConfigureLock)
                {
                    if (!Configured || config != null)
                    {
                        Configure(config);
                        Configured = true;
                    }
                }
            }

            return new Logger(className);
        }

        #endregion
    }
}
