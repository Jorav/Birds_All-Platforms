using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.utility;
using System;
using System.Collections.Generic;
using Birds.src.entities;
using Birds.src.menu;
using Birds.src.controllers.composite;

namespace Birds.src.factories;

public static class CompositeControllerFactory
{
  public static Stack<CompositeController> availableEntities = new();
  private static CompositeController GetEntity(Vector2 position, ID_COMPOSITE id)
  {
    CompositeController we;
    if (availableEntities.Count > 0)
    {
      we = availableEntities.Pop();
    }
    else
    {
      we = new CompositeController();
    }
    return we;
  }
  public static CompositeController CreateBaseComposite(Vector2 position) { return GetEntity(position, ID_COMPOSITE.DEFAULT_SINGLE);}
  public static List<CompositeController> CreateComposites(Vector2 position, int numberOfEntities, ID_COMPOSITE id)
  {
    List<CompositeController> composites = new List<CompositeController>();
    for (int i = 0; i < numberOfEntities; i++)
    {
      composites.Add(GetEntity(position, id));
    }
    return composites;
  }
  private static Blueprint GetBlueprintFromId(ID_COMPOSITE id)
  {
    switch (id)
    {
      case ID_COMPOSITE.DEFAULT: return X;
      default: throw new NotImplementedException();
    }
  }
}
