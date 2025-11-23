using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.menu;
using Birds.src.modules.collision;
using Birds.src.modules.shared.bounding_area;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.modules.controller;

public class GroupDrawModule : ModuleBase, IDrawModule
{
  protected override void Update(GameTime gameTime)
  {
  }

  public void Draw(SpriteBatch sb)
  {
    foreach (IEntity entity in container.Entities)
    {
      entity.Draw(sb);
    }
    if (GameState.DRAW_BC_OUTLINE)
    {
      var bcModule = container.GetModule<BCCollisionDetectionModule>();
      if (bcModule?.BoundingCircle != null)
      {
        if (container.Collisions.Count > 0)
        {
          DrawModule.DrawCircleOutline(sb, bcModule.BoundingCircle.Position, bcModule.BoundingCircle.Radius, Color.Red, 32, 3);
        }
        else
        {
          DrawModule.DrawCircleOutline(sb, bcModule.BoundingCircle.Position, bcModule.BoundingCircle.Radius, Color.Blue);
        }
      }
    }
    if (GameState.DRAW_AABB_OUTLINE)
    {
      var groupCDModule = container.GetModule<GroupCollisionDetectionModule>();
      if (groupCDModule?.CollisionManager != null)
      {
        groupCDModule.CollisionManager.DrawTree(sb, Color.Green);
      }
    }
  }
}

