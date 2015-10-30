
namespace PersistenceMap
{
    public static class CustomConfiguration
    {
        static CustomConfiguration()
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
