using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity;
using Birds.src.player;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Birds.src.utility;

public class DragHelper
{
  private IModuleContainer _container;
  private LongPressTimer _longPressTimer;
  private Vector2 _dragOffset;
  private Input _input;
  private Vector2 _originalEntityPosition;

  public bool IsDragging { get; private set; }
  public IEntity DraggedEntity { get; private set; }
  public bool SnapBackOnRelease { get; set; } = true;

  public DragHelper(IModuleContainer container, Input input)
  {
    _container = container;
    _input = input;
    _longPressTimer = new LongPressTimer(0.5f);
  }

  public void Update(GameTime gameTime)
  {
    Vector2 currentMouseWorld = _input.PositionGameCoords;

    if (_input.WasJustPressed)
    {
      Reset();
      SelectEntityAt(currentMouseWorld);
      return;
    }

    if (!_input.IsPressed)
    {
      HandleMouseReleased();
      return;
    }

    if (IsDragging && DraggedEntity != null)
    {
      _input.Camera.IsLocked = true;
      DraggedEntity.GetModule<MovementModule>()?.SetManualMoveTarget(currentMouseWorld + _dragOffset);
      return;
    }

    if (_longPressTimer.IsBeingHeld && DraggedEntity != null)
    {
      UpdateLongPress(gameTime, currentMouseWorld);
    }
  }

  private void HandleMouseReleased()
  {
    if (IsDragging && DraggedEntity != null && SnapBackOnRelease)
    {
      var movementModule = DraggedEntity.GetModule<MovementModule>();
      movementModule.SetManualMoveTarget(_originalEntityPosition);
      movementModule.PerformManualMove();
      _input.Camera.Position = _container.Position;
      _input.Camera.PreviousPosition = _container.Position;
    }

    Reset();
  }

  private void SelectEntityAt(Vector2 worldPos)
  {
    foreach (var entity in _container.Entities)
    {
      if (entity.Contains(worldPos))
      {
        DraggedEntity = entity;
        _longPressTimer.Start();
        return;
      }
    }
  }

  private void UpdateLongPress(GameTime gameTime, Vector2 worldPos)
  {
    if (!DraggedEntity.Contains(worldPos))
    {
      Reset();
      return;
    }

    _longPressTimer.Update(gameTime);
    if (_longPressTimer.JustTriggered)
    {
      IsDragging = true;
      _originalEntityPosition = DraggedEntity.Position.Value;
      _dragOffset = DraggedEntity.Position.Value - worldPos;
    }
  }

  public void Reset()
  {
    _input.Camera.IsLocked = false;
    DraggedEntity = null;
    IsDragging = false;
    _longPressTimer.Stop();
  }
}