using BepInEx;
using RoR2;
using System.Reflection;
using R2API.Utils;
using UnityEngine;

namespace Deluge
{
    [BepInPlugin(GUID,NAME,VERSION)]
    public class Deluge : BaseUnityPlugin
    {
        public const string
            NAME = "Deluge",
            GUID = "test.harbingerofme." + NAME,
            VERSION = "0.0.1";

        public static int DiffIndexOverride = 4;


        private DifficultyDef DelugeDef;

        private Deluge()
        {
            DelugeDef = new DifficultyDef(3.5f, "DIFFICULTY_DELUGE_NAME", "Textures/DifficultyIcons/texDifficultyHardIcon", "DIFFICULTY_DELUGE_DESCRIPTION", new Color(0.6f, 1, 0.2f));
        }

        public void Awake()
        {
            On.RoR2.DifficultyCatalog.GetDifficultyDef += DifficultyCatalog_GetDifficultyDef;
            On.RoR2.RuleDef.FromDifficulty += RuleDef_FromDifficulty;
        }


        public void Start()
        {
            DifficultyDef[] premod = (DifficultyDef[]) typeof(DifficultyCatalog).GetFieldCached("difficultyDefs").GetValue(null);//GetFieldValu<>("difficultyDefs");
            DifficultyDef[] newDefs = new DifficultyDef[premod.Length + 1];
            premod.CopyTo(newDefs,0);
            newDefs[premod.Length] = DelugeDef;
            DiffIndexOverride = premod.Length;
            typeof(DifficultyCatalog).GetFieldCached("difficultyDefs").SetValue(null, newDefs);
        }

        private DifficultyDef DifficultyCatalog_GetDifficultyDef(On.RoR2.DifficultyCatalog.orig_GetDifficultyDef orig, DifficultyIndex difficultyIndex)
        {
            Debug.Log(difficultyIndex);
            if((int) difficultyIndex == DiffIndexOverride)
            {
                return DelugeDef;
            }
            return orig(difficultyIndex);
        }
        private RuleDef RuleDef_FromDifficulty(On.RoR2.RuleDef.orig_FromDifficulty orig)
        {
            RuleDef def = orig();
            RuleChoiceDef choice = def.AddChoice("Deluge", null, false);
            choice.spritePath = DelugeDef.iconPath;
            choice.tooltipNameToken = DelugeDef.nameToken;
            choice.tooltipNameColor = DelugeDef.color;
            choice.tooltipBodyToken = DelugeDef.descriptionToken;
            choice.difficultyIndex = (DifficultyIndex)DiffIndexOverride;
            return def;
        }

    }
}
