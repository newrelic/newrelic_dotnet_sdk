using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using NewRelic.Platform.Sdk.Utils;
using NLog;
using System.Diagnostics;

namespace NewRelic.Platform.Sdk.Binding
{
    public class Context : IContext
    {
        private PlatformData _platformData;
        private string _licenseKey;

        internal string ServiceUri { get { return ConfigurationHelper.GetConfiguration(Constants.ConfigKeyServiceUri, Constants.DefaultServiceUri); } }
        internal string LicenseKey { get { return _licenseKey ?? ConfigurationHelper.GetConfiguration(Constants.ConfigKeyLicenseKey); } }

        public string Version 
        { 
            get { return _platformData.Version; }
            set { _platformData.Version = value; }  
        }

        private static Logger s_log = LogManager.GetLogger("Context");

        /// <summary>
        /// This class is responsible for maintaining metrics that have been reported as well as sending them to the New Relic 
        /// service. Any Components that share a Request reference will have their data sent to the service in one request.
        /// </summary>
        public Context() : this(null)
        {
        }

        /// <summary>
        /// This class is responsible for maintaining metrics that have been reported as well as sending them to the New Relic 
        /// service. Any Components that share a Request reference will have their data sent to the service in one request.
        /// </summary>
        public Context(string licenseKey)
        {
            s_log.Debug("Using service URL: {0}", this.ServiceUri);
            s_log.Debug("Using license key: {0}", this.LicenseKey);

            this._licenseKey = licenseKey;
            this._platformData = new PlatformData();
        }

        /// <summary>
        /// Prepare a New Relic metric to be delivered to the service at the end of this poll cycle.  
        /// </summary>
        /// <param name="guid">A string guid identifying the plugin this is associated with (e.g. 'com.yourcompany.pluginname')</param>
        /// <param name="componentName">A string name identifying the plugin agent that will appear in the service UI (e.g. 'MyPlugin')</param>
        /// <param name="metricName">A string name representing the name of the metric (e.g. 'Category/Name')</param>
        /// <param name="units">The units of the metric you are sending to the service (e.g. 'byte/second')</param>
        /// <param name="value">The non-negative float value representing this value</param>
        public void ReportMetric(string guid, string componentName, string metricName, string units, float? value)
        {
            s_log.Info("Reporting metric: {0} -> {1}[{2}]={3}", componentName, metricName, units, value);
            if (string.IsNullOrEmpty(guid))
            {
                throw new ArgumentNullException("guid", "Null parameter was passed to ReportMetric()");
            }

            if (string.IsNullOrEmpty(componentName))
            {
                throw new ArgumentNullException("componentName", "Null parameter was passed to ReportMetric()");
            }

            if (string.IsNullOrEmpty(metricName))
            {
                throw new ArgumentNullException("metricName", "Null parameter was passed to ReportMetric()");
            }

            if (string.IsNullOrEmpty(units))
            {
                throw new ArgumentNullException("units", "Null parameter was passed to ReportMetric()");
            }

            // Ensure the value is not null since EpochProcessors will return 0 on initial processing
            if (value.HasValue)
            {
                if (value.Value < 0)
                {
                    throw new ArgumentException("New Relic Platform does not currently support negative values", "value");
                }

                _platformData.AddMetric(guid, componentName, metricName, units, value.Value);
            }
        }

