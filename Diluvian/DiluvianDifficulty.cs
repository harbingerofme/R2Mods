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
                $">Difficulty Scaling: <style=cDeath>+{ScalingValue * 50 - 100}%</style>",
                $">Player Health Regeneration: <style=cDeath>{(int)(HealthRegenMod * 100)}%</style>",
                ">Player luck: <style=cSub>Reduced in some places.</style>",
                $">Monster Health Regeneration: <style=cIsHealing>+{MonsterRegenMod * 100}%</style> of MaxHP per second (out of danger)",
                ">Oneshot Protection: <style=cSub>Removed completely</style>",
                $">Elites: <style=cIsHealing>{(1 - EliteModifier) * 100}%</style> cheaper.",
                ">Shrine of Blood: Cost <style=cDeath>hidden</style> and <style=cShrine>random</style>."
                );
            desc += "</style>";
            return desc;
        }
    }
}
