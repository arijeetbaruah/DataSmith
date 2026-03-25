using System.Collections.Generic;
using UnityEngine;

namespace Baruah.DataSmith
{
    public abstract class ListAssetModel<T> : BaseAssetModel
    {
        public IReadOnlyList<T> Items => _items;
        
        [SerializeField] protected List<T> _items;
    }
}
