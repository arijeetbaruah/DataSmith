/*
 * This is a Auto-Generated code. DO NOT MODIFY
 */
 
using System;
using Baruah.DataSmith;

namespace Baruah.DataSmith.Sample
{
    public sealed partial class PlayerStatsModel : SingleGameModel<PlayerStats>
    {
        public PlayerStatsModel()
        {
            Value = new PlayerStats();
        }

        public System.Int32 GetHealth() => Value.Health;

        public void SetHealth(System.Int32 value)
        {
            if (Equals(Value.Health, value)) return;
            Value.Health = value;
            OnHealthChanged?.Invoke(value);
        }

        public event Action<System.Int32> OnHealthChanged;

        public System.Int32 GetMana() => Value.Mana;

        public void SetMana(System.Int32 value)
        {
            if (Equals(Value.Mana, value)) return;
            Value.Mana = value;
            OnManaChanged?.Invoke(value);
        }

        public event Action<System.Int32> OnManaChanged;

        public System.Single GetSpeed() => Value.Speed;

        public void SetSpeed(System.Single value)
        {
            if (Equals(Value.Speed, value)) return;
            Value.Speed = value;
            OnSpeedChanged?.Invoke(value);
        }

        public event Action<System.Single> OnSpeedChanged;

        public InventoryItem GetItem() => Value.item;

        public void SetItem(InventoryItem value)
        {
            if (Equals(Value.item, value)) return;
            Value.item = value;
            OnItemChanged?.Invoke(value);
        }

        public event Action<InventoryItem> OnItemChanged;


    }
}
