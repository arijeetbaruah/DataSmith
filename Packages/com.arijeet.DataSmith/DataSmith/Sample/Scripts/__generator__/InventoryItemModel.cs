/*
 * This is a Auto-Generated code. DO NOT MODIFY
 */

using System;
using System.Collections.Generic;
using Baruah.DataSmith;

namespace Baruah.DataSmith.Sample
{
    public sealed partial class InventoryItemModel : ListGameModel<Baruah.DataSmith.Sample.InventoryItem>
    {        
        public InventoryItemQuery Query()
        {
            return new InventoryItemQuery(_items);
        }

        public IEnumerable<Baruah.DataSmith.Sample.InventoryItem> FindById(System.String value)
        {
            foreach (var item in _items)
                if (Equals(item.Id, value))
                    yield return item;
        }

        public IEnumerable<Baruah.DataSmith.Sample.InventoryItem> FindByQuantity(System.Int32 value)
        {
            foreach (var item in _items)
                if (Equals(item.Quantity, value))
                    yield return item;
        }

        public IEnumerable<Baruah.DataSmith.Sample.InventoryItem> FindByIsEquipped(System.Boolean value)
        {
            foreach (var item in _items)
                if (Equals(item.IsEquipped, value))
                    yield return item;
        }


    }
}