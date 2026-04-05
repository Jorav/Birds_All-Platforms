namespace Birds.src.api.transport;

public enum MessageType : byte
{
  PlayerJoinRequest = 0,
  Input = 1,
  GameState = 2,
  ControllerSpawn = 3,
  ControllerDespawn = 4,
}
