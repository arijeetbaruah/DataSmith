using UnityEngine;

namespace Baruah.DataSmith.Database
{
    [System.Serializable]
    public abstract class DatabaseProviderAsset
    {
        /// <summary>
        /// Creates the runtime database instance.
        /// <summary>
/// Creates a runtime database instance configured by this asset.
/// </summary>
/// <returns>An <see cref="IDatabase"/> instance ready for runtime use.</returns>
        public abstract IDatabase CreateDatabase();
    }
}
