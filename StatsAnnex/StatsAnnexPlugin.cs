﻿using BepInEx;
using RoR2;
using System;

/*
using R2API;
using R2API.Utils;
*/


/// <summary>
/// Things to do: 
///     Make sure your references are located in a "libs" folder that's sitting next to the project folder.
///         This folder structure was chosen as it was noticed to be one of the more common structures.
///     Add a NuGet Reference to Mono.Cecil. The one included in bepinexpack3.0.0 on thunderstore is the wrong version 0.10.4. You want 0.11.1.
///         You can do this by right clicking your project (not your solution) and going to "Manage NuGet Packages".
///    Make sure the AUTHOR field is correct.
///    Make sure the MODNAME field is correct.
///    Delete this comment!
///    Oh and actually write some stuff.
/// </summary>



namespace StatsAnnex
{

    //[BepInDependency("com.bepis.r2api")]
    //[R2APISubmoduleDependency(nameof(yourDesiredAPI))]
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public sealed class StatsAnnexPlugin : BaseUnityPlugin
    {
        public const string
            MODNAME = "StatsAnnex",
            AUTHOR = "harbingerofme",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "0.0.0";

        private StatsAnnexPlugin()
        {
            //TODO: Conditinial scanning and disable self.
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Awake is automatically called by Unity")]
        private void Awake() //Called when loaded by BepInEx.
        {
            On.RoR2.CharacterBody.RecalculateStats += Annex;
        }

        private void Annex(On.RoR2.CharacterBody.orig_RecalculateStats orig, RoR2.CharacterBody self)
        {
            //Yeet orig(self);
            float oldHealth = self.maxHealth;
            float oldShield = self.maxShield;

            self.isElite = self.eliteBuffCount > 1;
            bool glasArtifact = RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.glassArtifactDef);
            self.hasOneShotProtection = self.isPlayerControlled && !glasArtifact;
            self.isGlass = ItemIndex.LunarDagger.GetCount(self) > 0;
        
            
        }

    }
}
