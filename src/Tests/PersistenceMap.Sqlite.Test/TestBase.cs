using PersistenceMap.Test.TableTypes;
using System.Configuration;

namespace PersistenceMap.Sqlite.Test
{
    public abstract class TestBase
    {
        protected string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["PersistenceMap.Test.Properties.Settings.ConnectionString"].ConnectionString;
            }
        }

        protected void CreateDatabase(bool insertData = false)
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                context.Database.Table<Weapon>().Key(wpn => wpn.ID).Create();

                context.Database.Table<Warrior>()
                    .Key(wrir => wrir.ID)
                    .ForeignKey<Weapon>(wrir => wrir.WeaponID, wpn => wpn.ID)
                    .Create();

                context.Database.Table<ArmourPart>()
                    .Key(ap => ap.ID)
                    .Create();

                context.Database.Table<Armour>()
                    .ForeignKey<Weapon>(a => a.WarriorID, wpn => wpn.ID)
                    .ForeignKey<ArmourPart>(a => a.ArmourPartID, ap => ap.ID)
                    .Create();

                context.Commit();

                if (!insertData)
                    return;

                context.Insert<Weapon>(() => new Weapon { ID = 1, Name = "Sword", Damage = 40 });
                context.Insert<Weapon>(() => new Weapon { ID = 2, Name = "Dagger", Damage = 5 });
                context.Insert<Weapon>(() => new Weapon { ID = 3, Name = "Ax", Damage = 50 });
                context.Insert<Weapon>(() => new Weapon { ID = 4, Name = "Hammer", Damage = 70 });
                context.Insert<Weapon>(() => new Weapon { ID = 5, Name = "Staff", Damage = 20 });

                context.Insert<Warrior>(() => new Warrior { ID = 1, Name = "Ruben", Race = "Elf", SpecialSkill = "Magic claw", WeaponID = 5 });
                context.Insert<Warrior>(() => new Warrior { ID = 2, Name = "Harry", Race = "Dwarf", SpecialSkill = "Fistblow", WeaponID = 3 });
                context.Insert<Warrior>(() => new Warrior { ID = 3, Name = "Burt", Race = "Human", SpecialSkill = "Stomp", WeaponID = 3 });

                context.Insert<ArmourPart>(() => new ArmourPart { ID = 1, Name = "Shoulderplate", Defense = 5 });
                context.Insert<ArmourPart>(() => new ArmourPart { ID = 2, Name = "Glove", Defense = 5 });
                context.Insert<ArmourPart>(() => new ArmourPart { ID = 3, Name = "Breastplate", Defense = 30 });
                context.Insert<ArmourPart>(() => new ArmourPart { ID = 4, Name = "Shield", Defense = 50 });
                context.Insert<ArmourPart>(() => new ArmourPart { ID = 5, Name = "Breastleather", Defense = 20 });

                context.Insert<Armour>(() => new Armour { ArmourPartID = 1, WarriorID = 1 });
                context.Insert<Armour>(() => new Armour { ArmourPartID = 5, WarriorID = 1 });
                context.Insert<Armour>(() => new Armour { ArmourPartID = 4, WarriorID = 1 });

                context.Insert<Armour>(() => new Armour { ArmourPartID = 1, WarriorID = 2 });
                context.Insert<Armour>(() => new Armour { ArmourPartID = 2, WarriorID = 2 });
                context.Insert<Armour>(() => new Armour { ArmourPartID = 3, WarriorID = 2 });

                context.Insert<Armour>(() => new Armour { ArmourPartID = 1, WarriorID = 3 });
                context.Insert<Armour>(() => new Armour { ArmourPartID = 2, WarriorID = 1 });
                context.Insert<Armour>(() => new Armour { ArmourPartID = 5, WarriorID = 1 });
                context.Insert<Armour>(() => new Armour { ArmourPartID = 4, WarriorID = 1 });

                context.Commit();
            }
        }
    }
}
