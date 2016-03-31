using PersistenceMap.Samples.Data;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeasureMap;
using Scribe;

namespace PersistenceMap.Samples.ContextSample
{
    class Sample
    {
        List<string> _log;

        public void Work()
        {
            var listener = new PersistenceMapLogListener();

            var factory = new LoggerFactory();
            factory.AddListener(listener);
            factory.AddWriter(new TraceLogWriter());
            
            _log = new List<string>();
            int count = 100;

            DatabaseManager.CreateDatabase();
            
            var provider = new SqliteContextProvider(DatabaseManager.ConnectionString);
            provider.Settings.AddLogger(listener);
            using (var context = provider.Open())
            {
                var profile1 = ProfilerSession.StartSession()
                    .SetIterations(count)
                    .Task(() => DoReadWork(context, 0))
                    .RunSession();

                _log.Add($"Creating one context for {count} selects calls took {profile1.TotalTime.TotalMilliseconds} ms");
            }

            var profile2 = ProfilerSession.StartSession()
                .SetIterations(count)
                .Task(() =>
                {
                    provider = new SqliteContextProvider(DatabaseManager.ConnectionString);
                    using (var context = provider.Open())
                    {
                        DoReadWork(context, 0);
                    }
                }).RunSession();
            
            _log.Add($"Creating a context per call for {count} selects calls took {profile2.TotalTime.TotalMilliseconds} ms");

            PrintLog();
        }


        private void DoReadWork(SqliteDatabaseContext context, int index)
        {
            var items = context.From<Warrior>()
                .Map(w => w.ID)
                .Map(w => w.Name)
                .Join<Armour>((a, w) => a.WarriorID == w.ID)
                .Join<ArmourPart>((ap, a) => ap.ID == a.ArmourPartID)
                .Join<Weapon, Warrior>((w, wa) => w.ID == wa.WeaponID)
                .For<WarriorWithArmour>()
                .Map<Weapon>(w => w.Name, wa => wa.WeaponName)
                .Map<Armour>(a => a.Name, wa => wa.ArmourName)
                .Select();

            WriteLog(index, items.Count());
        }

        private void WriteLog(int index, int count)
        {
            //Console.WriteLine(string.Format("Round {0} with itemcount {1}", index, count));
            Console.Write("-");
        }

        private void PrintLog()
        {
            Console.WriteLine();
            foreach (var log in _log)
            {
                Console.WriteLine(log);
            }
        }

        private class PersistenceMapLogListener : IListener, PersistenceMap.Diagnostics.ILogWriter
        {
            private ILogger _logger;

            public void Initialize(ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.GetLogger();
            }

            public void Write(string message, string source = null, string category = null, DateTime? logtime = null)
            {
                if (_logger == null)
                {
                    return;
                }

                _logger.Write(message, LogLevel.Information, Priority.Medium, category, logtime);
            }
        }

        class TraceLogWriter : ILogWriter
        {
            public void Write(ILogEntry logEntry)
            {
                Trace.WriteLine(logEntry.ToString());
            }
        }
    }
}
