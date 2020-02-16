using System;
using System.Collections.Generic;
using System.Linq;

namespace dotnet_gqlgen
{
    /// <summary>
    ///     Schema information class
    /// </summary>
    public class SchemaInfo
    {
        /// <summary>
        ///     The type mappings
        /// </summary>
        private readonly Dictionary<string, string> _typeMappings = new Dictionary<string, string>
        {
            {"String", "string"},
            {"ID", "string"},
            {"Int", "int?"},
            {"Float", "double?"},
            {"Boolean", "bool?"},
            {"String!", "string"},
            {"ID!", "string"},
            {"Int!", "int"},
            {"Float!", "double"},
            {"Boolean!", "bool"}
        };

        /// <summary>
        ///     Initializes a new instance of the <see cref="SchemaInfo" /> class.
        /// </summary>
        /// <param name="typeMappings">The type mappings.</param>
        public SchemaInfo(Dictionary<string, string> typeMappings)
        {
            if (typeMappings != null)
                foreach (var (key, value) in typeMappings)
                {
                    // overrides
                    _typeMappings[key] = value;
                }

            Schema = new List<Field>();
            Types = new Dictionary<string, TypeInfo>();
            Inputs = new Dictionary<string, TypeInfo>();
            Enums = new Dictionary<string, TypeInfo>();
            Scalars = new List<string>();
        }

        /// <summary>
        ///     The schema
        /// </summary>
        public List<Field> Schema { get; }

        /// <summary>
        ///     Return the query type info.
        /// </summary>
        public TypeInfo Query => Types[Schema.First(f => f.Name == "query").TypeName];

        /// <summary>
        ///     Return the mutation type info.
        /// </summary>
        public TypeInfo Mutation
        {
            get
            {
                var typeName = Schema.First(f => f.Name == "mutation")?.TypeName;
                return typeName != null ? Types[typeName] : null;
            }
        }

        /// <summary>
        ///     <see cref="Types" />
        /// </summary>
        public Dictionary<string, TypeInfo> Types { get; }

        /// <summary>
        ///     <see cref="Inputs" />
        /// </summary>
        public Dictionary<string, TypeInfo> Inputs { get; }

        /// <summary>
        ///     <see cref="Scalars" />
        /// </summary>
        public List<string> Scalars { get; }

        /// <summary>
        ///     <see cref="Enums" />
        /// </summary>
        public Dictionary<string, TypeInfo> Enums { get; }

        /// <summary>
        ///     Whether it has a dot net type
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns>
        /// </returns>
        internal bool HasDotNetType(string typeName)
        {
            return _typeMappings.ContainsKey(typeName) ||
                   Types.ContainsKey(typeName) ||
                   Inputs.ContainsKey(typeName) ||
                   Enums.ContainsKey(typeName);
        }

        /// <summary>
        ///     Get the dot net type
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns>
        /// </returns>
        internal string GetDotNetType(string typeName)
        {
            if (_typeMappings.ContainsKey(typeName)) return _typeMappings[typeName];
            if (Types.ContainsKey(typeName)) return Types[typeName].Name;
            return Enums.ContainsKey(typeName) ? Enums[typeName].Name : Inputs[typeName].Name;
        }
    }

    /// <summary>
    ///     <see cref="Type" /> information
    /// </summary>
    public class TypeInfo
    {
        /// <summary>
        ///     Create a new type information
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="isInput"></param>
        public TypeInfo(IEnumerable<Field> fields, string name, string description, bool isInput = false)
        {
            Fields = fields.ToList();
            Name = name;
            Description = description;
            IsInput = isInput;
        }

        /// <summary>
        ///     The fields
        /// </summary>
        public List<Field> Fields { get; }

        /// <summary>
        ///     The name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The description
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     The input state
        /// </summary>
        public bool IsInput { get; }
    }

    /// <summary>
    ///     A field
    /// </summary>
    public class Field
    {
        /// <summary>
        ///     The schema information
        /// </summary>
        private readonly SchemaInfo _schemaInfo;

        /// <summary>
        ///     The field
        /// </summary>
        /// <param name="schemaInfo"></param>
        public Field(SchemaInfo schemaInfo)
        {
            Args = new List<Arg>();
            _schemaInfo = schemaInfo;
        }

        /// <summary>
        ///     The name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The type name
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        ///     Is this an array
        /// </summary>
        public bool IsArray { get; set; }

        /// <summary>
        ///     The args
        /// </summary>
        public List<Arg> Args { get; set; }

        /// <summary>
        ///     The description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The implementation name
        /// </summary>
        public string DotNetName => Name[0].ToString().ToUpper() + string.Join("", Name.Skip(1));

        /// <summary>
        ///     The implementation type
        /// </summary>
        public string DotNetType => IsArray ? $"List<{DotNetTypeSingle}>" : DotNetTypeSingle;

        /// <summary>
        ///     The single type
        /// </summary>
        public string DotNetTypeSingle
        {
            get
            {
                if (!_schemaInfo.HasDotNetType(TypeName))
                    throw new SchemaException(
                        $"Unknown dotnet type for schema type '{TypeName}'. Please provide a mapping for any custom scalar types defined in the schema");
                return _schemaInfo.GetDotNetType(TypeName);
            }
        }

        /// <summary>
        ///     Should this be a property
        /// </summary>
        public bool ShouldBeProperty =>
            ((Args == null || Args.Count == 0) &&
             !_schemaInfo.Types.ContainsKey(TypeName) &&
             !_schemaInfo.Inputs.ContainsKey(TypeName)) ||
            _schemaInfo.Scalars.Contains(TypeName);

        /// <summary>
        ///     The args output
        /// </summary>
        /// <returns>
        /// </returns>
        public string ArgsOutput()
        {
            if (Args == null || !Args.Any()) return "";
            return string.Join(", ", Args.Select(a => $"{a.DotNetType} {a.Name}"));
        }

        /// <summary>
        ///     Make this a string
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {
            return $"{Name}:{(IsArray ? '[' + TypeName + ']' : TypeName)}";
        }
    }

    /// <summary>
    ///     An argument
    /// </summary>
    public class Arg : Field
    {
        /// <summary>
        ///     Init the arg
        /// </summary>
        /// <param name="schemaInfo"></param>
        public Arg(SchemaInfo schemaInfo) : base(schemaInfo) { }

        /// <summary>
        ///     Whether this is required
        /// </summary>

        public bool Required { get; set; }
    }
}
