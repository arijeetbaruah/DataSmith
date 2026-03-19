using System.Linq;
using Baruah.DataSmith.Sample;
using UnityEngine;

public class QueryTest : MonoBehaviour
{
    private InventoryItemModel m_inventoryItem;

    [SerializeField] private InventoryItem[] _sampleInventoryItems;
    
    private void Start()
    {
        m_inventoryItem = new InventoryItemModel();
        
        m_inventoryItem.AddRange(_sampleInventoryItems);

        int count = m_inventoryItem.Query()
            .IsEquippedEquals(false)
            .QuantityGreaterThan(3)
            .Count();
        
        Debug.Log(count);
    }
}
