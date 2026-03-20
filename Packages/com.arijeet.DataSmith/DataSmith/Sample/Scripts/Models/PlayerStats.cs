using Baruah.DataSmith;
using UnityEngine;

namespace Baruah.DataSmith.Sample
{
    [GameModel(ModelValueType.Single)]
    [System.Serializable]
    public class PlayerStats
    {
        public int Health;
        public int Mana;
        public float Speed;

        [Reference(typeof(InventoryItem))] public string ItemId;

        public bool IsAlive => Health > 0;
    }
    
    [GameModel(ModelValueType.List)]
    [System.Serializable]
    public class InventoryItem
    {
        [PrimaryKey] public string Id;
        public int Quantity;
        public bool IsEquipped;
    }
}
