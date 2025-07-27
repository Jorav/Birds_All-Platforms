using Microsoft.Xna.Framework;
using Birds.src.controllers;
using Birds.src.utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Birds.src.modules.entity;

namespace Birds.src.factories
{
  public class MovementModuleFactory
  {

    public static Stack<MovementModule> availableMovementModules = new();

    public static MovementModule GetMovementModule(ID_MOVEMENT_MODULE id = ID_MOVEMENT_MODULE.DEFAULT)
    {
      MovementModule m;
      if (availableMovementModules.Count > 0)
      {
        m = availableMovementModules.Pop();
      }
      else
      {
        m = new MovementModule();
      }
      return m;
    }
    public static MovementModule GetMovementModule(Vector2 position, float rotation, float mass, float thrust, float friction)
    {
      MovementModule m = GetMovementModule();
      m.SetAttributes(position, rotation, mass, thrust, friction);
      return m;
    }
  }
}
