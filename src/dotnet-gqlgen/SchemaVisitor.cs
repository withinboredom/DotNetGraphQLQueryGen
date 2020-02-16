using System.Collections.Generic;
using System.Linq;
using GraphQLSchema.Grammer;

namespace dotnet_gqlgen
{
    /// <summary>
    ///     Visit the schema
    /// </summary>
    /// <seealso cref="GraphQLSchema.Grammer.GraphQLSchemaBaseVisitor{System.Object}" />
    internal class SchemaVisitor : GraphQLSchemaBaseVisitor<object>
    {
        /// <summary>
        ///     The add fields to
        /// </summary>
        private List<Field> _addFieldsTo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SchemaVisitor" /> class.
        /// </summary>
        /// <param name="typeMappings">The type mappings.</param>
        public SchemaVisitor(Dictionary<string, string> typeMappings)
        {
            SchemaInfo = new SchemaInfo(typeMappings);
        }

        /// <summary>
        ///     Gets the schema information.
        /// </summary>
        /// <value>
        ///     The schema information.
        /// </value>
        public SchemaInfo SchemaInfo { get; }

        /// <summary>
        ///     Visit a parse tree produced by <see cref="M:GraphQLSchema.Grammer.GraphQLSchemaParser.fieldsDefinition" />.
        ///     <para>
        ///         The default implementation returns the result of calling
        ///         <see cref="M:Antlr4.Runtime.Tree.AbstractParseTreeVisitor`1.VisitChildren(Antlr4.Runtime.Tree.IRuleNode)" />
        ///         on <paramref name="context" />.
        ///     </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <returns></returns>
        /// <return>The visitor result.</return>
        public override object VisitFieldsDefinition(GraphQLSchemaParser.FieldsDefinitionContext context)
        {
            var result = base.VisitFieldsDefinition(context);
            var docComment = context.description();
            var desc = docComment != null ? (string) VisitDescription(docComment) : null;
            var name = context.NAME().GetText();
            var args = context.argumentsDefinition() == null
                ? null
                : (List<Arg>) VisitArgumentsDefinition(context.argumentsDefinition());
            var type = context.type_().GetText();
            var isArray = type[0] == '[';
            type = type.Trim('[', ']');
            _addFieldsTo.Add(
                new Field(SchemaInfo)
                {
                    Name = name,
                    TypeName = type,
                    IsArray = isArray,
                    Args = args,
                    Description = desc
                });
            return result;
        }

        /// <summary>
        ///     Visit a parse tree produced by <see cref="M:GraphQLSchema.Grammer.GraphQLSchemaParser.argumentsDefinition" />.
        ///     <para>
        ///         The default implementation returns the result of calling
        ///         <see cref="M:Antlr4.Runtime.Tree.AbstractParseTreeVisitor`1.VisitChildren(Antlr4.Runtime.Tree.IRuleNode)" />
        ///         on <paramref name="context" />.
        ///     </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <returns></returns>
        /// <return>The visitor result.</return>
        public override object VisitArgumentsDefinition(GraphQLSchemaParser.ArgumentsDefinitionContext context)
        {
            var args = new List<Arg>();

            if (context == null) return args;
            args.AddRange(
                from arg in context.inputValueDefinition()
                let doc = arg.description()
                let desc = doc == null ? null : (string) VisitDescription(doc)
                let name = arg.NAME().GetText()
                let type = arg.type_().typeName().GetText()
                let isRequired = arg.type_().nonNullType() != null
                let isArray = arg.type_().listType() != null
                select new Arg(SchemaInfo)
                {
                    Description = desc,
                    Name = name,
                    Required = isRequired,
                    TypeName = type,
                    IsArray = isArray
                });

            return args;
        }

        /// <inheritdoc />
        public override object VisitInputValueDefinition(GraphQLSchemaParser.InputValueDefinitionContext context)
        {
            var docComment = context.description();
            var desc = docComment == null ? null : (string) VisitDescription(docComment);
            var name = context.NAME().GetText();
            var type = context.type_().typeName().GetText();
            var isRequired = context.type_().nonNullType() != null;
            var isArray = context.type_().listType() != null;

            _addFieldsTo.Add(
                new Field(SchemaInfo) {Name = name, Description = desc, TypeName = type, IsArray = isArray});

            return base.VisitInputValueDefinition(context);
        }

