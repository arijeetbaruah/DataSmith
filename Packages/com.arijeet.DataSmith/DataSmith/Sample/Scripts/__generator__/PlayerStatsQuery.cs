/* Auto-generated. DO NOT MODIFY */

namespace Baruah.DataSmith.Sample
{
    public sealed class PlayerStatsQuery 
        : ModelQuery<Baruah.DataSmith.Sample.PlayerStats>
    {
        public PlayerStatsQuery(System.Collections.Generic.IReadOnlyList<Baruah.DataSmith.Sample.PlayerStats> source)
            : base(source) { }

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
