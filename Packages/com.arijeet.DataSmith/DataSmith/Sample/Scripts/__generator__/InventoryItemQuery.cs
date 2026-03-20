/* Auto-generated. DO NOT MODIFY */

namespace Baruah.DataSmith.Sample
{
    public sealed class InventoryItemQuery 
        : ModelQuery<Baruah.DataSmith.Sample.InventoryItem>
    {
        public InventoryItemQuery(System.Collections.Generic.IReadOnlyList<Baruah.DataSmith.Sample.InventoryItem> source)
            : base(source) { }

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
