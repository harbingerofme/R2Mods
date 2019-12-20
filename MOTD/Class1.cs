using BepInEx;
using RoR2;
using UnityEngine.Networking;

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

            On.RoR2.Chat.SendPlayerConnectedMessage += Chat_SendPlayerConnectedMessage;
        }

        private void Chat_SendPlayerConnectedMessage(On.RoR2.Chat.orig_SendPlayerConnectedMessage orig, RoR2.NetworkUser user)
        {
            orig(user);
            var message = new Chat.SimpleChatMessage() { baseToken = "{0}", paramTokens = new[] { $"Hello, {user.userName}" } };
            MotD.SendPrivateMessage(message, user);
        }
    }

    public static class MotD
    {
        public static void SendPrivateMessage(Chat.ChatMessageBase message, NetworkUser reciever)
        {
            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage((short)59);
            writer.Write(message.GetTypeIndex());
            writer.Write((MessageBase)message);
            writer.FinishMessage();
            foreach (NetworkConnection connection in NetworkServer.connections)
            {
                if(connection != null && connection == reciever.connectionToClient)
                    connection.SendWriter(writer, RoR2.Networking.QosChannelIndex.chat.intVal);
            }
        }
    }
}
