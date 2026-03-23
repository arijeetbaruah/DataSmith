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
        /// Creates a SQLite-backed database using the asset's FileName located under Application.persistentDataPath.
        /// </summary>
        /// <returns>An IDatabase instance backed by the SQLite file at the constructed persistent data path.</returns>
        public override IDatabase CreateDatabase()
        {
            string path = Path.Combine(Application.persistentDataPath, FileName);

            return new SqliteDatabase(path);
        }

        public class SqliteDatabase : IDatabase
        {
            private readonly string _connectionString;

            /// <summary>
            /// Initializes a SqliteDatabase that will connect to the SQLite file at the specified path.
            /// </summary>
            /// <param name="path">Filesystem path to the SQLite database file to be used for connections.</param>
            public SqliteDatabase(string path)
            {
                _connectionString = $"URI=file:{path}";
            }

            /// <summary>
            /// Executes a non-query SQL statement against the database using the instance's connection string.
            /// </summary>
            /// <param name="sql">The SQL command text to execute.</param>
            /// <param name="param">Optional object whose public fields are bound as parameters; each field becomes a parameter named "@" + field name.</param>
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
            /// Retrieve the first result of the given SQL query mapped to an instance of T.
            /// </summary>
            /// <param name="sql">The SQL query text to execute.</param>
            /// <param name="param">An optional parameter object whose public fields are bound to SQL parameters named by `@{FieldName}`.</param>
            /// <returns>The first mapped instance of T if a row is returned, `default(T)` otherwise.</returns>
            /// <remarks>Column values are assigned to T's public instance fields by matching column names; columns with `DBNull` are left as the field's default value.</remarks>
            public T QuerySingle<T>(string sql, object param = null)
            {
                foreach (var item in Query<T>(sql, param))
                    return item;

                return default;
            }

            /// <summary>
            /// Enumerates rows produced by the SQL query and maps each row to an instance of T.
            /// </summary>
            /// <param name="sql">The SQL query or command text to execute.</param>
            /// <param name="param">Optional object whose public fields are bound as query parameters using the names <c>@FieldName</c>.</param>
            /// <returns>An enumerable that yields instances of <typeparamref name="T"/> mapped from each result row; yields no elements if the result set is empty.</returns>
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
            /// Adds parameters to the provided <see cref="SqliteCommand"/> by reflecting over the public fields of the given parameter object.
            /// </summary>
            /// <param name="cmd">The command whose Parameters collection will be populated.</param>
            /// <param name="param">An object whose public fields are used as parameter names and values; each field is added as a parameter named "@" + field name. If <c>null</c>, no parameters are added.</param>

            private void AddParameters(SqliteCommand cmd, object param)
            {
                if (param == null) return;

                foreach (var p in param.GetType().GetFields())
                {
                    var value = p.GetValue(param);
                    cmd.Parameters.AddWithValue("@" + p.Name, value);
                }
            }

            /// <summary>
            /// Creates an instance of T and populates its public instance fields from the current row of the provided SqliteDataReader using matching column names.
            /// </summary>
            /// <param name="reader">A SqliteDataReader positioned at the row to map from.</param>
            /// <returns>An instance of T with public instance fields set from the corresponding columns; fields remain at their default values when the column value is DBNull.</returns>
            private T Map<T>(SqliteDataReader reader)
            {
                var obj = System.Activator.CreateInstance<T>();

                var fields = typeof(T)
                    .GetFields(System.Reflection.BindingFlags.Public |
                               System.Reflection.BindingFlags.Instance);

                foreach (var f in fields)
                {
                    int i = reader.GetOrdinal(f.Name);

                    if (!reader.IsDBNull(i))
                        f.SetValue(obj, reader.GetValue(i));
                }

                return obj;
            }
        }
    }
}
