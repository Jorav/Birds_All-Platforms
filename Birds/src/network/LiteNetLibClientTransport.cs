using Birds.src.api.contracts;
using Birds.src.api.transport;
using Birds.src.utility;
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
  public event Action<ControllerSpawnMessage> ControllerSpawnReceived;

  public async Task ConnectAsync()
  {
    _listener = new EventBasedNetListener();
    _netManager = new NetManager(_listener) { AutoRecycle = true };

    _listener.ConnectionRequestEvent += request => request.AcceptIfKey("Birds");
    _listener.PeerConnectedEvent += peer => { _serverPeer = peer; _connected = true; };
    _listener.PeerDisconnectedEvent += (peer, info) => { _connected = false; };
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

    if (!_connected) throw new Exception("Failed to connect to server");
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

  public void PollEvents() => _netManager?.PollEvents();

  private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
  {
    MessageType messageType = (MessageType)reader.GetByte();
    switch (messageType)
    {
      case MessageType.GameState:
        HandleGameState(reader);
        break;
      case MessageType.ControllerSpawn:
        HandleControllerSpawn(reader);
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

    int count = reader.GetInt();
    for (int i = 0; i < count; i++)
    {
      var entityId = reader.GetString();
      var update = new EntityStateUpdate { EntityId = entityId };

      if (reader.GetBool()) update.X = reader.GetFloat();
      if (reader.GetBool()) update.Y = reader.GetFloat();
      if (reader.GetBool()) update.VelX = reader.GetFloat();
      if (reader.GetBool()) update.VelY = reader.GetFloat();
      if (reader.GetBool()) update.Rotation = reader.GetFloat();

      gameState.EntityUpdatesPerPlayer[entityId] = update;
    }

    StateReceived?.Invoke(gameState);
  }

  private void HandleControllerSpawn(NetPacketReader reader)
  {
    var message = new ControllerSpawnMessage
    {
      ControllerId = reader.GetString(),
      ControllerType = (ID_CONTROLLER)reader.GetInt()
    };

    int directCount = reader.GetInt();
    for (int i = 0; i < directCount; i++)
    {
      message.DirectEntities.Add(new EntitySpawnData
      {
        EntityId = reader.GetString(),
        EntityType = (ID_ENTITY)reader.GetInt(),
        X = reader.GetFloat(),
        Y = reader.GetFloat(),
        Rotation = reader.GetFloat()
      });
    }

    int compositeCount = reader.GetInt();
    for (int i = 0; i < compositeCount; i++)
    {
      var composite = new CompositeSpawnData { CompositeId = reader.GetString() };

      int entityCount = reader.GetInt();
      for (int j = 0; j < entityCount; j++)
      {
        composite.Entities.Add(new EntitySpawnData
        {
          EntityId = reader.GetString(),
          EntityType = (ID_ENTITY)reader.GetInt(),
          X = reader.GetFloat(),
          Y = reader.GetFloat(),
          Rotation = reader.GetFloat()
        });
      }

      int connCount = reader.GetInt();
      for (int j = 0; j < connCount; j++)
      {
        composite.Connections.Add(new ConnectionData
        {
          EntityId1 = reader.GetString(),
          EntityId2 = reader.GetString(),
          LinkIndex1 = reader.GetInt(),
          LinkIndex2 = reader.GetInt()
        });
      }

      message.Composites.Add(composite);
    }

    ControllerSpawnReceived?.Invoke(message);
  }
}
