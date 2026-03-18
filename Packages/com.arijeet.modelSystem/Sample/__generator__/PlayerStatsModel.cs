using System;
using Baruah.ModelSystem;

namespace Baruah.ModelSystem.Sample
{
    public sealed partial class PlayerStatsModel : SingleGameModel<Baruah.ModelSystem.Sample.PlayerStats>
    {
        public PlayerStatsModel()
        {
            Value = new Baruah.ModelSystem.Sample.PlayerStats();
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

        public Baruah.ModelSystem.Sample.InventoryItem GetItem() => Value.item;

        public void SetItem(Baruah.ModelSystem.Sample.InventoryItem value)
        {
            if (Equals(Value.item, value)) return;
            Value.item = value;
            OnItemChanged?.Invoke(value);
        }

        public event Action<Baruah.ModelSystem.Sample.InventoryItem> OnItemChanged;


    }
}
