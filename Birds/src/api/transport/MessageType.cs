namespace Birds.src.api.transport;

public enum MessageType : byte
{
  Input = 1,
  GameState = 2,
  PlayerJoinRequest = 3,
  ControllerSpawn = 4,
  WorldSnapshot = 5,
}
