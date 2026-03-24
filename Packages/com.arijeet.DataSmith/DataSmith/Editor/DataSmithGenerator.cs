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
        /// Generates C# source files for the given model entry (model and query) and imports them into the Unity asset database.
        /// </summary>
        /// <param name="entry">Model metadata describing the CLR type and generation attributes (used to select the template and build code).</param>
        /// <param name="outputFolder">Destination folder path where the generated .cs files will be written.</param>
        /// <remarks>
        /// If no template matches the entry's value type, the method does nothing.
        /// </remarks>
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
        /// Generates a DataContext.cs file that lists the provided model types, writes it into the specified output folder, and imports the file into the Unity asset database.
        /// </summary>
        /// <param name="modelTypes">Collection of model CLR types to include; each type will be referenced as <c>typeof(&lt;FullName&gt;Model)</c> in the generated context.</param>
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
        /// Generates model and query source files for each provided model entry, creates or updates the project DataContext, and refreshes Unity's asset database.
        /// </summary>
        /// <param name="entries">The collection of model entries to generate code for.</param>
        /// <param name="outputFolder">Target folder path within the Unity project where generated files will be written.</param>
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
        /// Searches the Unity project for TextAsset generation templates and returns the found templates keyed by their ModelValueType.
        /// </summary>
        /// <returns>
        /// A dictionary mapping each discovered <see cref="ModelValueType"/> (for example, <c>Single</c>, <c>List</c>, <c>DB</c>) to the corresponding template text.
        /// Asset names containing "SingleModelTemplate", "ListModelTemplate", or "SQLGameModelTemplate" are mapped to <c>ModelValueType.Single</c>, <c>ModelValueType.List</c>, and <c>ModelValueType.DB</c> respectively; value types with no matching asset are omitted.
        /// </returns>
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
        /// Generates the accessor source fragment for a model described by the given entry based on its ValueType.
        /// </summary>
        /// <param name="entry">The model entry that provides the target Type and ValueType configuration used to select which accessor code to generate.</param>
        /// <returns>The generated C# source fragment containing accessor methods and related declarations for the model, or an empty string if no accessor generator applies.</returns>
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
        /// Produce the model source code by filling the provided template with values derived from the given model entry.
        /// </summary>
        /// <param name="entry">Model metadata (type and generation attributes) used to populate template placeholders.</param>
        /// <param name="template">Template text containing placeholders such as {{MODEL_NAME}}, {{TABLE_NAME}}, {{DATA_TYPE}}, {{ACCESSORS}}, {{NAMESPACE}}, and {{QUERY_NAME}}.</param>
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
        /// Generate C# source for a sealed query class for the model type described by the given entry.
        /// </summary>
        /// <param name="entry">ModelEntry that contains the model Type and metadata used to build the query class.</param>
        /// <returns>The C# source code for a sealed query class named &lt;TypeName&gt;Query; the output is wrapped in the model's namespace when the model type defines one and includes fluent condition methods for each public instance field.</returns>
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
        /// Generates C# accessor methods and change-event declarations for each public instance field of the provided type.
        /// </summary>
        /// <param name="dataType">The data model type whose public instance fields will be inspected to produce accessors.</param>
        /// <returns>A string containing the generated C# code: for each field a getter, a setter that updates the field and invokes a change event when the value changes, an event declaration, and any additional reference accessor code when applicable.</returns>
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
        /// Generate iterator-style FindBy methods for each public instance field of the provided type.
        /// </summary>
        /// <returns>A string containing C# source code that defines for each public instance field a method `IEnumerable{T} FindBy{Field}(FieldType value)` which yields items from `_items` where the field equals the provided value.</returns>
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
        /// Generates C# accessor code and SQL-backed CRUD method implementations for a database-backed model type.
        /// </summary>
        /// <param name="entry">Model metadata that contains the target type and generation attributes.</param>
        /// <returns>The generated source code fragment implementing DB accessors and CRUD methods for the model.</returns>
        /// <exception cref="Exception">Thrown when the target model does not declare exactly one field marked with [PrimaryKey].</exception>
        public static string BuildDBAccessors(ModelEntry entry)
        {
            var type = entry.Type;
            string modelName = type.Name;
            string dbClassName = modelName + "Model";

            var fields = type.GetFields(
                BindingFlags.Public | BindingFlags.Instance);

            var pkFields = fields
                .Where(f => f.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                .ToArray();

            if (pkFields.Length != 1)
            {
                throw new Exception($"Expected exactly one [PrimaryKey] on DB model {modelName}, found {pkFields.Length}.");
            }

            var pk = pkFields[0];

            string table = QuoteSqlIdentifier(modelName);
            
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

                bool isPk = f == pk;

                string col = $"    {QuoteSqlIdentifier(f.Name)} {sqlType}";

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
                string.Join(", ", fields.Select(f => "@" + f.Name));

            string paramNames =
                string.Join(", ", fields.Select(f => "@" + QuoteSqlIdentifier(f.Name)));

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
                    .Select(f => $"{QuoteSqlIdentifier(f.Name)}=@{f.Name}"));

            sb.AppendLine($@"        /// <summary>
        /// Updates an existing <see cref=""{type.Name}""/>.
        /// </summary>
        /// <param name=""item"">Item to add</param>");
            sb.AppendLine($"        public override void Update({modelName} item)");
            sb.AppendLine("        {");
            sb.AppendLine("            DataContext.Database.Execute(@\"");
            sb.AppendLine($"UPDATE {table}");
            sb.AppendLine($"SET {setClause}");
            sb.AppendLine($"WHERE {QuoteSqlIdentifier(pk.Name)}=@{pk.Name};\", item);");
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
            sb.AppendLine($"                \"DELETE FROM {table} WHERE {QuoteSqlIdentifier(pk.Name)}=@Id;\",");
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
            sb.AppendLine($"                \"SELECT * FROM {table} WHERE {QuoteSqlIdentifier(pk.Name)}=@Id;\",");
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
            /// Quote an SQL identifier for safe use in SQL statements by wrapping it in double quotes.
            /// </summary>
            /// <param name="name">The identifier to quote (e.g., table or column name).</param>
            /// <returns>The identifier wrapped in double quotes; any existing double quotes inside are doubled.</returns>
            private static string QuoteSqlIdentifier(string name)
            => "\"" + name.Replace("\"", "\"\"") + "\"";

        /// <summary>
        /// Finds the first public instance field on the given type that is annotated with <see cref="PrimaryKeyAttribute"/>.
        /// </summary>
        /// <param name="type">The type to inspect for a primary key field.</param>
        /// <returns>The <see cref="FieldInfo"/> for the primary key field if one is found; otherwise <c>null</c>.</returns>
        private static FieldInfo GetPrimaryKey(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(f => f.GetCustomAttribute<PrimaryKeyAttribute>() != null);
        }

        /// <summary>
        /// Generate C# source for a getter that resolves the referenced model instance for the given field.
        /// </summary>
        /// <param name="field">The field to inspect for a <c>ReferenceAttribute</c> and to generate the accessor for.</param>
        /// <returns>The generated getter method source as a string, or <c>null</c> if the field has no <c>ReferenceAttribute</c>.</returns>
        /// <exception cref="Exception">Thrown when the referenced target type does not declare a field marked with <c>PrimaryKeyAttribute</c>.</exception>
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
        /// Generates the C# source for a fluent query method on the owning query class that accepts the referenced model
        /// and adds a condition comparing the owner's reference field to the referenced model's primary key.
        /// </summary>
        /// <param name="dataType">The type that owns the reference field (the model for which the query class is generated).</param>
        /// <param name="field">The FieldInfo representing the reference field on <paramref name="dataType"/>.</param>
        /// <returns>The method source code to append to the owning query class, or <c>null</c> if the field is not decorated with <c>ReferenceAttribute</c>.</returns>
        /// <exception cref="Exception">Thrown when the referenced target model does not contain a field marked with <c>PrimaryKeyAttribute</c>.</exception>
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
        /// Appends fluent query-method source for the specified field to the given StringBuilder.
        /// </summary>
        /// <param name="sb">StringBuilder to which generated method code will be appended.</param>
        /// <param name="dataType">The model type that owns the field; used to form the query class name.</param>
        /// <param name="field">The FieldInfo describing the field for which equality, numeric comparison, and string containment methods will be generated when applicable.</param>
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
        /// Determines whether the provided <see cref="Type"/> is a supported numeric CLR type.
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
            /// Convert the first character of the provided string to uppercase and return the resulting string.
            /// </summary>
            /// <param name="s">The input string; must contain at least one character.</param>
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
        /// Produces the type's full name, formatting generic types with their type arguments.
        /// </summary>
        /// <returns>The full name of the type. For generic types, returns the generic definition name followed by `&lt;T1, T2, ...&gt;` where each argument is formatted recursively.</returns>
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
        /// Maps a CLR type to the corresponding SQLite column type name.
        /// </summary>
        /// <param name="t">The CLR type to map to an SQLite column type.</param>
        /// <returns>The SQLite column type name (for example, "INTEGER", "REAL", or "TEXT").</returns>
        /// <exception cref="NotSupportedException">Thrown when the provided CLR type has no supported SQLite mapping.</exception>
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

            throw new NotSupportedException($"Unsupported SQLite field type '{t.FullName}'. Add an explicit converter or extend GetSqlType().");
        }
    }
}
