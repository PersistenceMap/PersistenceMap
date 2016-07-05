using System.Diagnostics;

namespace PersistenceMap.Diagnostics
{
    public class Timer
    {
        private readonly Stopwatch _stopwatch;
        private readonly string _key;

        /// <summary>
        /// Creates a new timer
        /// </summary>
        /// <param name="key">The key for the timer</param>
        public Timer(string key)
        {
            _stopwatch = new Stopwatch();
            _key = key;
        }

        /// <summary>
        /// Gets the key value for this timer object
        /// </summary>
        public string Key => _key;

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in milliseconds
        /// </summary>
        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

        /// <summary>
        /// Starts or resumes measuring time
        /// </summary>
        public void Start()
        {
            _stopwatch.Start();
        }

        /// <summary>
        /// Stops measuring time
        /// </summary>
        public void Stop()
        {
            _stopwatch.Stop();
        }
    }
}
