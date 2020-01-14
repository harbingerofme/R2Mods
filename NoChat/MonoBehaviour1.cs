using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using RoR2;
using MonoMod;
using MonoMod.RuntimeDetour;
using UnityEngine.Networking;

namespace NoChat
{
    [BepInPlugin(GUID,MODNAME,VERSION)]
    public class NoChatPlugin : BaseUnityPlugin
    {
        const string AUTHOR = "PERSONWHOWANTSTOUPLOADTHIS";
        const string MODNAME = "NoChat";
        public const string GUID = "com."+AUTHOR+".nochat";
        const string VERSION = "0.0.0";

        private Dictionary<Type, byte> ChatMessageTypes;
        private static MethodInfo AddMessage;
        
        private void Awake()
        {
            Detour dropChatMessagesDetour = new Detour(
                typeof(RoR2.Chat).GetMethod("HandleBroadcastChat",BindingFlags.Static| BindingFlags.NonPublic),
                typeof(NoChatPlugin).GetMethod("FilterOutChat", BindingFlags.Static|BindingFlags.NonPublic)
            );
        }

        private void Start()
        {
            ChatMessageTypes = (Dictionary<Type, byte>) typeof(Chat.ChatMessageBase).GetField("chatMessageTypeToIndex",
                                                                            BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            AddMessage = typeof(RoR2.Chat).GetMethod("AddMessage", BindingFlags.Static | BindingFlags.NonPublic);
        }

        private static void FilterOutChat(NetworkMessage netMsg)
        {
            Chat.ChatMessageBase message = Chat.ChatMessageBase.Instantiate(netMsg.reader.ReadByte());
            if (message == null || message.GetType() == typeof(Chat.PlayerChatMessage))
                return;
            message.Deserialize(netMsg.reader);
            AddMessage.Invoke(null,new object[]{ message});
        }
        
    }
}