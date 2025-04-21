using Birds.src.bounding_areas;
using Birds.src.controllers;
using Birds.src.controllers.steering;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using static Birds.src.entities.WorldEntity;

namespace Birds.src.entities
{
    public class CompositeController : Controller, IController, IEntity
    {
        public new float Rotation { get { return rotation; }
            set
            {
                float dRotation = Rotation - value;
                foreach (WorldEntity e in entities)
                {
                    Vector2 relativePosition = e.Position - Position;
                    Vector2 newRelativePosition = Vector2.Transform(relativePosition, Matrix.CreateRotationZ(-dRotation));
                    e.Velocity = newRelativePosition - relativePosition;
                    e.Rotation = Rotation;

                }
                rotation = value;
            }
        }
        private float rotation;

        public Controller Manager { get; set; }

        public Vector2 Velocity => throw new NotImplementedException();

        public override void Update(GameTime gameTime)
        {
            if (Manager == null)
                Steer(gameTime);
            UpdateEntities(gameTime);
            //UpdatePosition
            UpdateRadius();
            CollisionManager.Update(gameTime); //Look over
        }

        public void AddControllable(IEntity c)
        {
            throw new NotImplementedException();//can probably be the same?
        }/*
        protected void Connect(IEntity entity)
        {
            if (entities.Count > 0 && !entity.IsFiller)
            {
                foreach (WorldEntity e in entities)
                {
                    if (entity != e && !e.IsFiller)
                    {
                        foreach (Link lE in e.Links)
                            if (lE.ConnectionAvailable)
                            {
                                foreach (Link lEntity in entity.Links)
                                    if (lEntity.ConnectionAvailable && e.Contains(lEntity.AbsolutePosition - lE.RelativePositionRotated / 2) && entity.Contains(lE.AbsolutePosition - lEntity.RelativePositionRotated / 2)) //divided by 2 because of edges of links connecting to others
                                    {
                                        lE.ConnectTo(lEntity);
                                    }
                            }
                    }
                }

            }
        }*/
        #region ConnectSeperatedEntities
        /*private List<HashSet<WorldEntity>> GetSetsOfEntities()
        {
            List<HashSet<WorldEntity>> sets = new List<HashSet<WorldEntity>>();
            //Entities.Sort((a, b) => a.Links.Count.CompareTo(a.Links.Count));
            foreach (WorldEntity e in entities)
            {
                bool containsEntity = false;
                foreach (HashSet<WorldEntity> s in sets)
                    if (s.Contains(e))
                        containsEntity = true;
                if (!containsEntity)
                {
                    HashSet<WorldEntity> set = new HashSet<WorldEntity>();
                    set.Add(e);
                    GetConnectedEntities(e, set);
                    sets.Add(set);
                }

            }
            return sets;
        }
        private HashSet<WorldEntity> GetConnectedEntities(WorldEntity e, HashSet<WorldEntity> foundEntities)
        {
            foreach (Link l in e.Links)
                if (!l.ConnectionAvailable)
                    if (!foundEntities.Contains(l.connection.Entity))
                    {
                        foundEntities.Add(l.connection.Entity);
                        GetConnectedEntities(l.connection.Entity, foundEntities);
                    }
            return foundEntities;
        }
        /*public bool Remove(IControllable c)
        {
            if (c is WorldEntity we)
            {
                if (we != null && Controllables.Remove(we))
                {
                    foreach (Link l in we.Links)
                        if (!l.ConnectionAvailable && l.connection.Entity.Links.Count == 1)
                            ;// RemoveEntity(l.connection.Entity);
                    if (we is Shooter s)
                        projectiles.Remove(s.Projectiles);

                    foreach (Link l in we.Links) //remove filler links
                        if (!l.ConnectionAvailable)
                        {
                            if (l.connection.Entity.IsFiller)
                                Controllables.Remove(l.connection.Entity);
                            l.SeverConnection();
                        }
                    List<HashSet<WorldEntity>> connectedEntities = GetSetsOfEntities();
                    for (int i = 1; i < connectedEntities.Count; i++)
                    {
                        WorldEntity[] tempEntities = new WorldEntity[connectedEntities[i].Count];
                        connectedEntities[i].CopyTo(tempEntities);
                        foreach (WorldEntity eSeperated in tempEntities)
                            Controllables.Remove(eSeperated);
                        EntityController ec = new EntityController(tempEntities, Rotation);

                        SeperatedEntities.Add(ec);
                    }
                    UpdatePosition();
                    UpdateRadius();
                    we.Manager = null;
                    return true;
                }
                else
                {
                    bool removed = false;
                    foreach (EntityController ec in SeperatedEntities)
                        if (ec.Remove(we))
                            removed = true;
                    return removed;
                }
            }
            else
                return false;
        }*/
        #endregion
        #region Links
        /*public void AddAvailableLinkDisplays()
        {
            List<WorldEntity> tempEntities = new List<WorldEntity>();
            foreach (WorldEntity e in Controllables)
            {
                if (!e.IsFiller)
                {
                    List<WorldEntity> fillers = e.FillEmptyLinks();
                    foreach (WorldEntity ee in fillers)
                        if (!Controllables.Contains(ee))
                            tempEntities.Add(ee);
                }
            }
            foreach (WorldEntity eT in tempEntities)
            {
                bool overlaps = false;
                foreach (WorldEntity eE in Controllables)
                    if (eT.CollidesWith(eE))//eT.Contains(eE.Position) || eE.Contains(eT.Position))
                        overlaps = true;
                if (!overlaps)
                    AddEntity(eT);
                else
                    eT.Links[0].connection.SeverConnection();
            }
        }
        public void ClearAvailableLinks()
        {
            List<WorldEntity> tempEntities = new List<WorldEntity>();
            foreach (WorldEntity e in Controllables)
            {
                if (e.IsFiller)
                {
                    foreach (Link l in e.Links)
                        l.SeverConnection();
                    tempEntities.Add(e);
                }
            }
            foreach (WorldEntity e in tempEntities)
                Controllables.Remove(e);
            foreach (EntityController ec in SeperatedEntities)
                ec.ClearAvailableLinks();
            UpdateRadius();
        }
        public bool ReplaceEntity(WorldEntity eOld, WorldEntity eNew)
        {
            if (Controllables.Contains(eOld))
            {
                eNew.ConnectTo(eOld.Links[0].connection.Entity, eOld.Links[0].connection);
                Controllables.Remove(eOld);
                eOld.Links[0].SeverConnection();
                if (!AddEntity(eNew))
                {
                    eOld.ConnectTo(eNew.Links[0].connection.Entity, eNew.Links[0].connection);
                    AddEntity(eOld);
                    return false;
                }

                return true;
            }
            return false;
        }
        */
        #endregion
        public void SetEntities(List<IEntity> newEntities)
        {
            throw new NotImplementedException();
        }

