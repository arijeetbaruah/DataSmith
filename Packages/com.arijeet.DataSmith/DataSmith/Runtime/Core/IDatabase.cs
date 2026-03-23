using System.Collections.Generic;

namespace Baruah.DataSmith.Database
{
    public interface IDatabase
    {
        /// <summary>
/// Executes a non-query SQL command represented by the provided SQL text.
/// </summary>
/// <param name="sql">The SQL command text to execute (e.g., INSERT, UPDATE, DELETE).</param>
/// <param name="param">Optional parameter object for parameterized queries; pass null if there are no parameters.</param>
void Execute(string sql, object param = null);

        /// <summary>
/// Retrieves a single record from the database and maps it to type T.
/// </summary>
/// <param name="sql">The SQL query to execute; expected to return a single row.</param>
/// <param name="param">Optional parameters for the SQL query; may be null.</param>
/// <returns>The mapped result of type T.</returns>
T QuerySingle<T>(string sql, object param = null);

        /// <summary>
/// Executes the given SQL query and returns all result rows mapped to <typeparamref name="T"/>.
/// </summary>
/// <param name="sql">The SQL query to execute.</param>
/// <param name="param">An optional object supplying parameters for the SQL query; may be null.</param>
/// <returns>An <see cref="IEnumerable{T}"/> of mapped results; empty if the query returns no rows.</returns>
IEnumerable<T> Query<T>(string sql, object param = null);
    }
}
