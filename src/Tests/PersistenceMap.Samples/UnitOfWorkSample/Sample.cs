using PersistenceMap.Samples.Data;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceMap.Samples.UnitOfWorkSample
{
    class Sample
    {
        List<string> _log;

        public void Work()
        {
            // this method shows how the 
            _log = new List<string>();
            int count = 100;

            DatabaseManager.CreateDatabase();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var provider = new SqliteContextProvider(DatabaseManager.ConnectionString);
            using (var context = provider.Open())
            {
                using (var uow = new UnitOfWork(context))
                {
                    for (int i = 0; i < count; i++)
                    {
                        DoReadWork(uow, i);
                    }
                }
            }

            stopwatch.Stop();
            _log.Add(string.Format("Creating one context for {0} selects calls took {1} ms", count, stopwatch.ElapsedMilliseconds));

            stopwatch.Reset();
            stopwatch.Start();

            for (int i = 0; i < count; i++)
            {
                using (var uow = new UnitOfWork(DatabaseManager.ConnectionString))
                {
                    DoReadWork(uow, i);
                }
            }

            stopwatch.Stop();
            _log.Add(string.Format("Creating a context per call for {0} selects calls took {1} ms", count, stopwatch.ElapsedMilliseconds));

            PrintLog();
        }


        private void DoReadWork(UnitOfWork uow, int index)
        {
            var items = uow.Context.From<Warrior>()
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
