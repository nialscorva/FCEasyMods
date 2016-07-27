using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nialsorva.FCEEasyMods
{
    public interface EntityFilter
    {
        bool Allow(SegmentEntity se);
        Vector3 ToVector3();
    }

    public class RelativeLocation : EntityFilter
    {
        private Vector3 face;
        private Vector3 worldDirection;
        
        public RelativeLocation(ModMachineEntity machine, Vector3 face)
        {
            this.face = face;
            machine.RotationEvent += this.OnRotation;
        }
        
        public void OnRotation(ModMachineEntity sender, Quaternion rotation)
        {
            Vector3 v=(rotation * face);
            worldDirection.x = (long)v.x + sender.mnX;
            worldDirection.y = (long)v.y + sender.mnY;
            worldDirection.z = (long)v.z + sender.mnZ;
        }
        public bool Allow(SegmentEntity se)
        {
            return se.mnX == worldDirection.x && se.mnY == worldDirection.y && se.mnZ == worldDirection.z;
        }
        public Vector3 ToVector3()
        {
            return worldDirection;
        }
    }
}
