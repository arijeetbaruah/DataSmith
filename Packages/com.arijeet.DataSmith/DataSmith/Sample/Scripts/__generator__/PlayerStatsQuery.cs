/* Auto-generated. DO NOT MODIFY */

namespace Baruah.DataSmith.Sample
{
    public sealed class PlayerStatsQuery 
        : ModelQuery<Baruah.DataSmith.Sample.PlayerStats>
    {
        /// <summary>
            /// Initializes a new <see cref="PlayerStatsQuery"/> that will build queries over the supplied collection of <see cref="Baruah.DataSmith.Sample.PlayerStats"/>.
            /// </summary>
            /// <param name="source">The read-only collection of player stats to query against.</param>
            public PlayerStatsQuery(System.Collections.Generic.IReadOnlyList<Baruah.DataSmith.Sample.PlayerStats> source)
            : base(source) { }

        /// <summary>
        /// Adds a condition requiring the PlayerStats' playerId to equal the specified value.
        /// </summary>
        /// <param name="value">The player identifier to match.</param>
        /// <returns>The same <see cref="PlayerStatsQuery"/> instance to allow fluent chaining.</returns>
        public PlayerStatsQuery PlayerIdEquals(System.String value)
        {
            AddCondition(i => i.playerId == value);
            return this;
        }

        /// <summary>
        /// Adds a condition that the player's ID contains the specified substring.
        /// </summary>
        /// <param name="value">Substring to search for within player IDs.</param>
        /// <returns>The same <see cref="PlayerStatsQuery"/> instance for chaining.</returns>
        public PlayerStatsQuery PlayerIdContains(string value)
        {
            AddCondition(i => i.playerId != null && i.playerId.Contains(value));
            return this;
        }

        /// <summary>
        /// Filters results to player stats whose Health equals the specified value.
        /// </summary>
        /// <param name="value">Health value to match.</param>
        /// <returns>The same <see cref="PlayerStatsQuery"/> instance for further chaining.</returns>
        public PlayerStatsQuery HealthEquals(System.Int32 value)
        {
            AddCondition(i => i.Health == value);
            return this;
        }

        public PlayerStatsQuery HealthGreaterThan(System.Int32 value)
        {
            AddCondition(i => i.Health > value);
            return this;
        }

        public PlayerStatsQuery HealthLessThan(System.Int32 value)
        {
            AddCondition(i => i.Health < value);
            return this;
        }

        public PlayerStatsQuery HealthGreaterThanEqualTo(System.Int32 value)
        {
            AddCondition(i => i.Health >= value);
            return this;
        }

        public PlayerStatsQuery HealthLessThanEqualTo(System.Int32 value)
        {
            AddCondition(i => i.Health <= value);
            return this;
        }

        public PlayerStatsQuery ManaEquals(System.Int32 value)
        {
            AddCondition(i => i.Mana == value);
            return this;
        }

        public PlayerStatsQuery ManaGreaterThan(System.Int32 value)
        {
            AddCondition(i => i.Mana > value);
            return this;
        }

        public PlayerStatsQuery ManaLessThan(System.Int32 value)
        {
            AddCondition(i => i.Mana < value);
            return this;
        }

        public PlayerStatsQuery ManaGreaterThanEqualTo(System.Int32 value)
        {
            AddCondition(i => i.Mana >= value);
            return this;
        }

        public PlayerStatsQuery ManaLessThanEqualTo(System.Int32 value)
        {
            AddCondition(i => i.Mana <= value);
            return this;
        }

        public PlayerStatsQuery SpeedEquals(System.Single value)
        {
            AddCondition(i => i.Speed == value);
            return this;
        }

        public PlayerStatsQuery SpeedGreaterThan(System.Single value)
        {
            AddCondition(i => i.Speed > value);
            return this;
        }

        public PlayerStatsQuery SpeedLessThan(System.Single value)
        {
            AddCondition(i => i.Speed < value);
            return this;
        }

        public PlayerStatsQuery SpeedGreaterThanEqualTo(System.Single value)
        {
            AddCondition(i => i.Speed >= value);
            return this;
        }

        public PlayerStatsQuery SpeedLessThanEqualTo(System.Single value)
        {
            AddCondition(i => i.Speed <= value);
            return this;
        }

        public PlayerStatsQuery ItemIdEquals(System.String value)
        {
            AddCondition(i => i.ItemId == value);
            return this;
        }

        public PlayerStatsQuery ItemIdContains(string value)
        {
            AddCondition(i => i.ItemId != null && i.ItemId.Contains(value));
            return this;
        }

        public PlayerStatsQuery ItemEquals(Baruah.DataSmith.Sample.InventoryItem value)
        {
            if (value == null)
                return this;

            AddCondition(i => i.ItemId == value.Id);
            return this;
        }

        public PlayerStatsQuery Where(System.Func<PlayerStats, bool> predicate)
        {
            AddCondition(predicate);
            return this;
        }

    }
}
