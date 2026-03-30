using System.Collections.Generic;
using System.Text;

namespace Baruah.DataSmith.Database
{
    public abstract class QueryBuilder<T>
    {
        private readonly string _tableName;
        
        protected readonly List<string> _where = new();
        protected readonly List<string> _orderBy = new();
        protected readonly List<string> _conditions = new();
        
        private int? _limit;
        
        public QueryBuilder()
        {
            _tableName = typeof(T).Name;
        }
        
        /// <summary>
        /// Where Condition
        /// </summary>
        /// <param name="condition">condition</param>
        /// <returns>QueryBuilder</returns>
        public QueryBuilder<T> Where(string condition)
        {
            if (!string.IsNullOrEmpty(condition))
                _where.Add(condition);

            return this;
        }
        
        /// <summary>
        /// Set the order of response
        /// </summary>
        /// <param name="clause"></param>
        /// <returns>QueryBuilder</returns>
        public QueryBuilder<T> OrderBy(string clause)
        {
            if (!string.IsNullOrEmpty(clause))
                _orderBy.Add(clause);

            return this;
        }
        
        /// <summary>
        /// set limit
        /// </summary>
        /// <param name="count">number of element count</param>
        /// <returns>QueryBuilder</returns>
        public QueryBuilder<T> Limit(int count)
        {
            _limit = count;
            return this;
        }
        
        /// <summary>
        /// Build select
        /// </summary>
        /// <param name="columns">columns</param>
        /// <returns>sql query</returns>
        public string BuildSelect(string columns = "*")
        {
            var sb = new StringBuilder();

            sb.Append($"SELECT {columns} FROM {_tableName}");

            AppendWhere(sb);
            AppendOrderBy(sb);
            AppendLimit(sb);

            return sb.ToString();
        }
        
        /// <summary>
        /// Build Aggregate
        /// </summary>
        /// <param name="function">function</param>
        /// <returns>sql query</returns>
        public string BuildAggregate(string function)
        {
            var sb = new StringBuilder();

            sb.Append($"SELECT {function} FROM {_tableName}");

            AppendWhere(sb);

            return sb.ToString();
        }
        
        /// <summary>
        /// Append Where condition to sql query
        /// </summary>
        /// <param name="sb"></param>
        private void AppendWhere(StringBuilder sb)
        {
            if (_where.Count == 0)
                return;

            sb.Append(" WHERE ");
            sb.Append(string.Join(" AND ", _where));
        }

        /// <summary>
        /// Append Order by to sql query
        /// </summary>
        /// <param name="sb"></param>
        private void AppendOrderBy(StringBuilder sb)
        {
            if (_orderBy.Count == 0)
                return;

            sb.Append(" ORDER BY ");
            sb.Append(string.Join(", ", _orderBy));
        }

        /// <summary>
        /// Append Limits
        /// </summary>
        /// <param name="sb"></param>
        private void AppendLimit(StringBuilder sb)
        {
            if (!_limit.HasValue)
                return;

            sb.Append($" LIMIT {_limit.Value}");
        }

        protected abstract TResult ExecuteScalar<TResult>(string sqlFunction);

        protected string BuildWhereClause()
        {
            if (_conditions.Count == 0)
                return string.Empty;

            return string.Join(" AND ", _conditions);
        }
        
        // Aggregate Functions

        public int Count(string columns = "*") => ExecuteScalar<int>(BuildAggregate($"COUNT({columns})"));
        
        public int Sum(string columns = "*") => ExecuteScalar<int>(BuildAggregate($"SUM({columns})"));

        public float Average(string columns = "*") => ExecuteScalar<float>(BuildAggregate($"AVG({columns})"));

        public int Max(string columns = "*") => ExecuteScalar<int>($"MAX({columns})");
        
        public int Min(string columns = "*") => ExecuteScalar<int>($"MIN({columns})");
        
        public T FirstOrDefault() => ExecuteScalar<T>(Limit(1).BuildSelect());
    }
}
