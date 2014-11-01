using System.Configuration;

namespace PersistanceMap.Configuration
{
    public class PersistanceMapSection : ConfigurationSection
    {
        [ConfigurationProperty("loggers", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(LoggerElement), AddItemName = "logger")]
        public LoggerElementCollection Loggers
        {
            get
            {
                return (LoggerElementCollection)this["loggers"];
            }
        }
    }
}
