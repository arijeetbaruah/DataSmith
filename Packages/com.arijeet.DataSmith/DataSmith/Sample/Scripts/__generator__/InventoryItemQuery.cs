/* Auto-generated. DO NOT MODIFY */

namespace Baruah.DataSmith.Sample
{
    public sealed class InventoryItemQuery 
        : ModelQuery<Baruah.DataSmith.Sample.InventoryItem>
    {
        /// <summary>
            /// Initializes a new InventoryItemQuery using the specified source collection of InventoryItem records.
            /// </summary>
            /// <param name="source">The read-only collection of InventoryItem objects to query against.</param>
            public InventoryItemQuery(System.Collections.Generic.IReadOnlyList<Baruah.DataSmith.Sample.InventoryItem> source)
            : base(source) { }

        /// <summary>
        /// Adds a predicate that matches inventory items whose Id equals the specified value.
        /// </summary>
        /// <param name="value">The Id value to match.</param>
        /// <returns>The current query instance for fluent chaining.</returns>
        public InventoryItemQuery IdEquals(System.String value)
        {
            AddCondition(i => i.Id == value);
            return this;
        }

        public InventoryItemQuery IdContains(string value)
        {
            AddCondition(i => i.Id != null && i.Id.Contains(value));
            return this;
        }

        public InventoryItemQuery QuantityEquals(System.Int32 value)
        {
            AddCondition(i => i.Quantity == value);
            return this;
        }

        public InventoryItemQuery QuantityGreaterThan(System.Int32 value)
        {
            AddCondition(i => i.Quantity > value);
            return this;
        }

        public InventoryItemQuery QuantityLessThan(System.Int32 value)
        {
            AddCondition(i => i.Quantity < value);
            return this;
        }

        public InventoryItemQuery QuantityGreaterThanEqualTo(System.Int32 value)
        {
            AddCondition(i => i.Quantity >= value);
            return this;
        }

        public InventoryItemQuery QuantityLessThanEqualTo(System.Int32 value)
        {
            AddCondition(i => i.Quantity <= value);
            return this;
        }

        public InventoryItemQuery IsEquippedEquals(System.Boolean value)
        {
            AddCondition(i => i.IsEquipped == value);
            return this;
        }

        public InventoryItemQuery Where(System.Func<InventoryItem, bool> predicate)
        {
            AddCondition(predicate);
            return this;
        }

    }
}
