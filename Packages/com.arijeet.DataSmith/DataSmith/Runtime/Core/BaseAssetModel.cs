using UnityEngine;

namespace Baruah.DataSmith
{
    /// <summary>
    /// Base class for ScriptableObject-backed models registered with DataContext.
    /// </summary>
    /// <remarks>
    /// Derived types should be created as Unity assets and registered via AssetContextsSO.
    /// </remarks>
    public abstract class BaseAssetModel : ScriptableObject, IGameModel
    {
    }
}