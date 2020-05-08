
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace HarbCrate.Items
{
    [Item]
    internal sealed class ShieldInfusionHelper : Item
    {
        public ShieldInfusionHelper() : base()
        {
            Tier = ItemTier.NoTier;
            Name = new TokenValue("HC_HIDDEN0", "WAT R U DOING");
            Description = new TokenValue("HC_HIDDEN1", "DONT LOOK AT ME");
            PickupText = new TokenValue("HC_HIDDEN2", "HOW ARE YOU SEEING THIS");
            AssetPath = "";
            SpritePath = "";
            Tags = new ItemTag[]
            {
                ItemTag.AIBlacklist
            };
            HarbCratePlugin.Started += HarbCratePlugin_Started;
        }

        private void HarbCratePlugin_Started(object sender, System.EventArgs e)
        {
            Definition.hidden = true;
        }

        public override void Hook()
        {
            IL.RoR2.CharacterBody.RecalculateStats += AddFlatShields;
        }

        private void AddFlatShields(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            int shieldsLoc = 33;
            c.GotoNext(
                MoveType.Before,
                x => x.MatchLdloc(out shieldsLoc),
                x => x.MatchCallvirt<CharacterBody>("set_maxShield")
            );
            c.Emit(OpCodes.Ldloc, shieldsLoc);
            c.EmitDelegate<Func<CharacterBody, float, float>>((self, shields) =>
            {
                if (!self.inventory)
                    return shields;
                shields += self.inventory.GetItemCount(Definition.itemIndex) * ShieldOnMultiKill.ShieldPerMK;
                return shields;
            });
            c.Emit(OpCodes.Stloc, shieldsLoc);
            c.Emit(OpCodes.Ldarg_0);
        }
    }
}