        /// <summary>
        /// Prepare a New Relic metric to be delivered to the service at the end of this poll cycle.
        /// </summary>
        /// <param name="guid">A string guid identifying the plugin this is associated with (e.g. 'com.yourcompany.pluginname')</param>
        /// <param name="componentName">A string name identifying the plugin agent that will appear in the service UI (e.g. 'MyPlugin')</param>
        /// <param name="metricName">A string name representing the name of the metric (e.g. 'Category/Name')</param>
        /// <param name="units">The units of the metric you are sending to the service (e.g. 'byte/second')</param>
        /// <param name="value">The non-negative float value representing this value</param>
        /// <param name="count">The int value representing how many poll cycle this metric has been aggregated for</param>
        /// <param name="min">The non-negative float value representing this min value for this poll cycle</param>
        /// <param name="max">The non-negative float value representing this max value for this poll cycle</param>
        /// <param name="sumOfSquares">The non-negative float value representing the sum of square values for this poll cycle</param>
        public void ReportMetric(string guid, string componentName, string metricName, string units, float value, int count, float min, float max, float sumOfSquares)
        {
            s_log.Info("Reporting metric: {0} -> {1}[{2}]={3}", componentName, metricName, units, value);
            if (string.IsNullOrEmpty(guid))
            {
                throw new ArgumentNullException("guid", "Null parameter was passed to ReportMetric()");
            }

            if (string.IsNullOrEmpty(componentName))
            {
                throw new ArgumentNullException("componentName", "Null parameter was passed to ReportMetric()");
            }

            if (string.IsNullOrEmpty(metricName))
            {
                throw new ArgumentNullException("metricName", "Null parameter was passed to ReportMetric()");
            }

            if (string.IsNullOrEmpty(units))
            {
                throw new ArgumentNullException("units", "Null parameter was passed to ReportMetric()");
            }

            if (count < 1)
            {
                throw new ArgumentException("New Relic Platform does not support count values less than 1", "count");
            }

            // Ensure the value is not null since EpochProcessors will return 0 on initial processing
            if (value < 0 || min < 0 || max < 0 || sumOfSquares < 0)
            {
                throw new ArgumentException("New Relic Platform does not currently support negative values");
            }

            _platformData.AddMetric(guid, componentName, metricName, units, value, count, min, max, sumOfSquares);
        }

        /// <summary>
        /// Will send all metrics that were reported to the Context since its last successful delivery to the New Relic service.
        /// The context will aggregate values for calls that fail due to transient issues.
        /// </summary>
        public void SendMetricsToService()
        {
            s_log.Info("Preparing to send metrics to service");

            // Check to see if the Agent data is valid before sending data
            if (!ValidatePlatformData())
            {
                return;
            }

            var request = (HttpWebRequest)WebRequest.Create(this.ServiceUri);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers["X-License-Key"] = this.LicenseKey;

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                var str = JsonHelper.Serialize(_platformData.Serialize());

                if (s_log.IsDebugEnabled)
                {
                    s_log.Debug("Sending JSON: {0}", str);
                }

                writer.Write(str);
            }

            HandleServiceResponse(request);
        }

        private bool ValidatePlatformData()
        {
            // Do not send any data if no agents reported
            if (!_platformData.HasComponents())
            {
                s_log.Info("No metrics reported for this poll cycle, continuing...");
                _platformData.Reset(); // Reset the aggregation timer
                return false;
            }

            // Reset the agent data if we have been aggregating for too long
            if (_platformData.IsPastAggregationLimit())
            {
                s_log.Warn("The service has not been successfully contacted in several minutes.  Starting a fresh aggregation cycle.");
                _platformData.Reset(); // Reset the aggregation timer
                return false;
            }

            return true;
        }

        private void HandleServiceResponse(HttpWebRequest request)
        {
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            s_log.Info("Metrics successfully sent");
                            // Reset aggregation after successful delivery
                            _platformData.Reset();
                        }
                    }
                }
            }
            catch (WebException we)
            {
                using (var response = (HttpWebResponse)we.Response)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var body = reader.ReadToEnd();

                        if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                        {
                            // Collector is being updated
                            s_log.Info("New Relic Service is currently undergoing an upgrade");
                        }
                        else if (response.StatusCode == HttpStatusCode.Forbidden 
                            && string.Equals(Constants.DisableNewRelic, body))
                        {
                            // Remotely disabled
                            s_log.Fatal("Remotely disabled by New Relic service");
                            Process.GetCurrentProcess().Kill();
                        }
                        else
                        {
                            // Log unknown exception
                            if(s_log.IsErrorEnabled) 
                            {
                                s_log.Error("Unexpected response from the New Relic service. StatusCode: {0} ({1}), BodyContents: {2}", 
                                    response.StatusCode, response.StatusDescription, body);
                            }
                        }
                    }
                } // End using()
            } // End catch()
        } // End HandleServiceResponse()
    } // End Context
}
