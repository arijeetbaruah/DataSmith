using Baruah.DataSmith;
using UnityEngine;

namespace Baruah.DataSmith.Sample
{
    [GameModel(ModelStorageType.Memory, ModelCollectionType.Single)]
    [System.Serializable]
    public class PlayerStats
    {
        [PrimaryKey] public string playerId;
        public int Health;
        public int Mana;
        public float Speed;

        [Reference(typeof(InventoryItem))] public string ItemId;

        public bool IsAlive => Health > 0;
    }
    
    [GameModel(ModelStorageType.DB)]
    [System.Serializable]
    public class InventoryItem
    {
        [PrimaryKey] public string Id;
        [Range(0, 99)] public int Quantity;
        public bool IsEquipped;
    }
}
