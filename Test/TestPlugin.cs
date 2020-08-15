using BepInEx;
using System.Reflection;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;

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

    [BepInDependency("com.bepis.r2api")]
    [BepInDependency(Diluvian.Diluvian.GUID)]
    //[R2APISubmoduleDependency(nameof(yourDesiredAPI))]
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public sealed class TestPlugin : BaseUnityPlugin
    {
        public const string
            MODNAME = "Test",
            AUTHOR = "Guido",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "0.0.0";

        private Dictionary<string, string> overrides;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Awake is automatically called by Unity")]
        private void Awake() //Called when loaded by BepInEx.
        {
            overrides = new Dictionary<string, string>();
            overrides.Add("ITEM_BEAR_PICKUP", "testing");
            overrides.Add("OBJECTIVE_FIND_TELEPORTER", "EAT BANANA");

            var origmethod = typeof(Diluvian.Diluvian).GetMethod("ReplaceString", BindingFlags.NonPublic | BindingFlags.Instance);
            var targetmethod = typeof(TestPlugin).GetMethod(nameof(OnReplaceText), BindingFlags.NonPublic | BindingFlags.Instance);
            new Hook(origmethod, targetmethod, this);
        }


        private void OnReplaceText(del_rep_text orig, Diluvian.Diluvian diluvian, string token, string text)
        {
            if (overrides.ContainsKey(token)) text = overrides[token];
            orig(diluvian, token, text);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Start is automatically called by Unity")]
        private void Start() //Called at the first frame of the game.
        {

        }
    }

    internal delegate void del_rep_text(Diluvian.Diluvian self,string token, string text);
}
