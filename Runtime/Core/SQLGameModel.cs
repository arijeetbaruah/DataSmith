using System.Collections.Generic;

namespace Baruah.DataSmith
{
    public abstract class SQLGameModel<T> : GameModel
    {
        /// <summary>Creates the database table if it does not exist.</summary>
        public abstract void CreateTable();

        /// <summary>
/// Inserts the provided entity as a new record in the underlying data store.
/// </summary>
/// <param name="item">The entity to insert.</param>
        public abstract void Insert(T item);

        /// <summary>
/// Updates the database record that corresponds to the provided item.
/// </summary>
/// <param name="item">The entity containing the updated values; its identifier is used to locate the existing record.</param>
        public abstract void Update(T item);

        /// <summary>
/// Retrieves all records of type <typeparamref name="T"/> from the model's data store.
/// </summary>
/// <returns>An <see cref="IEnumerable{T}"/> containing every record of type <typeparamref name="T"/> in the model's data store.</returns>
        public abstract IEnumerable<T> GetAll();
        
        /// <summary>
        /// Produces a serialized representation of this SQL-backed game model.
        /// </summary>
        /// <returns>An empty string.</returns>
        public override string Serialize()
        {
            return "";
        }

        /// <summary>
        /// Ignores the provided serialized data and performs no action.
        /// </summary>
        /// <param name="data">Serialized data to deserialize (ignored).</param>
        public override void Deserialize(string data)
        {
        }
    }
}