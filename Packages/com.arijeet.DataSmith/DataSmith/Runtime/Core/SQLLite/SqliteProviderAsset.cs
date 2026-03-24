using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Mono.Data.Sqlite;

namespace Baruah.DataSmith.Database
{
    [System.Serializable]
    public class SqliteProviderAsset : DatabaseProviderAsset
    {
        [Header("SQLite Settings")] [Tooltip("Database file name in persistentDataPath")]
        public string FileName = "game.db";

        /// <summary>
        /// Creates an IDatabase backed by an SQLite file located under Application.persistentDataPath using the configured FileName.
        /// </summary>
        /// <returns>An IDatabase instance that uses the SQLite file at the combined persistent data path and FileName.</returns>
        public override IDatabase CreateDatabase()
        {
            string path = Path.Combine(Application.persistentDataPath, FileName);

            return new SqliteDatabase(path);
        }

        public class SqliteDatabase : IDatabase
        {
            private readonly string _connectionString;

            /// <summary>
            /// Initializes a SqliteDatabase with a SQLite connection string that points to the specified database file.
            /// </summary>
            /// <param name="path">The full filesystem path to the SQLite database file; used to construct the connection string as "URI=file:{path}".</param>
            public SqliteDatabase(string path)
            {
                _connectionString = $"URI=file:{path}";
            }

            /// <summary>
            /// Executes a SQL non-query command against the configured SQLite database.
            /// </summary>
            /// <param name="sql">The SQL statement or command to execute.</param>
            /// <param name="param">An optional object whose public instance fields and readable properties are bound as parameters by name (prefixed with '@'); null if there are no parameters.</param>
            public void Execute(string sql, object param = null)
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                AddParameters(cmd, param);
                cmd.ExecuteNonQuery();
            }

            /// <summary>
            /// Fetches the first result of the provided SQL query and maps it to an instance of <typeparamref name="T"/>.
            /// </summary>
            /// <param name="param">An optional object whose public instance fields and readable properties are bound as query parameters (named by member). Null values become <see cref="System.DBNull.Value"/>.</param>
            /// <returns>The first mapped instance of <typeparamref name="T"/> if a row is returned, otherwise <c>default(T)</c>.</returns>
            public T QuerySingle<T>(string sql, object param = null)
            {
                foreach (var item in Query<T>(sql, param))
                    return item;

                return default;
            }

            /// <summary>
            /// Executes the provided SQL query and yields instances of <typeparamref name="T"/> mapped from each result row.
            /// </summary>
            /// <param name="sql">The SQL query to execute.</param>
            /// <param name="param">An optional parameter object whose public instance fields and readable public instance properties are bound as SQL parameters (parameter names are the member names prefixed with '@').</param>
            /// <returns>An enumerable that yields a mapped <typeparamref name="T"/> for each row in the result set. Enumeration opens a database connection and reader; the connection and reader are closed when the enumeration is disposed or completed.</returns>
            public IEnumerable<T> Query<T>(string sql, object param = null)
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                AddParameters(cmd, param);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                    yield return Map<T>(reader);
            }

            /// <summary>
            /// Adds parameters to a <see cref="SqliteCommand"/> by reflecting over the provided object's public instance fields and readable public instance properties.
            /// </summary>
            /// <param name="cmd">The command to which parameters will be added.</param>
            /// <param name="param">An object whose public instance fields and readable public instance properties (excluding indexers) are used as parameter names and values; if null, no parameters are added.</param>
            /// <remarks>Parameter names are the member name prefixed with '@'. Member values that are null are added as <see cref="System.DBNull.Value"/>.</remarks>

            private void AddParameters(SqliteCommand cmd, object param)
            {
                if (param == null) return;

                foreach (var p in param.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    var value = p.GetValue(param);
                    cmd.Parameters.AddWithValue("@" + p.Name, value ?? System.DBNull.Value);
                }

                foreach (var p in param.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (!p.CanRead || p.GetIndexParameters().Length != 0)
                        continue;

                    var value = p.GetValue(param, null);
                    cmd.Parameters.AddWithValue("@" + p.Name, value ?? System.DBNull.Value);
                }
            }

            /// <summary>
            /// Creates an instance of <typeparamref name="T"/> and populates its public instance fields from the current row of the provided <see cref="SqliteDataReader"/>, matching fields to columns by name.
            /// </summary>
            /// <param name="reader">A data reader positioned on the row to map; column names are matched to public instance field names of <typeparamref name="T"/>.</param>
            /// <returns>An instance of <typeparamref name="T"/> with values assigned for each public instance field whose matching column exists and is not NULL.</returns>
            private T Map<T>(SqliteDataReader reader)
            {
                var obj = System.Activator.CreateInstance<T>();

                var fields = typeof(T)
                    .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                foreach (var f in fields)
                {
                    int i;
                    try
                    {
                        i = reader.GetOrdinal(f.Name);
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        continue; // column not present in current projection
                    }

                    if (!reader.IsDBNull(i))
                    {
                        object raw = reader.GetValue(i);
                        object value = f.FieldType == typeof(bool)
                            ? System.Convert.ToInt64(raw) != 0
                            : f.FieldType.IsEnum
                                ? System.Enum.ToObject(f.FieldType, raw)
                                : System.Convert.ChangeType(raw, f.FieldType);

                        f.SetValue(obj, value);
                    }
                }

                return obj;
            }
        }
    }
}
