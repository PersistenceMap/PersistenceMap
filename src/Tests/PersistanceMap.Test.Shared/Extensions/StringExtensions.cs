
namespace PersistanceMap.Test
{
    public static class StringExtensions
    {
        public static string Flatten(this string value)
        {
            return value
                .Replace("\t", "")
                .Replace("\n", "")
                .Replace("\r", "")
                .TrimEnd();
        }
    }
}
