using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Birds.src.bounding_areas
{
    public class BoundingAreaFactory
    {
        public static Stack<BoundingCircle> circles = new();
        public static Stack<AxisAlignedBoundingBox> AABBs = new();
        public static Stack<OrientedBoundingBox> OBBs = new();
        
        public static BoundingCircle GetCircle(Vector2 position, float radius){
            if(circles.Count == 0)
                return new BoundingCircle(position, radius);
            else
            {
                BoundingCircle circle = circles.Pop();
                circle.Position = position;
                circle.Radius = radius;
                return circle;
            }
        }
        public static AxisAlignedBoundingBox GetAABB(Vector2 upperLeftCorner, int width, int height){
            if(AABBs.Count == 0)
                return new AxisAlignedBoundingBox(upperLeftCorner, width, height);
            else
            {
                AxisAlignedBoundingBox AABB = AABBs.Pop();
                AABB.SetBox(upperLeftCorner,width,height);
                return AABB;
            }
        }

        public static OrientedBoundingBox GetOBB(Vector2 upperLeftCorner, float rotation, int width, int height){
            if(OBBs.Count == 0)
                return new OrientedBoundingBox(upperLeftCorner, rotation, width, height);
            else
            {
                OrientedBoundingBox OBB = OBBs.Pop();
                OBB.SetBox(upperLeftCorner,rotation,width,height);
                return OBB;
            }
        }
    }
}