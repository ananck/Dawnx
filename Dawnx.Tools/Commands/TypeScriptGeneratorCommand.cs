﻿using Dawnx.Data;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TypeSharp;

namespace Dawnx.Tools
{
    [Command("TSGenerator", "tsg", Description = "Generate TypeScript model from CSharp model.")]
    public class TypeScriptGeneratorCommand : ICommand
    {
        private static string[] SearchDirs = new[]
        {
            Path.GetFullPath($"{Program.TargetProjectInfo.ProjectRoot}/bin/Debug/{Program.TargetProjectInfo.TargetFramework}"),
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}/dotnet/sdk/NuGetFallbackFolder",
            $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.nuget/packages",
        };

        public void PrintUsage()
        {
            Console.WriteLine($@"
Usage: dotnet nx (tsg|typescriptgenerator) [Options]

Options:
  {"-o|--out".PadRight(20)}{"\t"}Specify the output directory path.
  {"-i|--include".PadRight(20)}{"\t"}Specify to include other built-in models, such as 'JSend'.");
        }

        public void Run(ConsoleArgs cargs)
        {
            if (cargs.Properties.For(x => x.ContainsKey("-h") || x.ContainsKey("--help")))
            {
                PrintUsage();
                return;
            }

            var outFolder = cargs["--out"] ?? cargs["-o"] ?? "Typings";
            var includes = cargs["-i"]?.Split(",") ?? cargs["--include"]?.Split(",") ?? new string[0];

            GenerateTypeScript(outFolder, includes);
        }


        private static void GenerateTypeScript(string outFolder, string[] includes)
        {
            includes = includes?.Select(x => x.ToLower()).ToArray() ?? new string[0];

            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            var dllPath = Path.GetFullPath($"{SearchDirs[0]}/{Program.TargetProjectInfo.AssemblyName}.dll");

            var assembly = Assembly.LoadFrom(dllPath);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            #region Assembly Types
            {
                var builder = new TypeScriptModelBuilder();
                var fileName = $"{Path.GetFullPath($"{outFolder}/{Program.TargetProjectInfo.AssemblyName}.ts")}";
                var modelTypes = assembly.GetTypesWhichMarkedAs<TypeScriptModelAttribute>();

                builder.CacheTypes(modelTypes);
                builder.WriteTo(fileName);

                Console.WriteLine($"File saved: {fileName}");
            }
            #endregion

            #region JSend Types
            if (includes.Contains("jsend"))
            {
                var builder = new TypeScriptModelBuilder();
                var fileName = $"{Path.GetFullPath($"{outFolder}/JSend.ts")}";

                builder.CacheType<JSend>();
                builder.WriteTo(fileName);

                Console.WriteLine($"File saved: {fileName}");
            };
            #endregion
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var regex = new Regex("([^,]+), Version=([^,]+), Culture=[^,]+, PublicKeyToken=.+");
            var match = regex.Match(args.Name);

            var assemblyName = match.Groups[1].Value;
            var version = match.Groups[2].Value.For(ver =>
            {
                if (ver.EndsWith(".0"))
                    return ver.Substring(0, ver.Length - 2);
                else return ver;
            });

            foreach (var vi in SearchDirs.AsVI())
            {
                var dir = vi.Value;
                string file;

                if (vi.Index == 0)
                    file = $"{dir}/{assemblyName}.dll";
                else file = $"{dir}/{assemblyName}/{version}/lib/netstandard2.0/{assemblyName}.dll";

                if (File.Exists(file))
                    return Assembly.LoadFile(file);
            }

            return null;
        }

    }

}