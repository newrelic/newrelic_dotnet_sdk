using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewRelic.Platform.Sdk.Configuration;

namespace NewRelic.Platform.Sdk.UnitTests
{
    [TestClass]
    public class RunnerTest
    {
        [TestMethod]
        public void RunnerSetsUpProxy()
        {
            INewRelicConfig config = new TestConfig
            {
                ProxyHost = "Host",
                ProxyPort = 8080,
                ProxyUserName = "UserName",
                ProxyPassword = "Password",
            };

            Runner runner = new Runner(config);
            IWebProxy proxy = WebRequest.DefaultWebProxy;
            Uri proxyAddress = proxy.GetProxy(new Uri("http://www.google.com"));
            NetworkCredential credential = (NetworkCredential)proxy.Credentials;

            Assert.IsTrue(string.Equals(config.ProxyHost, proxyAddress.Host, StringComparison.InvariantCultureIgnoreCase), "Host is incorrect.");
            Assert.AreEqual(config.ProxyPort, proxyAddress.Port, "Port is incorrect.");
            Assert.AreEqual(config.ProxyUserName, credential.UserName, "Username is incorrect.");
            Assert.AreEqual(config.ProxyPassword, credential.Password, "Password is incorrect.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PortMustBeSpecified()
        {
            INewRelicConfig config = new TestConfig
            {
                ProxyHost = "Host",
                ProxyUserName = "UserName",
                ProxyPassword = "Password",
            };

            Runner runner = new Runner(config);
        }

        [TestMethod]
        public void ProxySetupAcceptsEmptyCredentials()
        {
            INewRelicConfig config = new TestConfig
            {
                ProxyHost = "Host",
                ProxyPort = 8080,
            };

            Runner runner = new Runner(config);
            IWebProxy proxy = WebRequest.DefaultWebProxy;
            Uri proxyAddress = proxy.GetProxy(new Uri("http://www.google.com"));
            NetworkCredential credential = (NetworkCredential)proxy.Credentials;

            Assert.IsNull(credential, "No credentials were provided, this should be null.");
        }

        [TestMethod]
        public void ProxySetupEmptyPassword()
        {
            INewRelicConfig config = new TestConfig
            {
                ProxyHost = "Host",
                ProxyPort = 8080,
                ProxyUserName = "Username",
            };

            Runner runner = new Runner(config);
            IWebProxy proxy = WebRequest.DefaultWebProxy;
            Uri proxyAddress = proxy.GetProxy(new Uri("http://www.google.com"));
            NetworkCredential credential = (NetworkCredential)proxy.Credentials;

            Assert.AreEqual(config.ProxyUserName, credential.UserName, "Username is incorrect.");
            Assert.IsTrue(string.IsNullOrEmpty(credential.Password), "Password should be empty.");
        }
    }
}
