using PersistanceMap.Sql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Compiler
{
    public class LambdaExpressionCompiler<T>
    {
        string sep = " ";
        //TODO: useFieldName has to be set as an option!
        bool useFieldName = true;

        public bool PrefixFieldWithTableName { get; set; }



        public LambdaExpressionCompiler()
        {
            PrefixFieldWithTableName = true;
        }


        



        

        protected virtual string GetQuotedColumnName(IEnumerable<FieldDefinition> tableDef, string memberName)
        {
            //TODO: CL check!
            if (useFieldName)
            {
                var fd = tableDef.FirstOrDefault(x => x.Name == memberName);
                var fieldName = fd != null ? fd.FieldName : memberName;

                return PrefixFieldWithTableName
                    ? DialectProvider.Instance.GetQuotedColumnName(fd.EntityName/*tableDef.Name*/, fieldName)
                    : DialectProvider.Instance.GetQuotedColumnName(fieldName);
            }

            return memberName;
        }







        protected internal virtual object Visit(Expression exp)
        {
            if (exp == null) 
                return string.Empty;

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return VisitLambda(exp as LambdaExpression);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess(exp as MemberExpression);
                case ExpressionType.Constant:
                    return VisitConstant(exp as ConstantExpression);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return VisitBinary(exp as BinaryExpression);
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary(exp as UnaryExpression);
                case ExpressionType.Parameter:
                    return VisitParameter(exp as ParameterExpression);
                case ExpressionType.Call:
                    return VisitMethodCall(exp as MethodCallExpression);
                case ExpressionType.New:
                    return VisitNew(exp as NewExpression);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray(exp as NewArrayExpression);
                case ExpressionType.MemberInit:
                    return VisitMemberInit(exp as MemberInitExpression);
                default:
                    return exp.ToString();
            }
        }

        protected virtual object VisitLambda(LambdaExpression lambda)
        {
            if (lambda.Body.NodeType == ExpressionType.MemberAccess /*&& sep == " "*/)
            {
                MemberExpression m = lambda.Body as MemberExpression;

                if (m.Expression != null)
                {
                    string r = VisitMemberAccess(m).ToString();
                    return string.Format("{0}={1}", r, GetQuotedTrueValue());
                }

            }

            return Visit(lambda.Body);
        }

        protected virtual object VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && (m.Expression.NodeType == ExpressionType.Parameter || m.Expression.NodeType == ExpressionType.Convert))
            {
                var propertyInfo = (PropertyInfo)m.Member;

                var modelType = propertyInfo.DeclaringType;
                if (m.Expression.NodeType == ExpressionType.Convert)
                {
                    var unaryExpr = m.Expression as UnaryExpression;
                    if (unaryExpr != null)
                    {
                        modelType = unaryExpr.Operand.Type;
                    }
                }

                var fieldDefinitions = modelType.GetFieldDefinitions();//.GetModelDefinition();
                if (propertyInfo.PropertyType.IsEnum)
                    return new EnumMemberAccess(GetQuotedColumnName(fieldDefinitions, m.Member.Name), propertyInfo.PropertyType);

                return new PartialSqlString(GetQuotedColumnName(fieldDefinitions, m.Member.Name));
            }

            var member = System.Linq.Expressions.Expression.Convert(m, typeof(object));
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<object>>(member);
            var getter = lambda.Compile();
            return getter();
        }

        protected virtual object VisitBinary(BinaryExpression expression)
        {
            object left, right;
            var operand = BindOperant(expression.NodeType);   //sep= " " ??

            if (operand == "AND" || operand == "OR")
            {
                var m = expression.Left as MemberExpression;
                if (m != null && m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
                {
                    left = new PartialSqlString(string.Format("{0}={1}", VisitMemberAccess(m), GetQuotedTrueValue()));
                }
                else
                    left = Visit(expression.Left);

                m = expression.Right as MemberExpression;
                if (m != null && m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
                {
                    right = new PartialSqlString(string.Format("{0}={1}", VisitMemberAccess(m), GetQuotedTrueValue()));
                }
                else
                    right = Visit(expression.Right);

                if (left as PartialSqlString == null && right as PartialSqlString == null)
                {
                    var result = System.Linq.Expressions.Expression.Lambda(expression).Compile().DynamicInvoke();
                    return new PartialSqlString(DialectProvider.Instance.GetQuotedValue(result, result.GetType()));
                }

                if (left as PartialSqlString == null)
                    left = ((bool)left) ? GetTrueExpression() : GetFalseExpression();

                if (right as PartialSqlString == null)
                    right = ((bool)right) ? GetTrueExpression() : GetFalseExpression();
            }
            else
            {
                left = Visit(expression.Left);
                right = Visit(expression.Right);

                var leftEnum = left as EnumMemberAccess;
                var rightEnum = right as EnumMemberAccess;
                var rightNeedsCoercing = leftEnum != null && rightEnum == null;
                var leftNeedsCoercing = rightEnum != null && leftEnum == null;

                if (rightNeedsCoercing)
                {
                    var rightPartialSql = right as PartialSqlString;
                    if (rightPartialSql == null)
                    {
                        right = ConvertToEnum(leftEnum.EnumType, right.ToString(), right);
                    }
                }
                else if (leftNeedsCoercing)
                {
                    var leftPartialSql = left as PartialSqlString;
                    if (leftPartialSql == null)
                    {
                        left = ConvertToEnum(rightEnum.EnumType, left.ToString(), left);
                    }
                }
                else if (left as PartialSqlString == null && right as PartialSqlString == null)
                {
                    var result = System.Linq.Expressions.Expression.Lambda(expression).Compile().DynamicInvoke();
                    return result;
                }
                else if (left as PartialSqlString == null)
                {
                    left = DialectProvider.Instance.GetQuotedValue(left, left != null ? left.GetType() : null);
                }
                else if (right as PartialSqlString == null)
                {
                    right = DialectProvider.Instance.GetQuotedValue(right, right != null ? right.GetType() : null);
                }
            }

            if (operand == "=" && right.ToString().Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                operand = "is";
            }
            else if (operand == "<>" && right.ToString().Equals("null", StringComparison.OrdinalIgnoreCase))
                operand = "is not";

            switch (operand)
            {
                case "MOD":
                case "COALESCE":
                    return new PartialSqlString(string.Format("{0}({1},{2})", operand, left, right));

                default:
                    return new PartialSqlString("(" + left + sep + operand + sep + right + ")");
            }
        }

        protected virtual object VisitConstant(ConstantExpression c)
        {
            if (c.Value == null)
                return new PartialSqlString("null");

            return c.Value;
        }

        protected virtual object VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    var o = Visit(u.Operand);

                    if (o as PartialSqlString == null)
                        return !((bool)o);

                    if (IsFieldName(o))
                        o = o + "=" + GetQuotedTrueValue();

                    return new PartialSqlString("NOT (" + o + ")");

                case ExpressionType.Convert:
                    if (u.Method != null)
                        return System.Linq.Expressions.Expression.Lambda(u).Compile().DynamicInvoke();
                    break;
            }

            return Visit(u.Operand);

        }

        protected virtual string VisitParameter(ParameterExpression p)
        {
            return p.Name;
        }

        protected virtual object VisitMethodCall(MethodCallExpression m)
        {
            //if (m.Method.DeclaringType == typeof(Sql))
            //    return VisitSqlMethodCall(m);

            if (IsStaticArrayMethod(m))
                return VisitStaticArrayMethodCall(m);

            if (IsEnumerableMethod(m))
                return VisitEnumerableMethodCall(m);

            if (IsColumnAccess(m))
                return VisitColumnAccessMethod(m);

            return System.Linq.Expressions.Expression.Lambda(m).Compile().DynamicInvoke();
        }

        protected virtual object VisitNew(NewExpression nex)
        {
            // TODO : check !
            var member = System.Linq.Expressions.Expression.Convert(nex, typeof(object));
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<object>>(member);
            try
            {
                var getter = lambda.Compile();
                return getter();
            }
            catch (InvalidOperationException)
            { // FieldName ?
                var exprs = VisitExpressionList(nex.Arguments);
                var sb = new StringBuilder();
                foreach (var e in exprs)
                {
                    if (sb.Length > 0)
                        sb.Append(",");

                    sb.Append(e);
                }
                return sb.ToString();
            }
        }

        protected virtual object VisitNewArray(NewArrayExpression na)
        {
            var exprs = VisitExpressionList(na.Expressions);
            StringBuilder sb = new StringBuilder();
            foreach (var e in exprs)
            {
                sb.Append(sb.Length > 0 ? "," + e : e);
            }

            return sb.ToString();
        }

        protected virtual object VisitMemberInit(MemberInitExpression exp)
        {
            return Expression.Lambda(exp).Compile().DynamicInvoke();
        }

        protected virtual List<Object> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Object> list = new List<Object>();
            for (int i = 0, n = original.Count; i < n; i++)
            {
                if (original[i].NodeType == ExpressionType.NewArrayInit || original[i].NodeType == ExpressionType.NewArrayBounds)
                {
                    list.AddRange(VisitNewArrayFromExpressionList(original[i] as NewArrayExpression));
                }
                else
                    list.Add(Visit(original[i]));
            }

            return list;
        }

        protected virtual IList<Object> VisitNewArrayFromExpressionList(NewArrayExpression na)
        {
            return VisitExpressionList(na.Expressions);
        }

        //protected virtual object VisitSqlMethodCall(MethodCallExpression m)
        //{
        //    List<Object> args = this.VisitExpressionList(m.Arguments);
        //    object quotedColName = args[0];
        //    args.RemoveAt(0);

        //    string statement;

        //    switch (m.Method.Name)
        //    {
        //        case "In":

        //            var member = Expression.Convert(m.Arguments[1], typeof(object));
        //            var lambda = Expression.Lambda<Func<object>>(member);
        //            var getter = lambda.Compile();

        //            var inArgs = Flatten(getter() as IEnumerable);

        //            var sIn = new StringBuilder();
        //            foreach (object e in inArgs)
        //            {
        //                if (!(e is ICollection))
        //                {
        //                    if (sIn.Length > 0)
        //                        sIn.Append(",");

        //                    sIn.Append(DialectProvider.Instance.GetQuotedValue(e, e.GetType()));
        //                }
        //                else
        //                {
        //                    var listArgs = e as ICollection;
        //                    foreach (object el in listArgs)
        //                    {
        //                        if (sIn.Length > 0)
        //                            sIn.Append(",");

        //                        sIn.Append(DialectProvider.Instance.GetQuotedValue(el, el.GetType()));
        //                    }
        //                }
        //            }

        //            statement = string.Format("{0} {1} ({2})", quotedColName, m.Method.Name, sIn.ToString());
        //            break;
        //        case "Desc":
        //            statement = string.Format("{0} DESC", quotedColName);
        //            break;
        //        case "As":
        //            statement = string.Format("{0} As {1}", quotedColName,
        //                DialectProvider.Instance.GetQuotedColumnName(RemoveQuoteFromAlias(args[0].ToString())));
        //            break;
        //        case "Sum":
        //        case "Count":
        //        case "Min":
        //        case "Max":
        //        case "Avg":
        //            statement = string.Format("{0}({1}{2})",
        //                                 m.Method.Name,
        //                                 quotedColName,
        //                                 args.Count == 1 ? string.Format(",{0}", args[0]) : "");
        //            break;
        //        default:
        //            throw new NotSupportedException();
        //    }

        //    return new PartialSqlString(statement);
        //}

        protected virtual object VisitStaticArrayMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":
                    List<Object> args = this.VisitExpressionList(m.Arguments);
                    object quotedColName = args[1];

                    Expression memberExpr = m.Arguments[0];
                    if (memberExpr.NodeType == ExpressionType.MemberAccess)
                        memberExpr = (m.Arguments[0] as MemberExpression);

                    return ToInPartialString(memberExpr, quotedColName);

                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual object VisitEnumerableMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":
                    List<Object> args = this.VisitExpressionList(m.Arguments);
                    object quotedColName = args[0];
                    return ToInPartialString(m.Object, quotedColName);

                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual object VisitColumnAccessMethod(MethodCallExpression m)
        {
            List<Object> args = this.VisitExpressionList(m.Arguments);
            var quotedColName = Visit(m.Object);
            var statement = "";

            var wildcardArg = args.Count > 0 ? DialectProvider.Instance.EscapeWildcards(args[0].ToString()) : "";
            var escapeSuffix = wildcardArg.IndexOf('^') >= 0 ? " escape '^'" : "";
            switch (m.Method.Name)
            {
                case "Trim":
                    statement = string.Format("ltrim(rtrim({0}))", quotedColName);
                    break;

                case "LTrim":
                    statement = string.Format("ltrim({0})", quotedColName);
                    break;

                case "RTrim":
                    statement = string.Format("rtrim({0})", quotedColName);
                    break;

                case "ToUpper":
                    statement = string.Format("upper({0})", quotedColName);
                    break;

                case "ToLower":
                    statement = string.Format("lower({0})", quotedColName);
                    break;

                case "StartsWith":
                    if (!Configuration.StripUpperInLike)
                    {
                        statement = string.Format("upper({0}) like {1}{2}",
                            quotedColName, DialectProvider.Instance.GetQuotedValue(wildcardArg.ToUpper() + "%"), escapeSuffix);
                    }
                    else
                    {
                        statement = string.Format("{0} like {1}{2}",
                            quotedColName, DialectProvider.Instance.GetQuotedValue(wildcardArg + "%"), escapeSuffix);
                    }
                    break;

                case "EndsWith":
                    if (!Configuration.StripUpperInLike)
                    {
                        statement = string.Format("upper({0}) like {1}{2}",
                            quotedColName, DialectProvider.Instance.GetQuotedValue("%" + wildcardArg.ToUpper()), escapeSuffix);
                    }
                    else
                    {
                        statement = string.Format("{0} like {1}{2}",
                            quotedColName, DialectProvider.Instance.GetQuotedValue("%" + wildcardArg), escapeSuffix);
                    }
                    break;

                case "Contains":
                    if (!Configuration.StripUpperInLike)
                    {
                        statement = string.Format("upper({0}) like {1}{2}",
                            quotedColName, DialectProvider.Instance.GetQuotedValue("%" + wildcardArg.ToUpper() + "%"), escapeSuffix);
                    }
                    else
                    {
                        statement = string.Format("{0} like {1}{2}",
                            quotedColName, DialectProvider.Instance.GetQuotedValue("%" + wildcardArg + "%"), escapeSuffix);
                    }
                    break;

                case "Substring":
                    var startIndex = Int32.Parse(args[0].ToString()) + 1;
                    if (args.Count == 2)
                    {
                        var length = Int32.Parse(args[1].ToString());
                        statement = string.Format("substring({0} from {1} for {2})", quotedColName, startIndex, length);
                    }
                    else
                        statement = string.Format("substring({0} from {1})", quotedColName, startIndex);
                    break;

                default:
                    throw new NotSupportedException();
            }

            return new PartialSqlString(statement);
        }


        protected string BindOperant(ExpressionType e)
        {
            switch (e)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "MOD";
                case ExpressionType.Coalesce:
                    return "COALESCE";
                default:
                    return e.ToString();
            }
        }

        protected PartialSqlString GetTrueExpression()
        {
            return new PartialSqlString(string.Format("({0}={1})", GetQuotedTrueValue(), GetQuotedTrueValue()));
        }

        protected PartialSqlString GetFalseExpression()
        {
            return new PartialSqlString(string.Format("({0}={1})", GetQuotedTrueValue(), GetQuotedFalseValue()));
        }

        protected PartialSqlString GetQuotedTrueValue()
        {
            return new PartialSqlString(DialectProvider.Instance.GetQuotedValue(true, typeof(bool)));
        }

        protected PartialSqlString GetQuotedFalseValue()
        {
            return new PartialSqlString(DialectProvider.Instance.GetQuotedValue(false, typeof(bool)));
        }

        private string ConvertToEnum(Type enumType, string enumStr, object otherExpr)
        {
            //enum value was returned by Visit(b.Right)
            long numvericVal;
            var result = Int64.TryParse(enumStr, out numvericVal)
                ? DialectProvider.Instance.GetQuotedValue(Enum.ToObject(enumType, numvericVal).ToString(), typeof(string))
                : DialectProvider.Instance.GetQuotedValue(otherExpr, otherExpr.GetType());
            return result;
        }

        private object ToInPartialString(Expression memberExpr, object quotedColName)
        {
            var member = Expression.Convert(memberExpr, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            var getter = lambda.Compile();

            var inArgs = Flatten(getter() as IEnumerable);

            var sb = new StringBuilder();
            if (inArgs.Count > 0)
            {
                foreach (object e in inArgs)
                {
                    if (sb.Length > 0)
                        sb.Append(",");

                    sb.Append(DialectProvider.Instance.GetQuotedValue(e, e.GetType()));
                }
            }
            else
            {
                sb.Append("NULL");
            }

            var statement = string.Format("{0} {1} ({2})", quotedColName, "In", sb);
            return new PartialSqlString(statement);
        }

        public static IList<object> Flatten(IEnumerable list)
        {
            var ret = new List<object>();
            if (list == null) 
                return ret;

            foreach (var item in list)
            {
                if (item == null) continue;

                var arr = item as IEnumerable;
                if (arr != null && !(item is string))
                {
                    ret.AddRange(arr.Cast<object>());
                }
                else
                {
                    ret.Add(item);
                }
            }

            return ret;
        }

        protected string RemoveQuoteFromAlias(string exp)
        {

            if ((exp.StartsWith("\"") || exp.StartsWith("`") || exp.StartsWith("'")) && (exp.EndsWith("\"") || exp.EndsWith("`") || exp.EndsWith("'")))
            {
                exp = exp.Remove(0, 1);
                exp = exp.Remove(exp.Length - 1, 1);
            }

            return exp;
        }






        protected bool IsFieldName(object quotedExp)
        {
            //FieldDefinition fd = modelDef.FieldDefinitions.FirstOrDefault(x => DialectProvider.Instance.GetQuotedColumnName(x.FieldName) == quotedExp.ToString());
            FieldDefinition fd = typeof(T).GetFieldDefinitions().FirstOrDefault(x => DialectProvider.Instance.GetQuotedColumnName(x.FieldName) == quotedExp.ToString());
            return (fd != default(FieldDefinition));
        }

        private bool IsStaticArrayMethod(MethodCallExpression m)
        {
            if (m.Object == null && m.Method.Name == "Contains")
            {
                return m.Arguments.Count == 2;
            }

            return false;
        }

        private bool IsEnumerableMethod(MethodCallExpression m)
        {
            if (m.Object != null
                && m.Object.Type.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>))
                && m.Object.Type != typeof(string)
                && m.Method.Name == "Contains")
            {
                return m.Arguments.Count == 1;
            }

            return false;
        }

        private bool IsColumnAccess(MethodCallExpression m)
        {
            if (m.Object != null && m.Object as MethodCallExpression != null)
                return IsColumnAccess(m.Object as MethodCallExpression);

            var exp = m.Object as MemberExpression;
            return exp != null
                && exp.Expression != null
                && exp.Expression.Type == typeof(T)
                && exp.Expression.NodeType == ExpressionType.Parameter;
        }






        

        

        







        public class BaseDialectProvider
        {
            public string StringLengthNonUnicodeColumnDefinitionFormat = "VARCHAR({0})";
            public string StringLengthUnicodeColumnDefinitionFormat = "NVARCHAR({0})";

            public string StringColumnDefinition;
            public string StringLengthColumnDefinitionFormat;

            protected bool CompactGuid;
            internal const string StringGuidDefinition = "VARCHAR2(37)";

            protected bool _useUnicode;
            public virtual bool UseUnicode
            {
                get
                {
                    return _useUnicode;
                }
                set
                {
                    _useUnicode = value;
                    UpdateStringColumnDefinitions();
                }
            }

            private int _defaultStringLength = 8000; //SqlServer express limit
            public int DefaultStringLength
            {
                get
                {
                    return _defaultStringLength;
                }
                set
                {
                    _defaultStringLength = value;
                    UpdateStringColumnDefinitions();
                }
            }

            private string maxStringColumnDefinition;
            public string MaxStringColumnDefinition
            {
                get { return maxStringColumnDefinition ?? StringColumnDefinition; }
                set { maxStringColumnDefinition = value; }
            }




            public BaseDialectProvider()
            {
                UpdateStringColumnDefinitions();
            }




            public virtual void UpdateStringColumnDefinitions()
            {
                this.StringLengthColumnDefinitionFormat = UseUnicode ? StringLengthUnicodeColumnDefinitionFormat : StringLengthNonUnicodeColumnDefinitionFormat;

                this.StringColumnDefinition = string.Format(StringLengthColumnDefinitionFormat, DefaultStringLength);
            }

            public virtual string GetQuotedValue(object value, Type fieldType)
            {
                if (value == null) 
                    return "NULL";

                //var dialectProvider = OrmLiteConfig.DialectProvider;
                //if (fieldType.IsRefType())
                //{
                //    return dialectProvider.GetQuotedValue(dialectProvider.StringSerializer.SerializeToString(value));
                //}

                var typeCode = fieldType.GetTypeCode();
                switch (typeCode)
                {
                    case TypeCode.Single:
                        return ((float)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Double:
                        return ((double)value).ToString(CultureInfo.InvariantCulture);
                    case TypeCode.Decimal:
                        return ((decimal)value).ToString(CultureInfo.InvariantCulture);

                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        if (fieldType.IsNumericType())
                            return Convert.ChangeType(value, fieldType).ToString();
                        break;
                }

                if (fieldType == typeof(TimeSpan))
                    return ((TimeSpan)value).Ticks.ToString(CultureInfo.InvariantCulture);

                return ShouldQuoteValue(fieldType) ? DialectProvider.Instance.GetQuotedValue(value.ToString()) : value.ToString();
            }


            public string AutoIncrementDefinition = "AUTOINCREMENT"; //SqlServer express limit
            public string IntColumnDefinition = "INTEGER";
            public string LongColumnDefinition = "BIGINT";
            public string GuidColumnDefinition = "GUID";
            public string BoolColumnDefinition = "BOOL";
            public string RealColumnDefinition = "DOUBLE";
            public string DecimalColumnDefinition = "DECIMAL";
            public string BlobColumnDefinition = "BLOB";
            public string DateTimeColumnDefinition = "DATETIME";
            public string TimeColumnDefinition = "BIGINT";
            public string DateTimeOffsetColumnDefinition = "DATETIMEOFFSET";

            public virtual bool ShouldQuoteValue(Type fieldType)
            {
                string fieldDefinition;
                //if (!DbTypeMap.ColumnTypeMap.TryGetValue(fieldType, out fieldDefinition))
                //{
                fieldDefinition = this.GetUndefinedColumnDefinition(fieldType, null);
                //}

                return fieldDefinition != IntColumnDefinition
                       && fieldDefinition != LongColumnDefinition
                       && fieldDefinition != RealColumnDefinition
                       && fieldDefinition != DecimalColumnDefinition
                       && fieldDefinition != BoolColumnDefinition;
            }

            protected virtual string GetUndefinedColumnDefinition(Type fieldType, int? fieldLength)
            {
                return fieldLength.HasValue
                    ? string.Format(StringLengthColumnDefinitionFormat, fieldLength.GetValueOrDefault(DefaultStringLength))
                    : MaxStringColumnDefinition;
            }

            public virtual string GetQuotedValue(string paramValue)
            {
                return "'" + paramValue.Replace("'", "''") + "'";
            }

            public virtual string GetQuotedColumnName(string columnName)
            {
                return string.Format("\"{0}\"", columnName/*namingStrategy.GetColumnName(columnName)*/);
            }

            public virtual string GetQuotedColumnName(string tableName, string fieldName)
            {
                //return dialect.GetQuotedTableName(tableName) + "." + dialect.GetQuotedColumnName(fieldName);
                return tableName + "." + fieldName;
            }

            public virtual string EscapeWildcards(string value)
            {
                if (value == null)
                    return null;

                return value
                    .Replace("^", @"^^")
                    .Replace(@"\", @"^\")
                    .Replace("_", @"^_")
                    .Replace("%", @"^%");
            }
        }


        public class DialectProvider : BaseDialectProvider
        {
            static DialectProvider _instance;
            public static DialectProvider Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new DialectProvider();
                    return _instance;
                }
            }

            public override string GetQuotedValue(object value, Type fieldType)
            {
                if (value == null) return "NULL";

                if (fieldType == typeof(Guid))
                {
                    var guid = (Guid)value;
                    return CompactGuid ? "'" + BitConverter.ToString(guid.ToByteArray()).Replace("-", "") + "'"
                                       : string.Format("CAST('{0}' AS {1})", guid, StringGuidDefinition);
                }

                if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
                {
                    var dateValue = (DateTime)value;
                    string iso8601Format = "yyyy-MM-dd";
                    string oracleFormat = "YYYY-MM-DD";
                    if (dateValue.ToString("yyyy-MM-dd HH:mm:ss.fff").EndsWith("00:00:00.000") == false)
                    {
                        iso8601Format = "yyyy-MM-dd HH:mm:ss.fff";
                        oracleFormat = "YYYY-MM-DD HH24:MI:SS.FF3";
                    }
                    return "TO_TIMESTAMP(" + base.GetQuotedValue(dateValue.ToString(iso8601Format), typeof(string)) + ", " + base.GetQuotedValue(oracleFormat, typeof(string)) + ")";
                }

                if ((value is TimeSpan) && (fieldType == typeof(Int64) || fieldType == typeof(Int64?)))
                {
                    var longValue = ((TimeSpan)value).Ticks;
                    return base.GetQuotedValue(longValue, fieldType);
                }

                if (fieldType == typeof(bool?) || fieldType == typeof(bool))
                {
                    var boolValue = (bool)value;
                    return base.GetQuotedValue(boolValue ? "1" : "0", typeof(string));
                }

                if (fieldType == typeof(decimal?) || fieldType == typeof(decimal) ||
                    fieldType == typeof(double?) || fieldType == typeof(double) ||
                    fieldType == typeof(float?) || fieldType == typeof(float))
                {
                    var s = base.GetQuotedValue(value, fieldType);
                    if (s.Length > 20) s = s.Substring(0, 20);
                    return "'" + s + "'"; // when quoted exception is more clear!
                }

                return base.GetQuotedValue(value, fieldType);
            }
        }
    }
}
