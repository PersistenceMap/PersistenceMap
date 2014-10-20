using PersistanceMap.Internals;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace PersistanceMap.Compiler
{
    public class LambdaExpressionToSqlCompiler
    {
        //TODO: separator has to be set as an option!
        readonly string _separator = " ";

        //TODO: useFieldName has to be set as an option!
        readonly bool _useFieldName = true;
        private Dictionary<Type, string> _aliasMap;

        public bool PrefixFieldWithTableName { get; set; }

        static LambdaExpressionToSqlCompiler instance;
        public static LambdaExpressionToSqlCompiler Instance
        {
            get
            {
                if (instance == null)
                    instance = new LambdaExpressionToSqlCompiler();
                return instance;
            }
        }


        public LambdaExpressionToSqlCompiler()
        {
            PrefixFieldWithTableName = true;
            _aliasMap = new Dictionary<Type, string>();
        }

        #region Compilers

        internal virtual object Compile(IExpressionQueryPart part)
        {
            _aliasMap = part.AliasMap;

            return Compile(part.Expression);
        }
        
        internal virtual object Compile(Expression exp)
        {
            if (exp == null) 
                return string.Empty;

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return CompileLambda(exp as LambdaExpression);
                case ExpressionType.MemberAccess:
                    return CompileMemberAccess(exp as MemberExpression);
                case ExpressionType.Constant:
                    return CompileConstant(exp as ConstantExpression);
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
                    return CompileBinary(exp as BinaryExpression);
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return CompileUnary(exp as UnaryExpression);
                case ExpressionType.Parameter:
                    return CompileParameter(exp as ParameterExpression);
                case ExpressionType.Call:
                    return CompileMethodCall(exp as MethodCallExpression);
                case ExpressionType.New:
                    return CompileNew(exp as NewExpression);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return CompileNewArray(exp as NewArrayExpression);
                case ExpressionType.MemberInit:
                    return CompileMemberInit(exp as MemberInitExpression);
                default:
                    return exp.ToString();
            }
        }

        protected virtual object CompileLambda(LambdaExpression lambda)
        {
            if (lambda.Body.NodeType == ExpressionType.MemberAccess /*&& sep == " "*/)
            {
                var m = lambda.Body as MemberExpression;

                if (m.Expression != null)
                {
                    string r = CompileMemberAccess(m).ToString();
                    //return string.Format("{0}={1}", r, GetQuotedTrueValue());
                    return r;
                }
            }

            return Compile(lambda.Body);
        }

        protected virtual object CompileMemberAccess(MemberExpression m)
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

                var fieldDefinitions = modelType.GetFieldDefinitions();
                if (propertyInfo.PropertyType.IsEnum)
                    return new EnumMemberAccess(GetQuotedColumnName(fieldDefinitions, m.Member.Name), propertyInfo.PropertyType);

                return new PartialSqlString(GetQuotedColumnName(fieldDefinitions, m.Member.Name));
            }

            var member = Expression.Convert(m, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            var getter = lambda.Compile();
            return getter();
        }

        protected virtual object CompileBinary(BinaryExpression expression)
        {
            object left, right;
            var operand = BindOperant(expression.NodeType);   //sep= " " ??

            if (operand == "AND" || operand == "OR")
            {
                var m = expression.Left as MemberExpression;
                if (m != null && m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
                {
                    left = new PartialSqlString(string.Format("{0}={1}", CompileMemberAccess(m), GetQuotedTrueValue()));
                }
                else
                    left = Compile(expression.Left);

                m = expression.Right as MemberExpression;
                if (m != null && m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
                {
                    right = new PartialSqlString(string.Format("{0}={1}", CompileMemberAccess(m), GetQuotedTrueValue()));
                }
                else
                    right = Compile(expression.Right);

                if (left as PartialSqlString == null && right as PartialSqlString == null)
                {
                    var result = Expression.Lambda(expression).Compile().DynamicInvoke();
                    return new PartialSqlString(DialectProvider.Instance.GetQuotedValue(result, result.GetType()));
                }

                if (left as PartialSqlString == null)
                    left = ((bool)left) ? GetTrueExpression() : GetFalseExpression();

                if (right as PartialSqlString == null)
                    right = ((bool)right) ? GetTrueExpression() : GetFalseExpression();
            }
            else
            {
                left = Compile(expression.Left);
                right = Compile(expression.Right);

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
                    var result = Expression.Lambda(expression).Compile().DynamicInvoke();
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
                    return new PartialSqlString("(" + left + _separator + operand + _separator + right + ")");
            }
        }

        protected virtual object CompileConstant(ConstantExpression c)
        {
            if (c.Value == null)
                return new PartialSqlString("null");

            return c.Value;
        }

        protected virtual object CompileUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    var o = Compile(u.Operand);

                    if (o as PartialSqlString == null)
                        return !((bool)o);

                    if (IsFieldName(o))
                        o = o + "=" + GetQuotedTrueValue();

                    return new PartialSqlString("NOT (" + o + ")");

                case ExpressionType.Convert:
                    if (u.Method != null)
                        return Expression.Lambda(u).Compile().DynamicInvoke();
                    break;
            }

            return Compile(u.Operand);
        }

        protected virtual string CompileParameter(ParameterExpression p)
        {
            return p.Name;
        }

        protected virtual object CompileMethodCall(MethodCallExpression m)
        {
            //if (m.Method.DeclaringType == typeof(Sql))
            //    return CompileSqlMethodCall(m);

            if (IsStaticArrayMethod(m))
                return CompileStaticArrayMethodCall(m);

            if (IsEnumerableMethod(m))
                return CompileEnumerableMethodCall(m);

            if (IsColumnAccess(m))
                return CompileColumnAccessMethod(m);

            return Expression.Lambda(m).Compile().DynamicInvoke();
        }

        protected virtual object CompileNew(NewExpression nex)
        {
            // TODO : check !
            var member = Expression.Convert(nex, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            try
            {
                var getter = lambda.Compile();
                return getter();
            }
            catch (InvalidOperationException)
            { 
                // FieldName ?
                var exprs = CompileExpressionList(nex.Arguments);
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

        protected virtual object CompileNewArray(NewArrayExpression na)
        {
            var exprs = CompileExpressionList(na.Expressions);
            var sb = new StringBuilder();
            foreach (var e in exprs)
            {
                sb.Append(sb.Length > 0 ? "," + e : e);
            }

            return sb.ToString();
        }

        protected virtual object CompileMemberInit(MemberInitExpression exp)
        {
            return Expression.Lambda(exp).Compile().DynamicInvoke();
        }

        protected virtual List<object> CompileExpressionList(ReadOnlyCollection<Expression> original)
        {
            var list = new List<object>();
            for (int i = 0, n = original.Count; i < n; i++)
            {
                if (original[i].NodeType == ExpressionType.NewArrayInit || original[i].NodeType == ExpressionType.NewArrayBounds)
                {
                    list.AddRange(CompileNewArrayFromExpressionList(original[i] as NewArrayExpression));
                }
                else
                    list.Add(Compile(original[i]));
            }

            return list;
        }

        protected virtual IList<object> CompileNewArrayFromExpressionList(NewArrayExpression na)
        {
            return CompileExpressionList(na.Expressions);
        }

        //protected virtual object CompileSqlMethodCall(MethodCallExpression m)
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

        protected virtual object CompileStaticArrayMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":
                    var args = CompileExpressionList(m.Arguments);
                    object quotedColName = args[1];

                    Expression memberExpr = m.Arguments[0];
                    if (memberExpr.NodeType == ExpressionType.MemberAccess)
                        memberExpr = (m.Arguments[0] as MemberExpression);

                    return ToInPartialString(memberExpr, quotedColName);

                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual object CompileEnumerableMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":
                    var args = CompileExpressionList(m.Arguments);
                    object quotedColName = args[0];
                    return ToInPartialString(m.Object, quotedColName);

                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual object CompileColumnAccessMethod(MethodCallExpression m)
        {
            var args = CompileExpressionList(m.Arguments);
            var quotedColName = Compile(m.Object);
            var statement = string.Empty;

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
                        statement = string.Format("upper({0}) like {1}{2}", quotedColName, DialectProvider.Instance.GetQuotedValue(wildcardArg.ToUpper() + "%"), escapeSuffix);
                    }
                    else
                    {
                        statement = string.Format("{0} like {1}{2}", quotedColName, DialectProvider.Instance.GetQuotedValue(wildcardArg + "%"), escapeSuffix);
                    }
                    break;

                case "EndsWith":
                    if (!Configuration.StripUpperInLike)
                    {
                        statement = string.Format("upper({0}) like {1}{2}", quotedColName, DialectProvider.Instance.GetQuotedValue("%" + wildcardArg.ToUpper()), escapeSuffix);
                    }
                    else
                    {
                        statement = string.Format("{0} like {1}{2}", quotedColName, DialectProvider.Instance.GetQuotedValue("%" + wildcardArg), escapeSuffix);
                    }
                    break;

                case "Contains":
                    if (!Configuration.StripUpperInLike)
                    {
                        statement = string.Format("upper({0}) like {1}{2}", quotedColName, DialectProvider.Instance.GetQuotedValue("%" + wildcardArg.ToUpper() + "%"), escapeSuffix);
                    }
                    else
                    {
                        statement = string.Format("{0} like {1}{2}", quotedColName, DialectProvider.Instance.GetQuotedValue("%" + wildcardArg + "%"), escapeSuffix);
                    }
                    break;

                case "Substring":
                    var startIndex = Int32.Parse(args[0].ToString()) + 1;
                    if (args.Count == 2)
                    {
                        var length = int.Parse(args[1].ToString());
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

        #endregion

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

        protected virtual string GetQuotedColumnName(IEnumerable<FieldDefinition> tableDef, string memberName)
        {
            if (_useFieldName)
            {
                var fd = tableDef.FirstOrDefault(x => x.MemberName == memberName);
                var fieldName = fd != null ? fd.FieldName : memberName;

                // get the prefix from the maptable
                var id = string.Empty;
                if (!_aliasMap.TryGetValue(fd.EntityType, out id))
                    id = fd.EntityName;

                // set the entityname as prefix
                if(string.IsNullOrEmpty(id))
                    id = fd.EntityName;

                return PrefixFieldWithTableName
                    ? DialectProvider.Instance.GetQuotedColumnName(id, fieldName)
                    : DialectProvider.Instance.GetQuotedColumnName(fieldName);
            }

            return memberName;
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

        protected virtual bool IsFieldName(object quotedExp)
        {
            return false;
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

        protected virtual bool IsColumnAccess(MethodCallExpression m)
        {
            if (m.Object != null && m.Object as MethodCallExpression != null)
                return IsColumnAccess(m.Object as MethodCallExpression);

            var exp = m.Object as MemberExpression;
            return exp != null
                && exp.Expression != null
                /*&& exp.Expression.Type == typeof(T)*/
                && exp.Expression.NodeType == ExpressionType.Parameter;
        }
    }

    //public class LambdaExpressionToSqlCompiler<T> : LambdaExpressionToSqlCompiler
    //{
    //    protected override bool IsFieldName(object quotedExp)
    //    {
    //        //FieldDefinition fd = modelDef.FieldDefinitions.FirstOrDefault(x => DialectProvider.Instance.GetQuotedColumnName(x.FieldName) == quotedExp.ToString());
    //        FieldDefinition fd = TypeDefinitionFactory.GetFieldDefinitions<T>().FirstOrDefault(x => DialectProvider.Instance.GetQuotedColumnName(x.FieldName) == quotedExp.ToString());
    //        return (fd != default(FieldDefinition));
    //    }

    //    protected override bool IsColumnAccess(MethodCallExpression m)
    //    {
    //        if (m.Object != null && m.Object as MethodCallExpression != null)
    //            return IsColumnAccess(m.Object as MethodCallExpression);

    //        var exp = m.Object as MemberExpression;
    //        return exp != null
    //            && exp.Expression != null
    //            && exp.Expression.Type == typeof(T)
    //            && exp.Expression.NodeType == ExpressionType.Parameter;
    //    }
    //}
}
