﻿
namespace PersistenceMap.Test.TableTypes
{
    public class Warrior
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int WeaponID { get; set; }

        public string Race { get; set; }

        public string SpecialSkill { get; set; }
    }

    public class Warrior2
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int WeaponID { get; set; }

        public string Race { get; set; }

        public string SpecialSkill { get; set; }

        public int? Streangth { get; set; }
    }
}
