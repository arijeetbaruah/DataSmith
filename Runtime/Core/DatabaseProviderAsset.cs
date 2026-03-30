using UnityEngine;

namespace Baruah.DataSmith.Database
{
    [System.Serializable]
    public abstract class DatabaseProviderAsset
    {
        /// <summary>
        /// Creates the runtime database instance.
        /// <summary>
/// Creates an IDatabase instance from this provider asset for use at runtime.
/// </summary>
/// <returns>An IDatabase implementation produced by the provider.</returns>
        public abstract IDatabase CreateDatabase();
    }
}
