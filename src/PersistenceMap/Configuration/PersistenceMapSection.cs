using System.Configuration;

namespace PersistenceMap.Configuration
{
    public class PersistenceMapSection : ConfigurationSection
    {
        [ConfigurationProperty("loggers", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(LoggerElement), AddItemName = "add")]
        public LoggerElementCollection Loggers
        {
            get
            {
                return (LoggerElementCollection)this["loggers"];
            }
        }
    }
}
