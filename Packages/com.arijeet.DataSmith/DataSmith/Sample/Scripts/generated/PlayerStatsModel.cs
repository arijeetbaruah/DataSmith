/*
 * This is a Auto-Generated code. DO NOT MODIFY
 */
 
using System;
using Baruah.DataSmith;

namespace Baruah.DataSmith.Sample
{
    /// <summary>
    /// DataSmith model wrapper for <see cref="Baruah.DataSmith.Sample.PlayerStats"/>.
    /// </summary>
    /// <remarks>
    /// This class is auto-generated. Do not modify manually.
    /// Use the DataSmith generator to regenerate.
    /// </remarks>
    public sealed partial class PlayerStatsModel : SingleGameModel<Baruah.DataSmith.Sample.PlayerStats>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PlayerStatsModel"/> and creates its wrapped <see cref="Baruah.DataSmith.Sample.PlayerStats"/> value.
        /// </summary>
        public PlayerStatsModel()
        {
            Value = new Baruah.DataSmith.Sample.PlayerStats();
        }


        /// <summary>
        /// Getter for playerId
        /// <summary>
/// Gets the current player identifier from the wrapped model.
/// </summary>
/// <returns>The current player identifier, or null if not set.</returns>
        public System.String GetPlayerId() => Value.playerId;

        /// <summary>
        /// Setter for playerId
        /// <summary>
        /// Sets the player's identifier and notifies subscribers if the identifier changed.
        /// </summary>
        /// <param name="value">The new player identifier. If equal to the current identifier, no change is made and no event is raised.</param>
        public void SetPlayerId(System.String value)
        {
            if (Equals(Value.playerId, value)) return;
            Value.playerId = value;
            OnPlayerIdChanged?.Invoke(value);
        }

        /// <summary>
        /// Event which trigger when value is changed
        /// </summary>
        public event Action<System.String> OnPlayerIdChanged;



        /// <summary>
        /// Getter for Health
        /// </summary>
        public System.Int32 GetHealth() => Value.Health;

        /// <summary>
        /// Setter for Health
        /// <summary>
        /// Sets the Health field to the specified value and invokes the OnHealthChanged event if the value changes.
        /// </summary>
        /// <param name="value">The new health value to assign.</param>
        public void SetHealth(System.Int32 value)
        {
            if (Equals(Value.Health, value)) return;
            Value.Health = value;
            OnHealthChanged?.Invoke(value);
        }

        /// <summary>
        /// Event which trigger when value is changed
        /// </summary>
        public event Action<System.Int32> OnHealthChanged;



        /// <summary>
        /// Getter for Mana
        /// </summary>
        public System.Int32 GetMana() => Value.Mana;

        /// <summary>
        /// Setter for Mana
        /// <summary>
        /// Updates the Mana value and raises the OnManaChanged event when the stored value changes.
        /// </summary>
        /// <param name="value">The new Mana value; if equal to the current value, no update or event occurs.</param>
        public void SetMana(System.Int32 value)
        {
            if (Equals(Value.Mana, value)) return;
            Value.Mana = value;
            OnManaChanged?.Invoke(value);
        }

        /// <summary>
        /// Event which trigger when value is changed
        /// </summary>
        public event Action<System.Int32> OnManaChanged;



        /// <summary>
        /// Getter for Speed
        /// </summary>
        public System.Single GetSpeed() => Value.Speed;

        /// <summary>
        /// Setter for Speed
        /// <summary>
        /// Sets the player's movement speed and invokes <see cref="OnSpeedChanged"/> when the value changes.
        /// </summary>
        /// <param name="value">The new movement speed; if equal to the current speed no update or event occurs.</param>
        public void SetSpeed(System.Single value)
        {
            if (Equals(Value.Speed, value)) return;
            Value.Speed = value;
            OnSpeedChanged?.Invoke(value);
        }

        /// <summary>
        /// Event which trigger when value is changed
        /// </summary>
        public event Action<System.Single> OnSpeedChanged;



        /// <summary>
        /// Getter for ItemId
        /// </summary>
        public System.String GetItemId() => Value.ItemId;

        /// <summary>
        /// Setter for ItemId
        /// <summary>
        /// Sets the wrapped ItemId to the specified value and invokes OnItemIdChanged if the value changed.
        /// </summary>
        /// <param name="value">The new inventory item identifier to assign to the model; may be null.</param>
        public void SetItemId(System.String value)
        {
            if (Equals(Value.ItemId, value)) return;
            Value.ItemId = value;
            OnItemIdChanged?.Invoke(value);
        }

        /// <summary>
        /// Event which trigger when value is changed
        /// </summary>
        public event Action<System.String> OnItemIdChanged;



        /// <summary>
        /// Getter for ItemId to InventoryItemModel
        /// </summary>
        public Baruah.DataSmith.Sample.InventoryItem GetInventoryItem()
        {
            if (Value.ItemId == null)
                return default;

            return DataContext.Get<InventoryItemModel>()
                .Query()
                .IdEquals(Value.ItemId)
                .FirstOrDefault();
        }


    }
}
