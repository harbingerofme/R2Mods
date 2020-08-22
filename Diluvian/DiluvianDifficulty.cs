using System.Collections.Generic;
using UnityEngine;

namespace Diluvian
{
    class DiluvianDifficulty : HarbDifficultyDef
    {
        internal static DiluvianDifficulty def;
        public DiluvianDifficulty()
        {
            Name = "Diluvian";
            BaseToken = "DIFFICULTY_DILUVIAN";
            Color = new Color(0.61f, 0.07f, 0.93f);
            ScalingValue = 3.5f;
            HealthRegenMod = -0.6f;
            Tag = "DIL";
            IconPath = "diluvian-"+DiluvianPlugin.diluvianArtist.Value+".png";
            StartMessages = new string[]{"A storm is brewing.","You feel as though your thoughts are drenched.", "The world shivers in anticipation."};
            MonsterRegenMod = 0.015f;
            EliteModifier = 0.8f;
            Description = buildDescription();
        }

        public override Dictionary<string, string> Language => DiluvianDictionary.DictionaryToUse;

        public override void ApplyHooks()
        {
            
        }

        public override void UndoHooks()
        {
            
        }

        private string buildDescription()
        {
            string desc = "For those found wanting. <style=cDeath>N'Kuhana</style> watches with interest.<style=cStack>\n";
            desc = string.Join("\n",
                desc,
                $">Difficulty Scaling: +{ScalingValue * 50 - 100}%",
                $">Player Health Regeneration: {(int)(HealthRegenMod * 100)}%",
                ">Player luck: Reduced in some places.",
                $">Monster Health Regeneration: +{MonsterRegenMod * 100}% of MaxHP per second (out of danger)",
                ">Oneshot Protection: Removed completely",
                DiluvianPlugin.ESOenabled ? ">Elites: Handled by Elite Spawning Overhaul." : $">Elites: {(1 - EliteModifier) * 100}% cheaper.",
                ">Shrine of Blood: Cost hidden and random."
                );
            desc += "</style>";
            return desc;
        }
    }
}
