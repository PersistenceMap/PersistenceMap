
namespace PersistanceMap.QueryParts
{
    internal class StringQueryPart : IQueryPart
    {
        readonly string _value;
        readonly string _prefix;
        readonly string _sufix;

        public StringQueryPart(OperationType operationtype, string value, string prefix = null, string sufix = null)
        {
            OperationType = operationtype;
            _value = value;

            _prefix = prefix ?? "";
            _sufix = sufix ?? "";
        }

        public OperationType OperationType { get; set; }

        public string Compile()
        {
            return string.Format("{0}{1}{2}", _prefix, _value, _sufix);
        }
    }
}
