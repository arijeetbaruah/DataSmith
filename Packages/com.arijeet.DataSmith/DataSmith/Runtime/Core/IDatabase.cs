using System.Collections.Generic;

namespace Baruah.DataSmith.Database
{
    public interface IDatabase
    {
        /// <summary>
/// Executes the provided SQL statement against the database.
/// </summary>
/// <param name="sql">The SQL command text to execute.</param>
/// <param name="param">Optional parameters for the SQL statement (e.g., anonymous object or parameter collection).</param>
void Execute(string sql, object param = null);

        /// <summary>
/// Executes a SQL query expected to return a single row and maps that row to <typeparamref name="T"/>.
— </summary>
/// <param name="sql">SQL query text to execute.</param>
/// <param name="param">Optional parameters for the query; may be null.</param>
/// <returns>An instance of <typeparamref name="T"/> representing the mapped result.</returns>
T QuerySingle<T>(string sql, object param = null);

        /// <summary>
/// Executes the provided SQL query and maps each result row to an instance of <typeparamref name="T"/>.
/// </summary>
/// <param name="sql">The SQL query to execute.</param>
/// <param name="param">Optional parameters for the query (e.g., a named-parameter object); pass null if none.</param>
/// <returns>An enumerable of results mapped to <typeparamref name="T"/>.</returns>
IEnumerable<T> Query<T>(string sql, object param = null);
    }
}
