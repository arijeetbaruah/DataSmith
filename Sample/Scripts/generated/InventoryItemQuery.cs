/* Auto-generated DB access code. DO NOT MODIFY */

using System.Collections.Generic;
using Baruah.DataSmith;
using Baruah.DataSmith.Database;

namespace Baruah.DataSmith.Sample
{
    /// <summary>
    /// Model files access sql commands for <see cref="InventoryItem"/>
    /// </summary>
    public class InventoryItemQuery : QueryBuilder<Baruah.DataSmith.Sample.InventoryItem>
    {
        protected override TResult ExecuteScalar<TResult>(string sqlFunction)
        {
            string table = typeof(Baruah.DataSmith.Sample.InventoryItem).Name;

            string where = BuildWhereClause();

            string sql = string.IsNullOrEmpty(where)
                ? $"SELECT {sqlFunction} FROM {table}"
                : $"SELECT {sqlFunction} FROM {table} WHERE {where}";

            return DataContext.Database.QuerySingle<TResult>(sql);
        }
        
        /// <summary>
        /// Where Condition for Baruah.DataSmith.Sample.InventoryItem
        /// </summary>
        /// <param name="condition">condition</param>
        /// <returns>InventoryItemQuery</returns>
        public InventoryItemQuery Where(string condition) => (InventoryItemQuery) base.Where(condition);
        
                public InventoryItemQuery IdEquals(System.String value)
        {
            return Where($"Id == {value} ");
        }

        public InventoryItemQuery QuantityEquals(System.Int32 value)
        {
            return Where($"Quantity == {value} ");
        }

        public InventoryItemQuery QuantityGreaterThan(System.Int32 value)
        {
            return Where($"Quantity > value");
        }

        public InventoryItemQuery QuantityLessThan(System.Int32 value)
        {
            return Where($"Quantity < value");
        }

        public InventoryItemQuery QuantityGreaterThanEqualTo(System.Int32 value)
        {
            return Where($"Quantity >= value");
        }

        public InventoryItemQuery QuantityLessThanEqualTo(System.Int32 value)
        {
            return Where($"Quantity <= value");
        }

        public InventoryItemQuery IsEquippedEquals(System.Boolean value)
        {
            return Where($"IsEquipped == {value} ");
        }


    }
}
