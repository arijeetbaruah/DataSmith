using System.Linq;
using UnityEngine;

namespace Baruah.DataSmith.Sample
{
    public class QueryTest : MonoBehaviour
    {
        //[SerializeField] private InventoryItem[] _sampleInventoryItems;
        [SerializeField] private AssetContextsSO _assetContexts;

        private void Start()
        {
            //DataContext.Get<InventoryItemModel>().AddRange(_sampleInventoryItems);

            DataContext.AssetContexts = _assetContexts;
            
            int count = DataContext.Get<InventoryItemModel>()
                .Query()
                .IsEquippedEquals(false)
                .QuantityGreaterThanEqualTo(3)
                .Count();

            Debug.Log(count);
        }
    }
}
