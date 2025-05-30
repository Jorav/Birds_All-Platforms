using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Birds.src.bounding_areas;
using Birds.src.controllers;

namespace Birds.src.bounding_areas
{
    public class AxisAlignedBoundingBox : IBoundingArea
    {
        private Vector2 UL { get; set; }
        private Vector2 DL { get; set; }
        private Vector2 DR { get; set; }
        private Vector2 UR { get; set; }
        public float Area { get { return Width * Height; } }
        public float Width { get { return (int)(Math.Round((UR - UL).Length())); } }
        public float Height { get { return (int)(Math.Round((UR - DR).Length())); } }
        public (float, float) MaxXY{get;set;}
        public (float, float) MinXY{get;set;}
        //public Vector2 AbsolutePosition { get { return (UL + DR) / 2; } }
        private Vector2 position;
        public Vector2 Position
        {
            set
            {
                Vector2 change = value - position;
                UL += change;
                DL += change;
                DR += change;
                UR += change;
                position = value;
                UpdateMaxAndMin();
            }
            get
            {
                return position;
            }
        }

        float IBoundingArea.Radius {get{return (UL-DR).Length();}}

        public float Radius;

        public AxisAlignedBoundingBox(Vector2 upperLeftCorner, int width, int height)
        {
            SetBox(upperLeftCorner,width,height);
        }

        public AxisAlignedBoundingBox(IBoundingArea boundingArea)
        {
            (float, float) minXY = boundingArea.MinXY;
            (float, float) maxXY = boundingArea.MaxXY;
            float width = maxXY.Item1-minXY.Item1;
            float height = maxXY.Item2-minXY.Item2;
            SetBox(new Vector2(minXY.Item1, minXY.Item2),width,height);
        }

        public void SetBox(Vector2 upperLeftCorner, float width, float height)
        {
            UL = new Vector2(upperLeftCorner.X, upperLeftCorner.Y);
            DL = new Vector2(upperLeftCorner.X, upperLeftCorner.Y + height);
            DR = new Vector2(upperLeftCorner.X + width, upperLeftCorner.Y + height);
            UR = new Vector2(upperLeftCorner.X + width, upperLeftCorner.Y);
            this.position = upperLeftCorner;// + new Vector2(width/2, height/2);
            Radius = (float)Math.Sqrt(Math.Pow(Width / 2, 2) + Math.Pow(Height / 2, 2));
            UpdateMaxAndMin();
        }

        public static AxisAlignedBoundingBox SurroundingAABB(IBoundingArea AABB1, IBoundingArea AABB2)
        {
            (float, float) minXY1 = AABB1.MinXY;
            (float, float) maxXY1 = AABB1.MaxXY;
            (float, float) minXY2 = AABB2.MinXY;
            (float, float) maxXY2 = AABB2.MaxXY;
            float xMin = Math.Min(minXY1.Item1, minXY2.Item1);
            float xMax = Math.Max(maxXY1.Item1, maxXY2.Item1);
            float yMin = Math.Min(minXY1.Item2, minXY2.Item2);
            float yMax = Math.Max(maxXY1.Item2, maxXY2.Item2);
            return BoundingAreaFactory.GetAABB(new Vector2(xMin, yMin), (int)Math.Round(xMax - xMin), (int)Math.Round(yMax - yMin));
        }

        public static AxisAlignedBoundingBox SurroundingAABB(IBoundingArea AABB)
        {
            (float, float) minXY = AABB.MinXY;
            (float, float) maxXY = AABB.MaxXY;
            float xMin = minXY.Item1;
            float xMax = maxXY.Item1;
            float yMin = minXY.Item2;
            float yMax = maxXY.Item2;
            return BoundingAreaFactory.GetAABB(new Vector2(xMin, yMin), (int)Math.Round(xMax - xMin), (int)Math.Round(yMax - yMin));
        }

        public static AxisAlignedBoundingBox SurroundingAABB(List<ICollidable> entities){
            IBoundingArea[] OBBs = new IBoundingArea[entities.Count]; //TODO: SORT LIST ON AXIS
            for (int i = 0; i < entities.Count; i++)
                OBBs[i] = entities[i].BoundingArea;
            return SurroundingAABB(OBBs);
        }

        public static AxisAlignedBoundingBox SurroundingAABB(IBoundingArea[] boundingAreas)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            foreach(IBoundingArea BC in boundingAreas){
                (float, float) maxXY = BC.MaxXY;
                (float, float) minXY = BC.MinXY;
                if (maxXY.Item1 > maxX)
                    maxX = maxXY.Item1;
                if (minXY.Item1 < minX)
                    minX = minXY.Item1;
                if (maxXY.Item2 > maxY)
                    maxY = maxXY.Item2;
                if (minXY.Item2 < minY)
                    minY = minXY.Item2;
            }
            return BoundingAreaFactory.GetAABB(new Vector2(minX, minY), (int)Math.Round(maxX - minX), (int)Math.Round(maxY - minY));
        }
        public void UpdateMaxAndMin(){
            MaxXY = ((float)Math.Max(Math.Max(UL.X, UR.X), Math.Max(DL.X, DR.X)),(float)Math.Max(Math.Max(UL.Y, UR.Y), Math.Max(DL.Y, DR.Y)));
            MinXY = ((float)Math.Min(Math.Min(UL.X, UR.X), Math.Min(DL.X, DR.X)),(float)Math.Min(Math.Min(UL.Y, UR.Y), Math.Min(DL.Y, DR.Y)));
        }
        public static int MajorAxis(AxisAlignedBoundingBox AABB)
        {
            (float, float) diffXY = (AABB.MaxXY.Item1-AABB.MinXY.Item1, AABB.MaxXY.Item2 - AABB.MinXY.Item2);
            if(diffXY.Item1>=diffXY.Item2)
                return 0;
            return 1;
        }
        public bool CollidesWith(AxisAlignedBoundingBox AABB)
        {
            return UL.X < AABB.UL.X + AABB.Width &&
                    UL.X + Width > AABB.UL.X &&
                    UL.Y < AABB.UL.Y + AABB.Height &&
                    UL.Y + Height > AABB.UL.Y;
        }

        public bool CollidesWith(IBoundingArea boundingArea)
        {
            if(boundingArea is AxisAlignedBoundingBox aabb)
                return CollidesWith(aabb);
            throw new NotImplementedException();
        }
    }
}