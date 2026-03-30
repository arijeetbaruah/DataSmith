using UnityEngine;

namespace Baruah.DataSmith
{
    public abstract class SingleAssetModel<T> : BaseAssetModel
    {
        public T Data => _data;
        
        [SerializeField] private T _data;
    }
}
