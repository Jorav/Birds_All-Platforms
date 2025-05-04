using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.utility;
using System;
using System.Collections.Generic;
using Birds.src.entities;
using Birds.src.menu;

namespace Birds.src.factories
{
  public static class CompositeControllerFactory
  {
    public static Stack<CompositeController> availableEntities = new();
    private static CompositeController GetEntity(Vector2 position, ID_ENTITY id)
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

    /*
    public static CompositeController GetEntity(Vector2 position, ID_ENTITY id, int nr)
    {
      CompositeController cc = GetEntity(position, id);
      //cc.AddEntity
    }*/
  }
}
