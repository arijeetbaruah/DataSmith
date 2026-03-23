using System.Collections.Generic;

namespace Baruah.DataSmith
{
    public abstract class SQLGameModel<T> : GameModel
    {
        /// <summary>Creates the database table if it does not exist.</summary>
        public abstract void CreateTable();

        /// <summary>
/// Inserts the provided item into the database as a new record.
/// </summary>
/// <param name="item">The item to insert into the model's underlying table.</param>
        public abstract void Insert(T item);

        /// <summary>
/// Updates the database record corresponding to the provided item.
/// </summary>
/// <param name="item">The item containing updated values; its identifier is used to locate the existing record.</param>
        public abstract void Update(T item);

        /// <summary>
/// Deletes the record identified by the given primary key from the underlying storage.
/// </summary>
/// <param name="id">Primary key of the record to delete.</param>
        public abstract void Delete(System.String id);

        /// <summary>
/// Retrieves a record by primary key.
/// </summary>
/// <param name="id">The primary key of the record to retrieve.</param>
/// <returns>The record with the matching primary key, or <c>null</c> if no matching record exists.</returns>
        public abstract T GetById(System.String id);

        /// <summary>
/// Retrieves all items of type T from the model's data store.
/// </summary>
/// <returns>An <see cref="IEnumerable{T}"/> containing every record for this model; empty if there are no records.</returns>
        public abstract IEnumerable<T> GetAll();
        
        /// <summary>
        /// Produce a string representation of the model suitable for storage or transmission; this override returns an empty string.
        /// </summary>
        /// <returns>An empty string.</returns>
        public override string Serialize()
        {
            return "";
        }

        /// <summary>
        /// Restores the model's state from a serialized representation.
        /// </summary>
        /// <param name="data">The serialized representation of the model's state.</param>
        public override void Deserialize(string data)
        {
        }
    }
}