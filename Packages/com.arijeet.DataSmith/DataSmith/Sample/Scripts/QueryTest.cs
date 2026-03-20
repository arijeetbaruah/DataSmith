using System.Linq;
using Baruah.DataSmith.Sample;
using UnityEngine;

public class QueryTest : MonoBehaviour
{
    [SerializeField] private InventoryItem[] _sampleInventoryItems;
    
    private void Start()
    {
        DataContext.Initialize();
        
        DataContext.InventoryItemModel.AddRange(_sampleInventoryItems);
        
        int count = DataContext.InventoryItemModel
            .Query()
            .IsEquippedEquals(false)
            .QuantityGreaterThanEqualTo(3)
            .Count();
        
        Debug.Log(count);
    }
}
