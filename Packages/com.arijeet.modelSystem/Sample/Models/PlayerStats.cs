using Baruah.ModelSystem;
using UnityEngine;

namespace Baruah.ModelSystem.Sample
{
    [GameModel(ModelValueType.Single)]
    [System.Serializable]
    public class PlayerStats
    {
        public int Health;
        public int Mana;
        public float Speed;
        public InventoryItem item;

        public bool IsAlive => Health > 0;
    }
    
    [GameModel(ModelValueType.List)]
    [System.Serializable]
    public class InventoryItem
    {
        public string Id;
        public int Quantity;
        public bool IsEquipped;
    }
}
