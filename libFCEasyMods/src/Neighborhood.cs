using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nialsorva.FCEEasyMods
{

    public abstract class Neighborhood
    {

    }

    public class AdjacencyNeighborhood : Neighborhood
    {
        public AdjacencyNeighborhood(params Vector3[] adjacencies)
        {

        }
    }
   /* public abstract class Neighborhood<T> : IEnumerable<T> where T : SegmentEntity
    {
        protected MachineEntity center;
        public Neighborhood(MachineEntity center)
        {
            this.center = center;
        }
        protected T getAt(Vector3 v)
        {
            return getAt((long)v.x, (long)v.y, (long)v.z);
        }
        protected T getAt(long x, long y, long z) 
        {
            return center.AttemptGetSegment(center.mnX+x, center.mnY+y, center.mnZ+z)
                ?.SearchEntity(center.mnX + x, center.mnY + y, center.mnZ + z) as T;
        }
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    
    public class DirectionalNeighborhood<T> : Neighborhood<T> where T:SegmentEntity
    {
        private Vector3 directionVector;

        public Vector3 direction {
            get { return directionVector; }
            set
            {
                this.directionVector = SegmentCustomRenderer.GetRotationQuaternion(center.mFlags) * value;
            }
        }

        public DirectionalNeighborhood(MachineEntity center,Vector3 dir)
            : base(center)
        {
            direction = dir;
        }
        
        public string DebugInfo()
        {
            Func<string, Vector3, string> f = ((prefix, v) => {
                return String.Format("{0}({1}) <{2}>\n", prefix, v, getAt(v)?.GetType().Name ?? "null");
            });

            return
                f("Center", Vector3.zero)
                + f("Direction", directionVector);
        }
    }
    */
}
