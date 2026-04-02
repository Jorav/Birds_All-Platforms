using Birds.src.containers.controller;

namespace Birds.src.player;

public class Player
{
  public string Id { get; }
  public Input Input { get; }
  public Camera Camera { get; }
  public Controller Controller { get; }
  public PlayerState CurrentState { get; set; }
  public bool IsBuilding => CurrentState == PlayerState.Building;
  public bool IsPlaying => CurrentState == PlayerState.Playing;
}

public enum PlayerState
{
  Playing,
  Building,
  Paused
}
