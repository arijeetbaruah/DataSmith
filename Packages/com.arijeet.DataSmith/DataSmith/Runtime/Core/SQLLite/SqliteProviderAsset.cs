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

        public override IDatabase CreateDatabase()
        {
            string path = Path.Combine(Application.persistentDataPath, FileName);

            return new SqliteDatabase(path);
        }

        public class SqliteDatabase : IDatabase
        {
            private readonly string _connectionString;

            public SqliteDatabase(string path)
            {
                _connectionString = $"URI=file:{path}";
            }

            public void Execute(string sql, object param = null)
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                AddParameters(cmd, param);
                cmd.ExecuteNonQuery();
            }

            public T QuerySingle<T>(string sql, object param = null)
            {
                foreach (var item in Query<T>(sql, param))
                    return item;

                return default;
            }

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

            // --- Helpers (simplified) ---

            private void AddParameters(SqliteCommand cmd, object param)
            {
                if (param == null) return;

                foreach (var p in param.GetType().GetFields())
                {
                    var value = p.GetValue(param);
                    cmd.Parameters.AddWithValue("@" + p.Name, value);
                }
            }

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
