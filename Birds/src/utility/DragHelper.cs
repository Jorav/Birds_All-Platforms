using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Birds.src.utility;

public class DragHelper
{
  private IModuleContainer _container;
  private LongPressTimer _longPressTimer;
  private Vector2 _dragOffset;
  private Vector2 _lastMousePosition;
  private Camera _camera;

  public bool IsDragging { get; private set; }
  public IEntity DraggedEntity { get; private set; }

  public DragHelper(IModuleContainer container, Camera camera)
  {
    _container = container;
    _camera = camera;
    _longPressTimer = new LongPressTimer(0.5f);
  }

  public void Update(GameTime gameTime)
  {
    Vector2 currentMouseWorld = Input.PositionGameCoords;

    if (!Input.IsPressed)
    {
      _longPressTimer.Stop();
      IsDragging = false;
      _camera.IsLocked = false;
      DraggedEntity = null;
      return;
    }

    if (IsDragging && DraggedEntity != null)
    {
      _camera.IsLocked = true;
      Vector2 targetPos = currentMouseWorld + _dragOffset;
      var movement = DraggedEntity.GetModule<MovementModule>();
      movement?.PerformManualMove(targetPos);

      _lastMousePosition = currentMouseWorld;
      return;
    }

    if (Input.WasPressed)
    {
      foreach (var entity in _container.Entities.Reverse())
      {
        if (entity.Contains(currentMouseWorld))
        {
          DraggedEntity = entity;
          _longPressTimer.Start();
          break;
        }
      }
    }
    else if (_longPressTimer.IsBeingHeld && DraggedEntity != null)
    {
      if (!DraggedEntity.Contains(currentMouseWorld))
      {
        _longPressTimer.Stop();
        DraggedEntity = null;
        return;
      }

      _longPressTimer.Update(gameTime);

      if (_longPressTimer.JustTriggered)
      {
        IsDragging = true;
        _dragOffset = DraggedEntity.Position.Value - currentMouseWorld;
      }
    }

    _lastMousePosition = currentMouseWorld;
  }

  public void Reset()
  {
    _camera.IsLocked = false;
    DraggedEntity = null;
    IsDragging = false;
    _longPressTimer.Stop();
  }
}