/* Auto-generated. DO NOT MODIFY */

namespace Baruah.DataSmith.Sample
{
    public sealed class PlayerStatsQuery 
        : ModelQuery<Baruah.DataSmith.Sample.PlayerStats>
    {
        /// <summary>
            /// Initializes a new instance that builds query conditions over the provided collection of PlayerStats.
            /// </summary>
            /// <param name="source">The read-only collection of PlayerStats to query against.</param>
            public PlayerStatsQuery(System.Collections.Generic.IReadOnlyList<Baruah.DataSmith.Sample.PlayerStats> source)
            : base(source) { }

        /// <summary>
        /// Adds a filter requiring a player's Health to equal the specified value.
        /// </summary>
        /// <param name="value">The Health value to match.</param>
        /// <returns>This query instance for fluent chaining.</returns>
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

        /// <summary>
        /// Adds a filter that requires a player's Speed to be less than or equal to the specified value.
        /// </summary>
        /// <param name="value">The maximum Speed value (inclusive) to match.</param>
        /// <returns>The current PlayerStatsQuery instance for fluent chaining.</returns>
        public PlayerStatsQuery SpeedLessThanEqualTo(System.Single value)
        {
            AddCondition(i => i.Speed <= value);
            return this;
        }

        /// <summary>
        /// Adds a filter that selects PlayerStats whose `item` field is equal to the specified inventory item.
        /// </summary>
        /// <param name="value">The inventory item to match against the PlayerStats' `item` field.</param>
        /// <returns>The current PlayerStatsQuery instance for fluent chaining.</returns>
        public PlayerStatsQuery ItemEquals(Baruah.DataSmith.Sample.InventoryItem value)
        {
            AddCondition(i => i.item == value);
            return this;
        }

        public PlayerStatsQuery Where(System.Func<PlayerStats, bool> predicate)
        {
            AddCondition(predicate);
            return this;
        }

    }
}
