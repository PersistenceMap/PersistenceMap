using NUnit.Framework;
using PersistenceMap.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceMap.UnitTest.Diagnostics
{
    [TestFixture]
    public class LoggerConfigurationTests
    {
        [Test]
        public void PersistenceMap_Diagnostics_LoggerConfiguration_SetToSettings()
        {
            var writer = new TraceLogger();
            var configuration = new LoggerConfiguration();
            configuration.AddWriter(writer);

            var settings = new Settings();
            configuration.SetToFactory(settings);

            var logger = settings.LoggerFactory.LogProviders.First();
            Assert.AreSame(writer, logger);
        }

        [Test]
        public void PersistenceMap_Diagnostics_LoggerConfiguration_SetDefault()
        {
            var writer = new TraceLogger();
            var configuration = new LoggerConfiguration();
            configuration.AddWriter(writer);
            
            configuration.SetDefault();

            var settings = new Settings();
            var logger = settings.LoggerFactory.LogProviders.First();
            Assert.AreSame(writer, logger);

            settings.Reset();
        }
    }
}
