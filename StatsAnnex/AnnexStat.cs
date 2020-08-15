using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsAnnex
{
    public class AnnexStat : IComparable
    {
        private static Random random;
        protected int tiebreaker;

        AnnexStat()
        {
            tiebreaker = random.Next(int.MinValue, int.MaxValue);
        }

        public Stat[] Dependencies;
        public bool RequiresInventory;
        public Stat Affects;

        public int CompareTo(object obj)
        {
            if(obj is AnnexStat other)
            {
                bool thisDependsOnThat = Dependencies.Contains(other.Affects);
                bool thatDependsOnthis = other.Dependencies.Contains(Affects);
                if (thisDependsOnThat && thatDependsOnthis)
                    return tiebreaker.CompareTo(other.tiebreaker);
                if (thisDependsOnThat)
                    return 1;
                return -1;
            }
            return 0;
        }
    }
}
