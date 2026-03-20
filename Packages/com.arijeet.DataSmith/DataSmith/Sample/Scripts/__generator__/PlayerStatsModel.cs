/*
 * This is a Auto-Generated code. DO NOT MODIFY
 */
 
using System;
using Baruah.DataSmith;

namespace Baruah.DataSmith.Sample
{
    public sealed partial class PlayerStatsModel : SingleGameModel<Baruah.DataSmith.Sample.PlayerStats>
    {
        public PlayerStatsModel()
        {
            Value = new Baruah.DataSmith.Sample.PlayerStats();
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

        public System.String GetItemId() => Value.ItemId;

        public void SetItemId(System.String value)
        {
            if (Equals(Value.ItemId, value)) return;
            Value.ItemId = value;
            OnItemIdChanged?.Invoke(value);
        }

        public event Action<System.String> OnItemIdChanged;

        public Baruah.DataSmith.Sample.InventoryItem GetInventoryItem()
        {
            if (Value.ItemId == null)
                return default;

            return DataContext.InventoryItemModel
                .Query()
                .IdEquals(Value.ItemId)
                .FirstOrDefault();
        }


    }
}
