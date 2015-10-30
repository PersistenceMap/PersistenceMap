using PersistenceMap.Samples.Data;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceMap.Samples.ContextSample
{
    class Sample
    {
        List<string> _log;

        public void Work()
        {
            _log = new List<string>();
            int count = 100;

            DatabaseManager.CreateDatabase();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var provider = new SqliteContextProvider(DatabaseManager.ConnectionString);
            using (var context = provider.Open())
            {
                for (int i = 0; i < count; i++)
                {
                    DoReadWork(context, i);
                }
            }

            stopwatch.Stop();
            _log.Add(string.Format("Creating one context for {0} selects calls took {1} ms", count, stopwatch.ElapsedMilliseconds));

            stopwatch.Reset();
            stopwatch.Start();

            for (int i = 0; i < count; i++)
            {
                provider = new SqliteContextProvider(DatabaseManager.ConnectionString);
                using (var context = provider.Open())
                {
                    DoReadWork(context, i);
                }
            }

            stopwatch.Stop();
            _log.Add(string.Format("Creating a context per call for {0} selects calls took {1} ms", count, stopwatch.ElapsedMilliseconds));

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
    }
}
