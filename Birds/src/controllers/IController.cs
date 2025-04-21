using Birds.src.bounding_areas;
using Birds.src.BVH;
using Birds.src.controllers.steering;
using Birds.src.entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.controllers
{
    public interface IController : ICollidable
    {
        public AABBTree CollisionManager { get; }
        public List<IEntity> Entities { get; set; }
        public Steering Steering { get; set; }
        public void SetEntities(List<IEntity> newEntities);
        public void AddControllable(IEntity c);
    }
}
