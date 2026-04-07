using Birds.src.api.contracts;
using Birds.src.api.transport;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Birds.src.network;

public class LiteNetLibServerTransport(int port) : IServerNetworkTransport
{
  private NetManager _netManager;
  private EventBasedNetListener _listener;
  private Dictionary<string, NetPeer> _playerPeers = new();

  public event Action<string> PlayerConnected;
  public event Action<string> PlayerDisconnected;
  public event Action<InputMessage> InputReceived;
  public event Action<PlayerJoinRequest, string> PlayerJoinRequested;


  public void Start()
  {
    _listener = new EventBasedNetListener();
    _netManager = new NetManager(_listener) { AutoRecycle = true };

    _listener.ConnectionRequestEvent += request => request.AcceptIfKey("Birds");
    _listener.PeerConnectedEvent += peer =>
    {
      Debug.WriteLine($"[Server] Peer connected: {peer.Address} (id: {peer.Id})");
      PlayerConnected?.Invoke(peer.Id.ToString());
    };
    _listener.PeerDisconnectedEvent += (peer, info) =>
    {
      Debug.WriteLine($"[Server] Peer disconnected: {peer.Address} (id: {peer.Id})");
      if (_playerPeers.ContainsValue(peer))
      {
        var playerId = peer.Id.ToString();
        _playerPeers.Remove(playerId);
        PlayerDisconnected?.Invoke(playerId);
      }
    };
    _listener.NetworkReceiveEvent += OnNetworkReceive;
    _netManager.Start(port);
  }

  public void Stop() => _netManager?.Stop();
  public void PollEvents() => _netManager?.PollEvents();

  public void SendGameState(GameStateMessage state, string playerId)
  {
    if (!_playerPeers.TryGetValue(playerId, out var peer))
    {
      Debug.WriteLine($"[Server] SendGameState failed - no peer for playerId: {playerId}");
      return;
    }
    Debug.WriteLine($"[Server] Sending GameState to {playerId} ({state.EntityUpdatesPerPlayer.Count} updates)");
    var writer = new NetDataWriter();
    writer.Put((byte)MessageType.GameState);
    writer.PutGameStateMessage(state);
    peer.Send(writer, DeliveryMethod.Unreliable);
  }

  public void SendControllerSpawn(ControllerSpawnMessage message, string playerId)
  {
    if (!_playerPeers.TryGetValue(playerId, out var peer))
    {
      Debug.WriteLine($"[Server] SendControllerSpawn failed - no peer for playerId: {playerId}");
      return;
    }
    Debug.WriteLine($"[Server] Sending ControllerSpawn to {playerId} (controllerId: {message.Id}, ownerId: {message.OwnerId ?? "none"}, type: {message.ControllerType})");
    var writer = new NetDataWriter();
    writer.Put((byte)MessageType.ControllerSpawn);
    writer.PutControllerSpawnMessage(message);
    peer.Send(writer, DeliveryMethod.ReliableOrdered);
  }

  public void SendWorldSnapshot(WorldSnapshotMessage snapshot, string playerId)
  {
    if (!_playerPeers.TryGetValue(playerId, out var peer))
    {
      Debug.WriteLine($"[Server] SendWorldSnapshot failed - no peer for playerId: {playerId}");
      return;
    }
    Debug.WriteLine($"[Server] Sending WorldSnapshot to {playerId} ({snapshot.Controllers.Count} controllers)");
    var writer = new NetDataWriter();
    writer.Put((byte)MessageType.WorldSnapshot);
    writer.PutWorldSnapshotMessage(snapshot);
    peer.Send(writer, DeliveryMethod.ReliableOrdered);
  }

  private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
  {
    MessageType messageType = (MessageType)reader.GetByte();
    Debug.WriteLine($"[Server] Received message type: {messageType} from peer: {peer.Id}");
    switch (messageType)
    {
      case MessageType.PlayerJoinRequest:
        HandlePlayerJoinRequest(peer, reader);
        break;
      case MessageType.Input:
        HandleInput(reader);
        break;
      default:
        Debug.WriteLine($"[Server] Unknown message type: {messageType}");
        break;
    }
  }

  private void HandlePlayerJoinRequest(NetPeer peer, NetPacketReader reader)
  {
    var request = reader.GetPlayerJoinRequest();
    string playerId = Guid.NewGuid().ToString();
    Debug.WriteLine($"[Server] PlayerJoinRequest from {request.DisplayName}, peer: {peer.Id}");
    _playerPeers[playerId] = peer;
    PlayerJoinRequested?.Invoke(request, playerId);
  }

  private void HandleInput(NetPacketReader reader)
  {
    var input = reader.GetInputMessage();
    InputReceived?.Invoke(input);
  }
}
