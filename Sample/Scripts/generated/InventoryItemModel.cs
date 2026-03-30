/* Auto-generated DB access code. DO NOT MODIFY */

using System.Collections.Generic;
using Baruah.DataSmith;

namespace Baruah.DataSmith.Sample
{
    /// <summary>
    /// Model files access sql commands for <see cref="InventoryItem"/>
    /// </summary>
    public class InventoryItemModel : SQLGameModel<Baruah.DataSmith.Sample.InventoryItem>
    {
        public const string TableName = "InventoryItem";

        public InventoryItemQuery Query()
        {
            return new InventoryItemQuery();
        }

                /// <summary>Creates the database table if it does not exist.</summary>
        public override void CreateTable()
        {
            DataContext.Database.Execute(@"
CREATE TABLE IF NOT EXISTS InventoryItem (
    Id TEXT PRIMARY KEY ,
    Quantity INTEGER  CHECK (Quantity BETWEEN 0 AND 99),
    IsEquipped INTEGER 
);");
        }

        /// <summary>
        /// Inserts a new record of <see cref="InventoryItem"/> into it. 
        /// </summary>
        /// <param name="item">Item to add</param>
        public override void Insert(InventoryItem item)
        {
            DataContext.Database.Execute(@"
INSERT INTO InventoryItem (@Id, @Quantity, @IsEquipped)
VALUES (@Id, @Quantity, @IsEquipped);", item);
        }

        /// <summary>
        /// Updates an existing <see cref="InventoryItem"/>.
        /// </summary>
        /// <param name="item">Item to add</param>
        public override void Update(InventoryItem item)
        {
            DataContext.Database.Execute(@"
UPDATE InventoryItem
SET Quantity=@Quantity, IsEquipped=@IsEquipped
WHERE Id=@Id;", item);
        }

        /// <summary>Deletes a <see cref="InventoryItem"/>. by primary key.</summary>
        /// <param name="id">item id to be delete</param>
        public void Delete(System.String id)
        {
            DataContext.Database.Execute(
                "DELETE FROM InventoryItem WHERE Id=@Id;",
                new { Id = id });
        }

        /// <summary>
        /// Retrieves a <see cref="InventoryItem"/> by id.
        /// </summary>
        /// <param name="id">id of the <see cref="InventoryItem"/></param>
        /// <returns><see cref="InventoryItem"/> in question</returns>
        public InventoryItem GetById(System.String id)
        {
            return DataContext.Database.QuerySingle<InventoryItem>(
                "SELECT * FROM InventoryItem WHERE Id=@Id;",
                new { Id = id });
        }

        /// <summary>
        /// Retrieves all <see cref="InventoryItem"/>.
        /// </summary>
        /// <returns>all of the <see cref="InventoryItem"/>.</returns>
        public override IEnumerable<InventoryItem> GetAll()
        {
            return DataContext.Database.Query<InventoryItem>(
                "SELECT * FROM InventoryItem;");
        }

    }
}
