﻿using Dawnx;
using Dawnx.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TypeSharp
{
    public class TypeScriptModelBuilder
    {
        public class ConstDefinition
        {
            public string OuterNamespace { get; set; }
            public string InnerNamespace { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }

            public string ReferenceName => $"{OuterNamespace}.{InnerNamespace}.{Name}";
        }

        public class TypeDefinition
        {
            /// <summary>
            /// Hint: If Namespace is null, then the code should be null.
            /// </summary>
            public string Namespace { get; set; }

            public string Name { get; set; }

            /// <summary>
            /// Hint: If Namespace is null, then the code should be null.
            /// </summary>
            public string Code { get; set; }

            public string ReferenceName => Namespace is null ? Name : $"{Namespace}.{Name}";
        }

        public Dictionary<string, ConstDefinition> ConstDefinitions = new Dictionary<string, ConstDefinition>();
        public Dictionary<string, TypeDefinition> TypeDefinitions = new Dictionary<string, TypeDefinition>()
        {
            [typeof(object).FullName] = new TypeDefinition { Name = "any" },
            [typeof(bool).FullName] = new TypeDefinition { Name = "boolean" },
            [typeof(byte).FullName] = new TypeDefinition { Name = "number" },
            [typeof(sbyte).FullName] = new TypeDefinition { Name = "number" },
            [typeof(char).FullName] = new TypeDefinition { Name = "number" },
            [typeof(short).FullName] = new TypeDefinition { Name = "number" },
            [typeof(ushort).FullName] = new TypeDefinition { Name = "number" },
            [typeof(int).FullName] = new TypeDefinition { Name = "number" },
            [typeof(uint).FullName] = new TypeDefinition { Name = "number" },
            [typeof(long).FullName] = new TypeDefinition { Name = "number" },
            [typeof(ulong).FullName] = new TypeDefinition { Name = "number" },
            [typeof(float).FullName] = new TypeDefinition { Name = "number" },
            [typeof(double).FullName] = new TypeDefinition { Name = "number" },
            [typeof(decimal).FullName] = new TypeDefinition { Name = "number" },
            [typeof(string).FullName] = new TypeDefinition { Name = "string" },
            [typeof(Guid).FullName] = new TypeDefinition { Name = "string" },
            [typeof(DateTime).FullName] = new TypeDefinition { Name = "Date" },
        };

        public void WriteTo(string path) => File.WriteAllText(path, Compile());

        public string Compile()
        {
            var code = new StringBuilder();
            code.AppendLine($"/* Generated by TypeSharp v{Assembly.GetCallingAssembly().GetName().Version.ToString()} */");

            #region Compile Consts
            {
                var constGroup1 = ConstDefinitions.Values.GroupBy(x => x.OuterNamespace);

                if (constGroup1.Any()) code.AppendLine();
                foreach (var constGroupItem1 in constGroup1)
                {
                    var constGroup2 = constGroupItem1.GroupBy(x => x.InnerNamespace);
                    code.AppendLine($"namespace {constGroupItem1.Key} {{");
                    foreach (var constGroupItem2 in constGroup2)
                    {
                        code.AppendLine($"    export namespace {constGroup2.First().Key} {{");
                        foreach (var definition in constGroupItem2)
                            code.AppendLine(definition.Code);
                        code.AppendLine($"    }}");
                    }
                    code.AppendLine($"}}");
                }
            }
            #endregion

            #region Compile Types
            {
                var typeGroup1 = TypeDefinitions.Values.Where(x => x.Namespace != null).GroupBy(x => x.Namespace);
                if (typeGroup1.Any()) code.AppendLine();
                foreach (var typeGroupItem1 in typeGroup1)
                {
                    code.AppendLine($"declare namespace {typeGroupItem1.Key} {{");
                    foreach (var definition in typeGroupItem1)
                        code.AppendLine(definition.Code);
                    code.AppendLine($"}}");
                }
            }
            #endregion

            return code.ToString();
        }

        private string GetTsNamespace(Type type)
        {
            var attr = type.GetCustomAttribute<TypeScriptModelAttribute>();
            if (attr?.Namespace is null)
            {
                var dType = type.DeclaringType;
                if (dType is null)
                    return type.Namespace;
                else return $"{GetTsNamespace(dType)}.{type.Name}";
            }
            else return attr.Namespace;
        }

        private string GetTypeValue(PropertyInfo propertyInfo)
        {
            var propType = propertyInfo.PropertyType;

            if (!TypeDefinitions.ContainsKey(propType.FullName))
                CacheType(propType);

            return $"{StringUtility.CamelCase(propertyInfo.Name)}? : {TypeDefinitions[propType.FullName].ReferenceName}";
        }

        private string GetConstValue(FieldInfo fieldInfo)
        {
            var fieldType = fieldInfo.FieldType;

            if (!TypeDefinitions.ContainsKey(fieldType.FullName))
                CacheType(fieldType);

            var value = fieldInfo.GetValue(null);
            string sValue;
            if (value is string)
                sValue = $"'{value}'";
            else sValue = value.ToString();

            return $"{fieldInfo.Name} : {TypeDefinitions[fieldType.FullName].ReferenceName} = {sValue}";
        }

        public void CacheTypes(params Type[] types) => types.Each(type => CacheType(type));
        public void CacheType<TType>(TypeScriptModelAttribute attr = null) => CacheType(typeof(TType), attr);
        public void CacheType(Type type, TypeScriptModelAttribute attr = null)
        {
            if (attr is null)
                attr = type.GetCustomAttribute<TypeScriptModelAttribute>();

            var tsNamespace = attr?.Namespace ?? GetTsNamespace(type);

            if (type.IsClass)
            {
                switch (type.FullName)
                {
                    case string _ when type.FullName.StartsWith("System.Collections.Generic.Dictionary"):
                        TypeDefinitions[type.FullName] = new TypeDefinition { Name = "any" };
                        break;

                    default:
                        #region Cache Consts
                        {
                            var consts = type.GetFields().Where(x => x.IsStatic && x.IsLiteral && x.IsPublic);
                            foreach (var field in consts)
                            {
                                ConstDefinitions[$"{field.DeclaringType.FullName}.{field.Name}"] = new ConstDefinition
                                {
                                    OuterNamespace = tsNamespace,
                                    InnerNamespace = type.Name,
                                    Name = field.Name,
                                    Code = $"        export const {GetConstValue(field)};",
                                };
                            }
                        }
                        #endregion

                        #region Cache Properties
                        {
                            var props = type.GetProperties();
                            var code = new StringBuilder();

                            code.AppendLine($"    interface {type.Name} {{");
                            foreach (var prop in props)
                                code.AppendLine($"        {GetTypeValue(prop)};");
                            code.Append($"    }}");

                            TypeDefinitions[type.FullName] = new TypeDefinition
                            {
                                Namespace = tsNamespace,
                                Name = type.Name,
                                Code = code.ToString(),
                            };
                        }
                        #endregion
                        break;
                }
            }
            else if (type.IsEnum)
            {
                var code = new StringBuilder();

                code.AppendLine($"    export const enum {type.Name} {{");
                foreach (var name in Enum.GetNames(type))
                    code.AppendLine($"        {name} = {(int)Enum.Parse(type, name)},");
                code.Append($"    }}");

                TypeDefinitions[type.FullName] = new TypeDefinition
                {
                    Namespace = tsNamespace,
                    Name = type.Name,
                    Code = code.ToString(),
                };
            }
            else throw new NotSupportedException($"{type.FullName} is not supported.");
        }

    }
}