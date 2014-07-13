
namespace PersistanceMap
{
    public static class Configuration
    {
        static Configuration()
        {
            TreatEnumAsInteger = true;
            StripUpperInLike = true;
        }

        static bool? _treatEnumAsInteger;
        public static bool TreatEnumAsInteger
        {
            get
            {
                return _treatEnumAsInteger != null ? _treatEnumAsInteger.Value : false;
            }
            set
            {
                _treatEnumAsInteger = value;
            }
        }

        public static bool StripUpperInLike { get; set; }
    }
}