        /// <inheritdoc />
        public override object VisitArguments(GraphQLSchemaParser.ArgumentsContext context)
        {
            var args = new List<Arg>();
            if (context == null) return args;

            foreach (var arg in context.argument())
            {
                var type = arg.valueOrVariable().value().GetText();
                var isArray = type[0] == '[';
                type = type.Trim('[', ']');
                args.Add(
                    new Arg(SchemaInfo)
                    {
                        Name = arg.NAME().GetText(), TypeName = type, Required = true, IsArray = isArray
                    });
            }

            return args;
        }

        /// <summary>
        ///     Sets the field consumer.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void SetFieldConsumer(List<Field> item)
        {
            _addFieldsTo = item;
        }

        /// <inheritdoc />
        public override object VisitDescription(GraphQLSchemaParser.DescriptionContext context)
        {
            return context.GetText().Trim('"', ' ', '\t', '\n', '\r', '#');
        }

        /// <inheritdoc />
        public override object VisitSchemaDefinition(GraphQLSchemaParser.SchemaDefinitionContext context)
        {
            using (new FieldConsumer(this, SchemaInfo.Schema))
            {
                return base.Visit(context.rootOperationTypeDefinitionList());
            }
        }

        /// <inheritdoc />
        public override object VisitEnumValue(GraphQLSchemaParser.EnumValueContext context)
        {
            _addFieldsTo.Add(new Field(SchemaInfo) {Name = context.NAME().GetText()});
            return base.VisitEnumValue(context);
        }

        /// <inheritdoc />
        public override object VisitEnumTypeDefinition(GraphQLSchemaParser.EnumTypeDefinitionContext context)
        {
            var docComment = context.description();
            var desc = docComment != null ? (string) VisitDescription(docComment) : null;

            var nums = new List<Field>();
            using (new FieldConsumer(this, nums))
            {
                var result = base.VisitEnumTypeDefinition(context);
                SchemaInfo.Enums.Add(context.NAME().GetText(), new TypeInfo(nums, context.NAME().GetText(), desc));
                return result;
            }
        }

        /// <inheritdoc />
        public override object VisitInputObjectTypeDefinition(
            GraphQLSchemaParser.InputObjectTypeDefinitionContext context
        )
        {
            var docComment = context.description();
            var desc = docComment != null ? (string) VisitDescription(docComment) : null;

            var fields = new List<Field>();
            using (new FieldConsumer(this, fields))
            {
                var result = base.Visit(context.inputFieldsDefinition());
                SchemaInfo.Inputs.Add(context.NAME().GetText(), new TypeInfo(fields, context.NAME().GetText(), desc));
                return result;
            }
        }

        /// <inheritdoc />
        public override object VisitObjectTypeDefinition(GraphQLSchemaParser.ObjectTypeDefinitionContext context)
        {
            var docComment = context.description();
            var desc = docComment != null ? (string) VisitDescription(docComment) : null;

            var fields = new List<Field>();
            using (new FieldConsumer(this, fields))
            {
                var result = base.Visit(context.fieldsDefinitions());
                SchemaInfo.Types.Add(context.NAME().GetText(), new TypeInfo(fields, context.NAME().GetText(), desc));
                return result;
            }
        }

        /// <inheritdoc />
        public override object VisitScalarTypeDefinition(GraphQLSchemaParser.ScalarTypeDefinitionContext context)
        {
            var result = base.VisitScalarTypeDefinition(context);
            SchemaInfo.Scalars.Add(context.NAME().GetText());
            return result;
        }

        /// <inheritdoc />
        public override object VisitRootOperationTypeDefinition(
            GraphQLSchemaParser.RootOperationTypeDefinitionContext context
        )
        {
            var name = context.namedType().GetText();
            var operation = context.operationType().GetText();

            _addFieldsTo.Add(
                new Field(SchemaInfo) {Name = operation, Description = null, TypeName = name, IsArray = false});

            return base.VisitRootOperationTypeDefinition(context);
        }
    }
}
