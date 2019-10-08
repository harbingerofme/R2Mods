using BepInEx;
using RoR2;
using System.Collections.Generic;
using R2API.Utils;
using RoR2.Skills;
using UnityEngine;
using System;
using System.Reflection;

[assembly: AssemblyVersionAttribute("0")]//Leave as "0" for magic.

namespace Shrine_of_Skill
{
    [BepInPlugin("com.harbingerofme.skillshrine", "skillshrine", "1.0.0")]
    public class SkillShrineMod : BaseUnityPlugin
    {
        private Run.FixedTimeStamp time;
        private bool ShouldRun = false;
        public List<GenericSkill> skillList;
        
        

        public void Start()
        {
            string[] survivors = { "CommandoBody", "EngiBody","HuntressBody","LoaderBody","MageBody","MercBody","'ToolbotBody","TreebotBody" };
            survivors.ForEachTry((bodyName) =>
            {
                Logger.LogMessage($"Expanding {bodyName} loadout");
                var prefab = BodyCatalog.FindBodyPrefab(bodyName);
                SkillLocator locator = prefab.GetComponent<SkillLocator>();
                Logger.LogMessage(locator.primary.skillFamily.variants.Length);
                Logger.LogMessage(locator.secondary.skillFamily.variants.Length);
                Logger.LogMessage(locator.utility.skillFamily.variants.Length);
                Logger.LogMessage(locator.special.skillFamily.variants.Length);
                var primary = locator.primary.skillFamily.variants;
                var prClone = (SkillFamily.Variant[]) primary.Clone();
                var secondary = locator.secondary.skillFamily.variants;
                var seClone = (SkillFamily.Variant[]) primary.Clone();
                var utility = locator.utility.skillFamily.variants;
                var utClone = (SkillFamily.Variant[]) primary.Clone();
                var special = locator.special.skillFamily.variants;
                ArrayAppend(ref primary, secondary, utility, special);
                ArrayAppend(ref secondary, prClone, utility, special);
                ArrayAppend(ref utility, prClone, seClone, special);
                ArrayAppend(ref special, prClone, seClone, utClone);
            });
        }

        private void ArrayAppend(ref SkillFamily.Variant[] into, SkillFamily.Variant[] from1, SkillFamily.Variant[] from2, SkillFamily.Variant[] from3)
        {
            AppendInto(ref into, from1);
            AppendInto(ref into, from2);
            AppendInto(ref into, from3);
            
        }
        private void AppendInto(ref SkillFamily.Variant[] into, SkillFamily.Variant[] from)
        {
            int oldLength = into.Length;
            Array.Resize(ref into, into.Length + from.Length);
            for(int i = 0; i < from.Length; i++)
            {
                into[oldLength + i] = from[i];
            }
        }

        private void Run_onRunDestroyGlobal(Run obj)
        {
            ShouldRun = false;
        }

        private void Run_onRunStartGlobal(Run obj)
        {
            time = Run.FixedTimeStamp.now;
            time += 10f;
            ShouldRun = true;
        }

        public void FixedUpdate()
        {

            Run.onRunStartGlobal += Run_onRunStartGlobal; //these are events.
            Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            if (ShouldRun)
            {
                if (time.hasPassed)
                {
                    time = Run.FixedTimeStamp.now + 10;

                    Debug.Log("10 seconds have passed!");
                }
            }
        }
    }
}
/*foreach(PlayerCharacterMasterController masterController in PlayerCharacterMasterController.instances)
{
    var cm = masterController.GetComponent<CharacterMaster>();
    var bi = cm.GetBody().bodyIndex;
    SkillCatalog.GetSkillFamilyName
    cm.SetLoadoutServer();
}
*/
