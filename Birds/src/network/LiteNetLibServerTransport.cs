using Birds.src.api.contracts;
using Birds.src.api.transport;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    _netManager = new NetManager(_listener);

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

  public void Stop()
  {
    _netManager?.Stop();
  }

  public void SendGameState(GameStateMessage state, string playerId)
  {
    if (!_playerPeers.TryGetValue(playerId, out var peer))
      return;

    var writer = new NetDataWriter();
    writer.Put((byte)MessageType.GameState);
    writer.Put(state.Tick);
    writer.Put(state.PlayerId);
    writer.Put(state.EntityUpdatesPerPlayer.Count);

    foreach (var update in state.EntityUpdatesPerPlayer.Values)
    {
      writer.Put(update.EntityId);

      writer.Put(update.X.HasValue);
      if (update.X.HasValue) writer.Put(update.X.Value);

      writer.Put(update.Y.HasValue);
      if (update.Y.HasValue) writer.Put(update.Y.Value);

      writer.Put(update.VelX.HasValue);
      if (update.VelX.HasValue) writer.Put(update.VelX.Value);

      writer.Put(update.VelY.HasValue);
      if (update.VelY.HasValue) writer.Put(update.VelY.Value);

      writer.Put(update.Rotation.HasValue);
      if (update.Rotation.HasValue) writer.Put(update.Rotation.Value);
    }

    peer.Send(writer, DeliveryMethod.ReliableOrdered);
  }

  public async Task SendInputAsync(InputMessage input) { }

  public async Task ConnectAsync() { }

  public async Task DisconnectAsync()
  {
    Stop();
  }

  public void PollEvents()
  {
    _netManager?.PollEvents();
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
    string playerId = reader.GetString();
    string displayName = reader.GetString();

    _playerPeers[playerId] = peer;

    var joinRequest = new PlayerJoinRequest
    {
      PlayerId = playerId,
      DisplayName = displayName
    };

    PlayerJoinRequested?.Invoke(joinRequest);
  }

  private void HandleInput(NetPacketReader reader)
  {
    var input = new InputMessage
    {
      PlayerId = reader.GetString(),
      Tick = reader.GetLong(),
      IsPressed = reader.GetBool(),
      PositionGameCoords = new Microsoft.Xna.Framework.Vector2(reader.GetFloat(), reader.GetFloat()),
      CameraPosition = new Microsoft.Xna.Framework.Vector2(reader.GetFloat(), reader.GetFloat()),
      CameraZoom = reader.GetFloat()
    };

    InputReceived?.Invoke(input);
  }
}
