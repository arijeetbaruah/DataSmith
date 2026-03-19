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
        /// <summary>
        /// Creates an InventoryItemQuery over the model's current item collection.
        /// </summary>
        /// <returns>An InventoryItemQuery initialized with the model's current items collection.</returns>
        public InventoryItemQuery Query()
        {
            return new InventoryItemQuery(_items);
        }

        /// <summary>
        /// Retrieves inventory items whose Id equals the specified value.
        /// </summary>
        /// <param name="value">The Id to match.</param>
        /// <returns>An enumerable of inventory items with Id equal to <paramref name="value"/>.</returns>
        public IEnumerable<Baruah.DataSmith.Sample.InventoryItem> FindById(System.String value)
        {
            foreach (var item in _items)
                if (Equals(item.Id, value))
                    yield return item;
        }

        /// <summary>
        /// Retrieves items whose Quantity equals the specified value.
        /// </summary>
        /// <param name="value">Quantity to match.</param>
        /// <returns>An enumeration of <see cref="Baruah.DataSmith.Sample.InventoryItem"/> whose Quantity equals <paramref name="value"/>.</returns>
        public IEnumerable<Baruah.DataSmith.Sample.InventoryItem> FindByQuantity(System.Int32 value)
        {
            foreach (var item in _items)
                if (Equals(item.Quantity, value))
                    yield return item;
        }

        /// <summary>
        /// Enumerates inventory items whose <c>IsEquipped</c> property matches the specified value.
        /// </summary>
        /// <param name="value">The equipment state to match.</param>
        /// <returns>An enumerable of inventory items with <c>IsEquipped</c> equal to <paramref name="value"/>.</returns>
        public IEnumerable<Baruah.DataSmith.Sample.InventoryItem> FindByIsEquipped(System.Boolean value)
        {
            foreach (var item in _items)
                if (Equals(item.IsEquipped, value))
                    yield return item;
        }


    }
}