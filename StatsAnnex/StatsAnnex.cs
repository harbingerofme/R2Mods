using System;
using System.Collections.Generic;
using System.Text;

namespace StatsAnnex
{
    public static class StatsAnnex
    {
        internal static StatsAnnexPlugin plugin;

        internal static List<AnnexStat> orderedStatDefs;

        public static void Add(AnnexStat statDef)
        {

        }

    }

    public enum Stat
    {
        Health,
        HealthMultiplier,
        HealthFinalMultiplier,
        Shield,
        ShieldMultiplier,
        ShieldFinalMultiplier,
        Regen,
        RegenMultiplier,
        RegenFinalMultiplier,
        Movespeed,
        MovespeedAddition,
        MovespeedDecrease,
        MovespeedFinalMultiplier,
        Damage,
        DamageMultiplier,
        DamageFinalMultiplier,
        Attackspeed,
        AttackspeedMultiplier,
        AttackspeedFinalMultiplier,
        Crit,
        Armor,
        ArmorMultiplier,
        GlobalCooldownScale,
        PrimaryCooldownScale,
        SecondaryCooldownScale,
        UtilityCooldownScale,
        SpecialCooldownScale,
        CritHeal,
        CursePenalty,
        Barrier,
        BarrierMultiplier,
        BarrierDecayRate,
        BarrierDecayRateMulitplier,
        HasOSP,
        OSPPercent,
        IsGlass
    }

}