using System;
using System.Runtime.Serialization;

namespace dotnet_gqlgen
{
    /// <summary>
    ///     A schema exception
    /// </summary>
    /// <seealso cref="T:System.Exception" />
    public class SchemaException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SchemaException" />
        ///     class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SchemaException(string message) : base(message) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SchemaException" />
        ///     class.
        /// </summary>
        /// <param name="message">
        ///     The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a
        ///     <see langword="null" /> reference ( <see langword="Nothing" /> in
        ///     Visual Basic) if no inner exception is specified.
        /// </param>
        public SchemaException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SchemaException" />
        ///     class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="SerializationInfo" /> that holds the serialized
        ///     object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        protected SchemaException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
