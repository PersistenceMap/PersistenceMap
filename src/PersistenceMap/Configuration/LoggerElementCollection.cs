using System.Collections.Generic;
using System.Configuration;

namespace PersistenceMap.Configuration
{
    [ConfigurationCollection(typeof(LoggerElement))]
    public class LoggerElementCollection: ConfigurationElementCollection, IEnumerable<LoggerElement>
    {
        private readonly List<LoggerElement> _elements;

        public LoggerElementCollection()
        {
            _elements = new List<LoggerElement>();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            var element = new LoggerElement();
            _elements.Add(element);

            return element;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoggerElement)element).Type;
        }

        public new IEnumerator<LoggerElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
