using Birds.src.api.contracts;
using Birds.src.api.transport;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
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

  public async Task ConnectAsync()
  {
    _listener = new EventBasedNetListener();
    _netManager = new NetManager(_listener);

    _listener.ConnectionRequestEvent += request => request.AcceptIfKey("Birds");
    _listener.PeerConnectedEvent += peer =>
    {
      _serverPeer = peer;
      _connected = true;
      Console.WriteLine("Connected to server");
    };

    _listener.PeerDisconnectedEvent += (peer, info) =>
    {
      _connected = false;
      Console.WriteLine("Disconnected from server");
    };

    _listener.NetworkReceiveEvent += OnNetworkReceive;

    _netManager.Start();
    _netManager.Connect(serverAddress, serverPort, "Birds");

    int timeout = 0;
    while (!_connected && timeout < 50)
    {
      await Task.Delay(100);
      timeout++;
    }

    if (!_connected)
      throw new Exception("Failed to connect to server");
  }

  public async Task DisconnectAsync()
  {
    _netManager?.Stop();
    await Task.Delay(100);
  }

  public async Task SendPlayerJoinAsync(PlayerJoinRequest joinRequest)
  {
    if (_serverPeer == null) return;

    var writer = new NetDataWriter();
    writer.Put((byte)MessageType.PlayerJoinRequest);
    writer.Put(joinRequest.PlayerId);
    writer.Put(joinRequest.DisplayName);

    _serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
  }

  public async Task SendInputAsync(InputMessage input)
  {
    if (_serverPeer == null) return;

    var writer = new NetDataWriter();
    writer.Put((byte)MessageType.Input);
    writer.Put(input.PlayerId);
    writer.Put(input.Tick);
    writer.Put(input.IsPressed);
    writer.Put(input.PositionGameCoords.X);
    writer.Put(input.PositionGameCoords.Y);
    writer.Put(input.CameraPosition.X);
    writer.Put(input.CameraPosition.Y);
    writer.Put(input.CameraZoom);

    _serverPeer.Send(writer, DeliveryMethod.Unreliable);
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
      case MessageType.GameState:
        HandleGameState(reader);
        break;
    }
  }

  private void HandleGameState(NetPacketReader reader)
  {
    var gameState = new GameStateMessage
    {
      Tick = reader.GetLong(),
      PlayerId = reader.GetString()
    };

    int entityCount = reader.GetInt();
    for (int i = 0; i < entityCount; i++)
    {
      var entityId = reader.GetString();
      var update = new EntityStateUpdate { EntityId = entityId };

      bool hasX = reader.GetBool();
      if (hasX) update.X = reader.GetFloat();

      bool hasY = reader.GetBool();
      if (hasY) update.Y = reader.GetFloat();

      bool hasVelX = reader.GetBool();
      if (hasVelX) update.VelX = reader.GetFloat();

      bool hasVelY = reader.GetBool();
      if (hasVelY) update.VelY = reader.GetFloat();

      bool hasRotation = reader.GetBool();
      if (hasRotation) update.Rotation = reader.GetFloat();

      gameState.EntityUpdatesPerPlayer[entityId] = update;
    }

    StateReceived?.Invoke(gameState);
  }
}

public enum MessageType : byte
{
  PlayerJoinRequest = 0,
  Input = 1,
  GameState = 2,
}
