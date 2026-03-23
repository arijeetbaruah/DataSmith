using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Baruah.DataSmith.Editor
{
    public static class DataSmithGenerator
    {
        /// <summary>
        /// Generates model and query C# source files for the given ModelEntry into the specified output folder.
        /// </summary>
        /// <remarks>
        /// Selects a code template based on the entry's ValueType; if no template is found, the method returns without producing files.
        /// Writes two files named &lt;TypeName&gt;Model.cs and &lt;TypeName&gt;Query.cs into <paramref name="outputFolder"/> and imports them into the Unity AssetDatabase using a synchronous import.
        /// </remarks>
        /// <param name="entry">Metadata describing the model type and generation attributes.</param>
        /// <summary>
        /// Generate model and query C# source files for the given ModelEntry and import them into the Unity AssetDatabase.
        /// </summary>
        /// <param name="entry">Metadata describing the model to generate (type and associated attributes).</param>
        /// <param name="outputFolder">Destination folder path where the generated .cs files will be written.</param>
        public static void GenerateEntry(ModelEntry entry, string outputFolder)
        {
            var templates = LoadTemplates();
            
            if (!templates.TryGetValue(entry.Attribute.ValueType, out var template))
                return;

            string modelCode = BuildModel(entry, template);

            string queryCode = BuildQuery(entry);

            string modelPath = Path.Combine(outputFolder,
                entry.Type.Name + "Model.cs");

            string queryPath = Path.Combine(outputFolder,
                entry.Type.Name + "Query.cs");

            File.WriteAllText(modelPath, modelCode);
            File.WriteAllText(queryPath, queryCode);

            AssetDatabase.ImportAsset(modelPath, ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.ImportAsset(queryPath, ImportAssetOptions.ForceSynchronousImport);
        }

        /// <summary>
        /// Generate the DataContext file where you can access all of the model files
        /// </summary>
        /// <param name="modelTypes">list of all model used</param>
        /// <summary>
        /// Generate the DataContext.cs source file from the TextAsset template named "DataContextTmp" by injecting the provided model types.
        /// </summary>
        /// <param name="modelTypes">Collection of model CLR types to include; each will be rendered as a `typeof(&lt;TypeName&gt;Model)` entry in the generated context.</param>
        /// <param name="outputFolder">Destination folder path where the generated .cs file will be written.</param>
        private static void GenerateDataContext(IEnumerable<Type> modelTypes, string outputFolder)
        {
            string filePath = Path.Combine(outputFolder, "DataContext.cs");

            var path = AssetDatabase.FindAssets("t:TextAsset DataContextTmp")
                .Select(g => AssetDatabase.GUIDToAssetPath(g)).FirstOrDefault();

            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            List<string> types = new();

            foreach (var type in modelTypes)
            {
                types.Add($"            typeof({type.FullName}Model)");
            }

            string tmp = asset.text;
            string code = tmp
                    .Replace("{{CONTEXT_TYPE}}", string.Join(",\n", types))
                ;

            File.WriteAllText(filePath, code);
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceSynchronousImport);

        }

        /// <summary>
        /// Generate source files for all provided model entries into the specified output folder.
        /// </summary>
        /// <param name="entries">ModelEntry definitions to generate files for.</param>
        /// <summary>
        /// Generates source files for each provided model entry, produces a DataContext that references all generated models, and refreshes the Unity asset database.
        /// </summary>
        /// <param name="entries">Collection of model metadata entries to generate code for.</param>
        /// <param name="outputFolder">Target folder path where generated files will be written (within the Unity project).</param>
        public static void GenerateAll(IEnumerable<ModelEntry> entries, string outputFolder)
        {
            foreach (var entry in entries)
                GenerateEntry(entry, outputFolder);

            GenerateDataContext(entries.Select(e => e.Type), outputFolder);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Loads TextAsset templates from the Unity project and maps value types to template text by detecting assets whose names contain "SingleModelTemplate" or "ListModelTemplate".
        /// </summary>
        /// <summary>
        /// Loads TextAsset templates from the Unity project and maps each found template to its corresponding ModelValueType key.
        /// </summary>
        /// <returns>A dictionary mapping found ModelValueType keys (e.g., Single, List, DB) to the matching template text; value types with no matching asset are omitted.</returns>
        public static Dictionary<ModelValueType, string> LoadTemplates()
        {
            var cache = new Dictionary<ModelValueType, string>();

            var paths = AssetDatabase.FindAssets("t:TextAsset")
                .Select(g => AssetDatabase.GUIDToAssetPath(g));

            foreach (var path in paths)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if (asset == null) continue;

                if (asset.name.Contains("SingleModelTemplate"))
                    cache[ModelValueType.Single] = asset.text;

                if (asset.name.Contains("ListModelTemplate"))
                    cache[ModelValueType.List] = asset.text;
                
                if (asset.name.Contains("SQLGameModelTemplate"))
                    cache[ModelValueType.DB] = asset.text;
            }

            return cache;
        }

        /// <summary>
        /// Generates the accessor and CRUD code fragment appropriate for the model's configured ValueType.
        /// </summary>
        /// <param name="entry">Metadata describing the model type and its ModelAttribute settings.</param>
        /// <returns>
        /// A C# source fragment with accessor methods and related members for the model:
        /// - For Single: instance property getters/setters, change events and any reference accessors.
        /// - For List: finder methods that enumerate matching items.
        /// - For DB: database CRUD methods and table/column mappings.
        /// Returns an empty string if the model's ValueType is not recognized.
        /// </returns>
        public static string BuildAccessors(ModelEntry entry)
        {
            var type = entry.Type;
            var kind = entry.Attribute.ValueType;
            
            switch (kind)
            {
                case ModelValueType.Single:
                    return BuildSingleAccessors(type);
                case ModelValueType.List:
                    return BuildListAccessors(type);
                case ModelValueType.DB:
                    return BuildDBAccessors(entry);;
            }
            
            return "";
        }

        /// <summary>
        /// Generate the C# source code for a model class described by a ModelEntry using a text template.
        /// </summary>
        /// <param name="entry">Metadata describing the model type and generation attributes.</param>
        /// <param name="template">Template text containing placeholders (e.g. {{MODEL_NAME}}, {{DATA_TYPE}}, {{ACCESSORS}}, {{NAMESPACE}}, {{QUERY_NAME}}).</param>
        /// <summary>
        /// Generate the model source by filling the provided template with values derived from the entry metadata.
        /// </summary>
        /// <param name="entry">ModelEntry containing the model Type and associated attributes used to generate accessors and names.</param>
        /// <param name="template">Template text containing placeholders: {{MODEL_NAME}}, {{TABLE_NAME}}, {{DATA_TYPE}}, {{ACCESSORS}}, {{NAMESPACE}}, and {{QUERY_NAME}}.</param>
        /// <returns>The generated model source code with placeholders replaced; if the model type has no namespace, the template's namespace wrapper is removed.</returns>
        public static string BuildModel(ModelEntry entry, string template)
        {
            var type = entry.Type;

            string accessors = BuildAccessors(entry);

            string namespaceName = type.Namespace;

            string result = template
                .Replace("{{MODEL_NAME}}", type.Name + "Model")
                .Replace("{{TABLE_NAME}}", type.Name)
                .Replace("{{DATA_TYPE}}", type.FullName)
                .Replace("{{ACCESSORS}}", accessors)
                .Replace("{{NAMESPACE}}", namespaceName ?? "")
                .Replace("{{QUERY_NAME}}", type.Name + "Query");

            if (string.IsNullOrEmpty(namespaceName))
                result = RemoveNamespaceBlock(result);

            return result;
        }

        /// <summary>
        /// Generates the C# source code for a strongly-typed query class corresponding to the model described by <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">The model entry whose Type and metadata are used to build the query class.</param>
        /// <summary>
        /// Generates the C# source for a sealed query class for the model type described by the entry.
        /// </summary>
        /// <param name="entry">The ModelEntry whose Type and metadata drive query-class generation.</param>
        /// <returns>The generated C# source for a sealed &lt;TypeName&gt;Query class, wrapped in the model's namespace when present, containing fluent condition methods based on the model's public instance fields.</returns>
        public static string BuildQuery(ModelEntry entry)
        {
            var type = entry.Type;
            string queryName = type.Name + "Query";

            var sb = new System.Text.StringBuilder();

            sb.AppendLine("/* Auto-generated. DO NOT MODIFY */");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(type.Namespace))
            {
                sb.AppendLine($"namespace {type.Namespace}");
                sb.AppendLine("{");
            }

            sb.AppendLine(
                $@"    public sealed class {queryName} 
        : ModelQuery<{type.FullName}>
    {{
        public {queryName}(System.Collections.Generic.IReadOnlyList<{type.FullName}> source)
            : base(source) {{ }}
");

            foreach (var field in type.GetFields(
                         BindingFlags.Public | BindingFlags.Instance))
            {
                BuildQueryMethods(sb, type, field);
            }

            sb.AppendLine(
                $@"        public {queryName} Where(System.Func<{type.Name}, bool> predicate)
        {{
            AddCondition(predicate);
            return this;
        }}
");

            sb.AppendLine("    }");

            if (!string.IsNullOrEmpty(type.Namespace))
                sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generates C# accessor members for each public instance field of the provided data type.
        /// </summary>
        /// <param name="dataType">The model type whose public instance fields will be turned into accessors and events.</param>
        /// <summary>
        /// Generate accessor methods and change-event declarations for each public instance field of the given data type.
        /// </summary>
        /// <returns>
        /// A C# source fragment that, for each public instance field, includes:
        /// - a getter method returning `Value.<field>`,
        /// - a setter method that assigns the field only when the new value differs and invokes an `On<FieldName>Changed` event,
        /// - and the corresponding `public event Action<T>` declaration.
        /// If a field has a reference attribute, additional reference accessor code may be appended.
        /// </returns>
        private static string BuildSingleAccessors(Type dataType)
        {
            var sb = new System.Text.StringBuilder();

            foreach (var field in dataType.GetFields(
                         BindingFlags.Public | BindingFlags.Instance))
            {
                string typeName = GetTypeName(field.FieldType);
                string name = field.Name;
                string pascal = UpperFirst(name);

                sb.AppendLine($@"
        /// <summary>
        /// Getter for {name}
        /// </summary>
        public {typeName} Get{pascal}() => Value.{name};

        /// <summary>
        /// Setter for {name}
        /// </summary>
        public void Set{pascal}({typeName} value)
        {{
            if (Equals(Value.{name}, value)) return;
            Value.{name} = value;
            On{pascal}Changed?.Invoke(value);
        }}

        /// <summery>
        /// Event which trigger when value is changed
        /// </summery>
        public event Action<{typeName}> On{pascal}Changed;

");

                var code = GenerateReferenceAccessor(field);
                if (!string.IsNullOrEmpty(code))
                {
                    sb.AppendLine(code);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates iterator-style `FindBy{Field}` methods for a list-backed model using the public instance fields of the provided data type.
        /// </summary>
        /// <param name="dataType">The data model type whose public instance fields will be used to create `FindBy...` methods.</param>
        /// <summary>
        /// Generate source for list-based accessor methods that return items matching a field value.
        /// </summary>
        /// <returns>A C# source fragment containing one method per public instance field named FindBy{FieldPascal}({FieldType} value) that yields items from _items where the field equals the provided value.</returns>
        private static string BuildListAccessors(Type dataType)
        {
            var sb = new System.Text.StringBuilder();

            foreach (var field in dataType.GetFields(
                         BindingFlags.Public | BindingFlags.Instance))
            {
                string typeName = GetTypeName(field.FieldType);
                string name = field.Name;
                string pascal = UpperFirst(name);

                sb.AppendLine($"        public IEnumerable<{dataType.FullName}> FindBy{pascal}({typeName} value)");
                sb.AppendLine("        {");
                sb.AppendLine("            foreach (var item in _items)");
                sb.AppendLine($"                if (Equals(item.{name}, value))");
                sb.AppendLine("                    yield return item;");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a C# source fragment that implements database CRUD accessors for the model described by the given entry.
        /// </summary>
        /// <param name="entry">ModelEntry describing the model type to generate DB accessors for; its Type must represent a DB-backed model.</param>
        /// <returns>A string containing the C# code fragment with CreateTable, Insert, Update, Delete, GetById, and GetAll method implementations for the model.</returns>
        /// <exception cref="System.Exception">Thrown when the model type does not declare a field with the [PrimaryKey] attribute.</exception>
        public static string BuildDBAccessors(ModelEntry entry)
        {
            var type = entry.Type;
            string modelName = type.Name;
            string dbClassName = modelName + "Model";

            var fields = type.GetFields(
                BindingFlags.Public | BindingFlags.Instance);

            var pk = fields.FirstOrDefault(f =>
                f.GetCustomAttribute<PrimaryKeyAttribute>() != null);

            if (pk == null)
                throw new Exception($"No [PrimaryKey] defined for DB model {modelName}");

            string table = modelName;
            
            StringBuilder sb = new();
            
            sb.AppendLine("        /// <summary>Creates the database table if it does not exist.</summary>");
            sb.AppendLine("        public override void CreateTable()");
            sb.AppendLine("        {");
            sb.AppendLine("            DataContext.Database.Execute(@\"");

            sb.AppendLine($"CREATE TABLE IF NOT EXISTS {table} (");

            var columns = new List<string>();

            foreach (var f in fields)
            {
                string sqlType = GetSqlType(f.FieldType);

                bool isPk =
                    f.GetCustomAttribute<PrimaryKeyAttribute>() != null;

                string col = $"    {f.Name} {sqlType}";

                if (isPk)
                    col += " PRIMARY KEY";

                columns.Add(col);
            }

            sb.AppendLine(string.Join(",\n", columns));
            sb.AppendLine(");\");");

            sb.AppendLine("        }");
            sb.AppendLine();

            // ================================
            // INSERT
            // ================================

            string colNames =
                string.Join(", ", fields.Select(f => f.Name));

            string paramNames =
                string.Join(", ", fields.Select(f => "@" + f.Name));

            sb.AppendLine($@"        /// <summary>
        /// Inserts a new record of <see cref=""{type.Name}""/> into it. 
        /// </summary>
        /// <param name=""item"">Item to add</param>");
            sb.AppendLine($"        public override void Insert({modelName} item)");
            sb.AppendLine("        {");
            sb.AppendLine("            DataContext.Database.Execute(@\"");
            sb.AppendLine($"INSERT INTO {table} ({colNames})");
            sb.AppendLine($"VALUES ({paramNames});\", item);");
            sb.AppendLine("        }");
            sb.AppendLine();

            // ================================
            // UPDATE
            // ================================

            var setClause = string.Join(", ",
                fields.Where(f => f != pk)
                    .Select(f => $"{f.Name}=@{f.Name}"));

            sb.AppendLine($@"        /// <summary>
        /// Updates an existing <see cref=""{type.Name}""/>.
        /// </summary>
        /// <param name=""item"">Item to add</param>");
            sb.AppendLine($"        public override void Update({modelName} item)");
            sb.AppendLine("        {");
            sb.AppendLine("            DataContext.Database.Execute(@\"");
            sb.AppendLine($"UPDATE {table}");
            sb.AppendLine($"SET {setClause}");
            sb.AppendLine($"WHERE {pk.Name}=@{pk.Name};\", item);");
            sb.AppendLine("        }");
            sb.AppendLine();

            // ================================
            // DELETE
            // ================================

            sb.AppendLine($@"        /// <summary>Deletes a <see cref=""{type.Name}""/>. by primary key.</summary>
        /// <param name=""id"">item id to be delete</param>");
            sb.AppendLine($"        public override void Delete({GetTypeName(pk.FieldType)} id)");
            sb.AppendLine("        {");
            sb.AppendLine("            DataContext.Database.Execute(");
            sb.AppendLine($"                \"DELETE FROM {table} WHERE {pk.Name}=@Id;\",");
            sb.AppendLine("                new { Id = id });");
            sb.AppendLine("        }");
            sb.AppendLine();

            // ================================
            // GET BY ID
            // ================================

            sb.AppendLine($@"        /// <summary>
        /// Retrieves a <see cref=""{type.Name}""/> by id.
        /// </summary>
        /// <param name=""id"">id of the <see cref=""{type.Name}""/></param>
        /// <returns><see cref=""{type.Name}""/> in question</returns>");
            sb.AppendLine($"        public override {modelName} GetById({GetTypeName(pk.FieldType)} id)");
            sb.AppendLine("        {");
            sb.AppendLine("            return DataContext.Database.QuerySingle<" + modelName + ">(");
            sb.AppendLine($"                \"SELECT * FROM {table} WHERE {pk.Name}=@Id;\",");
            sb.AppendLine("                new { Id = id });");
            sb.AppendLine("        }");
            sb.AppendLine();

            // ================================
            // GET ALL
            // ================================

            sb.AppendLine($@"        /// <summary>
        /// Retrieves all <see cref=""{type.Name}""/>.
        /// </summary>
        /// <returns>all of the <see cref=""{type.Name}""/>.</returns>");
            sb.AppendLine($"        public override IEnumerable<{modelName}> GetAll()");
            sb.AppendLine("        {");
            sb.AppendLine($"            return DataContext.Database.Query<{modelName}>(");
            sb.AppendLine($"                \"SELECT * FROM {table};\");");
            sb.AppendLine("        }");

            return sb.ToString();
        }

        /// <summary>
        /// Finds the first public instance field on the specified type that is marked with PrimaryKeyAttribute.
        /// </summary>
        /// <param name="type">The type to inspect for a primary key field.</param>
        /// <returns>The FieldInfo for the first field decorated with PrimaryKeyAttribute, or null if none is found.</returns>
        private static FieldInfo GetPrimaryKey(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(f => f.GetCustomAttribute<PrimaryKeyAttribute>() != null);
        }

        /// <summary>
        /// Generates the C# source fragment for a reference getter method for the specified field.
        /// </summary>
        /// <param name="field">The field annotated with <c>ReferenceAttribute</c> that references another model type.</param>
        /// <returns>The source code string for a getter method that returns the referenced model instance, or <c>null</c> if the field has no <c>ReferenceAttribute</c>.</returns>
        /// <exception cref="System.Exception">Thrown when the referenced target model type does not declare a field marked with <c>[PrimaryKey]</c>.</exception>
        private static string GenerateReferenceAccessor(FieldInfo field)
        {
            var referenceAttr =
                field.GetCustomAttribute<ReferenceAttribute>();

            if (referenceAttr == null)
                return null;

            Type targetType = referenceAttr.TargetModelType;
            FieldInfo pk = GetPrimaryKey(targetType);

            if (pk == null)
                throw new Exception(
                    $"No [PrimaryKey] found in {targetType.Name}");

            string targetModelName = targetType.Name + "Model";
            string methodName = "Get" +
                                targetType.Name; // GetInventoryItem

            string pkPascal =
                char.ToUpper(pk.Name[0]) + pk.Name.Substring(1);

            string fieldName = field.Name;

            return
                $@"
        /// <summary>
        /// Getter for {fieldName} to {targetModelName}
        /// </summary>
        public {targetType.FullName} {methodName}()
        {{
            if (Value.{fieldName} == null)
                return default;

            return DataContext.Get<{targetModelName}>()
                .Query()
                .{pkPascal}Equals(Value.{fieldName})
                .FirstOrDefault();
        }}
";
        }

        /// <summary>
        /// Generates the source code for a fluent query method that filters by a referenced model's primary key.
        /// </summary>
        /// <param name="dataType">The owner model type for which the query class is being generated.</param>
        /// <param name="field">The reference field on <paramref name="dataType"/> that targets another model.</param>
        /// <returns>
        /// A C# method source string that implements a fluent `<OwnerType>Query.{BaseName}Equals(<TargetType> value)` method,
        /// or `null` if the provided <paramref name="field"/> does not have a <see cref="ReferenceAttribute"/>.
        /// </returns>
        /// <exception cref="Exception">Thrown when the referenced target model does not declare a field with <see cref="PrimaryKeyAttribute"/>.</exception>
        private static string GenerateReferenceQueryMethod(Type dataType, FieldInfo field)
        {
            var refAttr = field.GetCustomAttribute<ReferenceAttribute>();

            if (refAttr == null)
                return null;

            Type targetType = refAttr.TargetModelType;

            FieldInfo pk = targetType
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(f =>
                    f.GetCustomAttribute<PrimaryKeyAttribute>() != null);

            if (pk == null)
                throw new Exception(
                    $"No [PrimaryKey] found in {targetType.Name}");

            string queryName = dataType.Name + "Query";

            // Field name (ItemId)
            string fieldName = field.Name;

            // Strip "Id" → Item
            string baseName = fieldName.EndsWith("Id")
                ? fieldName.Substring(0, fieldName.Length - 2)
                : targetType.Name;

            string methodName = baseName + "Equals";

            string pkName = pk.Name;

            return
                $@"        public {queryName} {methodName}({targetType.FullName} value)
        {{
            if (value == null)
                return this;

            AddCondition(i => i.{fieldName} == value.{pkName});
            return this;
        }}
";
        }

        /// <summary>
        /// Appends fluent query-method source code for a single field to the provided StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder to receive generated method code.</param>
        /// <param name="dataType">The model type that owns the field; used to name the generated query class.</param>
        /// <summary>
        /// Appends fluent query methods for a single field to the provided StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder that will receive the generated query method source text.</param>
        /// <param name="dataType">The model type that owns the query class for which methods are being generated.</param>
        /// <param name="field">The FieldInfo describing the field for which equality, numeric comparison, containment, and reference-based methods will be generated as applicable.</param>
        private static void BuildQueryMethods(System.Text.StringBuilder sb, Type dataType, FieldInfo field)
        {
            string typeName = GetTypeName(field.FieldType);
            string name = field.Name;
            string pascal = UpperFirst(name);
            string queryName = dataType.Name + "Query";

            // Equality (all types)
            sb.AppendLine(
                $@"        public {queryName} {pascal}Equals({typeName} value)
        {{
            AddCondition(i => i.{name} == value);
            return this;
        }}
");

            // Numeric comparisons
            if (IsNumeric(field.FieldType))
            {
                sb.AppendLine(
                    $@"        public {queryName} {pascal}GreaterThan({typeName} value)
        {{
            AddCondition(i => i.{name} > value);
            return this;
        }}

        public {queryName} {pascal}LessThan({typeName} value)
        {{
            AddCondition(i => i.{name} < value);
            return this;
        }}

        public {queryName} {pascal}GreaterThanEqualTo({typeName} value)
        {{
            AddCondition(i => i.{name} >= value);
            return this;
        }}

        public {queryName} {pascal}LessThanEqualTo({typeName} value)
        {{
            AddCondition(i => i.{name} <= value);
            return this;
        }}
");
            }

            // String helpers
            if (field.FieldType == typeof(string))
            {
                sb.AppendLine(
                    $@"        public {queryName} {pascal}Contains(string value)
        {{
            AddCondition(i => i.{name} != null && i.{name}.Contains(value));
            return this;
        }}
");
            }

            string refMethod = GenerateReferenceQueryMethod(dataType, field);

            if (!string.IsNullOrEmpty(refMethod))
            {
                sb.AppendLine(refMethod);
            }
        }

        /// <summary>
        /// Determines whether the provided Type represents a supported numeric primitive.
        /// </summary>
        /// <param name="type">The CLR type to check.</param>
        /// <summary>
        /// Determines whether the provided Type is one of the supported numeric CLR primitives.
        /// </summary>
        /// <returns>`true` if the type is one of: `int`, `float`, `double`, `long`, `short`, or `byte`; `false` otherwise.</returns>
        private static bool IsNumeric(Type type)
        {
            return type == typeof(int) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(long) ||
                   type == typeof(short) ||
                   type == typeof(byte);
        }

        /// <summary>
        /// Converts the first character of the provided string to uppercase.
        /// </summary>
        /// <param name="s">The input string; must be non-empty.</param>
        /// <summary>
            /// Converts the first character of the provided string to uppercase.
            /// </summary>
            /// <param name="s">A non-empty input string.</param>
            /// <returns>The input string with its first character converted to uppercase and the remainder unchanged.</returns>
        private static string UpperFirst(string s)
            => char.ToUpper(s[0]) + s.Substring(1);

        /// <summary>
        /// Removes an outer `namespace` wrapper from a C# source text and returns the inner contents without the enclosing namespace block.
        /// </summary>
        /// <param name="text">C# source text that may contain a top-level `namespace { ... }` wrapper.</param>
        /// <returns>The contents between the first `{` after the `namespace` keyword and the last `}` in the input, trimmed; if no `namespace` keyword is found, returns the original text.</returns>
        private static string RemoveNamespaceBlock(string text)
        {
            const string keyword = "namespace ";

            int start = text.IndexOf(keyword, StringComparison.Ordinal);
            if (start < 0) return text;

            int open = text.IndexOf('{', start);
            int close = text.LastIndexOf('}');

            return text.Substring(open + 1, close - open - 1).Trim();
        }

        /// <summary>
        /// Produce a C#-style full name for the given Type, formatting generic types with their type arguments in angle brackets.
        /// </summary>
        /// <param name="type">The type to format.</param>
        /// <summary>
        /// Produces a C# type name suitable for source generation, formatting generic types with their argument types.
        /// </summary>
        /// <param name="type">The type to format; may be a generic or non-generic type.</param>
        /// <returns>`type.FullName` for non-generic types. For generic types, the generic definition name (without the arity suffix) followed by `<T1, T2, ...>`, where each generic argument is formatted recursively.</returns>
        private static string GetTypeName(Type type)
        {
            if (!type.IsGenericType)
                return type.FullName;

            var generic = type.GetGenericTypeDefinition();
            var args = type.GetGenericArguments()
                .Select(GetTypeName);

            return $"{generic.FullName.Split('`')[0]}<{string.Join(", ", args)}>";
        }

        /// <summary>
        /// Generates the C# source code for a database-backed model class based on the provided model entry.
        /// </summary>
        /// <param name="entry">Metadata describing the model type to generate (contains the reflected Type and attributes).</param>
        /// <returns>The generated C# source text for the DB model class.</returns>
        /// <exception cref="System.Exception">Thrown when the model type does not define a field marked with <c>[PrimaryKey]</c>.</exception>
        public static string BuildDbModel(ModelEntry entry)
        {
            var type = entry.Type;
            string modelName = type.Name;
            string dbClassName = modelName + "Model";

            var fields = type.GetFields(
                BindingFlags.Public | BindingFlags.Instance);

            var pk = fields.FirstOrDefault(f =>
                f.GetCustomAttribute<PrimaryKeyAttribute>() != null);

            if (pk == null)
                throw new Exception($"No [PrimaryKey] defined for DB model {modelName}");

            string table = modelName;

            var sb = new StringBuilder();

            sb.AppendLine("/* Auto-generated DB access code. DO NOT MODIFY */");
            sb.AppendLine();

            sb.AppendLine("using System.Collections.Generic;");
            
            // ================================
            // Namespace
            // ================================

            if (!string.IsNullOrEmpty(type.Namespace))
            {
                sb.AppendLine($"namespace {type.Namespace}");
                sb.AppendLine("{");
            }

            sb.AppendLine($"    public class {dbClassName} : SQLGameModel<{type.FullName}>");
            sb.AppendLine("    {");

            sb.AppendLine($"        public const string TableName = \"{table}\";");
            sb.AppendLine();

            // ================================
            // CREATE TABLE
            // ================================

            sb.AppendLine("    }");

            if (!string.IsNullOrEmpty(type.Namespace))
                sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Maps a CLR type to its corresponding SQLite-like column type name.
        /// </summary>
        /// <param name="t">The CLR type to map.</param>
        /// <returns>`"INTEGER"` for integer-like and boolean types; `"REAL"` for floating-point types; `"TEXT"` for string and all other types.</returns>
        private static string GetSqlType(Type t)
        {
            if (t == typeof(int) || t == typeof(long) ||
                t == typeof(short) || t == typeof(byte))
                return "INTEGER";

            if (t == typeof(float) || t == typeof(double))
                return "REAL";

            if (t == typeof(bool))
                return "INTEGER";

            if (t == typeof(string))
                return "TEXT";

            return "TEXT"; // fallback
        }
    }
}
