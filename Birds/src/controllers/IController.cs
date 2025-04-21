using Birds.src.bounding_areas;
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
        public List<IEntity> Entities { get; set; }
        public BoundingCircle BoundingCircle { get; set; }
        public Steering Steering { get; set; }
        public void SetEntities(List<IEntity> newEntities);
        public void AddControllable(IEntity c);
    }
}