        public void Collide(ICollidable otherCollidable)
        {
            if(otherCollidable is CompositeController cc)
            {
                throw new NotImplementedException();//kollas åt båda hållen så denna behöver bara göra sin egna check
            }
            if(otherCollidable is IEntity entity)
            {
                Collide(entity);
            }
            if (otherCollidable is IController controller)
            {
                CollisionManager.CollideWithTree(controller.CollisionManager);
            }
        }
        public void Collide(IEntity e)
        {
            /*
            //collission repulsion
            Vector2 vectorFromOther = e.Position - position;
            float distance = vectorFromOther.Length();
            vectorFromOther.Normalize();
            Vector2 collissionRepulsion = 0.5f * Vector2.Normalize(-vectorFromOther) * (Vector2.Dot(velocity, vectorFromOther) * Mass + Vector2.Dot(e.Velocity, -vectorFromOther) * e.Mass); //make velocity depend on position
            TotalExteriorForce += collissionRepulsion;

            //overlap repulsion
            float distance2 = (position - e.Position).Length();
            if (distance2 < 5)
                distance2 = 5;
            float radius = Radius * (e.Mass + Mass) / 2;
            Vector2 overlapRepulsion = 30f * Vector2.Normalize(position - e.Position) / distance2;
            TotalExteriorForce += overlapRepulsion;*/
        }

        public bool CollidesWith(ICollidable otherCollidable)
        {
            if (IsCollidable && otherCollidable.IsCollidable)
            {
                if (otherCollidable is IEntity entity)
                {
                    if (entity.BoundingCircle.CollidesWith(BoundingCircle))
                        return BoundingArea.CollidesWith(otherCollidable.BoundingArea);
                    else
                        return false;
                }
                else
                    return BoundingArea.CollidesWith(otherCollidable.BoundingArea);
            }
            else
                return false;
        }

        public void Deprecate()
        {
            throw new NotImplementedException();
        }

        public override void RotateTo(Vector2 position)
        {
            Rotation = Movable.CalculateRotation(position, Position);
        }
    }
}
