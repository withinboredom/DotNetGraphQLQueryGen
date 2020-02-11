using System;
using System.Collections.Generic;
using System.Linq;
using GraphQLSchema.Grammer;

namespace dotnet_gqlgen
{
    internal class SchemaVisitor : GraphQLSchemaBaseVisitor<object>
    {
        private readonly SchemaInfo schemaInfo;
        private List<Field> addFieldsTo;

        public SchemaInfo SchemaInfo => schemaInfo;

        public SchemaVisitor(Dictionary<string, string> typeMappings)
        {
            this.schemaInfo = new SchemaInfo(typeMappings);
        }

        public override object VisitFieldsDefinition(GraphQLSchemaParser.FieldsDefinitionContext context)
        {
            var result = base.VisitFieldsDefinition(context);
            var docComment = context.description();
            var desc = docComment != null ? (string)VisitDescription(docComment) : null;
            var name = context.NAME().GetText();
            var args = context.argumentsDefinition() == null ? null : (List<Arg>)VisitArgumentsDefinition(context.argumentsDefinition());
            var type = context.type_().GetText();
            var isArray = type[0] == '[';
            type = type.Trim('[', ']');
            addFieldsTo.Add(new Field(this.schemaInfo)
            {
                Name = name,
                TypeName = type,
                IsArray = isArray,
                Args = args,
                Description = desc,
            });
            return result;
        }

        /// <inheritdoc />
        public override object VisitInputFieldsDefinition(GraphQLSchemaParser.InputFieldsDefinitionContext context)
        {
            return base.VisitInputFieldsDefinition(context);
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

            addFieldsTo.Add(new Field(schemaInfo)
            {
                Name = name,
                Description = desc,
                TypeName = type,
                IsArray = isArray
            });

            return base.VisitInputValueDefinition(context);
        }

        /// <inheritdoc />
        public override object VisitArguments(GraphQLSchemaParser.ArgumentsContext context)
        {
            var args = new List<Arg>();
            if (context != null)
            {
                foreach (var arg in context.argument())
                {
                    var type = arg.valueOrVariable().value().GetText();
                    var isArray = type[0] == '[';
                    type = type.Trim('[', ']');
                    args.Add(new Arg(this.schemaInfo)
                    {
                        Name = arg.NAME().GetText(),
                        TypeName = type,
                        Required = arg != null,
                        IsArray = isArray
                    });
                }
            }
            return args;
        }

        internal void SetFieldConsumer(List<Field> item)
        {
            this.addFieldsTo = item;
        }

        /// <inheritdoc />
        public override object VisitDescription(GraphQLSchemaParser.DescriptionContext context)
        {
            return context.GetText().Trim('"', ' ', '\t', '\n', '\r', '#');
        }

        /// <inheritdoc />
        public override object VisitSchemaDefinition(GraphQLSchemaParser.SchemaDefinitionContext context)
        {
            using (new FieldConsumer(this, schemaInfo.Schema))
            {
                return base.Visit(context.rootOperationTypeDefinitionList());
            }
        }

        /// <inheritdoc />
        public override object VisitEnumValue(GraphQLSchemaParser.EnumValueContext context)
        {
            addFieldsTo.Add(new Field(schemaInfo) {Name = context.NAME().GetText()});
            return base.VisitEnumValue(context);
        }

        /// <inheritdoc />
        public override object VisitEnumTypeDefinition(GraphQLSchemaParser.EnumTypeDefinitionContext context)
        {
            var docComment = context.description();
            var desc = docComment != null ? (string)VisitDescription(docComment) : null;

            var nums = new List<Field>();
            using (new FieldConsumer(this, nums))
            {
                var result = base.VisitEnumTypeDefinition(context);
                schemaInfo.Enums.Add(context.NAME().GetText(), new TypeInfo(nums, context.NAME().GetText(), desc, isInput: false));
                return result;
            }
        }

        /// <inheritdoc />
        public override object VisitInputObjectTypeDefinition(GraphQLSchemaParser.InputObjectTypeDefinitionContext context)
        {
            var docComment = context.description();
            var desc = docComment != null ? (string)VisitDescription(docComment) : null;

            var fields = new List<Field>();
            using (new FieldConsumer(this, fields))
            {
                var result = base.Visit(context.inputFieldsDefinition());
                schemaInfo.Inputs.Add(context.NAME().GetText(), new TypeInfo(fields, context.NAME().GetText(), desc, isInput:true));
                return result;
            }
        }

        /// <inheritdoc />
        public override object VisitObjectTypeDefinition(GraphQLSchemaParser.ObjectTypeDefinitionContext context)
        {
            var docComment = context.description();
            var desc = docComment != null ? (string)VisitDescription(docComment) : null;

            var fields = new List<Field>();
            using (new FieldConsumer(this, fields))
            {
                var result = base.Visit(context.fieldsDefinitions());
                schemaInfo.Types.Add(context.NAME().GetText(), new TypeInfo(fields, context.NAME().GetText(), desc));
                return result;
            }
        }

        /// <inheritdoc />
        public override object VisitScalarTypeDefinition(GraphQLSchemaParser.ScalarTypeDefinitionContext context)
        {
            var result = base.VisitScalarTypeDefinition(context);
            schemaInfo.Scalars.Add(context.NAME().GetText());
            return result;
        }

        /// <inheritdoc />
        public override object VisitRootOperationTypeDefinition(GraphQLSchemaParser.RootOperationTypeDefinitionContext context)
        {
            var name = context.namedType().GetText();
            var operation = context.operationType().GetText();

            addFieldsTo.Add(new Field(schemaInfo)
            {
                Name = operation,
                Description = null,
                TypeName = name,
                IsArray = false
            });

            return base.VisitRootOperationTypeDefinition(context);
        }
    }
}