using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewRelic.Platform.Sdk.Binding;
using System.Threading;
using NewRelic.Platform.Sdk.Utils;
using NLog;

namespace NewRelic.Platform.Sdk
{
    public class Runner
    {
        private List<ComponentFactory> _factories;
        private List<Component> _components;

        private static Logger s_log = LogManager.GetLogger("Runner");

        public Runner()
        {
            _factories = new List<ComponentFactory>();
            _components = new List<Component>();
        }

        /// <summary>
        /// Add an instance of agent to the Runner.  Any components added prior to invoking SetupAndRun() will have their
        /// PollCycle() method invoked each polling interval.
        /// </summary>
        /// <param name="component"></param>
        public void Add(Component component)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component", "You must pass in a non-null component");
            }

            s_log.Info("Adding new component: {0}", component.GetComponentName());
            _components.Add(component);
        }

        /// <summary>
        /// Add an instance of a factory to the Runner.  Any factories added prior to invoking SetupAndRun() will have
        /// their CreateComponentWithConfiguration() method invoked which will create a list of Components initialized through
        /// the factory's configuration file that will be used for polling intervals.
        /// </summary>
        /// <param name="factory"></param>
        public void Add(ComponentFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory", "You must pass in a non-null factory");
            }

            s_log.Info("Adding new factory {0}", factory.GetType());
            _factories.Add(factory);
        }

        /// <summary>
        /// This method only returns during a fatal error.  It will initialize components if necessary, and then begin polling once
        /// per configurable PollInterval invoking registered Component's PollCycle() methods.  Then sending the data to the New Relic service.
        /// </summary>
        public void SetupAndRun()
        {
            if (_factories.Count == 0 && _components.Count == 0)
            {
                throw new InvalidOperationException("You must first call 'Add()' at least once with a valid factory or component");
            }

            // Initialize components if they added a ComponentFactory, otherwise they have explicitly added initialized components already
            if (_factories.Count > 0)
            {
                InitializeFactoryComponents();
            }

            // Initialize components with the same Context so they aggregate to a single a request
            var context = new Context();

            foreach (var component in _components)
            {
                component.PrepareToRun(context);
            }

            var pollInterval = GetPollInterval(); // Fetch poll interval here so we can report any issues early

            while (true)
            {
                try
                {
                    foreach (var component in _components)
                    {
                        component.PollCycle();
                    }

                    context.SendMetricsToService();

                    Thread.Sleep(pollInterval);
                }
                catch (Exception e)
                {
                    s_log.Fatal("Fatal error occurred. Shutting down the application", e);
                    throw e;
                }
            }
        }

        private void InitializeFactoryComponents()
        {
            foreach (ComponentFactory factory in _factories)
            {
                _components = _components.Union(factory.CreateComponents()).ToList();
            }
        }

        private int GetPollInterval()
        {
            int pollInterval = 0;
            var configVal = ConfigurationHelper.GetConfiguration(Constants.ConfigKeyPollInterval);

            Int32.TryParse(configVal, out pollInterval);
            s_log.Debug("Using poll interval: {0} seconds", pollInterval);

            if (pollInterval < 30)
            {
                throw new ArgumentOutOfRangeException("PollInterval", "A poll interval below 30 seconds is not supported");
            }

            return pollInterval *= 1000; // Convert to milliseconds since that's what system calls expect;
        }
    }
}
