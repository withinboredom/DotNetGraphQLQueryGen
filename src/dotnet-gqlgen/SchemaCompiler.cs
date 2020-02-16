using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using GraphQLSchema.Grammer;

namespace dotnet_gqlgen
{
    /// <summary>
    ///     The schema compiler
    /// </summary>
    public static class SchemaCompiler
    {
        /// <summary>
        ///     Compiles the specified schema text.
        /// </summary>
        /// <param name="schemaText">The schema text.</param>
        /// <param name="typeMappings">The type mappings.</param>
        /// <exception cref="SchemaException">
        ///     A schema definition is required and must define the query type e.g.
        ///     \"schema { query: MyQueryType }\" or Error: line
        ///     {nve.OffendingToken.Line}:{nve.OffendingToken.Column} no viable
        ///     alternative at input '{nve.OffendingToken.Text}' or Error: line
        ///     {ime.OffendingToken.Line}:{ime.OffendingToken.Column} extraneous
        ///     input '{ime.OffendingToken.Text}' expecting {expecting} or or
        /// </exception>
        /// <returns>
        /// </returns>
        public static SchemaInfo Compile(string schemaText, Dictionary<string, string> typeMappings = null)
        {
            try
            {
                var stream = new AntlrInputStream(schemaText);
                var lexer = new GraphQLSchemaLexer(stream);
                var tokens = new CommonTokenStream(lexer);
                var parser =
                    new GraphQLSchemaParser(tokens) {BuildParseTree = true, ErrorHandler = new BailErrorStrategy()};

                var tree = parser.document();
                var visitor = new SchemaVisitor(typeMappings);
                // visit each node. it will return a linq expression for each entity requested
                visitor.Visit(tree);

                if (visitor.SchemaInfo.Schema == null || visitor.SchemaInfo.Schema.All(f => f.Name != "query"))
                    throw new SchemaException(
                        "A schema definition is required and must define the query type e.g. \"schema { query: MyQueryType }\"");

                return visitor.SchemaInfo;
            }
            catch (ParseCanceledException pce)
            {
                switch (pce.InnerException)
                {
                    case null:
                        throw new SchemaException(pce.Message, pce);
                    case NoViableAltException exception:
                    {
                        var nve = exception;
                        throw new SchemaException(
                            $"Error: line {nve.OffendingToken.Line}:{nve.OffendingToken.Column} no viable alternative at input '{nve.OffendingToken.Text}'",
                            pce);
                    }
                    case InputMismatchException exception:
                    {
                        var ime = exception;
                        var expecting = string.Join(", ", ime.GetExpectedTokens());
                        throw new SchemaException(
                            $"Error: line {ime.OffendingToken.Line}:{ime.OffendingToken.Column} extraneous input '{ime.OffendingToken.Text}' expecting {expecting}",
                            pce);
                    }
                    default:
                        throw new SchemaException(pce.InnerException.Message, pce);
                }
            }
        }
    }
}
