using System;
using System.Collections.Generic;
using Baruah.ModelSystem;

namespace Baruah.ModelSystem.Sample
{
    public sealed partial class InventoryItemModel : ListGameModel<Baruah.ModelSystem.Sample.InventoryItem>
    {        
        public InventoryItemQuery Query()
        {
            return new InventoryItemQuery(_items);
        }

        public IEnumerable<Baruah.ModelSystem.Sample.InventoryItem> FindById(System.String value)
        {
            foreach (var item in _items)
                if (Equals(item.Id, value))
                    yield return item;
        }

        public IEnumerable<Baruah.ModelSystem.Sample.InventoryItem> FindByQuantity(System.Int32 value)
        {
            foreach (var item in _items)
                if (Equals(item.Quantity, value))
                    yield return item;
        }

        public IEnumerable<Baruah.ModelSystem.Sample.InventoryItem> FindByIsEquipped(System.Boolean value)
        {
            foreach (var item in _items)
                if (Equals(item.IsEquipped, value))
                    yield return item;
        }


    }
}