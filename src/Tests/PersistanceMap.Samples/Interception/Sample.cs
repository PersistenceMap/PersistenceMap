using PersistanceMap.Samples.Data;
using PersistanceMap.Test;
using PersistanceMap.Test.TableTypes;
using System.Diagnostics;

namespace PersistanceMap.Samples.Interception
{
    class Sample
    {
        public void Work()
        {
            var provider = new SqliteContextProvider(DatabaseManager.ConnectionString);

            // add interceptor that traces the query string before it is executed
            provider.Interceptor<WarriorWithArmour>().BeforeExecute(q => Trace.WriteLine(q.QueryString.Flatten()));

            using (var context = provider.Open())
            {
                var item = context.From<Warrior>()
                .Map(w => w.ID)
                .Map(w => w.Name)
                .Join<Armour>((a, w) => a.WarriorID == w.ID)
                .Join<ArmourPart>((ap, a) => ap.ID == a.ArmourPartID)
                .Join<Weapon, Warrior>((w, wa) => w.ID == wa.WeaponID)
                .For<WarriorWithArmour>()
                .Map<Weapon>(w => w.Name, wa => wa.WeaponName)
                .Map<Armour>(a => a.Name, wa => wa.ArmourName)
                .Select();
            }

            provider.Interceptor<Warrior>().BeforeExecute(q => Trace.WriteLine(q.QueryString.Flatten()));
            using (var context = provider.Open())
            {
                var tmp = context.From<Warrior>().Select(w => new
                {
                    w.ID,
                    w.Name,
                    w.Race
                });
            }
        }
    }
}
