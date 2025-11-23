using Birds.src.collision.bounding_areas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Birds.src.collision.BVH;

public class AABBNode
{
  public AABBNode Parent { get; set; }
  public AABBNode[] children = new AABBNode[2];
  public Vector2 Position
  {
    get { return position; }
    set
    {
      Vector2 posChange = value - Position;
      foreach (AABBNode c in children)
        if (c != null)
          c.Position += posChange;
      position = value;
    }
  }
  protected Vector2 position;
  public int Count { get; set; }
  public AxisAlignedBoundingBox AABB { get; private set; }
  public ICollidable Entity
  {//=!null implies leaf
    get { return entity; }
    set
    {
      entity = value;
      AABB = AxisAlignedBoundingBox.SurroundingAABB(entity.BoundingArea);
      position = entity.Position;
      Count = 1;
    }
  }
  private ICollidable entity;
  public AABBNode()
  {
  }
  #region node-functionality
  public void Add(AABBNode node)
  {
    if (children.Count(x => x != null) < 2)
    {
      if (children[0] == null)
        children[0] = node;
      else if (children[1] == null)
        children[1] = node;
      node.Parent = this;
      Count += node.Count;
    }
  }

  public void RefitBoundingBox()
  {
    if (AABB != null)
      BoundingAreaFactory.AABBs.Push(AABB);
    if (children.Count(x => x != null) == 1)
    {
      if (children[0] != null)
      {
        AABB = AxisAlignedBoundingBox.SurroundingAABB(children[0].AABB);
        Count = children[0].Count;
      }
      else
      {
        AABB = AABB = AxisAlignedBoundingBox.SurroundingAABB(children[1].AABB);
        Count = children[1].Count;
      }
    }
    else if (children.Count(x => x != null) == 2)
    {
      AABB = AxisAlignedBoundingBox.SurroundingAABB(children[0].AABB, children[1].AABB);
      Count = children[0].Count + children[1].Count;
    }
    position = AABB.Position;
  }

  public void Reset()
  {
    if (entity == null && AABB != null)
      BoundingAreaFactory.AABBs.Push(AABB);
    AABB = null;
    entity = null;
    children[0] = null;
    children[1] = null;
    Parent = null;
    Count = 0;
    position = Vector2.Zero;
  }
  #endregion
  #region update-logic

  public void AddInternalCollissionsToEntities()
  {
    if (children.Count(x => x != null) == 2)
      children[0].AddCollisionsToEntities(children[1]);
    if (children[0] != null)
      children[0].AddInternalCollissionsToEntities();
    if (children[1] != null)
      children[1].AddInternalCollissionsToEntities();
    if(children[0] == null && children[1] == null && Entity != null)
      Entity.AddInternalCollisions();
  }

  public void AddCollisionsToEntities(AABBNode node)
  {
    if (!AABB.CollidesWith(node.AABB))
    {
      return;
    }
    if (Entity == null)
    {
      foreach (AABBNode child in children)
      {
        if (child != null)
        {
          child.AddCollisionsToEntities(node);
        }
      }
    }
    else if (node.Entity == null)
    {
      foreach (AABBNode childOther in node.children)
      {
        if (childOther != null)
        {
          AddCollisionsToEntities(childOther);
        }
      }
    }
    else
    {
      Entity.AddCollisionsToEntities(node.Entity);
    }
  }

  public void AddCollisionsToEntities(ICollidable collidable)
  {
    if (!IBoundingArea.CollidesWith(AABB, collidable.BoundingArea))//AABB.CollidesWith(collidable.BoundingArea))
    {
      return;
    }
    if (Entity != null)
    {
      Entity.AddCollisionsToEntities(collidable);
    }
    else
    {
      foreach (AABBNode child in children)
      {
        if (child != null)
        {
          child.AddCollisionsToEntities(collidable);
        }
      }
    }
  }

  public void Update(GameTime gameTime)
  {
    foreach (AABBNode child in children)
      if (child != null)
        child.Update(gameTime);
  }
  #endregion

  public void DrawNode(SpriteBatch sb, Color color)
  {
    if (AABB != null)
    {
      DrawModule.DrawRectangleOutline(sb, AABB.UL, AABB.UR, AABB.DR, AABB.DL, color, 1);
    }

    if (children[0] != null)
      children[0].DrawNode(sb, color);
    if (children[1] != null)
      children[1].DrawNode(sb, color);
  }
}
