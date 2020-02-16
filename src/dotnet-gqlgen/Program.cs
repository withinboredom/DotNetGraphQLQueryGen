using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using RazorLight;

namespace dotnet_gqlgen
{
    /// <summary>
    ///     Main entry point
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Gets the schema file.
        /// </summary>
        /// <value>
        ///     The schema file.
        /// </value>
        [Argument(0, Description = "Path the the GraphQL schema file")]
        [Required]
        private string SchemaFile { get; }

        /// <summary>
        ///     Gets the namespace.
        /// </summary>
        /// <value>
        ///     The namespace.
        /// </value>
        [Option(LongName = "namespace", ShortName = "n", Description = "Namespace to generate code under")]
        private string Namespace { get; } = "Generated";

        /// <summary>
        ///     Gets the name of the client class.
        /// </summary>
        /// <value>
        ///     The name of the client class.
        /// </value>
        [Option(LongName = "client_class_name", ShortName = "c", Description = "Name for the client class")]
        private string ClientClassName { get; } = "GraphQLClient";

        /// <summary>
        ///     Gets the scalar mapping.
        /// </summary>
        /// <value>
        ///     The scalar mapping.
        /// </value>
        [Option(
            LongName = "scalar_mapping",
            ShortName = "m",
            Description =
                "Map of custom schema scalar types to dotnet types. Use \"GqlType=DotNetClassName,ID=Guid,...\"")]
        private string ScalarMapping { get; }

        /// <summary>
        ///     Gets the output dir.
        /// </summary>
        /// <value>
        ///     The output dir.
        /// </value>
        [Option(LongName = "output", ShortName = "o", Description = "Output directory")]
        private string OutputDir { get; } = "output";

        /// <summary>
        ///     Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// </returns>
        public static int Main(string[] args)
        {
            return CommandLineApplication.Execute<Program>(args);
        }

        /// <summary>
        ///     Called when [execute].
        /// </summary>
        private async void OnExecute()
        {
            try
            {
                Console.WriteLine($"Loading {SchemaFile}...");
                var schemaText = File.ReadAllText(SchemaFile);

                var mappings = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(ScalarMapping))
                    mappings = ScalarMapping.Split(',').Select(s => s.Split('=')).ToDictionary(k => k[0], v => v[1]);

                // parse into AST
                var typeInfo = SchemaCompiler.Compile(schemaText, mappings);

                Console.WriteLine($"Generating types in namespace {Namespace}, outputting to {ClientClassName}.cs");

                // pass the schema to the template
                var engine = new RazorLightEngineBuilder().UseEmbeddedResourcesProject(typeof(Program))
                    .UseMemoryCachingProvider()
                    .Build();

                var allTypes = typeInfo.Types.Concat(typeInfo.Inputs).ToDictionary(k => k.Key, v => v.Value);

                var result = await engine.CompileRenderAsync(
                    "types.cshtml",
                    new
                    {
                        Namespace,
                        SchemaFile,
                        Types = allTypes,
                        typeInfo.Enums,
                        typeInfo.Mutation,
                        CmdArgs = $"-n {Namespace} -c {ClientClassName} -m {ScalarMapping}"
                    });
                Directory.CreateDirectory(OutputDir);
                File.WriteAllText($"{OutputDir}/GeneratedTypes.cs", result);

                result = await engine.CompileRenderAsync(
                    "client.cshtml",
                    new
                    {
                        Namespace,
                        SchemaFile,
                        typeInfo.Query,
                        typeInfo.Mutation,
                        ClientClassName
                    });
                File.WriteAllText($"{OutputDir}/{ClientClassName}.cs", result);

                Console.WriteLine("Done.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }
    }
}
