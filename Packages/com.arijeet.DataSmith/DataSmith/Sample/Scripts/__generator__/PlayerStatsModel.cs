/*
 * This is a Auto-Generated code. DO NOT MODIFY
 */
 
using System;
using Baruah.DataSmith;

namespace Baruah.DataSmith.Sample
{
    public sealed partial class PlayerStatsModel : SingleGameModel<Baruah.DataSmith.Sample.PlayerStats>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStatsModel"/> class and assigns a new <see cref="Baruah.DataSmith.Sample.PlayerStats"/> to <c>Value</c>.
        /// </summary>
        public PlayerStatsModel()
        {
            Value = new Baruah.DataSmith.Sample.PlayerStats();
        }

        /// <summary>
/// Gets the player's current health.
/// </summary>
/// <returns>The player's current health.</returns>
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

        /// <summary>
/// Gets the current inventory item for the player.
/// </summary>
/// <returns>The current <see cref="Baruah.DataSmith.Sample.InventoryItem"/> stored on the model; may be null.</returns>
public Baruah.DataSmith.Sample.InventoryItem GetItem() => Value.item;

        /// <summary>
        /// Sets the player's inventory item and invokes <c>OnItemChanged</c> if the stored item changes.
        /// </summary>
        /// <param name="value">The new inventory item to assign; if equal to the current item, no change occurs and the event is not raised.</param>
        public void SetItem(Baruah.DataSmith.Sample.InventoryItem value)
        {
            if (Equals(Value.item, value)) return;
            Value.item = value;
            OnItemChanged?.Invoke(value);
        }

        public event Action<Baruah.DataSmith.Sample.InventoryItem> OnItemChanged;


    }
}
