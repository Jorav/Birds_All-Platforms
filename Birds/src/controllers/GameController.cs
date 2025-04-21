using System.Collections.Generic;
using System.Linq;
using Birds.src.BVH;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.controllers
{
  public class GameController{
    public static List<Controller> controllers;
    private AABBTree collisionManager = new();

    public GameController(){
      controllers = new();
    }

    public void Add(Controller c){
      controllers.Add(c);
      collisionManager.UpdateTree(controllers.Cast<ICollidable>().ToList());
    }


    public void Update(GameTime gameTime){
      foreach(Controller c in controllers)
        c.Update(gameTime);
      collisionManager.Update(gameTime);
    }

    public void Draw(SpriteBatch sb){
      foreach(Controller c in controllers)
        c.Draw(sb);
    }
  }
}