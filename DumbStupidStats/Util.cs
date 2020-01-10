using System;
using RoR2;
using RoR2.Stats;
namespace DumbStupidStats
{
    public static class Util
    {
        public static void PushToAllPlayers<T>(StatDef statDef, T value)
        {
            foreach (NetworkUser user in NetworkUser.readOnlyInstancesList)
            {
                var body = user.GetCurrentBody();
                if (!body)
                    continue;
                switch (statDef.dataType)
                {
                    case StatDataType.Double:
                    { 
                        Double? val = value as double?;
                        if (val.HasValue)
                            PlayerStatsComponent.FindBodyStatSheet(body).PushStatValue(statDef, val.GetValueOrDefault());
                    }break;
                    case StatDataType.ULong:
                    {
                        ulong? val = value as ulong?;
                        if (val.HasValue)
                            PlayerStatsComponent.FindBodyStatSheet(body).PushStatValue(statDef, val.GetValueOrDefault());
                    }break;
                }
            }
        }
    }
}
