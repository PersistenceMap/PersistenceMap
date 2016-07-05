using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistenceMap.Diagnostics
{
    public class TimeLogger
    {
        private readonly ISettings _settings;
        private readonly List<Timer> _timers;
        private readonly StringBuilder _stringBuilder;
        private bool _isRunning;

        /// <summary>
        /// Creates a new instance of a timelogger
        /// </summary>
        public TimeLogger() : this(new Settings())
        {
        }

        /// <summary>
        /// Creates a new instance of a timelogger
        /// </summary>
        /// <param name="settings">The settings for the context</param>
        public TimeLogger(ISettings settings)
        {
            _settings = settings;
            _timers = new List<Timer>();
            _isRunning = true;
            _stringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Gets an enumeration of all launched timers
        /// </summary>
        public IEnumerable<Timer> Timers => _timers;

        /// <summary>
        /// Starts a new timer
        /// </summary>
        /// <param name="key">The key name of the timer</param>
        /// <returns>The current object</returns>
        public TimeLogger StartTimer(string key)
        {
            if (_settings.LogLevel == LogDebth.Simple)
            {
                return this;
            }

            var timer = new Timer(key);
            _timers.Add(timer);

            timer.Start();

            return this;
        }

        /// <summary>
        /// Stops the timer with the given key
        /// </summary>
        /// <param name="key">The ky for the timer</param>
        public TimeLogger StopTimer(string key)
        {
            _timers.Where(t => t.Key == key)
                .ForEach(t => t.Stop());

            return this;
        }

        /// <summary>
        /// Stops all timers
        /// </summary>
        /// <returns>The current object</returns>
        public TimeLogger Stop()
        {
            foreach (var timer in _timers)
            {
                timer.Stop();
            }

            return this;
        }

        /// <summary>
        /// Add a new text to the logger
        /// </summary>
        /// <param name="value">The line to log</param>
        /// <returns>The current object</returns>
        public TimeLogger AppendLine(string value)
        {
            if (_settings.LogLevel == LogDebth.Simple)
            {
                return this;
            }

            _stringBuilder.AppendLine($"## {value}");

            return this;
        }

        /// <summary>
        /// Creates a loggable sting of this object
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            if (_isRunning)
            {
                foreach (var timer in _timers)
                {
                    timer.Stop();
                    _stringBuilder.AppendLine($"## {timer.Key} {timer.ElapsedMilliseconds} ms");
                }
            }

            _isRunning = false;

            return _stringBuilder.ToString().TrimEnd();
        }
    }
}
