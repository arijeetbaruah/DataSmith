using UnityEngine;

namespace Baruah.DataSmith.Database
{
    [System.Serializable]
    public abstract class DatabaseProviderAsset
    {
        /// <summary>
        /// Creates the runtime database instance.
        /// </summary>
        public abstract IDatabase CreateDatabase();
    }
}
