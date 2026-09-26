using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gameplay.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pieces;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Networking
{
    #nullable enable

    /// <summary>
    /// A two player game over Relay. The host plays White. Both players run the game
    /// themselves and send each other the commands they make, which stays in sync
    /// since games are deterministic from their seed.
    /// </summary>
    public class OnlineSession : MonoBehaviour
    {
        /// <summary>
        /// Either starts a new game, or is the opponent's command.
        /// </summary>
        public readonly struct Message
        {
            public readonly ulong?       Seed;
            public readonly GameCommand? Command;

            public Message(ulong? seed, GameCommand? command)
            {
                Seed    = seed;
                Command = command;
            }
        }

        private enum MessageType : byte
        {
            Start,
            Command,
            /// <summary>
            /// The sender is leaving.
            /// </summary>
            Quit
        }

        private const string MessageName    = "jress";
        private const string ConnectionType = "dtls";

        /// <summary>
        /// Both players are connected and the first game's seed is known.
        /// </summary>
        public event Action?         Started;
        /// <summary>
        /// With the reason. Not raised by <see cref="Leave"/>.
        /// </summary>
        public event Action<string>? Disconnected;

        private static OnlineSession? current;

        private readonly Queue<Message> inbox = new();

        private NetworkManager network   = null!;
        private UnityTransport transport = null!;
        private string         joinCode  = "";
        private bool           isHost;
        private ulong?         peer;
        private bool           started;
        private bool           connected;
        private bool           disconnected;
        private bool           leaving;

        public string      GetJoinCode  () => joinCode;
        public bool        IsHost       () => isHost;
        public bool        HasStarted   () => started;
        public bool        IsConnected  () => connected;
        public Piece.Color GetLocalColor() => isHost ? Piece.Color.White : Piece.Color.Black;

        /// <summary>
        /// Creates a Relay allocation and waits for an opponent to join with
        /// <see cref="GetJoinCode"/>. Replaces any existing session.
        /// </summary>
        /// <exception cref="OperationCanceledException">If left before hosting finished.</exception>
        public static async Task<OnlineSession> Host()
        {
            OnlineSession session = await Create(true);

            try
            {
                await SignIn();
                session.ThrowIfLeft();

                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
                session.ThrowIfLeft();

                session.joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                session.ThrowIfLeft();

                session.transport.SetRelayServerData(allocation.ToRelayServerData(ConnectionType));

                if (!session.network.StartHost()) throw new Exception("Couldn't start hosting");

                session.Listen();
            }
            catch
            {
                if (session != null) session.Leave();

                throw;
            }

            return session;
        }

        /// <summary>
        /// Connects to a host. <see cref="Started"/> is raised once the host sends the
        /// first game. Replaces any existing session.
        /// </summary>
        /// <exception cref="OperationCanceledException">If left before joining finished.</exception>
        public static async Task<OnlineSession> Join(string code)
        {
            OnlineSession session = await Create(false);

            try
            {
                await SignIn();
                session.ThrowIfLeft();

                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(code);
                session.ThrowIfLeft();

                session.joinCode = code;
                session.transport.SetRelayServerData(allocation.ToRelayServerData(ConnectionType));

                if (!session.network.StartClient()) throw new Exception("Couldn't start connecting");

                session.peer = NetworkManager.ServerClientId;
                session.Listen();
            }
            catch
            {
                if (session != null) session.Leave();

                throw;
            }

            return session;
        }

        /// <summary>
        /// Leaves whichever session is open, if any.
        /// </summary>
        public static void LeaveCurrent()
        {
            if (current != null) current.Leave();
        }

        private static async Task<OnlineSession> Create(bool host)
        {
            // A left session's NetworkManager lingers until it has shut down, and only
            // one can be the singleton
            while (NetworkManager.Singleton != null)
            {
                LeaveCurrent();

                await Task.Yield();
            }

            GameObject obj = new("Online Session");
            DontDestroyOnLoad(obj);

            UnityTransport transport = obj.AddComponent<UnityTransport>();
            NetworkManager network   = obj.AddComponent<NetworkManager>();

            network.NetworkConfig = new NetworkConfig
            {
                NetworkTransport      = transport,
                EnableSceneManagement = false
            };

            OnlineSession session = obj.AddComponent<OnlineSession>();
            session.network   = network;
            session.transport = transport;
            session.isHost    = host;

            current = session;

            return session;
        }

        private static async Task SignIn()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized) await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void ThrowIfLeft()
        {
            if (this == null || leaving) throw new OperationCanceledException();
        }

        private void Listen()
        {
            network.CustomMessagingManager.RegisterNamedMessageHandler(MessageName, OnMessage);

            network.OnClientConnectedCallback  += OnClientConnected;
            network.OnClientDisconnectCallback += OnClientDisconnected;
            network.OnTransportFailure         += OnTransportFailure;
        }

        /// <summary>
        /// Tells the opponent, then shuts down. The session is destroyed once the
        /// shutdown finishes.
        /// </summary>
        public void Leave()
        {
            if (leaving) return;

            if (connected)
            {
                using FastBufferWriter writer = new(1, Allocator.Temp);
                writer.WriteValueSafe((byte)MessageType.Quit);
                Send(writer);
            }

            leaving   = true;
            connected = false;

            if (current == this) current = null;

            if (network == null) return;

            network.OnClientConnectedCallback  -= OnClientConnected;
            network.OnClientDisconnectCallback -= OnClientDisconnected;
            network.OnTransportFailure         -= OnTransportFailure;

            network.Shutdown();
        }

        /// <summary>
        /// Destroying the transport before the shutdown finishes breaks the shutdown.
        /// </summary>
        private void Update()
        {
            if (leaving && (network == null || !network.ShutdownInProgress)) Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (current == this) current = null;
        }

        /// <summary>
        /// Host only. Starts a new game for both players, arriving as a message.
        /// </summary>
        public void StartNewGame()
        {
            if (!isHost || !connected) return;

            ulong seed = (ulong)DateTime.UtcNow.Ticks;

            using FastBufferWriter writer = new(16, Allocator.Temp);
            writer.WriteValueSafe((byte)MessageType.Start);
            writer.WriteValueSafe(seed);
            Send(writer);

            Receive(new Message(seed, null));
        }

        /// <summary>
        /// For a command already run locally.
        /// </summary>
        public void SendCommand(GameCommand command)
        {
            if (!connected) return;

            string json = command.Serialize().ToString(Formatting.None);

            using FastBufferWriter writer = new(256, Allocator.Temp, 64 * 1024);
            writer.WriteValueSafe((byte)MessageType.Command);
            writer.WriteValueSafe(json);
            Send(writer);
        }

        public bool TryReceive(out Message message) => inbox.TryDequeue(out message);

        private void Send(FastBufferWriter writer)
        {
            if (peer is not ulong target) return;

            network.CustomMessagingManager.SendNamedMessage(MessageName, target, writer, NetworkDelivery.ReliableFragmentedSequenced);
        }

        private void Receive(Message message)
        {
            inbox.Enqueue(message);

            if (started || message.Seed == null) return;

            started = true;

            Started?.Invoke();
        }

        private void OnMessage(ulong sender, FastBufferReader reader)
        {
            if (sender != peer) return;

            reader.ReadValueSafe(out byte type);

            switch ((MessageType)type)
            {
                case MessageType.Start when !isHost:
                    reader.ReadValueSafe(out ulong seed);

                    Receive(new Message(seed, null));
                    break;

                case MessageType.Command:
                    reader.ReadValueSafe(out string json);

                    Receive(new Message(null, GameCommand.Deserialize(JToken.Parse(json))));
                    break;

                case MessageType.Quit:
                    Disconnect(isHost ? "Your opponent quit the game" : "The host quit the game");
                    break;

                default:
                    Debug.LogWarning($"Unexpected {(MessageType)type} message");
                    break;
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            if (isHost)
            {
                if (clientId == NetworkManager.ServerClientId || peer != null) return;

                peer      = clientId;
                connected = true;

                StartNewGame();
            }
            else if (clientId == network.LocalClientId)
            {
                connected = true;
            }
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (isHost && clientId != peer) return;

            Disconnect(isHost || connected ? "Lost connection to your opponent" : "Couldn't connect to host");
        }

        private void OnTransportFailure() => Disconnect("Connection lost");

        private void Disconnect(string reason)
        {
            if (leaving || disconnected) return;

            disconnected = true;
            connected    = false;

            Disconnected?.Invoke(reason);
        }
    }
}
