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
        /// Initializes a new instance of PlayerStatsModel and sets its underlying Value to a new Baruah.DataSmith.Sample.PlayerStats.
        /// </summary>
        public PlayerStatsModel()
        {
            Value = new Baruah.DataSmith.Sample.PlayerStats();
        }


        /// <summary>
        /// Getter for playerId
        /// <summary>
/// Gets the player identifier from the underlying PlayerStats value.
/// </summary>
/// <returns>The player's identifier, or null if not set.</returns>
        public System.String GetPlayerId() => Value.playerId;

        /// <summary>
        /// Setter for playerId
        /// <summary>
        /// Sets the model's playerId and notifies subscribers if the value changed.
        /// </summary>
        /// <param name="value">The new player identifier to assign to the model.</param>
        public void SetPlayerId(System.String value)
        {
            if (Equals(Value.playerId, value)) return;
            Value.playerId = value;
            OnPlayerIdChanged?.Invoke(value);
        }

        /// <summery>
        /// Event which trigger when value is changed
        /// </summery>
        public event Action<System.String> OnPlayerIdChanged;



        /// <summary>
        /// Getter for Health
        /// </summary>
        public System.Int32 GetHealth() => Value.Health;

        /// <summary>
        /// Setter for Health
        /// </summary>
        public void SetHealth(System.Int32 value)
        {
            if (Equals(Value.Health, value)) return;
            Value.Health = value;
            OnHealthChanged?.Invoke(value);
        }

        /// <summery>
        /// Event which trigger when value is changed
        /// </summery>
        public event Action<System.Int32> OnHealthChanged;



        /// <summary>
        /// Getter for Mana
        /// </summary>
        public System.Int32 GetMana() => Value.Mana;

        /// <summary>
        /// Setter for Mana
        /// </summary>
        public void SetMana(System.Int32 value)
        {
            if (Equals(Value.Mana, value)) return;
            Value.Mana = value;
            OnManaChanged?.Invoke(value);
        }

        /// <summery>
        /// Event which trigger when value is changed
        /// </summery>
        public event Action<System.Int32> OnManaChanged;



        /// <summary>
        /// Getter for Speed
        /// </summary>
        public System.Single GetSpeed() => Value.Speed;

        /// <summary>
        /// Setter for Speed
        /// </summary>
        public void SetSpeed(System.Single value)
        {
            if (Equals(Value.Speed, value)) return;
            Value.Speed = value;
            OnSpeedChanged?.Invoke(value);
        }

        /// <summery>
        /// Event which trigger when value is changed
        /// </summery>
        public event Action<System.Single> OnSpeedChanged;



        /// <summary>
        /// Getter for ItemId
        /// </summary>
        public System.String GetItemId() => Value.ItemId;

        /// <summary>
        /// Setter for ItemId
        /// </summary>
        public void SetItemId(System.String value)
        {
            if (Equals(Value.ItemId, value)) return;
            Value.ItemId = value;
            OnItemIdChanged?.Invoke(value);
        }

        /// <summery>
        /// Event which trigger when value is changed
        /// </summery>
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
