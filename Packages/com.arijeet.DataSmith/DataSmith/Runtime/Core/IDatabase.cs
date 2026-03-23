using System.Collections.Generic;

namespace Baruah.DataSmith.Database
{
    public interface IDatabase
    {
        void Execute(string sql, object param = null);

        T QuerySingle<T>(string sql, object param = null);

        IEnumerable<T> Query<T>(string sql, object param = null);
    }
}
