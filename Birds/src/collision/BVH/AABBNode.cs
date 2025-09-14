using Birds.src.collision.bounding_areas;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.collision.BVH;

public class AABBNode
{
  public AABBNode Parent { get; set; }
  public AABBNode[] children = new AABBNode[2];
  public float Radius { get { return radius; } protected set { radius = value; } }
  protected float radius;
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
  public float Mass { get; set; }
  public int Count { get; set; }
  public Vector2 MassCenter { get; private set; }
  public AxisAlignedBoundingBox AABB { get; private set; }
  public ICollidable Entity
  {//=!null implies leaf
    get { return entity; }
    set
    {
      entity = value;
      AABB = AxisAlignedBoundingBox.SurroundingAABB(entity.BoundingArea);
      position = entity.Position;
      radius = Entity.Radius;
      Count = 1;
      Mass = entity.Mass;
      MassCenter = entity.Position;
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
      Mass += node.Mass;
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
    radius = AABB.Radius;

    UpdateMassCenter();
  }
  public void UpdateMassCenter()
  {
    MassCenter = Vector2.Zero;
    Mass = 0;
    foreach (AABBNode child in children)
    {
      if (child != null)
      {
        Mass += child.Mass;
        MassCenter += child.MassCenter * child.Mass;
      }
    }
    MassCenter /= Mass;
  }

  public void Reset()
  {
    if (entity == null)
      BoundingAreaFactory.AABBs.Push(AABB);
    AABB = null;
    entity = null;
    children[0] = null;
    children[1] = null;
    Parent = null;
    Count = 0;
    Radius = 0;
    Mass = 0;
    position = Vector2.Zero;
    MassCenter = Vector2.Zero;
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

  public void Update(GameTime gameTime)
  {
    foreach (AABBNode child in children)
      if (child != null)
        child.Update(gameTime);
  }
  #endregion
}
