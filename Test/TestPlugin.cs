using BepInEx;
using UnityEngine;

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



namespace Test
{

    //[BepInDependency("com.bepis.r2api")]
    //[R2APISubmoduleDependency(nameof(yourDesiredAPI))]
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public sealed class TestPlugin : BaseUnityPlugin
    {
        public const string
            MODNAME = "Test",
            AUTHOR = "Guido",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "0.0.0";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Awake is automatically called by Unity")]
        private void Awake() //Called when loaded by BepInEx.
        {
            On.RoR2.SpiteBombController.OnFinalBounce += SpiteBombController_OnFinalBounce;
        }

        private void SpiteBombController_OnFinalBounce(On.RoR2.SpiteBombController.orig_OnFinalBounce orig, RoR2.SpiteBombController self)
        {
            Debug.LogWarning(self.delayBlast.procCoefficient);
            orig(self);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Start is automatically called by Unity")]
        private void Start() //Called at the first frame of the game.
        {

        }
    }
}
