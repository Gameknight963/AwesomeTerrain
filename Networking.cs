using MultiSide.shared;
using Photon.Client;
using System;
using System.Linq;
using UnityEngine.Profiling;

namespace AwesomeTerrain
{
    public static class Networking
    {
        const string ChannelSeedRequest = "awesometerrain.seed.request";
        const string ChannelSeedResponse = "awesometerrain.seed.response";
        public static bool Available { get; set; }

        static Networking()
        {
            Available = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.GetName().Name == "Multiside.shared");
            if (Available) InitNetwork();
        }

        private static void InitNetwork()
        {
            if (NetworkRegistry.Provider != null)
            {
                Subscribe(NetworkRegistry.Provider);
                return;
            }
            NetworkRegistry.OnProviderRegistered += Subscribe;
        }

        private static void Subscribe(INetworkProvider provider)
        {
            provider.OnRoomJoined += Provider_OnRoomJoined;
            provider.OnReceived += Provider_OnReceived;
        }

        private static void Provider_OnRoomJoined()
        {
            if (!(NetworkRegistry.Provider is INetworkProvider provider)) return;

            if (provider.IsMasterClient)
            {
                Mod.SetSeedAndGenerate(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
                return;
            }
            provider.Send(ChannelSeedRequest, new PhotonHashtable());
        }

        private static void Provider_OnReceived(int actor, string channel, object data)
        {
            if (!(NetworkRegistry.Provider is INetworkProvider provider)) return;

            switch (channel)
            {
                case ChannelSeedRequest:
                    if (!provider.IsMasterClient) return;
                    provider.SendTo(actor, ChannelSeedResponse, Mod.Seed);
                    break;
                case ChannelSeedResponse:
                    if (provider.MasterClientActorNumber != actor) return;
                    Mod.SetSeedAndGenerate((int)data);
                    break;
            }
        }
    }
}
