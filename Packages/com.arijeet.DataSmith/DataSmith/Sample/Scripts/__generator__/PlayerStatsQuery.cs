/* Auto-generated. DO NOT MODIFY */

namespace Baruah.DataSmith.Sample
{
    public sealed class PlayerStatsQuery 
        : ModelQuery<Baruah.DataSmith.Sample.PlayerStats>
    {
        /// <summary>
            /// Initializes a new PlayerStatsQuery using the provided source collection.
            /// </summary>
            /// <param name="source">The collection of PlayerStats to query against.</param>
            public PlayerStatsQuery(System.Collections.Generic.IReadOnlyList<Baruah.DataSmith.Sample.PlayerStats> source)
            : base(source) { }

        /// <summary>
        /// Adds a filter matching records whose playerId equals the specified value.
        /// </summary>
        /// <param name="value">The playerId to match.</param>
        /// <returns>The current <see cref="PlayerStatsQuery"/> instance for chaining.</returns>
        public PlayerStatsQuery PlayerIdEquals(System.String value)
        {
            AddCondition(i => i.playerId == value);
            return this;
        }

        /// <summary>
        /// Adds a condition requiring the PlayerStats.playerId to contain the specified substring.
        /// </summary>
        /// <param name="value">The substring to match within `playerId`.</param>
        /// <returns>The query instance with the added condition for chaining.</returns>
        public PlayerStatsQuery PlayerIdContains(string value)
        {
            AddCondition(i => i.playerId != null && i.playerId.Contains(value));
            return this;
        }

        /// <summary>
        /// Adds a filter that matches records whose Health is equal to the specified value.
        /// </summary>
        /// <param name="value">The health value to match.</param>
        /// <returns>The current query instance with the added condition, allowing method chaining.</returns>
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
