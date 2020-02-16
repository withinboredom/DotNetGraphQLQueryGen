using System;

namespace dotnet_gqlgen
{
    /// <summary>
    ///     A name attribute
    /// </summary>
    /// <seealso cref="T:System.Attribute" />
    public sealed class GqlFieldNameAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="GqlFieldNameAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public GqlFieldNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; }
    }
}
