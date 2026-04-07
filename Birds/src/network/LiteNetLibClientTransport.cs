using Birds.src.api.contracts;
using Birds.src.api.transport;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Birds.src.network;

public class LiteNetLibClientTransport(
    string serverAddress,
    int serverPort) : IClientNetworkTransport
{
  private NetManager _netManager;
  private NetPeer _serverPeer;
  private EventBasedNetListener _listener;
  private bool _connected = false;

  public event Action<GameStateMessage> StateReceived;
  public event Action<ControllerSpawnMessage> ControllerSpawnReceived;
  public event Action<WorldSnapshotMessage> WorldSnapshotReceived;

  public async Task Connect()
  {
    _listener = new EventBasedNetListener();
    _netManager = new NetManager(_listener) { AutoRecycle = true };

    _listener.ConnectionRequestEvent += request => request.AcceptIfKey("Birds");
    _listener.PeerConnectedEvent += peer =>
    {
      Debug.WriteLine($"[Client] Connected to server: {peer.Address}");
      _serverPeer = peer;
      _connected = true;
    };
    _listener.PeerDisconnectedEvent += (peer, info) =>
    {
      Debug.WriteLine($"[Client] Disconnected from server, reason: {info.Reason}");
      _connected = false;
    };
    _listener.NetworkReceiveEvent += OnNetworkReceive;

    _netManager.Start();
    _netManager.Connect(serverAddress, serverPort, "Birds");

    int timeout = 0;
    while (!_connected && timeout < 50)
    {
      _netManager.PollEvents();
      await Task.Delay(100);
      timeout++;
    }

    if (!_connected)
    {
      Debug.WriteLine($"[Client] Failed to connect after {timeout * 100}ms");
      throw new Exception("Failed to connect to server");
    }

    Debug.WriteLine("[Client] Connected successfully");
  }

  public async Task Disconnect()
  {
    Debug.WriteLine("[Client] Disconnecting");
    _netManager?.Stop();
    await Task.Delay(100);
  }

  public void SendPlayerJoin(PlayerJoinRequest joinRequest)
  {
    if (_serverPeer == null)
    {
      Debug.WriteLine("[Client] SendPlayerJoin failed - no server peer");
      return;
    }
    Debug.WriteLine($"[Client] Sending PlayerJoinRequest (playerId: {joinRequest.PlayerId}, name: {joinRequest.DisplayName})");
    var writer = new NetDataWriter();
    writer.Put((byte)MessageType.PlayerJoinRequest);
    writer.PutPlayerJoinRequest(joinRequest);
    _serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
  }

  public void SendInput(InputMessage input)
  {
    if (_serverPeer == null) return;
    var writer = new NetDataWriter();
    writer.Put((byte)MessageType.Input);
    writer.PutInputMessage(input);
    _serverPeer.Send(writer, DeliveryMethod.Unreliable);
  }

  public void PollEvents() => _netManager?.PollEvents();

  private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
  {
    MessageType messageType = (MessageType)reader.GetByte();
    Debug.WriteLine($"[Client] Received message type: {messageType}");
    switch (messageType)
    {
      case MessageType.GameState:
        HandleGameState(reader);
        break;
      case MessageType.ControllerSpawn:
        HandleControllerSpawn(reader);
        break;
      case MessageType.WorldSnapshot:
        HandleWorldSnapshot(reader);
        break;
      default:
        Debug.WriteLine($"[Client] Unknown message type: {messageType}");
        break;
    }
  }

  private void HandleGameState(NetPacketReader reader)
  {
    var gameState = reader.GetGameStateMessage();
    Debug.WriteLine($"[Client] GameState received (tick: {gameState.Tick}, updates: {gameState.EntityUpdatesPerPlayer.Count})");
    StateReceived?.Invoke(gameState);
  }

  private void HandleControllerSpawn(NetPacketReader reader)
  {
    var message = reader.GetControllerSpawnMessage();
    Debug.WriteLine($"[Client] ControllerSpawn received (id: {message.Id}, ownerId: {message.OwnerId ?? "none"}, type: {message.ControllerType})");
    ControllerSpawnReceived?.Invoke(message);
  }

  private void HandleWorldSnapshot(NetPacketReader reader)
  {
    var snapshot = reader.GetWorldSnapshotMessage();
    Debug.WriteLine($"[Client] WorldSnapshot received ({snapshot.Controllers.Count} controllers)");
    WorldSnapshotReceived?.Invoke(snapshot);
  }
}
