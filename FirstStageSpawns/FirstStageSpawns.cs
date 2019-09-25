using BepInEx;
using BepInEx.Configuration;

/*
    Code By Guido "Harb". 
     */

namespace FirstStageSpawns
{
    [BepInPlugin("com.harbingerofme.firststagespawns", "FirstStageSpawns", "1.1.0")]
    public class FirstStageSpawns : BaseUnityPlugin
    {

        public void Awake()
        {
            On.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
        }

        private void SceneDirector_PopulateScene(On.RoR2.SceneDirector.orig_PopulateScene orig, RoR2.SceneDirector self)
        {
            if (RoR2.Run.instance && self)
            {
                if (RoR2.Run.instance.stageClearCount == 0)
                {
                    RoR2.Run.instance.stageClearCount = 1;
                    float gold = self.expRewardCoefficient;
                    self.expRewardCoefficient *= 2;
                    orig(self);
                    self.expRewardCoefficient = gold;
                    RoR2.Run.instance.stageClearCount = 0;
                    return;
                }
            }
            orig(self);
        }
    }
}
