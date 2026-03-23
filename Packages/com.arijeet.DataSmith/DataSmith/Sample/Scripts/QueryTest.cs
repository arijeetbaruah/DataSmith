using System.Linq;
using UnityEngine;

namespace Baruah.DataSmith.Sample
{
    public class QueryTest : MonoBehaviour
    {
        [SerializeField] private InventoryItem[] _sampleInventoryItems;

        private void Start()
        {
            DataContext.Get<InventoryItemModel>().AddRange(_sampleInventoryItems);

            int count = DataContext.Get<InventoryItemModel>()
                .Query()
                .IsEquippedEquals(false)
                .QuantityGreaterThanEqualTo(3)
                .Count();

            Debug.Log(count);
        }
    }
}
