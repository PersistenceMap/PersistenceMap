using NUnit.Framework;
using PersistenceMap.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PersistenceMap.UnitTest.Diagnostics
{
    [TestFixture]
    public class TimeLoggerTests
    {
        [Test]
        public void PersistanceMap_Diagnostics_TimeLogger()
        {
            var timer = new TimeLogger().StartTimer("Timer 1").StartTimer("Timer 2");

            var value = timer.ToString();
            Assert.IsTrue(timer.Timers.Count() == 2);
        }

        [Test]
        public void PersistanceMap_Diagnostics_TimeLogger_SimpleLogLevel()
        {
            var timer = new TimeLogger(new Settings { LogLevel = LogDebth.Simple }).StartTimer("Timer 1").StartTimer("Timer 2");

            var value = timer.ToString();
            Assert.IsFalse(timer.Timers.Any());
        }

        [Test]
        public void PersistanceMap_Diagnostics_TimeLogger_ToString()
        {
            var timer = new TimeLogger().StartTimer("Timer 1").StartTimer("Timer 2");

            var value = timer.ToString();
            Assert.IsTrue(value.Contains("Timer 1"));
            Assert.IsTrue(value.Contains("Timer 2"));
        }

        [Test]
        public void PersistanceMap_Diagnostics_TimeLogger_ToString_SimpleLogLevel()
        {
            var timer = new TimeLogger(new Settings { LogLevel = LogDebth.Simple }).StartTimer("Timer 1").StartTimer("Timer 2");

            var value = timer.ToString();
            Assert.IsTrue(string.IsNullOrEmpty(value));
        }

        [Test]
        public void PersistanceMap_Diagnostics_TimeLogger_ToString_MultipleTimes()
        {
            var timer = new TimeLogger().StartTimer("Timer 1");
            timer.ToString();
            var value = timer.ToString();

            var match = Regex.Matches(value, "Timer 1");

            Assert.IsTrue(match.Count == 1);
        }

        [Test]
        public void PersistanceMap_Diagnostics_TimeLogger_WriteLine()
        {
            var timer = new TimeLogger().AppendLine("Timer 1");

            var value = timer.ToString();
            Assert.IsTrue(value.Equals("## Timer 1"));
        }

        [Test]
        public void PersistanceMap_Diagnostics_TimeLogger_WriteLineToString_MultipleTimes()
        {
            var timer = new TimeLogger().StartTimer("Timer 1").AppendLine("test line");
            timer.ToString();
            var value = timer.ToString();

            var match = Regex.Matches(value, "test line");

            Assert.IsTrue(match.Count == 1);
        }
    }
}
