using System;
using System.Diagnostics.CodeAnalysis;
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

        public static bool TryParseValue(string str, Type type, [MaybeNullWhen(false)]out object value)
        {
            value = Type.GetTypeCode(type) switch
            {
                TypeCode.String => str,
                TypeCode.Char => char.Parse(str),
                TypeCode.Byte => byte.Parse(str),
                TypeCode.SByte => sbyte.Parse(str),
                TypeCode.Int16 => short.Parse(str),
                TypeCode.UInt16 => ushort.Parse(str),
                TypeCode.Int32 => int.Parse(str),
                TypeCode.UInt32 => uint.Parse(str),
                TypeCode.Int64 => long.Parse(str),
                TypeCode.UInt64 => ulong.Parse(str),
                TypeCode.Single => float.Parse(str, CultureInfo.InvariantCulture),
                TypeCode.Double => double.Parse(str, CultureInfo.InvariantCulture),
                TypeCode.Decimal => decimal.Parse(str, CultureInfo.InvariantCulture),
                TypeCode.Boolean =>
                    str.Equals("yes", StringComparison.InvariantCultureIgnoreCase) ? true :
                    str.Equals("no", StringComparison.InvariantCultureIgnoreCase) ? false :
                    bool.Parse(str),
                _ => null
            };

            return value != null;
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
