namespace Birds.src.network;

public enum PacketType : byte
{
  Input = 1,
  GameState = 2,
  PlayerJoin = 3,
  PlayerLeave = 4
}