using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace dotnet_gqlgen
{
    /// <summary>
    ///     Base query type
    /// </summary>
    public abstract class BaseGraphQlClient
    {
        /// <summary>
        ///     Make a <paramref name="query" /> string
        /// </summary>
        /// <typeparam name="TSchema"></typeparam>
        /// <typeparam name="TQuery"></typeparam>
        /// <param name="query"></param>
        /// <param name="mutation"></param>
        /// <exception cref="ArgumentException">Condition.</exception>
        /// <returns>
        /// </returns>
        protected string MakeQuery<TSchema, TQuery>(Expression<Func<TSchema, TQuery>> query, bool mutation = false)
        {
            var gql = new StringBuilder();
            gql.AppendLine($"{(mutation ? "mutation" : "query")} BaseGraphQLClient {{");

            if (query.NodeType != ExpressionType.Lambda)
                throw new ArgumentException("Must provide a LambdaExpression", "query");
            var lambda = (LambdaExpression) query;

            if (lambda.Body.NodeType != ExpressionType.New && lambda.Body.NodeType != ExpressionType.MemberInit)
                throw new ArgumentException("LambdaExpression must return a NewExpression or MemberInitExpression");

            BaseGraphQlClient.GetObjectSelection(gql, lambda.Body);

            gql.Append(@"}");
            return gql.ToString();
        }

        /// <summary>
        ///     Get's an object
        /// </summary>
        /// <param name="gql"></param>
        /// <param name="exp"></param>
        private static void GetObjectSelection(StringBuilder gql, Expression exp)
        {
            if (exp.NodeType == ExpressionType.New)
            {
                var newExp = (NewExpression) exp;
                for (var i = 0; i < newExp.Arguments.Count; i++)
                {
                    var fieldVal = newExp.Arguments[i];
                    var fieldProp = newExp.Members[i];
                    gql.AppendLine($"{fieldProp.Name}: {BaseGraphQlClient.GetFieldSelection(fieldVal)}");
                }
            }
            else
            {
                var mi = (MemberInitExpression) exp;
                foreach (var binding in mi.Bindings)
                {
                    var valExp = ((MemberAssignment) binding).Expression;
                    gql.AppendLine($"{binding.Member.Name}: {BaseGraphQlClient.GetFieldSelection(valExp)}");
                }
            }
        }

        /// <summary>
        ///     Get a <paramref name="field" />
        /// </summary>
        /// <param name="field"></param>
        /// <returns>
        /// </returns>
        private static string GetFieldSelection(Expression field)
        {
            switch (field.NodeType)
            {
                case ExpressionType.MemberAccess:
                {
                    var member = ((MemberExpression) field).Member;
                    var attribute = member.GetCustomAttributes(typeof(GqlFieldNameAttribute))
                        .Cast<GqlFieldNameAttribute>()
                        .FirstOrDefault();
                    return attribute != null ? attribute.Name : member.Name;
                }
                case ExpressionType.Call:
                {
                    var call = (MethodCallExpression) field;
                    return BaseGraphQlClient.GetSelectionFromMethod(call);
                }
                case ExpressionType.Quote:
                    return BaseGraphQlClient.GetFieldSelection(((UnaryExpression) field).Operand);
                default:
                    throw new ArgumentException(
                        "Field expression should be a call or member access expression",
                        "field");
            }
        }

        /// <summary>
        ///     Get a method
        /// </summary>
        /// <param name="call"></param>
        /// <returns>
        /// </returns>
        private static string GetSelectionFromMethod(MethodCallExpression call)
        {
            var select = new StringBuilder();

            var attribute = call.Method.GetCustomAttributes(typeof(GqlFieldNameAttribute))
                .Cast<GqlFieldNameAttribute>()
                .FirstOrDefault();
            select.Append(attribute != null ? attribute.Name : call.Method.Name);

            if (call.Arguments.Count > 1)
            {
                var argVals = new List<object>();
                for (var i = 0; i < call.Arguments.Count - 1; i++)
                {
                    var arg = call.Arguments.ElementAt(i);
                    var param = call.Method.GetParameters().ElementAt(i);
                    Type argType = null;
                    object argVal = null;
                    if (arg.NodeType == ExpressionType.Convert) arg = ((UnaryExpression) arg).Operand;

                    switch (arg.NodeType)
                    {
                        case ExpressionType.Constant:
                        {
                            var constArg = (ConstantExpression) arg;
                            argType = constArg.Type;
                            argVal = constArg.Value;
                            break;
                        }
                        case ExpressionType.MemberAccess:
                        {
                            var ma = (MemberExpression) arg;
                            var ce = (ConstantExpression) ma.Expression;
                            argType = ma.Type;
                            argVal = ma.Member.MemberType == MemberTypes.Field
                                ? ((FieldInfo) ma.Member).GetValue(ce.Value)
                                : ((PropertyInfo) ma.Member).GetValue(ce.Value);
                            break;
                        }
                        case ExpressionType.New:
                            argVal = Expression.Lambda(arg).Compile().DynamicInvoke();
                            argType = argVal.GetType();
                            break;
                        default:
                            throw new Exception($"Unsupported argument type {arg.NodeType}");
                    }

                    if (argVal == null) continue;
                    if (argType == typeof(string) || argType == typeof(Guid) || argType == typeof(Guid?))
                        argVals.Add($"{param.Name}: \"{argVal}\"");
                    else if (argType == typeof(DateTime) || argType == typeof(DateTime?))
                        argVals.Add($"{param.Name}: \"{((DateTime) argVal).ToString("o")}\"");
                    else
                        argVals.Add($"{param.Name}: {argVal}");
                }

                ;
                if (argVals.Any()) select.Append($"({string.Join(", ", argVals)})");
            }

            select.Append(" {\n");
            if (call.Arguments.Count == 0)
            {
                select.Append(
                    call.Method.ReturnType.GetInterfaces()
                        .Select(i => i.GetTypeInfo().GetGenericTypeDefinition())
                        .Contains(typeof(IEnumerable<>))
                        ? BaseGraphQlClient.GetDefaultSelection(call.Method.ReturnType.GetGenericArguments().First())
                        : BaseGraphQlClient.GetDefaultSelection(call.Method.ReturnType));
            }
            else
            {
                var exp = call.Arguments.Last();
                if (exp.NodeType == ExpressionType.Quote) exp = ((UnaryExpression) exp).Operand;
                if (exp.NodeType == ExpressionType.Lambda) exp = ((LambdaExpression) exp).Body;
                BaseGraphQlClient.GetObjectSelection(select, exp);
            }

            select.Append("}");
            return select.ToString();
        }

        /// <summary>
        ///     Default selection
        /// </summary>
        /// <param name="returnType"></param>
        /// <returns>
        /// </returns>
        private static string GetDefaultSelection(Type returnType)
        {
            var select = new StringBuilder();
            foreach (var field in returnType.GetProperties())
            {
                var name = field.Name;
                var attribute = field.GetCustomAttributes(typeof(GqlFieldNameAttribute))
                    .Cast<GqlFieldNameAttribute>()
                    .FirstOrDefault();
                if (attribute != null) name = attribute.Name;

                select.AppendLine($"{field.Name}: {name}");
            }

            return select.ToString();
        }
    }
}
