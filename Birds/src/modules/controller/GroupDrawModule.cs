using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.menu;
using Birds.src.modules.collision;
using Birds.src.modules.shared.collision_detection;
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
    if (Game1.DRAW_BC_OUTLINE)
    {
      var cdModule = container.GetModule<BaseCollisionDetectionModule>();
      if (cdModule?.BoundingCircle != null)
      {
        var color = container.Collisions.Count > 0 ? Color.Red : Color.Blue;
        DrawModule.DrawCircleOutline(sb, cdModule.BoundingCircle.Position, cdModule.BoundingCircle.Radius, color, 32, 3);
      }
    }
    if (Game1.DRAW_AABB_OUTLINE)
    {
      var groupCDModule = container.GetModule<GroupCollisionDetectionModule>();
      if (groupCDModule?.CollisionManager != null)
      {
        groupCDModule.CollisionManager.DrawTree(sb, Color.Green);
      }
    }
  }
}

