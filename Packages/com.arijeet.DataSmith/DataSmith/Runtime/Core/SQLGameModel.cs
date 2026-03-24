using System.Collections.Generic;

namespace Baruah.DataSmith
{
    public abstract class SQLGameModel<T> : GameModel
    {
        /// <summary>Creates the database table if it does not exist.</summary>
        public abstract void CreateTable();

        /// <summary>Inserts a new record.</summary>
        public abstract void Insert(T item);

        /// <summary>Updates an existing record.</summary>
        public abstract void Update(T item);

        /// <summary>Retrieves all records.</summary>
        public abstract IEnumerable<T> GetAll();
        
        public override string Serialize()
        {
            return "";
        }

        public override void Deserialize(string data)
        {
        }
    }
}