using BepInEx;
using RoR2;
using UnityEngine.Networking;
using MonoMod.Cil;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;
using Mono.Cecil.Cil;

namespace MotD
{
    [BepInPlugin(GUID,NAME,VERSION)]
    public class MotDPlugin : BaseUnityPlugin
    {
        public const string
            NAME = "SimpleMotD",
            GUID = "com.harbingerofme." + NAME,
            VERSION = "0.0.1";



        public void Awake()
        {
            On.RoR2.RoR2Application.UnitySystemConsoleRedirector.Redirect += orig => { };
            IL.RoR2.Networking.GameNetworkManager.OnServerAddPlayerInternal += GameNetworkManager_OnServerAddPlayerInternal1;
        }

        private void GameNetworkManager_OnServerAddPlayerInternal1(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchCallOrCallvirt(typeof(UnityEngine.Debug), "LogFormat"));
            c.GotoNext(MoveType.Before,
                x => x.MatchRet());
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<Action<NetworkConnection>>((conn) =>
            {
                var steam = RoR2.Networking.ServerAuthManager.FindAuthData(conn).steamId;
                var message = new Chat.SimpleChatMessage() { baseToken = "{0}", paramTokens = new[] { $"Hello user with steamid: {steam}" } };
                MotD.SendPrivateMessage(message, conn);
            });
        }
    }

    public static class MotD
    {
        public static void SendPrivateMessage(Chat.ChatMessageBase message, NetworkConnection connection)
        {
            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage((short)59);
            writer.Write(message.GetTypeIndex());
            writer.Write((MessageBase)message);
            writer.FinishMessage();
            connection.SendWriter(writer, RoR2.Networking.QosChannelIndex.chat.intVal);
        }
    }
}
