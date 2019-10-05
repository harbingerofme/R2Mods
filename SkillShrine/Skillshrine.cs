using BepInEx;
using RoR2;
using System.Collections.Generic;
using R2API.Utils;
using RoR2.Skills;

namespace Shrine_of_Skill
{
    [BepInPlugin("com.harbingerofme.skillshrine", "skillshrine", "0.0.0")]
    public class SkillShrineMod : BaseUnityPlugin
    {
        public List<GenericSkill> skillList;
        public void Start()
        {
            skillList = new List<GenericSkill>();
            var a = RoR2.Skills.SkillCatalog.allSkillDefs;
            a.ForEachTry((
                skillDef) =>
            {
                Logger.LogInfo(skillDef.skillName);
            });
            
        }
    }
}
