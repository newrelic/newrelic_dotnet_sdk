using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewRelic.Platform.Sdk.Utils;
using NLog.Config;
using NLog.Targets;
using NLogManager = NLog.LogManager;

namespace NewRelic.Platform.Sdk.UnitTests.Utils
{
    [TestClass]
    public class LoggerTests
    {
        private TestConfig testConfig;

        [TestInitialize]
        public void Initialize()
        {
            // setup a default config
            this.testConfig = new TestConfig
            {
                LogFileName = "default.log",
                LogFilePath = @".\logs",
                LogLevel = Configuration.LogLevel.Info,
                LogLimitInKiloBytes = 5000,
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            string logPath = Path.Combine(this.testConfig.LogFilePath, this.testConfig.LogFileName);
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
        }

        [TestMethod]
        public void LoggingIsConfiguredProperly()
        {
            this.testConfig = new TestConfig
            {
                LogLevel = Configuration.LogLevel.Error,
                LogFilePath = @".\path\to\logs",
                LogFileName = "test_log.log",
                LogLimitInKiloBytes = 10000,
            };

            Logger logger = Logger.GetLogger("LoggerTests", this.testConfig);
            LoggingConfiguration nLogConfig = NLogManager.Configuration;

            Assert.AreEqual(2, nLogConfig.AllTargets.Count, "There should be exactly 2 targets");
            
            Target consoleTarget = nLogConfig.AllTargets.FirstOrDefault(t => t.Name.Equals("Console", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsNotNull(consoleTarget, "There is no console target");

            FileTarget fileTarget = (FileTarget)nLogConfig.AllTargets.FirstOrDefault(t => t.Name.Equals("File", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsNotNull(fileTarget, "There is no file target");
            Assert.IsTrue(fileTarget.FileName.ToString().Contains(Path.Combine(this.testConfig.LogFilePath, this.testConfig.LogFileName)), "The file name is incorrect.");
            Assert.AreEqual(this.testConfig.LogLimitInKiloBytes * 1024, fileTarget.ArchiveAboveSize, "The log limit is incorrect.");
        }

        [TestMethod]
        public void LogsAreEmittedBasedOnLogLevel()
        {
            Logger logger = Logger.GetLogger("LoggerTests", this.testConfig);
            string logMessage = "This is a log message";
            logger.Info(logMessage);
            NLogManager.Shutdown();

            string pathToLogFile = Path.Combine(this.testConfig.LogFilePath, this.testConfig.LogFileName);
            Assert.IsTrue(File.Exists(pathToLogFile), "Log file was not created");
            Assert.IsTrue(File.ReadAllText(pathToLogFile).Trim().EndsWith(logMessage), "The log doesn't contain the log message.");
        }

        [TestMethod]
        public void LogsAreIgnoredBasedOnLogLevel()
        {
            Logger logger = Logger.GetLogger("LoggerTests", this.testConfig);
            string logMessage = "This is a log message";
            logger.Debug(logMessage);
            NLogManager.Shutdown();

            string pathToLogFile = Path.Combine(this.testConfig.LogFilePath, this.testConfig.LogFileName);
            Assert.IsFalse(File.Exists(pathToLogFile), "Log file should not be created.");
        }
    }
}
