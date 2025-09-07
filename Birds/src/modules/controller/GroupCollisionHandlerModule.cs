using Birds.src.collision;
using Birds.src.events;
using Birds.src.modules.collision;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.controller;
public class GroupCollisionHandlerModule : ModuleBase
{
  public bool ResolveInternalCollisions { get; set; }

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => ResolveInternalCollisions, container.ResolveInternalCollisions);
  }

  protected override void Update(GameTime gameTime)
  {
    if (!ResolveInternalCollisions)
    {
      return;
    }
    var collisionPairs = container.GetModule<GroupCollisionDetectionModule>().CollisionPairs;
    ICollidable.ResolveCollisions(collisionPairs);
  }

  public override object Clone()
  {
    GroupCollisionHandlerModule cloned = (GroupCollisionHandlerModule)base.Clone();
    cloned.ResolveInternalCollisions = ResolveInternalCollisions;
    return cloned;
  }
}

