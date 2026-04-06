using Birds.src.api.contracts;
using Birds.src.api.transport;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace Birds.src.network;

public class LiteNetLibServerTransport(int port) : IServerNetworkTransport
{
  private NetManager _netManager;
  private EventBasedNetListener _listener;
  private Dictionary<string, NetPeer> _playerPeers = new();

  public event Action<string> PlayerConnected;
  public event Action<string> PlayerDisconnected;
  public event Action<InputMessage> InputReceived;
  public event Action<PlayerJoinRequest> PlayerJoinRequested;

  public void Start()
  {
    _listener = new EventBasedNetListener();
    _netManager = new NetManager(_listener) { AutoRecycle = true };

    _listener.ConnectionRequestEvent += request => request.AcceptIfKey("Birds");
    _listener.PeerConnectedEvent += peer =>
    {
      Console.WriteLine($"Peer connected: {peer.Address}");
      PlayerConnected?.Invoke(peer.Id.ToString());
    };
    _listener.PeerDisconnectedEvent += (peer, info) =>
    {
      Console.WriteLine($"Peer disconnected: {peer.Address}");
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
    if (!_playerPeers.TryGetValue(playerId, out var peer)) return;

    var writer = new NetDataWriter();
    writer.Put((byte)MessageType.GameState);
    writer.PutGameStateMessage(state);

    peer.Send(writer, DeliveryMethod.Unreliable);
  }

  public void SendControllerSpawn(ControllerSpawnMessage message, string playerId)
  {
    if (!_playerPeers.TryGetValue(playerId, out var peer)) return;

    var writer = new NetDataWriter();
    writer.Put((byte)MessageType.ControllerSpawn);
    writer.PutControllerSpawnMessage(message);

    peer.Send(writer, DeliveryMethod.ReliableOrdered);
  }

  private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
  {
    MessageType messageType = (MessageType)reader.GetByte();
    switch (messageType)
    {
      case MessageType.PlayerJoinRequest:
        HandlePlayerJoinRequest(peer, reader);
        break;
      case MessageType.Input:
        HandleInput(reader);
        break;
    }
  }

  private void HandlePlayerJoinRequest(NetPeer peer, NetPacketReader reader)
  {
    var request = reader.GetPlayerJoinRequest();
    _playerPeers[request.PlayerId] = peer;
    PlayerJoinRequested?.Invoke(request);
  }

  private void HandleInput(NetPacketReader reader)
  {
    var input = reader.GetInputMessage();
    InputReceived?.Invoke(input);
  }
}
