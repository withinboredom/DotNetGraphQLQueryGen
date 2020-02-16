using System;
using System.Collections.Generic;

namespace dotnet_gqlgen
{
    /// <summary>
    ///     A field consumer
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    internal class FieldConsumer : IDisposable
    {
        /// <summary>
        ///     The schema visitor
        /// </summary>
        private readonly SchemaVisitor _schemaVisitor;

        /// <summary>
        ///     The schema
        /// </summary>
        private List<Field> _schema;

        /// <summary>
        ///     <para>
        ///         Initializes a new instance of the <see cref="FieldConsumer" />
        ///     </para>
        ///     <para>class.</para>
        /// </summary>
        /// <param name="schemaVisitor">
        ///     The <paramref name="schema" /> visitor.
        /// </param>
        /// <param name="schema">The schema.</param>
        public FieldConsumer(SchemaVisitor schemaVisitor, List<Field> schema)
        {
            _schemaVisitor = schemaVisitor;
            _schema = schema;
            schemaVisitor.SetFieldConsumer(schema);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing,
        ///     releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _schemaVisitor.SetFieldConsumer(null);
        }
    }
}
