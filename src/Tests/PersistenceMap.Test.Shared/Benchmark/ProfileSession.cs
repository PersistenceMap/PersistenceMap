using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PersistenceMap.Test.Benchmark
{
    /// <summary>
    /// TODO: Memorytests: http://www.codeproject.com/Articles/5171/Advanced-Unit-Testing-Part-IV-Fixture-Setup-Teardo
    /// </summary>
    public class ProfileSession : ISession
    {
        private readonly List<Func<Profile, bool>> _conditions;
        private int _iterations = 1;
        private Action _task;

        private ProfileSession()
        {
            _iterations = 1;
            _conditions = new List<Func<Profile, bool>>();
        }

        public static ProfileSession StartSession()
        {
            return new ProfileSession();
        }

        /// <summary>
        /// Sets the amount of iterations that the profileing session should run the task
        /// </summary>
        /// <param name="iterations">The iterations to run the task</param>
        /// <returns>The current profiling session</returns>
        public ProfileSession SetIterations(int iterations)
        {
            _iterations = iterations;

            return this;
        }

        /// <summary>
        /// Adds the Task that will be profiled
        /// </summary>
        /// <param name="task">The Task</param>
        /// <returns>The current profiling session</returns>
        public ProfileSession Task(Action task)
        {
            _task = task;

            return this;
        }

        /// <summary>
        /// Adds a condition to the profiling session
        /// </summary>
        /// <param name="condition">The condition that will be checked</param>
        /// <returns>The current profiling session</returns>
        public ProfileSession AddCondition(Func<Profile, bool> condition)
        {
            _conditions.Add(condition);

            return this;
        }

        /// <summary>
        /// Starts the profiling session
        /// </summary>
        /// <returns>The resulting profile</returns>
        public Profile RunSession()
        {
            // warmup
            Trace.WriteLine("Running Task once for warmup on Performance Analysis Benchmark");
            _task();

            var profile = new Profile();
            var stopwatch = new Stopwatch();

            var process = Process.GetCurrentProcess();
            process.ProcessorAffinity = new IntPtr(2); // Uses the second Core or Processor for the Test
            process.PriorityClass = ProcessPriorityClass.High; // Prevents "Normal" processes from interrupting Threads
            Thread.CurrentThread.Priority = ThreadPriority.Highest; // Prevents "Normal" Threads from interrupting this thread

            // clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Trace.WriteLine(string.Format("Running Task for {0} iterations for Perfomance Analysis Benchmark", _iterations));
            for (int i = 0; i < _iterations; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();

                _task();

                stopwatch.Stop();

                Trace.WriteLine(string.Format("Task ran for {0} Ticks {1} Milliseconds", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds));

                profile.Add(new Iteration(stopwatch.ElapsedTicks, stopwatch.Elapsed));
            }

            foreach (var condition in _conditions)
            {
                if (!condition(profile))
                {
                    throw new AssertionException(string.Format("Condition failed: {0}", condition.ToString()));
                }
            }

            Trace.WriteLine(string.Format("Running Task for {0} iterations with an Average of {1} Milliseconds", _iterations, profile.AverageMilliseconds));

            return profile;
        }
    }

    public interface ISession
    {
        Profile RunSession();
    }

    public class Iteration
    {
        public Iteration(long ticks, TimeSpan duration)
        {
            TimeStamp = DateTime.Now;
            Ticks = ticks;
            Duration = duration;
        }

        /// <summary>
        /// Gets the Ticks that the iteration took to run the Task
        /// </summary>
        public long Ticks { get; private set; }

        /// <summary>
        /// Gets the Milliseconds that the iteration took to run the Task
        /// </summary>
        public TimeSpan Duration { get; private set; }

        public DateTime TimeStamp { get; private set; } 

        public override string ToString()
        {
            return "Ticks: " + Ticks + " mS: " + Duration;
        }
    }

    public class Profile
    {
        private readonly List<Iteration> _iterations;

        public Profile()
        {
            _iterations = new List<Iteration>();
        }

        /// <summary>
        /// The iterations that were run
        /// </summary>
        public IEnumerable<Iteration> Iterations
        {
            get
            {
                return _iterations;
            }
        }

        /// <summary>
        /// Gets the average Milliseconds that all iterations took to run the task
        /// </summary>
        public long AverageMilliseconds
        {
            get
            {
                return _iterations.Select(i => i.Duration.Milliseconds).Sum() / _iterations.Count;
            }
        }

        /// <summary>
        /// Gets the average Ticks that all iterations took to run the task
        /// </summary>
        public long AverageTicks
        {
            get
            {
                return _iterations.Select(i => i.Ticks).Sum() / _iterations.Count;
            }
        }

        internal void Add(Iteration iteration)
        {
            _iterations.Add(iteration);
        }
    }

    public class AssertionException : Exception
    {
        public AssertionException(string message) 
            : base(message)
        {
        }
    }
}
