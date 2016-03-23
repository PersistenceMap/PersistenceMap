using System;
using System.Collections;

namespace PersistenceMap.Interception
{
    internal class Result
    {
        private readonly Func<IEnumerable> _enumerator;
        private readonly Type _type;

        public Result(Func<IEnumerable> enumerator, Type type)
        {
            _enumerator = enumerator;
            _type = type;
        }

        public IEnumerator Enumerator => _enumerator.Invoke().GetEnumerator();

        public Type Type => _type;
    }
}
