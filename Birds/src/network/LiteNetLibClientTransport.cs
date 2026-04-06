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
  public event Action<ControllerSpawnMessage> ControllerSpawnReceived;

  public async Task Connect()
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

  public async Task Disconnect()
  {
    _netManager?.Stop();
    await Task.Delay(100);
  }

  public void SendPlayerJoin(PlayerJoinRequest joinRequest)
  {
    if (_serverPeer == null) return;
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
    var gameState = reader.GetGameStateMessage();
    StateReceived?.Invoke(gameState);
  }

  private void HandleControllerSpawn(NetPacketReader reader)
  {
    var message = reader.GetControllerSpawnMessage();
    ControllerSpawnReceived?.Invoke(message);
  }
}
