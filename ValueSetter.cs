using System;
using System.Globalization;
using System.Linq.Expressions;

namespace LogicCommandLineParser
{
    internal static class ValueSetter
    {
        public static void SetValue(object target, Expression memberExpr, object value)
        {
            var type = target.GetType();

            var objParam = Expression.Parameter(typeof(object), "obj");

            memberExpr = new MyExpressionVisitor(Expression.Convert(objParam, type)).Visit(memberExpr);

            Expression.Lambda<Action<object>>(Expression.Assign(memberExpr, Expression.Constant(value)), objParam).Compile()(target);
        }

        public static object ParseValue(string str, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return str;
                case TypeCode.Char:
                    return char.Parse(str);
                case TypeCode.Byte:
                    return byte.Parse(str);
                case TypeCode.SByte:
                    return sbyte.Parse(str);
                case TypeCode.Int16:
                    return short.Parse(str);
                case TypeCode.UInt16:
                    return ushort.Parse(str);
                case TypeCode.Int32:
                    return int.Parse(str);
                case TypeCode.UInt32:
                    return uint.Parse(str);
                case TypeCode.Int64:
                    return long.Parse(str);
                case TypeCode.UInt64:
                    return ulong.Parse(str);
                case TypeCode.Single:
                    return float.Parse(str, CultureInfo.InvariantCulture);
                case TypeCode.Double:
                    return double.Parse(str, CultureInfo.InvariantCulture);
                case TypeCode.Decimal:
                    return decimal.Parse(str, CultureInfo.InvariantCulture);
                case TypeCode.Boolean:
                    if (str.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                        return true;
                    if (str.Equals("no", StringComparison.InvariantCultureIgnoreCase))
                        return false;

                    return bool.Parse(str);
            }

            throw new BuilderException($"Cannot parse type {type.Name}");
        }

        private class MyExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression NewParameter;

            public MyExpressionVisitor(Expression newParameter)
            {
                this.NewParameter = newParameter ?? throw new ArgumentNullException(nameof(newParameter));
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return NewParameter;
            }
        }
    }
}
