using Birds.src.containers.entity;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.cohesion;

public class KNearestTree
{
  private KNearestNode root;
  private Dictionary<IEntity, KNearestNode> entityToLeaf;
  
  public void Build(IEnumerable<IEntity> entities)
  {
    var entityList = entities.ToList();
    entityToLeaf = new Dictionary<IEntity, KNearestNode>();
    
    if (entityList.Count == 0)
    {
      root = null;
      return;
    }
    
    root = BuildRecursive(entityList, 0, entityList.Count, 0, null);
  }
  
  private KNearestNode BuildRecursive(List<IEntity> entities, int start, int count, int depth, KNearestNode parent)
  {
    if (count == 0) return null;
    
    if (count == 1)
    {
      var leaf = new KNearestNode
      {
        Entity = entities[start],
        Parent = parent,
        CenterOfMass = entities[start].Position.Value,
        TotalMass = entities[start].Mass.Value,
        EntityCount = 1,
        SubtreeRadius = 0
      };
      
      entityToLeaf[entities[start]] = leaf;
      return leaf;
    }
    
    int axis = depth % 2;
    
    if (axis == 0)
      entities.Sort(start, count, Comparer<IEntity>.Create((a, b) => a.Position.Value.X.CompareTo(b.Position.Value.X)));
    else
      entities.Sort(start, count, Comparer<IEntity>.Create((a, b) => a.Position.Value.Y.CompareTo(b.Position.Value.Y)));
    
    int mid = count / 2;
    
    var node = new KNearestNode { Parent = parent };
    
    node.Left = BuildRecursive(entities, start, mid, depth + 1, node);
    node.Right = BuildRecursive(entities, start + mid, count - mid, depth + 1, node);
    
    CalculateAggregates(node);
    
    return node;
  }
  
  private void CalculateAggregates(KNearestNode node)
  {
    if (node.IsLeaf) return;
    
    node.EntityCount = 0;
    node.TotalMass = 0;
    Vector2 weightedPosition = Vector2.Zero;
    
    if (node.Left != null)
    {
      node.EntityCount += node.Left.EntityCount;
      node.TotalMass += node.Left.TotalMass;
      weightedPosition += node.Left.CenterOfMass * node.Left.TotalMass;
    }
    
    if (node.Right != null)
    {
      node.EntityCount += node.Right.EntityCount;
      node.TotalMass += node.Right.TotalMass;
      weightedPosition += node.Right.CenterOfMass * node.Right.TotalMass;
    }
    
    if (node.TotalMass > 0)
    {
      node.CenterOfMass = weightedPosition / node.TotalMass;
    }
    
    node.SubtreeRadius = 0;
    if (node.Left != null)
    {
      float leftDist = Vector2.Distance(node.CenterOfMass, node.Left.CenterOfMass) + node.Left.SubtreeRadius;
      node.SubtreeRadius = Math.Max(node.SubtreeRadius, leftDist);
    }
    if (node.Right != null)
    {
      float rightDist = Vector2.Distance(node.CenterOfMass, node.Right.CenterOfMass) + node.Right.SubtreeRadius;
      node.SubtreeRadius = Math.Max(node.SubtreeRadius, rightDist);
    }
  }
  
  public List<IEntity> GetEntitiesWithinRadius(IEntity queryEntity, float radius)
  {
    var results = new List<IEntity>();
    
    if (!entityToLeaf.TryGetValue(queryEntity, out var leafNode))
      return results;
    
    SearchRadius(root, queryEntity.Position.Value, radius, queryEntity, results);
    return results;
  }
  
  private void SearchRadius(KNearestNode node, Vector2 center, float radius, IEntity exclude, List<IEntity> results)
  {
    if (node == null) return;
    
    if (node.IsLeaf)
    {
      if (node.Entity != exclude)
      {
        float dist = Vector2.Distance(center, node.Entity.Position.Value);
        if (dist <= radius)
        {
          results.Add(node.Entity);
        }
      }
      return;
    }

    float distToCenterOfMass = Vector2.Distance(center, node.CenterOfMass);
    if (distToCenterOfMass - node.SubtreeRadius > radius)
    {
      return;
    }
    
    SearchRadius(node.Left, center, radius, exclude, results);
    SearchRadius(node.Right, center, radius, exclude, results);
  }
}