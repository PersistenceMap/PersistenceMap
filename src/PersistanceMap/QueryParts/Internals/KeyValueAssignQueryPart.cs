using PersistanceMap.Sql;

namespace PersistanceMap.QueryParts
{
    internal class KeyValueAssignQueryPart<T> : IQueryPart
    {
        readonly object _valueObject;
        readonly FieldDefinition _field;

        public KeyValueAssignQueryPart(OperationType operationtype, object valueObject, FieldDefinition field)
        {
            OperationType = operationtype;
            _valueObject = valueObject;
            _field = field;
        }

        #region IQueryPart Implementation

        public OperationType OperationType { get; set; }

        public string Compile()
        {
            var value = _field.GetValueFunction(_valueObject);
            var quotated = DialectProvider.Instance.GetQuotedValue(value, _field.MemberType);

            return string.Format("{0} = {1}", _field.FieldName, quotated ?? "NULL");
        }

        #endregion
    }
}
