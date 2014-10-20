
namespace PersistanceMap
{
    public static class Configuration
    {
        static Configuration()
        {
            TreatEnumAsInteger = true;
            StripUpperInLike = true;
        }

        static bool? treatEnumAsInteger;
        public static bool TreatEnumAsInteger
        {
            get
            {
                return treatEnumAsInteger != null ? treatEnumAsInteger.Value : false;
            }
            set
            {
                treatEnumAsInteger = value;
            }
        }

        public static bool StripUpperInLike { get; set; }
    }
}
