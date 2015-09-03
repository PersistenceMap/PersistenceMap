using PersistanceMap.Samples.Data;
using PersistanceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Samples.ContextMockSample
{
    class Sample
    {
        public void Work()
        {
            var provider = new SqliteContextProvider(DatabaseManager.ConnectionString);
            //provider.Interceptor<Warrior>().BeforeExecute(q => sql = q.QueryString.Flatten());
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
        }
    }
}
